namespace OpenTPW.RSSEQ.Opcodes
{
    public class TrigAnimSpeedOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.TRIGANIMSPEED };

        public override void Invoke(Operand[] args)
        {

        }

        public override int MinArgs => 4;
        public override int MaxArgs => 4;
        public override string Description => "Unknown";

        public override string[] Args => new[] { "unknown", "unknown", "unknown", "unknown" };
    }
}