namespace OpenTPW;

public static class LZSS
{
	public static bool Decompress( BitReader inputBuffer, BinaryWriter outputBuffer )
	{
		while ( true )
		{
			var isDelta = inputBuffer.GetBits( 1 );

			// One byte literal
			if ( isDelta == 0 )
			{
				var literalValue = inputBuffer.GetBits( 8 );
				outputBuffer.Write( (byte)literalValue );
			}
			else
			{
				int offset = (int)inputBuffer.GetBits( 12 );

				if ( offset == 0 )
					break;

				int length = (int)(inputBuffer.GetBits( 7 )) + 1;

				if ( offset > outputBuffer.BaseStream.Length )
					throw new Exception( "Offset > stream length" );

				long initialPosition = outputBuffer.BaseStream.Position;

				outputBuffer.BaseStream.Seek( initialPosition - offset, SeekOrigin.Begin );

				var window = new byte[offset];
				for ( int i = 0; i < offset; ++i )
				{
					window[i] = (byte)outputBuffer.BaseStream.ReadByte();
				}

				// Resize window to fit length
				var buffer = new byte[length];
				for ( int i = 0; i < length; ++i )
				{
					buffer[i] = window[i % offset];
				}

				outputBuffer.BaseStream.Seek( initialPosition, SeekOrigin.Begin );

				for ( int i = 0; i < length; ++i )
					outputBuffer.Write( buffer[i] );
			}
		}

		return true;
	}
}
