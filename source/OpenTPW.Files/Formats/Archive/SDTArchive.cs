
namespace OpenTPW;
public class SDTArchive
{
	private ExpandedMemoryStream memoryStream;
	private MP2Reader mp2Reader;

	public byte[] buffer;
	public List<MP2File> soundFiles;

	public SDTArchive( string path )
	{
		soundFiles = new List<MP2File>();
		using var fileStream = File.OpenRead( path );
		ReadFromStream( fileStream );
	}

	public SDTArchive( Stream stream )
	{
		soundFiles = new List<MP2File>();
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
		memoryStream = new ExpandedMemoryStream( buffer );
		mp2Reader = new MP2Reader( new MemoryStream(buffer) );

		ReadArchive();
	}

	public void ReadArchive(bool dataOnly = false)
	{
		memoryStream.Seek( 0, SeekOrigin.Begin );

		/*
			#File header
				4 bytes: File count
			#For each file
				4 bytes: File offset
			#For each file (at offset)
				4 bytes: Header size
				4 bytes: Data size
				16 bytes: File name (usually null terminated)
				4 bytes: Sample rate
				4 bytes: Resolution
				4 bytes: Sound type
					*See enum above (also: https://github.com/ufdada/dk2-tools/blob/6b4e49b607bbb7e0aa843856e584f6dd1365e7fc/Formats/Sound/sdt_struct.bt)
				4 bytes: Unknown
				4 bytes: Samples
				4 bytes: Unknown
				n bytes: File data
		*/

		var fileCount = memoryStream.ReadInt32();
		Log.Info( $"File Count: {fileCount}", true );
		Log.Info( $"", true );
		Log.Info( $"", true );

		List<int> offsets = new List<int>();

		// Gather Offsets
		for ( int i = 0; i < fileCount; i++ ) 
		{
			offsets.Add( memoryStream.ReadInt32() );
			Log.Info( $"Offset Found at: {offsets[i] }", true );
		}

		Log.Info( $"", true );
		Log.Info( $"", true );

		foreach ( int offset in offsets )
		{
			MP2File mp2File = mp2Reader.GetFile( memoryStream, offset );
			soundFiles.Add( mp2File );
		}
	}

	public string[] GetFiles( string internalPath )
	{
		return soundFiles.Select( x => x.Name ).ToArray();
	}
	public string[] GetDirectories( string internalPath )
	{
		return soundFiles.Select( x => x.Name ).ToArray();
	}

	public MP2File GetFile( string name )
	{
		int index = soundFiles.FindIndex( x => x.Name.StartsWith( name ) );
		return soundFiles[index];
	}

}
