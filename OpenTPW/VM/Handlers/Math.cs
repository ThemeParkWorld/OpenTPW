namespace OpenTPW;

public class OpcodeHandlers
{
	public class Math
	{
		[OpcodeHandler( Opcode.ADD )]
		public int Add( int a, int b )
		{
			return a + b;
		}

		// etc...
	}
}
