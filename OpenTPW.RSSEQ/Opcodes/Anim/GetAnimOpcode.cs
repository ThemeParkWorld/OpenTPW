namespace OpenTPW.RSSEQ.Opcodes
{
    public class GetAnimOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.GETANIM };

        public override void Invoke(Operand[] args)
        {

        }

        public override int MinArgs => 1;
        public override int MaxArgs => 1;
        public override string Description => "Unknown";

        public override string[] Args => new[] { "unknown" };
    }
}