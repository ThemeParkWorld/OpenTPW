namespace OpenTPW.RSSEQ.Opcodes
{
    public class ReturnOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.RETURN };

        public override void Invoke(Operand[] args)
        {

        }

        public override int MinArgs => 0;
        public override int MaxArgs => 0;
        public override string Description => "Return to the previous subroutine / position (after the last JSR)";

        public override string[] Args => new string[0];
    }
}