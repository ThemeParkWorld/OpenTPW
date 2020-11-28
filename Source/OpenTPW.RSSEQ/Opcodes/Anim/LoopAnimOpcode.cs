namespace OpenTPW.RSSEQ.Opcodes
{
    public class LoopAnimOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.LOOPANIM };

        public override void Invoke(Operand[] args)
        {

        }

        public override int MinArgs => 2;
        public override int MaxArgs => 2;
        public override string Description => "Start playing an animation on loop";

        public override string[] Args => new[] { "type", "unknown" };
    }
}