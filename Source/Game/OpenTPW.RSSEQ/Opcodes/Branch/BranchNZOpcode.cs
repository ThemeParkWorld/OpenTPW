namespace OpenTPW.RSSEQ.Opcodes
{
    public class BranchNZOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.BRANCH_NZ };

        public override void Invoke(Operand[] args)
        {
            if (!vmInstance.Flags.Zero)
                vmInstance.BranchTo(args[0].Value);
        }
        
        public override int MinArgs => 1;
        public override int MaxArgs => 1;
        public override string Description => "Branch to another location if the 'zero' flag has a value of zero.";

        public override string[] Args => new[] { "location" };
    }
}