using System.Collections.Generic;

namespace Engine.Utils.DebugUtils.Rcon
{
    public class RconPacket
    {
        public RconPacketType type;

        public Dictionary<string, string> data;

        public RconPacket(RconPacketType type, Dictionary<string, string> data)
        {
            this.type = type;
            this.data = data;
        }
    }
}
