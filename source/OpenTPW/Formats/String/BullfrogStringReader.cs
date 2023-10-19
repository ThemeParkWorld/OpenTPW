using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTPW.Formats.String;
internal class BullfrogStringReader
{
	private BullfrogStringStream memoryStream;
	public byte[] buffer;

	public BullfrogStringReader (string path )
	{
		using var fileStream = File.OpenRead( path );
		ReadFromStream( fileStream );
	}

	public BullfrogStringReader (Stream stream )
	{
		ReadFromStream(stream);
	}
	public void Dispose()
	{
		memoryStream.Dispose();
	}

	private void ReadFromStream(Stream stream)
	{
		// Set up read buffer
		var tempStreamReader = new StreamReader( stream );
		var fileLength = (int)tempStreamReader.BaseStream.Length;

		buffer = new byte[fileLength];
		tempStreamReader.BaseStream.Read( buffer, 0, fileLength );
		tempStreamReader.Close();

		memoryStream = new BullfrogStringStream( buffer );

		ReadFile();
	}

	private void ReadFile()
	{
		/*	
		Header
			4 bytes: Magic number - "BFST"
			4 bytes: Unknown
			4 bytes: String count
			
		#For each string
			4 bytes - String offset (from the end of the string count)
			
		#For each string (at offset)
			1 byte - Unknown, always 0x01
			3 bytes - String length
			n bytes - Each character, specified with an offset in a BFMU file.
			4 bytes - Padding (may be longer?)
		*/

		var magicNumber = memoryStream.ReadString( 4 );

		if ( magicNumber != "BFST" )
			throw new Exception( $"Magic number did not match: {magicNumber}" );

		//Unknown
		_ = memoryStream.ReadInt32();

		//String Count
		var count = memoryStream.ReadInt32();
		Log.Info($"String Count: {count}", true);

		var offset = memoryStream.ReadInt32();
		Log.Info( $"String Offset: {offset}", true );

		//Start looping through strings
		//First lets get the office
		for ( int i = 0; i < count; i++ )
		{

			memoryStream.Seek( offset, SeekOrigin.Begin );
			
			//unknown
			_ = memoryStream.ReadUIntN( 1 );

			//String Length
			var stringLength = memoryStream.ReadUIntN( 3 );

			// Char encoding
			var encodingOffset = 0;

			var stringChars = "";
			//loop through character encoding
			for( int j = 0; j < encodingOffset; j++ )
			{
				return;
			}

			//Padding (possibly longer?)
			_ = memoryStream.Seek( 4, SeekOrigin.Current );
		}
	}
}
