namespace OpenTPW.RSSEQ.Opcodes
{
    class CritLock : OpcodeHandler
    {
        public override OpcodeID opcodeId => OpcodeID.CRIT_LOCK;
        public override void Invoke(Operand[] args) { }

        public override int minArgs => 0;
        public override int maxArgs => 0;
        public override string description => "Lock the ride, preventing visitors from using it.";
    }
}
