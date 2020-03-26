namespace OpenTPW.RSSEQ.Opcodes
{
    public class NameOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.NAME };

        public override int MinArgs => 1;
        public override int MaxArgs => 1;
        public override string Description => "Set the script's name.";

        public override void Invoke(Operand[] args)
        {
            // This is the only (known) instruction that actually uses a string
            vmInstance.ScriptName = vmInstance.Strings[args[0].Value];
        }
    }
}
