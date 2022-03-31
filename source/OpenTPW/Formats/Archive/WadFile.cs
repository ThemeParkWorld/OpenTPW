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

		var refpack = new Refpack( Data );
		Data = refpack.Decompress().ToArray();
	}
}
