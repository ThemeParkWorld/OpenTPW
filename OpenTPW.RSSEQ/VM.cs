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

        public struct Config
        {
            public int stackSize, limboSize, bounceSize, walkSize, timeSlice;
        }

        public Config config;
        public string scriptName = "Unnamed";
        public List<Instruction> instructions { get; } = new List<Instruction>();
        public List<string> strings { get; } = new List<string>();
        public List<int> variables { get; private set; }
        public string disassembly => rsseqReader.disassembly;

        public VM(byte[] rseData)
        {
            rsseqReader = new RSSEQReader(this);
            rsseqReader.ReadFile(rseData);
            variables = new List<int>(rsseqReader.variableCount);
            RegisterOpcodeHandlers(); 
        }

        public VM(string rsePath)
        {
            rsseqReader = new RSSEQReader(this);
            rsseqReader.ReadFile(rsePath);
            variables = new List<int>(rsseqReader.variableCount);
            RegisterOpcodeHandlers();
        }

        private void RegisterOpcodeHandlers()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.BaseType == typeof(OpcodeHandler) && t != typeof(OpcodeHandler)))
            {
                var handler = (OpcodeHandler) Activator.CreateInstance(type);
                handler.vmInstance = this;
                opcodeHandlers.Add(handler.opcodeId, handler);
            }
        }

        public void Step()
        {
            Debug.Log($"Current pos: {currentPos}");

            instructions[currentPos++].Invoke();
        }

        public OpcodeHandler FindOpcodeHandler(OpcodeID opcode)
        {
            return opcodeHandlers[opcode];
        }
    }
}
