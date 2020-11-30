namespace OpenTPW.RSSEQ.Opcodes
{
    public class FadeObjOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.FADEOBJ };

        public override void Invoke(Operand[] args)
        {
            // Fade object in args[0]
        }

        public override int MinArgs => 1;
        public override int MaxArgs => 1;
        public override string Description => "Fade out an object, and then remove it.";

        public override string[] Args => new[] { "slot" };
    }
}
