namespace OpenTPW.RSSEQ.Opcodes
{
    public class WaitAnimOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.WAITANIM };

        public override void Invoke(Operand[] args)
        {

        }

        public override int MinArgs => 2;
        public override int MaxArgs => 2;
        public override string Description => "Start playing an animation, and wait for it to end before continuing.";

        public override string[] Args => new[] { "type", "unknown" };
    }
}
