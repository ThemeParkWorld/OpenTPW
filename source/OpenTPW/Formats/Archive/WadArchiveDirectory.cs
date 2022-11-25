namespace OpenTPW;

/// <summary>
/// Represents a directory within a <see cref="WadArchive"/>. Can contain
/// other directories or files as children.
/// </summary>
public sealed class WadArchiveDirectory : WadArchiveItem
{
	public List<WadArchiveItem> Children { get; set; } = new();

	public WadArchiveDirectory( string name )
	{
		Name = name;
	}
}
