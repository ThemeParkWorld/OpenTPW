namespace OpenTPW;

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
		return archive.Files.Any( x => FileMatchesPredicate( x, path ) );
	}

	public Stream OpenRead( string path )
	{
		if ( !FileExists( path ) )
			throw new FileNotFoundException( $"Path {path} doesn't exist on this archive" );

		var archiveFile = archive.Files.First( x => FileMatchesPredicate( x, path ) );
		var stream = new MemoryStream( archiveFile.Data );
		return stream;
	}
}
