using OpenTPW.Formats.String;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTPW.Formats.Save;
public class SaveFileReader : BaseFormat
{
	private SaveFileStream memoryStream;
	public byte[] buffer;

	public SaveFileReader( string path )
	{
		using var fileStream = File.OpenRead( path );
		ReadFromStream( fileStream );
	}

	public SaveFileReader( Stream stream )
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
		memoryStream = new SaveFileStream( buffer );

		var magicNumber = memoryStream.ReadHex( 4 );

		if( magicNumber != "F4010000" )
			throw new Exception( $"Magic number did not match: {magicNumber}" );

		ReadFromFile();
	}

	private void ReadFromFile()
	{
		int copyrightSize = 824; //Character count with spaces adds to 824

		var copyrightCharacters = memoryStream.ReadChars( copyrightSize );
		string copyright = "";
		foreach( char c in copyrightCharacters )
		{
			copyright += c;
		}

		if ( memoryStream.ReadByte() != 0 )
			_ = memoryStream.ReadByte();


	}
}
