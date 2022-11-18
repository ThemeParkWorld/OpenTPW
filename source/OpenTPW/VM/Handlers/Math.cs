namespace OpenTPW;

partial class OpcodeHandlers
{
	public static class Math
	{
		[OpcodeHandler( Opcode.ADD, "Add two values together" )]
		public static void Add( ref RideVM vm, Operand a, Operand b )
		{
			a.Value = a.Value + b.Value;
		}

		[OpcodeHandler( Opcode.SUB, "Subtract one value from another" )]
		public static void Sub( ref RideVM vm, Operand a, Operand b, Operand c )
		{
			c.Value = a.Value - b.Value;
		}

		[OpcodeHandler( Opcode.TEST, "Set flags depending on the value given." )]
		public static void Test( ref RideVM vm, Operand value )
		{
			vm.Flags = RideVM.VMFlags.None;

			if ( value.Value == 0 )
				vm.Flags |= RideVM.VMFlags.Zero;

			// TODO: What the fuck is the crit flag for

			if ( value.Value < 0 )
				vm.Flags |= RideVM.VMFlags.Sign;
		}
	}
}
