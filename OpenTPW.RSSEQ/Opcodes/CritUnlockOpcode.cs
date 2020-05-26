namespace OpenTPW.RSSEQ.Opcodes
{
    public class CritUnlockOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.CRIT_UNLOCK };

        public override void Invoke(Operand[] args)
        {
            vmInstance.Flags.Crit = false;
        }

        public override int MinArgs => 0;
        public override int MaxArgs => 0;
        public override string Description => "Unlock the ride, allowing for visitors to use it.";

        public override string[] Args => new string[0];
    }
}
