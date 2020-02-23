namespace OpenTPW.RSSEQ
{
    public abstract class OpcodeHandler
    {
        public abstract OpcodeID opcodeId { get; }

        public abstract int minArgs { get; }

        public abstract int maxArgs { get; }

        public abstract string description { get; }

        public abstract void Invoke(Operand[] args);

        public VM vmInstance;
    }
}
