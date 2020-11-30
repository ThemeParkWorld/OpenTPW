using Engine.ECS.Observer;
using Engine.ECS.Managers;
using Engine.Utils;
using Engine.Utils.DebugUtils;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Engine.Managers
{
    class RconWebFrontendManager : Manager<RconWebFrontendManager>
    {
        private readonly HttpListener listener;
        private readonly Thread httpThread;
        private bool shouldRun = true;

        public RconWebFrontendManager()
        {
            if (!GameSettings.RconEnabled)
                return;

            listener = new HttpListener();
            listener.Prefixes.Add($"http://127.0.0.1:{GameSettings.WebPort}/");
            listener.Start();

            Logging.Log($"Web console is listening on :{GameSettings.WebPort}");

            httpThread = new Thread(HttpThread);
            httpThread.Start();
        }

        private void HttpThread()
        {
            while (shouldRun)
            {
                var httpListenerContext = listener.GetContext();
                var request = httpListenerContext.Request;
                var response = httpListenerContext.Response;

                // Check which file was requested; if none, use index.html
                var fileRequested = request.Url.AbsolutePath;
                if (fileRequested == "/")
                    fileRequested = "/index.html";

                var filePath = $"Content/WebConsole/{fileRequested}";

                if (File.Exists(filePath))
                {
                    byte[] data = File.ReadAllBytes(filePath);

                    response.ContentType = GetContentType(filePath);
                    response.ContentEncoding = Encoding.Unicode;
                    response.ContentLength64 = data.Length;

                    response.OutputStream.Write(data, 0, data.Length);
                    response.Close();
                }
                else
                {
                    response.ContentType = "none";
                    response.StatusCode = 404;
                    response.Close();
                }
            }
        }

        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath)?.Substring(1);
            return extension switch
            {
                "css" => "text/css",
                "html" => "text/html",
                "js" => "text/javascript",
                _ => "text/plain",
            };
        }

        public override void OnNotify(NotifyType eventType, INotifyArgs notifyArgs)
        {
            if (eventType != NotifyType.GameEnd)
                return;

            shouldRun = false;
            listener.Close();
        }
    }
}
