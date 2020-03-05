namespace OpenTPW.RSSEQ
{
    public abstract class OpcodeHandler
    {
        public abstract OpcodeID OpcodeId { get; }

        public abstract int MinArgs { get; }

        public abstract int MaxArgs { get; }

        public abstract string Description { get; }

        public abstract void Invoke(Operand[] args);

        public VM vmInstance;
    }
}
