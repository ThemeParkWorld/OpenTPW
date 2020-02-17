using System;

namespace OpenTPW.RSSEQ
{
    public struct Instruction
    {
        public readonly string[] operands;
        public readonly int opcode;

        public Instruction(int opcode, string[] operands)
        {
            this.operands = operands;
            this.opcode = opcode;
        }

        public int GetCount()
        {
            return 1 + operands.Length;
        }

        public override string ToString()
        {
            string operandString = "";
            for (int i = 0; i < operands.Length; ++i)
            {
                operandString += operands[i];
                if (i != operands.Length - 1) operandString += "\t";
            }

            // See if we can get the opcode name from the opcode enum
            if (Enum.IsDefined(typeof(Opcode), opcode))
            {
                string padding = "\t";
                string opcodeName = ((Opcode)opcode).ToString();
                if (opcodeName.Length < 8)
                    padding += "\t";

                return $"{(Opcode)opcode} {operandString}";
            }
            else
            {
                return $"Unknown opcode {opcode}: {operandString}";
            }
        }
    }
}
