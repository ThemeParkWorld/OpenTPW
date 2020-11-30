namespace OpenTPW.RSSEQ.Opcodes
{
    public class JsrOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.JSR };

        public override void Invoke(Operand[] args)
        {

        }

        public override int MinArgs => 1;
        public override int MaxArgs => 1;
        public override string Description => "Jump to a subroutine";

        public override string[] Args => new[] { "subroutine" };
    }
}