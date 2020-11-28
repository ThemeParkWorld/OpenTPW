namespace OpenTPW.RSSEQ.Opcodes
{
    public class CmpOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.CMP };

        public override void Invoke(Operand[] args)
        {

        }

        public override int MinArgs => 0;
        public override int MaxArgs => 0;
        public override string Description => "Unknown";
        public override string[] Args => new[] { "value / variable", "value / variable" };
    }
}