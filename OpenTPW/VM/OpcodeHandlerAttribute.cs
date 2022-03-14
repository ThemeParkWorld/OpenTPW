namespace OpenTPW;

[System.AttributeUsage( AttributeTargets.Method, AllowMultiple = false, Inherited = false )]
public class OpcodeHandlerAttribute : Attribute
{
	public Opcode Opcode { get; set; }

	public OpcodeHandlerAttribute( Opcode opcode )
	{
		Opcode = opcode;
	}
}
