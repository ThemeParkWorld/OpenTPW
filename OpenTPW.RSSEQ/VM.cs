using ECSEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenTPW.RSSEQ
{
    public class VM
    {
        private int stackSize, limboSize, bounceSize, walkSize, timeSlice;
        private List<int> branches = new List<int>();
        private List<string> strings = new List<string>();
        private List<string> variables;
        private List<Instruction> instructions = new List<Instruction>();

        public VM(byte[] rseData)
        {
            using var memoryStream = new MemoryStream(rseData);
            using var binaryReader = new BinaryReader(memoryStream);

            ReadRSEFile(binaryReader);
        }

        public VM(string rsePath)
        {
            using var fileStream = new FileStream(rsePath, FileMode.Open);
            using var binaryReader = new BinaryReader(fileStream);

            ReadRSEFile(binaryReader);
        }

        private void ReadRSEFile(BinaryReader binaryReader)
        {
            ReadFileHeader(binaryReader);
            ReadFileBody(binaryReader);
            LogDisassembly();
        }

        private void ReadFileHeader(BinaryReader binaryReader)
        {
            char[] magicNumber = binaryReader.ReadChars(8);
            if (!Enumerable.SequenceEqual(magicNumber, new[] { 'R', 'S', 'S', 'E', 'Q', (char)0x0F, (char)0x01, (char)0x00 }))
                Debug.Log("Magic number was not 'RSSEQ'", Debug.DebugSeverity.High);

            int variableCount = binaryReader.ReadInt32();
            stackSize = binaryReader.ReadInt32();
            timeSlice = binaryReader.ReadInt32();
            limboSize = binaryReader.ReadInt32();
            bounceSize = binaryReader.ReadInt32();
            walkSize = binaryReader.ReadInt32();

            for (int i = 0; i < 4; ++i)
            {
                char[] paddingChars = binaryReader.ReadChars(4);
                if (!Enumerable.SequenceEqual(paddingChars, new[] { 'P', 'a', 'd', ' ' }))
                    Debug.Log("Invalid padding!", Debug.DebugSeverity.High);
            }
        }

        private void ReadFileBody(BinaryReader binaryReader)
        {
            List<string> currentOperands = new List<string>();

            int currentOpcode = 0;

            // First 4 bytes are # of expected opcodes & operands
            int expectedInstructions = binaryReader.ReadInt32();

            // Ignore first "NOP" instruction
            binaryReader.ReadBytes(4);

            // Read string table
            long instructionOffset = binaryReader.BaseStream.Position;

            // FF to string table
            binaryReader.BaseStream.Seek((expectedInstructions - 1) * 4, SeekOrigin.Current); // -1 offset is due to "NOP" instruction being ignored
            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
            {
                // Read string
                int stringLength = binaryReader.ReadInt32();
                char[] stringChars = binaryReader.ReadChars(stringLength);
                strings.Add(new string(stringChars));
            }

            // Go back to instructions
            binaryReader.BaseStream.Seek(instructionOffset, SeekOrigin.Begin);

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length - 1)
            {
                int currentValue = binaryReader.ReadInt32();
                int flag = (currentValue >> 24 & 0xFF);
                int truncValue = (short)currentValue;

                if ((binaryReader.BaseStream.Position - instructionOffset) / 4 >= expectedInstructions)
                {
                    Debug.Log($"Hit max count ({(binaryReader.BaseStream.Position - instructionOffset) / 4} of {expectedInstructions})");
                    instructions.Add(new Instruction(currentOpcode, currentOperands.ToArray()));
                    break;
                }

                switch (flag)
                {
                    case 0x80:
                        // Opcode
                        instructions.Add(new Instruction(currentOpcode, currentOperands.ToArray()));
                        currentOpcode = (short)currentValue;
                        currentOperands = new List<string>();
                        break;
                    case 0x10:
                        // String
                        currentOperands.Add($"\"{strings[truncValue].Replace("\0", "")}\"");
                        break;
                    case 0x20:
                        // Branch
                        // currentOperands.Add(branches[truncValue]);
                        currentOperands.Add($"branch_{truncValue}");
                        branches.Add(truncValue);
                        break;
                    case 0x40:
                        // Variable
                        // currentOperands.Add(variables[truncValue]);
                        currentOperands.Add($"VAR_{truncValue}");
                        break;
                    case 0x00:
                        // Literal
                        currentOperands.Add(truncValue.ToString());
                        break;
                }
            }
        }

        private void LogDisassembly()
        {
            int currentCount = 1;
            for (int i = 0; i < instructions.Count; ++i)
            {
                if (branches.Contains(currentCount - 1))
                {
                    Debug.Log($".branch_{currentCount - 1}");
                }
                Debug.Log($"\t{instructions[i].ToString()}");
                Console.ForegroundColor = ConsoleColor.Gray;
                currentCount += instructions[i].GetCount();
            }
        }
    }
}
