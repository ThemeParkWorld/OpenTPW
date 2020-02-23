namespace OpenTPW.RSSEQ
{
    public class Opcode<T> : IOpcode
    {
        public virtual OpcodeID opcodeId { get; }

        public virtual void Invoke(string[] args) { }

        protected VM vmInstance;
    }
}
