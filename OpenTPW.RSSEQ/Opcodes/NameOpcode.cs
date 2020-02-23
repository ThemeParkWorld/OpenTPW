namespace OpenTPW.RSSEQ.Opcodes
{
    public class NameOpcode : Opcode<NameOpcode>
    {
        public new OpcodeID opcodeId => OpcodeID.NAME;

        public new void Invoke(string[] args)
        {
            vmInstance.SetRideName(args[0]);
        }
    }
}
