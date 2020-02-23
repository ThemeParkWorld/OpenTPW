namespace OpenTPW.RSSEQ.Opcodes
{
    public class NameOpcode : OpcodeHandler
    {
        public override OpcodeID opcodeId => OpcodeID.NAME;

        public override int minArgs => 1;
        public override int maxArgs => 1;
        public override string description => "Set the script's name.";

        public override void Invoke(Operand[] args)
        {
            // This is the only (known) instruction that actually uses a string
            vmInstance.scriptName = vmInstance.strings[args[0].value];
        }
    }
}
