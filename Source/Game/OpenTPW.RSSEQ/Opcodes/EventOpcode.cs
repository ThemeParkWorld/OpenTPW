namespace OpenTPW.RSSEQ.Opcodes
{
    public class EventOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.EVENT, OpcodeID.EVENT_EXT };

        public override void Invoke(Operand[] args)
        {
            if (args.Length < 4)
            {
                // Event
            }
            else
            {
                // Event ext
            }
        }

        public override int MinArgs => 3;
        public override int MaxArgs => 4;
        public override string Description => "Trigger an in-game event.";

        public override string[] Args => new[] { "type", "unknown", "event" };
    }
}
