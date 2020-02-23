namespace OpenTPW.RSSEQ.Opcodes
{
    public class CritUnlock : OpcodeHandler
    {
        public override OpcodeID opcodeId => OpcodeID.CRIT_UNLOCK;
        public override void Invoke(Operand[] args) { }

        public override int minArgs => 0;
        public override int maxArgs => 0;
        public override string description => "Unlock the ride, allowing for visitors to use it.";
    }
}
