using ECSEngine;
using System.Collections.Generic;

namespace OpenTPW.RSSEQ
{
    public class VM
    {
        public struct Config
        {
            public int stackSize, limboSize, bounceSize, walkSize, timeSlice;
        }

        private readonly RSSEQReader rsseqReader;

        private bool shouldRun;
        private int currentPos;

        public Config config;
        public List<Instruction> instructions { get; private set; } = new List<Instruction>();
        public List<int> variables { get; private set; }
        public string disassembly => rsseqReader.disassembly;

        public VM(byte[] rseData)
        {
            rsseqReader = new RSSEQReader(this);
            rsseqReader.ReadFile(rseData);
            variables = new List<int>(rsseqReader.variableCount);
        }

        public VM(string rsePath)
        {
            rsseqReader = new RSSEQReader(this);
            rsseqReader.ReadFile(rsePath);
            variables = new List<int>(rsseqReader.variableCount);
        }

        public void Run()
        {
            shouldRun = true;
            while (shouldRun)
            {
                Debug.Log($"Current pos: {currentPos}");

                currentPos++;
            }
        }

        public void SetRideName(string rideName)
        {
            Debug.Log($"New ride name set: {rideName}");
        }
    }
}
