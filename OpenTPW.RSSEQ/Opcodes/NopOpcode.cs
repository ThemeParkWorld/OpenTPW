namespace OpenTPW.RSSEQ.Opcodes
{
    public class NopOpcode : OpcodeHandler
    {
        public override OpcodeID opcodeId => OpcodeID.NOP;
        public override void Invoke(Operand[] args) { }

        public override int minArgs => 0;
        public override int maxArgs => 0;
        public override string description => "No operation.";
    }
}
