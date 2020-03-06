﻿using System;

namespace OpenTPW.RSSEQ.Opcodes
{
    public class AddObjOpcode : OpcodeHandler
    {
        public override OpcodeID[] OpcodeIds => new[] { OpcodeID.ADDOBJ, OpcodeID.ADDOBJ_EXT };

        public override void Invoke(Operand[] args)
        {
            if (args.Length == 4)
            {
                // non-EXT
                // <type> <slot> <id> <quantity>
                vmInstance.Objects.Add(args[1].Value, new VMObject(args[0].Value, args[2].Value));
            }
            else
            {
                // EXT
                throw new NotImplementedException();
            }
        }

        public override int MinArgs => 4;
        public override int MaxArgs => 5;
        public override string Description => "Add an object of a specific type to the ride.";
    }
}
