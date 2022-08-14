namespace OpenTPW;

/*
 * TODO: Read from .wads as if they're folders
 */

public class FileSystem
{
	public static FileSystem Game => new FileSystem( Settings.Default.GamePath );

	private string BasePath { get; }

	public FileSystem( string relativePath )
	{
		this.BasePath = Path.GetFullPath( relativePath, Directory.GetCurrentDirectory() );
	}

	public string GetAbsolutePath( string relativePath, bool ignorePathNotFound = false )
	{
		var path = Path.Combine( this.BasePath, relativePath );

		if ( !File.Exists( path ) && !Directory.Exists( path ) && !ignorePathNotFound )
			Log.Warning( $"Path not found: {path}. Continuing anyway." );

		return path;
	}

	public string ReadAllText( string relativePath )
	{
		return File.ReadAllText( GetAbsolutePath( relativePath ) );
	}

	public byte[] ReadAllBytes( string relativePath )
	{
		return File.ReadAllBytes( GetAbsolutePath( relativePath ) );
	}

	public FileStream OpenRead( string relativePath )
	{
		return File.OpenRead( GetAbsolutePath( relativePath ) );
	}

	public bool Exists( string relativePath )
	{
		return File.Exists( GetAbsolutePath( relativePath, ignorePathNotFound: true ) );
	}

	public FileSystemWatcher CreateWatcher( string relativeDir, string filter )
	{
		var directoryName = GetAbsolutePath( relativeDir );
		var watcher = new FileSystemWatcher( directoryName, filter );

		watcher.NotifyFilter = NotifyFilters.Attributes
							 | NotifyFilters.CreationTime
							 | NotifyFilters.DirectoryName
							 | NotifyFilters.FileName
							 | NotifyFilters.LastAccess
							 | NotifyFilters.LastWrite
							 | NotifyFilters.Security
							 | NotifyFilters.Size;

		watcher.EnableRaisingEvents = true;

		return watcher;
	}
}
