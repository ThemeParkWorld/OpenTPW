using System;

namespace OpenTPW.RSSEQ
{
    public struct Instruction
    {
        public readonly Operand[] operands;
        public readonly OpcodeID opcode;

        private VM vmInstance;

        public Instruction(VM vmInstance, OpcodeID opcode, Operand[] operands)
        {
            this.operands = operands;
            this.opcode = opcode;

            this.vmInstance = vmInstance;
        }

        public int GetCount()
        {
            return 1 + operands.Length;
        }

        public void Invoke() => vmInstance.FindOpcodeHandler(opcode).Invoke(operands);

        public override string ToString()
        {
            string operandString = "";
            for (int i = 0; i < operands.Length; ++i)
            {
                operandString += operands[i];
                if (i != operands.Length - 1) operandString += "\t";
            }

            string padding = "\t";
            string opcodeName = ((OpcodeID)opcode).ToString();
            if (opcodeName.Length < 8)
                padding += "\t";

            return $"{opcode}{padding}{operandString}";
        }
    }
}
