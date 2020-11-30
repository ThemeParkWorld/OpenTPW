namespace OpenTPW.RSSEQ.Opcodes
{
    public class WaitAnimChOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.WAITANIM_CH };

        public override void Invoke(Operand[] args)
        {

        }

        public override int MinArgs => 3;
        public override int MaxArgs => 3;
        public override string Description => "Unknown";

        public override string[] Args => new[] { "unknown", "unknown", "unknown" };
    }
}