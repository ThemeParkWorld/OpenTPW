namespace OpenTPW.RSSEQ.Opcodes
{
    public class SetLVOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new [] { OpcodeID.SETLV };

        public override void Invoke(Operand[] args)
        {

        }

        public override int MinArgs => 0;
        public override int MaxArgs => 0;
        public override string Description => "Unknown";
    }
}
