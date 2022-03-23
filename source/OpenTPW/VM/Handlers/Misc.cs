namespace OpenTPW;

public partial class OpcodeHandlers
{
	public static class Misc
	{
		[OpcodeHandler( Opcode.NOP, "No-op" )]
		public static void NoOp( ref RideVM vm )
		{
			// Do nothing
		}

		[OpcodeHandler( Opcode.CRIT_LOCK, "Crit lock" )]
		public static void CritLock( ref RideVM vm )
		{
			Log.Trace( "TODO: Crit lock" );
		}

		[OpcodeHandler( Opcode.COPY, "Copy a value" )]
		public static void Copy( ref RideVM vm, Operand a, Operand b )
		{
			Log.Trace( $"{a.Value} -> {b.Value}" );
			a.Value = b.Value;
			Log.Trace( $"res: {a.Value}" );
		}

		[OpcodeHandler( Opcode.NAME, "Set ride name" )]
		public static void Name( ref RideVM vm, Operand value )
		{
			vm.ScriptName = vm.Strings[value.Value];
			Log.Trace( $"Set ride name to {vm.ScriptName}" );
		}

		[OpcodeHandler( Opcode.SETLV, "Set level" )]
		public static void SetLv( ref RideVM vm, Operand value )
		{
			Log.Trace( "TODO: Set level" );
		}

		[OpcodeHandler( Opcode.ENDSLICE, "End current slice" )]
		public static void EndSlice( ref RideVM vm )
		{
			Log.Trace( "TODO: End slice" );
		}

		[OpcodeHandler( Opcode.GETTIME, "Gets the time the ride has been alive for" )]
		public static void GetTime( ref RideVM vm, Operand dest )
		{
			dest.Value = 0; // TODO
			Log.Trace( "TODO: Get time" );
		}

		[OpcodeHandler( Opcode.ADDOBJ, "Add an object" )]
		public static void AddObj( ref RideVM vm, Operand type, Operand parameter, Operand id, Operand slot )
		{
			switch ( (ScriptDefs)type.Value )
			{
				//case ScriptDefs.OBJ_PTCL:
				//case ScriptDefs.OBJ_SOUND_LOC_AMB:
				//case ScriptDefs.OBJ_PTCL_D:
				//case ScriptDefs.OBJ_SOUND_LOC_RID:
				//case ScriptDefs.OBJ_SOUND_GLO_RID:
				//case ScriptDefs.OBJ_SOUND_GLO_KID:
				//case ScriptDefs.OBJ_SOUND_GLO_STA:
				//case ScriptDefs.OBJ_SOUND_GLO_AMB:
				//case ScriptDefs.OBJ_SOUND_GLO_UI:
				//case ScriptDefs.OBJ_SOUND_GLO_BMP:
				//	break;
				default:
					break;
			}
		}
	}
}
