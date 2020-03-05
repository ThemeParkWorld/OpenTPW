using System;
using ECSEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenTPW.RSSEQ
{
    public class VM
    {
        private readonly RSSEQReader rsseqReader;
        private int currentPos;

        private readonly Dictionary<OpcodeID, OpcodeHandler> opcodeHandlers = new Dictionary<OpcodeID, OpcodeHandler>();

        private VMConfig config = new VMConfig();

        private VMFlags flags = new VMFlags();

        public bool Crit
        {
            get => flags.crit;
            set => flags.crit = value;
        }
        public bool Sign
        {
            get => flags.sign;
            set => flags.sign = value;
        }
        public bool Zero
        {
            get => flags.zero;
            set => flags.zero = value;
        }

        public int StackSize
        {
            get => config.stackSize;
            set => config.stackSize = value;
        }
        public int LimboSize
        {
            get => config.limboSize;
            set => config.limboSize = value;
        }
        public int TimeSlice
        {
            get => config.timeSlice;
            set => config.timeSlice = value;
        }
        public int BounceSize
        {
            get => config.bounceSize;
            set => config.bounceSize = value;
        }
        public int WalkSize
        {
            get => config.walkSize;
            set => config.walkSize = value;
        }

        public string ScriptName { get; set; } = "Unnamed";
        public List<Instruction> Instructions { get; } = new List<Instruction>();
        public List<string> Strings { get; } = new List<string>();
        public List<int> Variables { get; private set; }
        public string Disassembly => rsseqReader.Disassembly;
        public Dictionary<int, VMObject> Objects { get; set; }

        public VM(byte[] rseData)
        {
            rsseqReader = new RSSEQReader(this);
            rsseqReader.ReadFile(rseData);
            Variables = new List<int>(rsseqReader.VariableCount);
            RegisterOpcodeHandlers(); 
        }

        public VM(string rsePath)
        {
            rsseqReader = new RSSEQReader(this);
            rsseqReader.ReadFile(rsePath);
            Variables = new List<int>(rsseqReader.VariableCount);
            RegisterOpcodeHandlers();
        }

        private void RegisterOpcodeHandlers()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.BaseType == typeof(OpcodeHandler) && t != typeof(OpcodeHandler)))
            {
                var handler = (OpcodeHandler) Activator.CreateInstance(type);
                handler.vmInstance = this;
                opcodeHandlers.Add(handler.OpcodeId, handler);
            }
        }

        public void Step()
        {
            Debug.Log($"Current pos: {currentPos}");

            Instructions[currentPos++].Invoke();
        }

        public OpcodeHandler FindOpcodeHandler(OpcodeID opcode)
        {
            return opcodeHandlers[opcode];
        }
    }
}
