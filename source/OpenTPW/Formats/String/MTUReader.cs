using OpenTPW.Formats.String;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTPW;
public sealed class MTUReader : BaseFormat
{
	private MTUStream memoryStream;
	public byte[] buffer;

	public MTUReader( string path )
	{
		using var fileStream = File.OpenRead( path );
		ReadFromStream( fileStream );
	}

	public MTUReader( Stream stream )
	{
		ReadFromStream( stream );
	}
	public void Dispose()
	{
		memoryStream.Dispose();
	}

	protected override void ReadFromStream( Stream stream )
	{
		// Set up read buffer
		var tempStreamReader = new StreamReader( stream );
		var fileLength = (int)tempStreamReader.BaseStream.Length;

		buffer = new byte[fileLength];
		tempStreamReader.BaseStream.Read( buffer, 0, fileLength );
		tempStreamReader.Close();

		memoryStream = new MTUStream( buffer );

		var magicNumber = memoryStream.ReadString( 4 );

		if ( magicNumber != "BFMU" )
			throw new Exception( $"Magic number did not match: {magicNumber}" );

		//ReadFile();
		//CharacterArray();
	}

	public char[] CharacterArray()
	{
		// Skip Header
		memoryStream.Seek( 6, SeekOrigin.Begin );

		// Read Character Length
		var charCount = memoryStream.ReadByte();
		char[] output = new char[charCount];
		
		//Skip 0x0 unsued byte
		_ = memoryStream.ReadByte();

		int arrayPos = 0;
		for ( int i = 0; i < memoryStream.Length - memoryStream.Position; i++ )
		{
			var currentChar = memoryStream.ReadChars( 1 );
			if ( currentChar[0] != '\0' )
			{
				output[arrayPos] = currentChar[0];
				arrayPos++;
			}
		}


		var stringOutput = new string( output ); 
		Log.Info( $"Character Array: {stringOutput}" , true );
		return output;
	}

	public void ReadFile()
	{
		/*
		
		Header

		4 bytes: Magic number - "BFMU"
		2 bytes: Likely specifies the character encoding - usually 0x00
		2 bytes: Character count

		# For each character

		2 bytes - The character itself in either Unicode or multibyte form

		*/

		var encoding = memoryStream.ReadBytes( 2 );
		Log.Info($"Encoding: {encoding}", true );
		var charCount = memoryStream.ReadBytes( 2 );

		for ( int i = 0; i < memoryStream.Length - memoryStream.Position; i++ )
		{
			Log.Info( $"Character: {GetCharacter( memoryStream.Position )} @ Position: {memoryStream.Position}", true );
			_= memoryStream.ReadByte();
		}

	}

	public string GetCharacter(long position)
	{
		memoryStream.Seek( position , SeekOrigin.Begin );
		var character = Encoding.ASCII.GetChars( memoryStream.ReadBytes(2) );
		Log.Info($"Getting char: {new string(character)} @ MemPos: {memoryStream.Position}", true);
		return new string(character).Trim();
	}

}
