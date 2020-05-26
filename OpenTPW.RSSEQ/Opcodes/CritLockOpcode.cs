namespace OpenTPW.RSSEQ.Opcodes
{
    class CritLockOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.CRIT_LOCK };

        public override void Invoke(Operand[] args)
        {
            vmInstance.Flags.Crit = true;
        }

        public override int MinArgs => 0;
        public override int MaxArgs => 0;
        public override string Description => "Lock the ride, preventing visitors from using it.";

        public override string[] Args => new string[0];
    }
}
