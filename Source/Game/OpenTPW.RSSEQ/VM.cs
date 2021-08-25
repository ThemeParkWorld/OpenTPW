using Engine.Utils.Attributes;
using Engine.Utils.DebugUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenTPW.RSSEQ
{
    public class VM
    {
        private readonly RSSEQReader rsseqReader;
        private readonly Dictionary<OpcodeID[], OpcodeHandler> opcodeHandlers = new Dictionary<OpcodeID[], OpcodeHandler>();

        public string ScriptName { get; set; } = "Unnamed";

        public bool Run { get; set; }
        [HideInImGui] public int CurrentPos { get; set; }
        [HideInImGui] public string Disassembly => rsseqReader.Disassembly;
        [HideInImGui] public List<Instruction> Instructions { get; } = new List<Instruction>();
        
        /// <summary>
        /// Key: String offset
        /// Value: String value
        /// </summary>
        [HideInImGui] public Dictionary<long, string> Strings { get; } = new Dictionary<long, string>();
        [HideInImGui] public List<int> Variables { get; set; } = new List<int>();
        [HideInImGui] public List<string> VariableNames { get; set; } = new List<string>();
        [HideInImGui] public Dictionary<int, VMObject> Objects { get; set; } = new Dictionary<int, VMObject>();
        [HideInImGui] public VMFlags Flags { get; private set; } = new VMFlags();
        [HideInImGui] public VMConfig Config { get; private set; } = new VMConfig();
        [HideInImGui] public List<VMBranch> Branches { get; set; } = new List<VMBranch>();

        public VM(byte[] rseData)
        {
            rsseqReader = new RSSEQReader(this);
            rsseqReader.ReadFile(rseData);
            RegisterOpcodeHandlers();
        }

        public VM(string rsePath)
        {
            rsseqReader = new RSSEQReader(this);
            rsseqReader.ReadFile(rsePath);
            RegisterOpcodeHandlers();
        }

        private void RegisterOpcodeHandlers()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.BaseType == typeof(OpcodeHandler) && t != typeof(OpcodeHandler)))
            {
                var handler = (OpcodeHandler)Activator.CreateInstance(type);
                handler.vmInstance = this;
                opcodeHandlers.Add(handler.OpcodeIds, handler);
            }
        }

        public void Step()
        {
            var instruction = Instructions[CurrentPos++];
            Logging.Log($"Invoking {instruction.opcode} at position {CurrentPos}");

            instruction.Invoke();
        }

        public OpcodeHandler FindOpcodeHandler(OpcodeID opcodeId)
        {
            // TODO: Optimize
            var handler = (opcodeHandlers.FirstOrDefault(opcodeHandler => opcodeHandler.Key.Contains(opcodeId))).Value;
            if (handler == null)
                Logging.Log($"Opcode ID {opcodeId} has no appropriate handler");
            return handler;
        }

        public void BranchTo(int value)
        {
            var destBranch = Branches.First(b => b.CompiledOffset == value);
            CurrentPos = destBranch.InstructionOffset;
        }
    }
}
