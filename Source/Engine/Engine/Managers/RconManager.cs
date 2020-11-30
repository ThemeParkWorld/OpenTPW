using Engine.ECS.Managers;
using Engine.Utils;
using Engine.Utils.DebugUtils;
using Engine.Utils.DebugUtils.Rcon;
using Fleck;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Managers
{
    // TODO: either optimize this or scrap it
    //          - protobufs?
    // TODO: switch to more extensible (i.e. use class-based solution)
    public sealed class RconManager : Manager<RconManager>
    {
        #region Fields
        private bool connected;
        private bool authenticated;
        private IWebSocketConnection localSocket;
        #endregion

        #region Constructor
        public RconManager()
        {
            // WARNING! DO NOT LOGGING.LOG IN HERE!

            if (!GameSettings.RconEnabled)
                return;

            FleckLog.LogAction = CustomFleckLog;

            var socketServer = new WebSocketServer($"ws://0.0.0.0:{GameSettings.RconPort}")
            {
                SupportedSubProtocols = new[] { "engineRcon" },
                ListenerSocket =
                {
                    NoDelay = true
                }
            };

            Logging.onDebugLog += SendDebugLog;

            socketServer.Start(InitConnection);
        }
        #endregion

        #region Fleck / Websocket Events
        private void CustomFleckLog(LogLevel level, string message, Exception ex)
        {
            if (level != LogLevel.Error) // We don't care about anything that isn't an error (currently).
                return;

            var logSeverity = Logging.Severity.Low;
            switch (level)
            {
                case LogLevel.Warn:
                    logSeverity = Logging.Severity.Medium;
                    break;
                case LogLevel.Error:
                    logSeverity = Logging.Severity.High;
                    break;
            }

            Logging.Log(message, logSeverity);
            if (ex != null)
                Logging.Log(ex.ToString(), Logging.Severity.High);
        }

        private void InitConnection(IWebSocketConnection socket)
        {
            localSocket = socket;
            socket.OnOpen = OnOpen;
            socket.OnClose = OnClose;
            socket.OnMessage = OnMessage;
        }

        private void OnOpen()
        {
            Logging.Log($"Remote console connection started: {localSocket.ConnectionInfo.ClientIpAddress}:{localSocket.ConnectionInfo.ClientPort}");
            connected = true;
        }

        private void OnClose()
        {
            Logging.Log("Remote console connection closed");
            connected = false;
        }

        private void OnMessage(string message)
        {
            var rconPacket = JsonConvert.DeserializeObject<RconPacket>(message);

            switch (rconPacket.type)
            {
                case RconPacketType.Handshake:
                    HandleHandshake(rconPacket);
                    break;
                case RconPacketType.Authenticate:
                    HandleAuthentication(rconPacket);
                    break;
                default:
                    if (!authenticated)
                    {
                        SendPacket(RconPacketType.RequestAuth, new Dictionary<string, string>());
                        return;
                    }

                    break;
            }

            switch (rconPacket.type)
            {
                case RconPacketType.Input:
                    HandleInput(rconPacket);
                    break;
                case RconPacketType.InputInProgress:
                    HandleInputInProgress(rconPacket);
                    break;
                case RconPacketType.RequestLogHistory:
                    HandleRequestLogHistory(rconPacket);
                    break;
            }
        }
        #endregion

        #region Packet Handlers
        private void HandleHandshake(RconPacket rconPacket)
        {
            Logging.Log("Received handshake from client.");
            if (string.IsNullOrEmpty(GameSettings.RconPassword))
            {
                Logging.Log("Rcon authentication is disabled! Please enter a password in GameSettings if this is incorrect", Logging.Severity.Medium);

                if (localSocket.ConnectionInfo.ClientIpAddress == "127.0.0.1")
                {
                    authenticated = true;
                }
                else
                {
                    Logging.Log("Unauthorized rcon connection attempt from non-local machine was blocked; set a password", Logging.Severity.Medium);
                    return;
                }
            }
        }

        private void HandleAuthentication(RconPacket rconPacket)
        {
            if (rconPacket.data["password"] == GameSettings.RconPassword)
            {
                authenticated = true;
                SendLogHistory();
            }
            else
            {
                Logging.Log("Rcon password was incorrect");
                SendPacket(RconPacketType.Error, new Dictionary<string, string>()
                {
                    { "errorMessage", "Password incorrect :(" }
                });
                localSocket.Close();
            }
        }

        private void HandleInput(RconPacket rconPacket)
        {
            Logging.Log($"Received input {rconPacket.data["input"]}");
        }

        private void HandleInputInProgress(RconPacket rconPacket)
        {
            SendSuggestions(rconPacket.data["input"]);
        }

        private void HandleRequestLogHistory(RconPacket rconPacket)
        {
            SendLogHistory();
        }
        #endregion

        #region Packet Senders
        private void SendPacket(RconPacketType type, Dictionary<string, string> data)
        {
            var str = JsonConvert.SerializeObject(new RconPacket(type, data));
            var bytes = Encoding.UTF8.GetBytes(str);
            localSocket.Send(bytes);
        }

        private void SendLogHistory(int offset = 0)
        {
            var logEntries = new List<Dictionary<string, string>>();
            int count = 0;
            int maxChunkSize = 3;
            bool limitWasReached = false;
            for (int i = offset; i < Logging.LogEntries.Count; ++i)
            {
                var logEntry = Logging.LogEntries[i];
                logEntries.Add(GetResultDictionary(logEntry));
                count++;
                if (count >= maxChunkSize)
                {
                    limitWasReached = true;
                    break;
                }
            }

            SendPacket(RconPacketType.LogHistory, new Dictionary<string, string>()
            {
                { "entries", JsonConvert.SerializeObject(logEntries) }
            });

            if (limitWasReached)
            {
                SendLogHistory(offset + count);
            }
        }

        private void SendSuggestions(string input)
        {
            var suggestions = CommandRegistry.GiveSuggestions(input);

            if (suggestions.Count == 0)
            {
                SendPacket(RconPacketType.Suggestions, new Dictionary<string, string>() { });
            }
            else
            {
                SendPacket(RconPacketType.Suggestions, new Dictionary<string, string>()
                {
                    { "suggestions", JsonConvert.SerializeObject(suggestions) }
                });
            }
        }

        public void SendDebugLog(LogEntry logEntry)
        {
            if (connected && authenticated)
            {
                SendPacket(RconPacketType.Response, GetResultDictionary(logEntry));
            }
        }

        private Dictionary<string, string> GetResultDictionary(LogEntry logEntry)
        {
            return new Dictionary<string, string>()
            {
                {"timestamp", logEntry.timestamp.ToLongTimeString()},
                {"stackTrace", logEntry.stackTrace.ToString()},
                {"str", logEntry.str},
                {"severity", logEntry.ToString().ToLower()}
            };
        }
        #endregion
    }
}
