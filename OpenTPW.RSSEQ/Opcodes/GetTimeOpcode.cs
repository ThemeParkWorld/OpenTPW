namespace OpenTPW.RSSEQ.Opcodes
{
    public class GetTimeOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.GETTIME };

        public override void Invoke(Operand[] args)
        {
            // Get VM alive time (in slices)
            args[1].Value = 0;
        }

        public override int MinArgs => 1;
        public override int MaxArgs => 1;
        public override string Description => "Gets the time that the ride has been alive for.";

        public override string[] Args => new[] { "dest" };
    }
}
