namespace OpenTPW;

public partial class OpcodeHandlers
{
	public static class Misc
	{
		[OpcodeHandler( Opcode.NOP, "No-op" )]
		public static void NoOp()
		{
			return;
		}
	}
}
