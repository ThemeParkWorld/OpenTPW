using ECSEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenTPW.RSSEQ
{
    public class RSSEQReader
    {
        private VM vmInstance;
        private List<int> branches = new List<int>();
        private List<string> variables = new List<string>();
        private int expectedInstructions;
        private long instructionOffset;
        
        public string disassembly { get; private set; } = "";
        public int variableCount => variables.Count;

        public RSSEQReader(VM vmInstance)
        {
            this.vmInstance = vmInstance;
        }

        public void ReadFile(byte[] rseData)
        {
            using var memoryStream = new MemoryStream(rseData);
            using var binaryReader = new BinaryReader(memoryStream);

            ReadFileContents(binaryReader);
        }

        public void ReadFile(string rsePath)
        {
            using var fileStream = new FileStream(rsePath, FileMode.Open);
            using var binaryReader = new BinaryReader(fileStream);

            ReadFileContents(binaryReader);
        }

        private void ReadFileContents(BinaryReader binaryReader)
        {
            ReadFileHeader(binaryReader);

            // First 4 bytes are # of expected opcodes & operands
            expectedInstructions = binaryReader.ReadInt32();

            // Read string table
            instructionOffset = binaryReader.BaseStream.Position;

            // Forward to string table
            binaryReader.BaseStream.Seek((expectedInstructions) * 4, SeekOrigin.Current);

            ReadStringTable(binaryReader);

            // Go back to instructions
            binaryReader.BaseStream.Seek(instructionOffset, SeekOrigin.Begin);

            ReadFileBody(binaryReader);
            WriteDisassembly();
        }

        private void ReadFileHeader(BinaryReader binaryReader)
        {
            char[] magicNumber = binaryReader.ReadChars(8);
            if (!Enumerable.SequenceEqual(magicNumber, new[] { 'R', 'S', 'S', 'E', 'Q', (char)0x0F, (char)0x01, (char)0x00 }))
                Debug.Log("Magic number was not 'RSSEQ'", Debug.DebugSeverity.High);

            // Variable count
            var variableCount = binaryReader.ReadInt32();

            vmInstance.config.stackSize = binaryReader.ReadInt32();
            vmInstance.config.timeSlice = binaryReader.ReadInt32();
            vmInstance.config.limboSize = binaryReader.ReadInt32();
            vmInstance.config.bounceSize = binaryReader.ReadInt32();
            vmInstance.config.walkSize = binaryReader.ReadInt32();

            for (int i = 0; i < 4; ++i)
            {
                char[] paddingChars = binaryReader.ReadChars(4);
                if (!Enumerable.SequenceEqual(paddingChars, new[] { 'P', 'a', 'd', ' ' }))
                    Debug.Log("Invalid padding!", Debug.DebugSeverity.High);
            }
        }

        private void ReadFileBody(BinaryReader binaryReader)
        {
            List<Operand> currentOperands = new List<Operand>();

            int currentOpcode = 0;

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length - 1)
            {
                int currentValue = binaryReader.ReadInt32();
                int flag = (currentValue >> 24 & 0xFF);
                int truncValue = (short)currentValue;

                if ((binaryReader.BaseStream.Position - instructionOffset) / 4 >= expectedInstructions)
                {
                    Debug.Log($"Hit max count ({(binaryReader.BaseStream.Position - instructionOffset) / 4} of {expectedInstructions})");
                    vmInstance.instructions.Add(new Instruction(vmInstance, (OpcodeID)currentOpcode, currentOperands.ToArray()));
                    break;
                }

                switch (flag)
                {
                    case 0x80:
                        // Opcode
                        vmInstance.instructions.Add(new Instruction(vmInstance, (OpcodeID)currentOpcode, currentOperands.ToArray()));
                        currentOpcode = (short)currentValue;
                        currentOperands = new List<Operand>();
                        break;
                    case 0x10:
                        // String
                        // currentOperands.Add($"\"{vmInstance.strings[truncValue].Replace("\0", "")}\"");
                        currentOperands.Add(new Operand(Operand.Type.String, truncValue));
                        break;
                    case 0x20:
                        // Branch
                        // currentOperands.Add($"branch_{truncValue}");
                        currentOperands.Add(new Operand(Operand.Type.Location, truncValue));
                        branches.Add(truncValue);
                        break;
                    case 0x40:
                        // Variable
                        // currentOperands.Add(variables[truncValue]);
                        currentOperands.Add(new Operand(Operand.Type.Variable, truncValue));
                        break;
                    case 0x00:
                        // Literal
                        // currentOperands.Add(truncValue.ToString());
                        currentOperands.Add(new Operand(Operand.Type.Literal, truncValue));
                        break;
                }
            }
        }

        private void ReadStringTable(BinaryReader binaryReader)
        {
            // First entry will be the strings used within the application
            int stringEntryLength = binaryReader.ReadInt32();
            long stringEntryPos = binaryReader.BaseStream.Position;
            string currentString = "";

            while (binaryReader.BaseStream.Position - stringEntryPos < stringEntryLength)
            {
                char currentChar = binaryReader.ReadChar();
                if (currentChar == '\0')
                {
                    vmInstance.strings.Add(currentString);
                    currentString = "";
                }
                else
                {
                    currentString += currentChar;
                }
            }

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
            {
                // Read remaining variables
                int variableNameLength = binaryReader.ReadInt32();
                char[] stringChars = binaryReader.ReadChars(variableNameLength);
                variables.Add(new string(stringChars).Replace("\0", ""));
            }
        }

        private void WriteDisassembly()
        {
            int currentCount = 1;

            for (int i = 0; i < vmInstance.instructions.Count; ++i)
            {
                if (branches.Contains(currentCount - 1))
                {
                    disassembly += $".branch_{currentCount - 1}\n";
                }
                disassembly += $"\t{vmInstance.instructions[i].ToString()}\n";
                currentCount += vmInstance.instructions[i].GetCount();
            }
        }
    }
}
