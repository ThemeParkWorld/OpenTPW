namespace OpenTPW.RSSEQ
{
    public class Operand
    {
        private VM vmInstance;

        public enum Type
        {
            Variable,
            Literal,
            String,
            Location
        }

        private int value_;
        private Type type;

        public Operand(Type type, int value_)
        {
            this.type = type;
            this.value_ = value_;
        }

        public int value
        {
            get
            {
                switch (type)
                {
                    case Type.Variable:
                        return vmInstance.variables[value_];
                    default:
                        return value_;
                }
            }
            set
            {
                switch (type)
                {
                    case Type.Variable:
                        vmInstance.variables[value_] = value;
                        break;
                    default:
                        value_ = value;
                        break;
                }
            }
        }
    }
}
