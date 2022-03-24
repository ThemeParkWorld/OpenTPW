namespace OpenTPW;

/*
 * Special thanks to Fatbag: http://wiki.niotso.org/RefPack, 
 * WATTO: http://wiki.xentax.com/index.php/WAD_DWFB, 
 * and Rhys: https://github.com/RHY3756547/FreeSO/blob/master/TSOClient/tso.files/FAR3/Decompresser.cs.
 */
public class WadArchive
{
	private WadStream memoryStream;
	public byte[] Buffer { get; internal set; }
	public List<WadArchiveFile> Files { get; internal set; }

	public WadArchive( string path )
	{
		using var fileStream = File.OpenRead( path );
		LoadArchive( fileStream );
	}

	public WadArchive( Stream stream )
	{
		LoadArchive( stream );
	}

	public void Dispose()
	{
		memoryStream.Dispose();
	}

	private void ReadArchive()
	{
		memoryStream.Seek( 0, SeekOrigin.Begin );

		/* 
		 * Header:
		 * 4 - magic number ('DWFB')
		 * 4 - version
		 * 64 - padding??
		 * 4 - file count
		 * 4 - file list offset
		 * 4 - file list length
		 * 4 - null
		 */

		var magicNumber = memoryStream.ReadString( 4 );

		// Magic number
		if ( magicNumber != "DWFB" )
			throw new Exception( $"Magic number did not match: {magicNumber}" );

		// Version
		_ = memoryStream.ReadInt32();

		// Padding
		memoryStream.Seek( 64, SeekOrigin.Current ); // Skip padding

		// File count
		var fileCount = memoryStream.ReadInt32();

		// File list offset
		_ = memoryStream.ReadInt32();

		// File list length
		_ = memoryStream.ReadInt32();

		// Unused / unknown
		_ = memoryStream.ReadInt32();

		// Details directory
		for ( var i = 0; i < fileCount; ++i )
		{
			/*
			 * File
			 * 4 - unused
			 * 4 - filename offset
			 * 4 - filename length
			 * 4 - data offset
			 * 4 - data length
			 * 4 - compression type ('4' for refpack)
			 * 4 - decompressed size
			 * 12 - null
			 */

			GC.Collect();
			// Save the current position so that we can go back to it later
			var initialPos = memoryStream.Position;

			var newFile = new WadArchiveFile();

			// Unused / unknown
			memoryStream.Seek( 4, SeekOrigin.Current );

			// Filename offset
			var filenameOffset = memoryStream.ReadUInt32();

			// Filename length
			var filenameLength = memoryStream.ReadUInt32();

			// Data offset
			var dataOffset = memoryStream.ReadUInt32();

			// Data length
			var dataLength = memoryStream.ReadUInt32();

			// Compression type
			newFile.Compressed = memoryStream.ReadUInt32() == 4;

			// Decompressed size
			newFile.DecompressedSize = memoryStream.ReadUInt32();

			// Set file's name name
			memoryStream.Seek( filenameOffset, SeekOrigin.Begin );
			newFile.Name = memoryStream.ReadString( (int)filenameLength );

			// Get file's raw data
			memoryStream.Seek( dataOffset, SeekOrigin.Begin );
			newFile.Data = memoryStream.ReadBytes( (int)dataLength );

			newFile.ArchiveOffset = (int)dataOffset;
			newFile.ParentArchive = this;

			newFile.Decompress();
			Files.Add( newFile );

			// Return to initial position, skip to the next file's data
			memoryStream.Seek( initialPos + 40, SeekOrigin.Begin );
		}
	}

	public void LoadArchive( Stream stream )
	{
		// Set up read buffer
		var tempStreamReader = new StreamReader( stream );
		var fileLength = (int)tempStreamReader.BaseStream.Length;

		Buffer = new byte[fileLength];
		tempStreamReader.BaseStream.Read( Buffer, 0, fileLength );
		tempStreamReader.Close();

		memoryStream = new WadStream( Buffer );
		Files = new List<WadArchiveFile>();

		ReadArchive();
	}
}
