namespace OpenTPW;

partial class OpcodeHandlers
{
	public static class Math
	{
		[OpcodeHandler( Opcode.ADD, "Add two values together" )]
		public static int Add( int a, int b )
		{
			return a + b;
		}
	}
}
