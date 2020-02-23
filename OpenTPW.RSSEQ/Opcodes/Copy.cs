namespace OpenTPW.RSSEQ.Opcodes
{
    public class Copy : OpcodeHandler
    {
        public override OpcodeID opcodeId => OpcodeID.COPY;

        public override void Invoke(Operand[] args) { }

        public override int minArgs => 2;
        public override int maxArgs => 2;
        public override string description => "Copies data from a specific location.";
    }
}
