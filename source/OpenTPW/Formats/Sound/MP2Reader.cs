
namespace OpenTPW;


// BIG SHOUT TO Toksisitee - https://github.com/Toksisitee/PopSoundEditor

public class MP2Reader : BaseFormat
{
	private MP2Stream memoryStream;
	public byte[] buffer;

	public MP2Reader( string path )
	{
		using var fileStream = File.OpenRead( path );
		ReadFromStream( fileStream );
	}

	public MP2Reader( Stream stream )
	{
		ReadFromStream( stream );
	}
	public void Dispose()
	{
		memoryStream.Dispose();
	}

	protected void ReadFromStream( Stream stream )
	{
		// Set up read buffer
		var tempStreamReader = new StreamReader( stream );
		var fileLength = (int)tempStreamReader.BaseStream.Length;

		buffer = new byte[fileLength];
		tempStreamReader.BaseStream.Read( buffer, 0, fileLength );
		tempStreamReader.Close();
		memoryStream = new MP2Stream( buffer );
	}

	public MP2File GetFile( MemoryStream stream, int offset = 0, bool dataOnly = false )
	{

		/*
		 * 4 bytes: Header size
		 * 4 bytes: Data size
		 * 16 bytes: File name (usually null terminated)
		 * 4 bytes: Sample rate (Int16)
		 * 4 bytes: BitsPerSample
		 * 4 bytes: Sound type
		 * 4 bytes: Unknown
		 * 4 bytes: Samples
		 * 4 bytes: Unknown
		 * n bytes: File data
		*/
		memoryStream.Seek( offset, SeekOrigin.Begin );

		var headerSize = memoryStream.ReadInt32();
		Log.Info( $"Header Size: {headerSize}", true );

		var soundDataSize = memoryStream.ReadInt32();
		Log.Info( $"Data Size: {soundDataSize}", true );

		var fileName = memoryStream.ReadString( 16 ).TrimEnd( '\0' );
		Log.Info( $"File Name: {fileName}", true );

		if ( !fileName.EndsWith( ".mp2" ) )
		{
			bool hasExtension = fileName.Contains( '.' );
			if ( !hasExtension )
			{
				fileName += ".mp2";
			}
			else
			{
				fileName = fileName.Substring( 0, fileName.LastIndexOf( "." ) ) + ".mp2";
			}

		}

		var sampleRate = memoryStream.ReadInt16();
		Log.Info( $"Sample Rate: {sampleRate}", true );

		var bitsPerSample = memoryStream.ReadInt32();
		Log.Info( $"Resolution: {bitsPerSample}", true );

		var soundType = memoryStream.ReadInt32();
		Log.Info( $"Sound/File Type: {soundType}", true );


		// Unknown
		_ = memoryStream.ReadInt32();

		var samples = memoryStream.ReadInt32();
		Log.Info( $"Samples: {samples}", true );
		Log.Info( $"", true );
		Log.Info( $"", true );

		// Unknown
		_ = memoryStream.ReadInt32();

		// Rest of Data
		var soundData = memoryStream.ReadBytes( soundDataSize );

		// Gather full byte data of file
		var dataSize = headerSize + soundDataSize;

		// We seek back to offset to capture full data set
		memoryStream.Seek ( offset, SeekOrigin.Begin );
		byte[] data = memoryStream.ReadBytes( dataSize );
		
		return new MP2File( headerSize, fileName, soundData, sampleRate, bitsPerSample, soundType, samples, data );


	}
}
