namespace OpenTPW.RSSEQ.Opcodes
{
    public class EndSliceOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.ENDSLICE };

        public override void Invoke(Operand[] args)
        {
            // End slice?
        }

        public override int MinArgs => 0;
        public override int MaxArgs => 0;
        public override string Description => "Unknown";
    }
}
