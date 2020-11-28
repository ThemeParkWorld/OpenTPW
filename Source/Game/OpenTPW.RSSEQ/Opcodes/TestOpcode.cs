using System;

namespace OpenTPW.RSSEQ.Opcodes
{
    public class TestOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.TEST };

        public override void Invoke(Operand[] args)
        {
            vmInstance.Flags.Sign = Math.Sign(args[0].Value) >= 0;
            vmInstance.Flags.Zero = args[0].Value == 0;
        }

        public override int MinArgs => 1;
        public override int MaxArgs => 1;
        public override string Description => "Set flags depending on the value given.";

        public override string[] Args => new[] { "value" };
    }
}