using OpenTPW.Formats.String;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vulkan;

namespace OpenTPW;
public sealed class BullfrogStringReader : BaseFormat
{
	private BullfrogStringStream memoryStream;
	private MTUReader mtuReader = new MTUReader( $"{Settings.Default.GamePath}\\data\\Language\\English\\MBToUni.dat" );
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

	protected override void ReadFromStream(Stream stream)
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
		memoryStream.Seek( 0, SeekOrigin.Begin );

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
		var count = 3;
		var stringCount = memoryStream.ReadInt32();
		Log.Info($"String Count: {count}", true);
		
		var initialMemPos = memoryStream.Position;

		for ( int i = 0; i < count - 1; i++ )
		{
			Log.Info($"Back to initial pos: {initialMemPos}", true);

			memoryStream.Seek( 4 * (i), SeekOrigin.Current );

			// Find offset number
			int offset = memoryStream.ReadInt32();

			//Log.Info( $"Memory Position: {memoryStream.Position}", true );
			Log.Info( $"Offset: {offset}" , true);

			// go to offset address
			// need to account offset for inital 12 bytes
			memoryStream.Seek( offset + 12 , SeekOrigin.Begin );

			// Unknown - we still want to verify it is 0x1
			var unknownOne = memoryStream.ReadByte();
			if(unknownOne != 1 )
			{
				throw new Exception( $"String Offset not working - offset set to {unknownOne} @ Pos:{memoryStream.Position}" );
			}

			// String Length
			var stringLength = memoryStream.ReadByte();
			StringBuilder output = new StringBuilder();

			//unused after string length
			_ = memoryStream.ReadByte();
			_ = memoryStream.ReadByte();

			// Characters
			for ( int j = 0; j < stringLength - 1; j++ )
			{
				var mtuPos = memoryStream.ReadByte();
				var character = mtuReader.GetCharacter( mtuPos );
				output.Append( character );
			}

			Log.Info($"{output}", true );

			// Padding (possibly longer?)
			_ = memoryStream.Seek( 4, SeekOrigin.Current );

			// Go back to intial position
			memoryStream.Seek( initialMemPos, SeekOrigin.Begin );
			
		}

	}
}
