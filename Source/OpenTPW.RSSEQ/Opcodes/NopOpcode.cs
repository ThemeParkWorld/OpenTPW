namespace OpenTPW.RSSEQ.Opcodes
{
    public class NopOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.NOP };
        public override void Invoke(Operand[] args) { }

        public override int MinArgs => 0;
        public override int MaxArgs => 0;
        public override string Description => "No operation.";

        public override string[] Args => new string[0];
    }
}
