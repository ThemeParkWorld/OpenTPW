namespace OpenTPW.RSSEQ.Opcodes
{
    public class CopyOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new [] { OpcodeID.COPY };

        public override void Invoke(Operand[] args)
        {
            args[0].Value = args[1].Value;
        }

        public override int MinArgs => 2;
        public override int MaxArgs => 2;
        public override string Description => "Copy a value from one variable to another.";
    }
}
