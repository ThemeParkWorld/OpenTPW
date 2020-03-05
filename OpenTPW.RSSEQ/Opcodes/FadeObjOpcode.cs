namespace OpenTPW.RSSEQ.Opcodes
{
    public class FadeObjOpcode : OpcodeHandler
    {
        public override OpcodeID OpcodeId => OpcodeID.FADEOBJ;

        public override void Invoke(Operand[] args)
        {
            // Fade object in args[0]
        }

        public override int MinArgs => 0;
        public override int MaxArgs => 0;
        public override string Description => "Unknown";
    }
}
