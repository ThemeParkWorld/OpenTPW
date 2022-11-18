namespace OpenTPW;

partial class OpcodeHandlers
{
	public static class Logic
	{
		[OpcodeHandler( Opcode.BRANCH_NZ, "Branch to another location if the \"zero\" flag hasn't been set." )]
		public static void BranchNotZero( ref RideVM vm, Operand location )
		{
			if ( vm.Flags.HasFlag( RideVM.VMFlags.Zero ) )
				return;

			vm.BranchTo( location.Value );
		}

		[OpcodeHandler( Opcode.BRANCH_Z, "Branch to another location if the \"zero\" flag has been set." )]
		public static void BranchZero( ref RideVM vm, Operand location )
		{
			if ( !vm.Flags.HasFlag( RideVM.VMFlags.Zero ) )
				return;

			vm.BranchTo( location.Value );
		}
	}
}
