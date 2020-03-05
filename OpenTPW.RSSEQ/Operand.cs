namespace OpenTPW.RSSEQ
{
    public class Operand
    {
        public enum Type
        {
            Variable,
            Literal,
            String,
            Location
        }

        private VM vmInstance;
        private int value;
        private readonly Type type;

        public Operand(VM vmInstance, Type type, int value)
        {
            this.vmInstance = vmInstance;
            this.type = type;
            this.value = value;
        }

        public int Value
        {
            get
            {
                switch (type)
                {
                    case Type.Variable:
                        return vmInstance.Variables[value];
                    default:
                        return value;
                }
            }
            set
            {
                switch (type)
                {
                    case Type.Variable:
                        vmInstance.Variables[this.value] = value;
                        break;
                    default:
                        this.value = value;
                        break;
                }
            }
        }
    }
}
