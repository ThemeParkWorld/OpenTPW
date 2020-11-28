namespace OpenTPW.RSSEQ.Opcodes
{
    public class FlushAnimOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.FLUSHANIM };

        public override void Invoke(Operand[] args)
        {
            // Stop animations
        }

        public override int MinArgs => 0;
        public override int MaxArgs => 0;
        public override string Description => "Stop all active animations";

        public override string[] Args => new string[0];
    }
}
