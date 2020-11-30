namespace OpenTPW.RSSEQ.Opcodes
{
    public class BranchOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.BRANCH };

        public override void Invoke(Operand[] args)
        {
            vmInstance.BranchTo(args[0].Value);
        }

        public override int MinArgs => 1;
        public override int MaxArgs => 1;
        public override string Description => "Branch to another location";

        public override string[] Args => new[] { "location" };
    }
}