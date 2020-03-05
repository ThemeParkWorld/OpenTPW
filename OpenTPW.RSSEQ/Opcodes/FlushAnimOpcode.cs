namespace OpenTPW.RSSEQ.Opcodes
{
    public class FlushAnimOpcode : OpcodeHandler
    {
        public override OpcodeID OpcodeId => OpcodeID.FLUSHANIM;

        public override void Invoke(Operand[] args)
        {
            // Stop animations
        }

        public override int MinArgs => 0;
        public override int MaxArgs => 0;
        public override string Description => "Stop all active animations";
    }
}
