namespace OpenTPW.RSSEQ.Opcodes
{
    public class SetObjParamOpcode : OpcodeHandler
    {
        public override OpcodeID OpcodeId => OpcodeID.SETOBJPARAM;

        public override void Invoke(Operand[] args)
        {
            var destObject = vmInstance.Objects[args[0].Value];
            var property = args[1].Value;
            var value = args[2].Value;

            if (!destObject.Properties.ContainsKey(property))
                destObject.Properties.Add(property, value);
            else
                vmInstance.Objects[args[0].Value].Properties[property] = value;
        }

        public override int MinArgs => 3;
        public override int MaxArgs => 3;
        public override string Description => "Set a sub-object's parameters";
    }
}