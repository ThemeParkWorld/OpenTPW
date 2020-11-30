namespace OpenTPW.RSSEQ.Opcodes
{
    public class TrigWaitAnimOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.TRIGWAITANIM };

        public override void Invoke(Operand[] args)
        {

        }

        public override int MinArgs => 3;
        public override int MaxArgs => 3;
        public override string Description => "Unknown";

        public override string[] Args => new[] { "unknown", "unknown", "unknown" };
    }
}