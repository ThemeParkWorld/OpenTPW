using System;

namespace OpenTPW.RSSEQ.Opcodes
{
    public class RandOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.RAND };
        private Random random = new Random();

        public override void Invoke(Operand[] args)
        {
            vmInstance.Variables[args[0].Value] = random.Next(0, args[1].Value);
        }

        public override int MinArgs => 2;
        public override int MaxArgs => 2;
        public override string Description => "Generate a random number";

        public override string[] Args => new[] { "dest", "max value" };
    }
}