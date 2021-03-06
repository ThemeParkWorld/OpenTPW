﻿namespace OpenTPW.RSSEQ.Opcodes
{
    public class BranchPVOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.BRANCH_PV };

        public override void Invoke(Operand[] args)
        {
            if (vmInstance.Flags.Sign)
                vmInstance.BranchTo(args[0].Value);
        }

        public override int MinArgs => 0;
        public override int MaxArgs => 0;
        public override string Description => "Branch to another location if the 'sign' flag has a value of one.";

        public override string[] Args => new[] { "location" };
    }
}