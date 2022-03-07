using System.Reflection;

namespace OpenTPW;

public class WadArchiveFile
{
	public string? Name { get; set; }
	public bool Compressed { get; set; }
	public uint DecompressedSize { get; set; }
	public WadArchive? ParentArchive { get; set; }
	public int ArchiveOffset { get; set; }

	public byte[] Data { get; set; }

	public void Decompress()
	{
		if ( !Compressed )
			return;

		// Refpack is big-endian, unlike the rest of the DWFB format
		// Therefore all memorystream operations have bigEndian set to true
		var decompressedData = new List<byte>();
		using var memoryStream = new WadStream( Data );

		var refpackHeader = memoryStream.ReadBytes( 2, bigEndian: true );

		// 0x10: LU01000C - 00010000 - large files & compressed size are not supported.
		if ( refpackHeader[0] != 0xFB || refpackHeader[1] != 0x10 )
			throw new Exception( "Data was not compressed using refpack (header does not match) - possibly corrupted?" );

		// Skip decompressed size
		memoryStream.Seek( 3, SeekOrigin.Current );

		var currentByte = memoryStream.ReadBytes( 1, bigEndian: true );

		var commands = new List<Refpack.IRefpackCommand>();
		var commandCount = new Dictionary<Type, int>();

		foreach ( var type in Assembly.GetExecutingAssembly().GetTypes() )
		{
			if ( type.GetInterfaces().Contains( typeof( Refpack.IRefpackCommand ) ) )
			{
				commands.Add( Activator.CreateInstance( type ) as Refpack.IRefpackCommand );
			}
		}

		while ( memoryStream.Position < Data.Length )
		{
			foreach ( var command in commands )
			{
				if ( command.OpcodeMatches( currentByte[0] ) )
				{
					var commandType = command.GetType();
					if ( commandCount.ContainsKey( commandType ) )
						commandCount[commandType]++;
					else
						commandCount.Add( commandType, 1 );

					command.Decompress( Data, ref decompressedData, (int)memoryStream.Position - 1, out var skipAhead );
					memoryStream.Seek( command.Length + skipAhead - 1, SeekOrigin.Current );

					if ( command.StopAfterFound )
						memoryStream.Seek( Data.Length, SeekOrigin.Current );
				}
			}
			currentByte = memoryStream.ReadBytes( 1, bigEndian: true );
		}

		Data = decompressedData.ToArray();
	}
}
