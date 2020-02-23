namespace OpenTPW.RSSEQ
{
    public interface IOpcode
    {
        OpcodeID opcodeId { get; }

        void Invoke(string[] args);
    }
}
