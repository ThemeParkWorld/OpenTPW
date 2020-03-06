namespace OpenTPW.RSSEQ.Opcodes
{
    public class KillObjOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new [] { OpcodeID.KILLOBJ };

        public override void Invoke(Operand[] args)
        {
            // Delete object in args[0]
            vmInstance.Objects.Remove(args[0].Value);
        }

        public override int MinArgs => 1;
        public override int MaxArgs => 1;
        public override string Description => "Remove an object of a specific slot type.";
    }
}
