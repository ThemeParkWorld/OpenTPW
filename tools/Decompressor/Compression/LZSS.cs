namespace OpenTPW;

public static class LZSS
{
	public static bool Decompress( BitReader inputBuffer, BinaryWriter outputBuffer )
	{
		while ( true )
		{
			var isDelta = inputBuffer.GetBits( 1 );
			Console.WriteLine( $"IsDelta {isDelta:D2}" );

			// One byte literal
			if ( isDelta == 0 )
			{
				var literalValue = inputBuffer.GetBits( 8 );

				Console.WriteLine( $"Literal {literalValue:X2}" );
				outputBuffer.Write( literalValue );
			}
			else
			{
				int offset = (int)inputBuffer.GetBits( 12 );

				if ( offset == 0 )
					break;

				int length = (int)(inputBuffer.GetBits( 7 )) + 1;

				if ( offset > outputBuffer.BaseStream.Length )
					throw new Exception( "Offset > stream length" );

				Console.WriteLine( $"Delta -{offset}[{length}]" );

				long initialPosition = outputBuffer.BaseStream.Position;
				outputBuffer.BaseStream.Seek( -offset, SeekOrigin.Current );

				var buffer = new int[length];
				for ( int i = 0; i < length; ++i )
					buffer[i] = outputBuffer.BaseStream.ReadByte();

				outputBuffer.BaseStream.Seek( initialPosition, SeekOrigin.Begin );

				for ( int i = 0; i < length; ++i )
					outputBuffer.Write( buffer[i] );
			}
		}

		return true;
	}
}
