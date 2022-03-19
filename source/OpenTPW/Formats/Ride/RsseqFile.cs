namespace OpenTPW;

public class RsseqFile
{
	private RideVM vmInstance;
	private List<string> variables = new List<string>();
	private int expectedInstructions;
	private long instructionOffset;

	public string Disassembly { get; private set; } = "";
	public int VariableCount => variables.Count;

	public RsseqFile( RideVM vmInstance )
	{
		this.vmInstance = vmInstance;
	}

	public void ReadFile( Stream stream )
	{
		using var binaryReader = new BinaryReader( stream );
		ReadFileContents( binaryReader );
	}

	private void ReadFileContents( BinaryReader binaryReader )
	{
		ReadFileHeader( binaryReader );

		// First 4 bytes are # of expected opcodes & operands
		expectedInstructions = binaryReader.ReadInt32();

		// Read string table
		instructionOffset = binaryReader.BaseStream.Position;

		// Forward to string table
		binaryReader.BaseStream.Seek( (expectedInstructions) * 4, SeekOrigin.Current );

		ReadStringTable( binaryReader );

		// Go back to instructions
		binaryReader.BaseStream.Seek( instructionOffset, SeekOrigin.Begin );

		ReadFileBody( binaryReader );
		WriteDisassembly();
	}

	private void ReadFileHeader( BinaryReader binaryReader )
	{
		var magicNumber = binaryReader.ReadChars( 8 );
		if ( !Enumerable.SequenceEqual( magicNumber, new[] { 'R', 'S', 'S', 'E', 'Q', (char)0x0F, (char)0x01, (char)0x00 } ) )
			Log.Error( "Magic number was not 'RSSEQ'" );

		// Variable count
		var variableCount = binaryReader.ReadInt32();

		vmInstance.Variables = new List<int>( variableCount );
		vmInstance.Config = new()
		{
			StackSize = binaryReader.ReadInt32(),
			TimeSlice = binaryReader.ReadInt32(),
			LimboSize = binaryReader.ReadInt32(),
			BounceSize = binaryReader.ReadInt32(),
			WalkSize = binaryReader.ReadInt32(),
		};

		for ( var i = 0; i < 4; ++i )
		{
			var paddingChars = binaryReader.ReadChars( 4 );
			if ( !Enumerable.SequenceEqual( paddingChars, new[] { 'P', 'a', 'd', ' ' } ) )
				Log.Error( "Invalid padding!" );
		}
	}

	private void ReadFileBody( BinaryReader binaryReader )
	{
		var currentOperands = new List<Operand>();
		var currentOpcode = 0;

		while ( binaryReader.BaseStream.Position < binaryReader.BaseStream.Length - 1 )
		{
			var currentValue = binaryReader.ReadInt32();
			var flag = (currentValue >> 24 & 0xFF);
			int truncValue = (short)currentValue;

			if ( (binaryReader.BaseStream.Position - instructionOffset) / 4 >= expectedInstructions + 1 )
			{
				Log.Warning( $"Hit max instruction count" );
				vmInstance.Instructions.Add( new Instruction( vmInstance, (Opcode)currentOpcode, currentOperands.ToArray() ) );
				break;
			}

			switch ( flag )
			{
				case 0x80:
					// Opcode
					vmInstance.Instructions.Add( new Instruction( vmInstance, (Opcode)currentOpcode, currentOperands.ToArray() ) );
					currentOpcode = (short)currentValue;
					currentOperands = new List<Operand>();
					break;
				case 0x10:
					// String
					currentOperands.Add( new Operand( vmInstance, Operand.Type.String, truncValue, truncValue ) );
					break;
				case 0x20:
					// Branch
					currentOperands.Add( new Operand( vmInstance, Operand.Type.Location, truncValue ) );
					vmInstance.Branches.Add( new Branch( vmInstance.Instructions.Count - 2 /* ignore NOP and array starts at 0 */, truncValue ) );
					break;
				case 0x40:
					// Variable
					currentOperands.Add( new Operand( vmInstance, Operand.Type.Variable, truncValue, truncValue ) );
					break;
				case 0x00:
					// Literal
					currentOperands.Add( new Operand( vmInstance, Operand.Type.Literal, truncValue ) );
					break;
			}
		}
	}

	private void ReadStringTable( BinaryReader binaryReader )
	{
		// First entry will be the strings used within the application
		var stringEntryLength = binaryReader.ReadInt32();
		var stringEntryPos = binaryReader.BaseStream.Position;
		var currentString = "";
		var stringOffsetPos = 0L;

		while ( binaryReader.BaseStream.Position - stringEntryPos < stringEntryLength )
		{
			var currentChar = binaryReader.ReadChar();
			if ( currentChar == '\0' )
			{
				vmInstance.Strings.Add( stringOffsetPos, currentString );
				currentString = "";
				stringOffsetPos = binaryReader.BaseStream.Position - stringEntryPos;
			}
			else
			{
				currentString += currentChar;
			}
		}

		while ( binaryReader.BaseStream.Position < binaryReader.BaseStream.Length )
		{
			// Read remaining variables
			var variableNameLength = binaryReader.ReadInt32();
			var stringChars = binaryReader.ReadChars( variableNameLength );

			vmInstance.VariableNames.Add( new string( stringChars ) );
			vmInstance.Variables.Add( 0 );
		}
	}

	private void WriteDisassembly()
	{
		var currentCount = 1;

		for ( var i = 0; i < vmInstance.Instructions.Count; ++i )
		{
			if ( vmInstance.Branches.Any( b => b.CompiledOffset == currentCount - 1 ) )
			{
				Disassembly += $".branch_{currentCount - 1}\n";
			}
			Disassembly += $"\t{vmInstance.Instructions[i]}\n";
			currentCount += vmInstance.Instructions[i].GetCount();
		}
	}
}
