using OpenGL;
using OpenGL.CoreUI;
using Quincy.DebugUtils;
using System;
using System.Runtime.InteropServices;

namespace Quincy
{
    public class Quincy
    {
        #region Variables
        protected NativeWindow window;

        public bool isRunning = true;
        private Scene scene;
        private Gl.DebugProc debugProc;
        private QuincyImGui imgui;
        #endregion

        #region Methods
        public Quincy(string windowTitle, int windowResX, int windowResY)
        {
            window = NativeWindow.Create();

            window.Render += Render;
            window.Close += Closing;
            window.ContextCreated += ContextCreated;
            window.ContextDestroying += ContextDestroyed;

            window.Animation = false;
            window.CursorVisible = true;
            window.DepthBits = 24;
            window.SwapInterval = 0;
            window.MultisampleBits = 16;

            window.Create(128, 128, (uint)windowResX + 16, (uint)windowResY + 16, NativeWindowStyles.Caption | NativeWindowStyles.Border);
            window.Caption = windowTitle;
            // window.Fullscreen = true;

            window.Show();
            window.Run();
        }

        private void ContextCreated(object sender, NativeWindowEventArgs e)
        {
            debugProc = GlDebugCallback;
            Gl.DebugMessageCallback(debugProc, null);
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.Blend);
            Gl.Enable(EnableCap.CullFace);
            Gl.Enable(EnableCap.FramebufferSrgb);
            Gl.Enable((EnableCap)Gl.TEXTURE_CUBE_MAP_SEAMLESS);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            imgui = new QuincyImGui();
            scene = new Scene();
        }

        private void GlDebugCallback(DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            var messageStr = Marshal.PtrToStringAnsi(message, length);
            Logging.Log($"{type}: {id} {messageStr}");
        }

        public void RenderImGui() { }

        private void RenderToScreen()
        {
            scene.Render();
        }

        private void RenderToShadowMap()
        {
            scene.RenderShadows();
        }

        private void Render(object sender, NativeWindowEventArgs e)
        {
            RenderToShadowMap();
            RenderToScreen();
            // imgui.Render();
        }

        public void Close()
        {
            window.Stop();
            window.Destroy();
        }
        #endregion

        private void Closing(object sender, EventArgs e)
        {
            isRunning = false;
        }

        private void ContextDestroyed(object sender, NativeWindowEventArgs e)
        {
            isRunning = false;
        }
    }
}
