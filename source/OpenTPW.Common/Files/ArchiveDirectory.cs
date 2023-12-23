namespace OpenTPW;

/// <summary>
/// Represents a directory within a <see cref="WadArchive"/>. Can contain
/// other directories or files as children.
/// </summary>
public sealed class ArchiveDirectory : ArchiveItem
{
	public List<ArchiveItem> Children { get; set; } = new();

	public ArchiveDirectory( string name )
	{
		Name = name;
	}
}
