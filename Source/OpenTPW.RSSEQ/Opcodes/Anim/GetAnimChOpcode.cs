namespace OpenTPW.RSSEQ.Opcodes
{
    public class GetAnimChOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.GETANIM_CH };

        public override void Invoke(Operand[] args)
        {

        }

        public override int MinArgs => 2;
        public override int MaxArgs => 2;
        public override string Description => "Unknown";

        public override string[] Args => new[] { "unknown", "unknown" };
    }
}