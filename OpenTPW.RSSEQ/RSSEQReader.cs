using Engine.Utils.DebugUtils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenTPW.RSSEQ
{
    // TODO: Move to OpenTPW.FileFormats
    public class RSSEQReader
    {
        private VM vmInstance;
        private List<string> variables = new List<string>();
        private int expectedInstructions;
        private long instructionOffset;

        public string Disassembly { get; private set; } = "";
        public int VariableCount => variables.Count;

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

            Logging.Log(Disassembly);
        }

        private void ReadFileHeader(BinaryReader binaryReader)
        {
            var magicNumber = binaryReader.ReadChars(8);
            if (!Enumerable.SequenceEqual(magicNumber, new[] { 'R', 'S', 'S', 'E', 'Q', (char)0x0F, (char)0x01, (char)0x00 }))
                Logging.Log("Magic number was not 'RSSEQ'", Logging.Severity.High);

            // Variable count
            var variableCount = binaryReader.ReadInt32();

            vmInstance.Variables = new List<int>(variableCount);
            vmInstance.Config.StackSize = binaryReader.ReadInt32();
            vmInstance.Config.TimeSlice = binaryReader.ReadInt32();
            vmInstance.Config.LimboSize = binaryReader.ReadInt32();
            vmInstance.Config.BounceSize = binaryReader.ReadInt32();
            vmInstance.Config.WalkSize = binaryReader.ReadInt32();

            for (var i = 0; i < 4; ++i)
            {
                var paddingChars = binaryReader.ReadChars(4);
                if (!Enumerable.SequenceEqual(paddingChars, new[] { 'P', 'a', 'd', ' ' }))
                    Logging.Log("Invalid padding!", Logging.Severity.High);
            }
        }

        private void ReadFileBody(BinaryReader binaryReader)
        {
            var currentOperands = new List<Operand>();
            var currentOpcode = 0;

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length - 1)
            {
                var currentValue = binaryReader.ReadInt32();
                var flag = (currentValue >> 24 & 0xFF);
                int truncValue = (short)currentValue;

                if ((binaryReader.BaseStream.Position - instructionOffset) / 4 >= expectedInstructions)
                {
                    Logging.Log($"Hit max count ({(binaryReader.BaseStream.Position - instructionOffset) / 4} of {expectedInstructions})");
                    vmInstance.Instructions.Add(new Instruction(vmInstance, (OpcodeID)currentOpcode, currentOperands.ToArray()));
                    break;
                }

                switch (flag)
                {
                    case 0x80:
                        // Opcode
                        vmInstance.Instructions.Add(new Instruction(vmInstance, (OpcodeID)currentOpcode, currentOperands.ToArray()));
                        currentOpcode = (short)currentValue;
                        currentOperands = new List<Operand>();
                        break;
                    case 0x10:
                        // String
                        currentOperands.Add(new Operand(vmInstance, Operand.Type.String, truncValue, truncValue));
                        break;
                    case 0x20:
                        // Branch
                        currentOperands.Add(new Operand(vmInstance, Operand.Type.Location, truncValue));
                        vmInstance.Branches.Add(new VMBranch(vmInstance.Instructions.Count - 2 /* ignore NOP and array starts at 0 */, truncValue));
                        break;
                    case 0x40:
                        // Variable
                        currentOperands.Add(new Operand(vmInstance, Operand.Type.Variable, truncValue, truncValue));
                        break;
                    case 0x00:
                        // Literal
                        currentOperands.Add(new Operand(vmInstance, Operand.Type.Literal, truncValue));
                        break;
                }
            }
        }

        private void ReadStringTable(BinaryReader binaryReader)
        {
            // First entry will be the strings used within the application
            var stringEntryLength = binaryReader.ReadInt32();
            var stringEntryPos = binaryReader.BaseStream.Position;
            var currentString = "";

            while (binaryReader.BaseStream.Position - stringEntryPos < stringEntryLength)
            {
                var currentChar = binaryReader.ReadChar();
                if (currentChar == '\0')
                {
                    vmInstance.Strings.Add(currentString);
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
                var variableNameLength = binaryReader.ReadInt32();
                var stringChars = binaryReader.ReadChars(variableNameLength);
                vmInstance.VariableNames.Add(new string(stringChars));
                vmInstance.Variables.Add(0);
                //vmInstance.Variables.Add(new string(stringChars).Replace("\0", ""));
            }
        }

        private void WriteDisassembly()
        {
            var currentCount = 1;

            for (var i = 0; i < vmInstance.Instructions.Count; ++i)
            {
                if (vmInstance.Branches.Any(b => b.compiledOffset == currentCount - 1))
                {
                    Disassembly += $".branch_{currentCount - 1}\n";
                }
                Disassembly += $"\t{vmInstance.Instructions[i]}\n";
                currentCount += vmInstance.Instructions[i].GetCount();
            }
        }
    }
}
