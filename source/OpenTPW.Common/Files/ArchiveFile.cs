namespace OpenTPW;

/// <summary>
/// A file containing data within an archive
/// </summary>
public sealed class ArchiveFile : ArchiveItem
{
	public IArchive? Archive { get; set; }

	public bool Compressed { get; set; }
	public uint DecompressedSize { get; set; }
	public int ArchiveOffset { get; set; }

	public byte[] Data { get; set; }
}
