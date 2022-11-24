namespace OpenTPW;

[Obsolete]
public class WadFileSystem : IDisposable
{
	private WadArchive archive;

	public WadFileSystem( string wadPath )
	{
		archive = new( wadPath );
	}

	void IDisposable.Dispose()
	{
		archive.Dispose();
	}

	private bool FileMatchesPredicate( WadArchiveFile file, string path )
	{
		return file.Name?.Equals( path + "\0", StringComparison.CurrentCultureIgnoreCase ) ?? false;
	}

	public bool FileExists( string path )
	{
		return false;
	}

	public Stream OpenRead( string path )
	{
		var stream = new MemoryStream( new byte[0] );
		return stream;
	}
}
