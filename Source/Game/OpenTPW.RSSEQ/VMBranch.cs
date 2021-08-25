using System.Runtime.InteropServices;

namespace OpenTPW.RSSEQ
{
    [StructLayout(LayoutKind.Auto)]
    public struct VMBranch
    {
        /// <summary>
        /// Instruction count (vm offset);
        /// </summary>
        public int InstructionOffset { get; set; }

        /// <summary>
        /// Instruction count + opcode count (compiled value)
        /// </summary>
        public int CompiledOffset { get; set; }

        public VMBranch(int instructionOffset, int compiledOffset)
        {
            this.InstructionOffset = instructionOffset;
            this.CompiledOffset = compiledOffset;
        }
    }
}
