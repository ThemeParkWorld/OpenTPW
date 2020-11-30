using System.Runtime.InteropServices;

namespace OpenTPW.RSSEQ
{
    [StructLayout(LayoutKind.Auto)]
    public struct VMBranch
    {
        /// <summary>
        /// Instruction count (vm offset);
        /// </summary>
        public int instructionOffset { get; set; }

        /// <summary>
        /// Instruction count + opcode count (compiled value)
        /// </summary>
        public int compiledOffset { get; set; }

        public VMBranch(int instructionOffset, int compiledOffset)
        {
            this.instructionOffset = instructionOffset;
            this.compiledOffset = compiledOffset;
        }
    }
}
