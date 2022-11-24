namespace OpenTPW;

/// <summary>
/// A file containing data within a <see cref="WadArchive"/>.
/// </summary>
public class WadArchiveFile : WadArchiveItem
{
	public bool Compressed { get; set; }
	public uint DecompressedSize { get; set; }
	public WadArchive? ParentArchive { get; set; }
	public int ArchiveOffset { get; set; }

	public byte[] Data { get; set; }
}
