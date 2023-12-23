using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace OpenTPW;

public class BaseFileSystem
{
	private string BasePath { get; }
	private Dictionary<string, IArchive> ArchiveCache { get; } = new();
	private Dictionary<string, Type> ArchiveHandlers { get; } = new();

	public void RegisterArchiveHandler<T>( string extension ) where T : IArchive
	{
		ArchiveHandlers.Add( extension, typeof( T ) );
	}

	public BaseFileSystem( string relativePath )
	{
		BasePath = Path.GetFullPath( relativePath, Directory.GetCurrentDirectory() );
	}

	public string GetRelativePath( string absolutePath )
	{
		var path = Path.GetRelativePath( BasePath, absolutePath ).Replace( "\\", "/" );
		return $"/{path}";
	}

	public string GetAbsolutePath( string relativePath )
	{
		if ( relativePath.StartsWith( "/" ) )
			relativePath = relativePath[1..];

		var path = Path.Combine( BasePath, relativePath ).Replace( "/", "\\" );
		return path;
	}

	public string ReadAllText( string relativePath )
	{
		return Encoding.ASCII.GetString( ReadAllBytes( relativePath ) );
	}

	public byte[] ReadAllBytes( string relativePath )
	{
		var stream = InternalOpenFile( GetAbsolutePath( relativePath ) );
		var bytes = new byte[stream.Length];
		stream.Read( bytes, 0, (int)stream.Length );

		return bytes;
	}

	public Stream OpenRead( string relativePath )
	{
		return InternalOpenFile( GetAbsolutePath( relativePath ) );
	}

	private bool HasExtension( string name, string extension )
	{
		if ( name.EndsWith( extension, StringComparison.OrdinalIgnoreCase ) )
			return true;

		if ( name.EndsWith( $".{extension}", StringComparison.OrdinalIgnoreCase ) )
			return true;

		return false;
	}

	public string[] GetDirectories( string relativePath )
	{
		var absolutePath = GetAbsolutePath( relativePath );
		var directories = InternalGetDirectories( absolutePath );

		// Add archives
		{
			var archives = InternalGetFiles( absolutePath ).Where( x => HasExtension( x, ".wad" ) || HasExtension( x, ".sdt" ) );
			directories = directories.Concat( archives ).ToArray();
		}

		var relativeDirectories = directories.Select( GetRelativePath );
		return relativeDirectories.ToArray();
	}

	public string[] GetFiles( string relativePath )
	{
		var files = InternalGetFiles( GetAbsolutePath( relativePath ) );
		var filteredFiles = files.Where( x => !HasExtension( x, ".wad" ) && !HasExtension( x, ".sdt" ) );
		var relativeFiles = filteredFiles.Select( GetRelativePath );

		return relativeFiles.ToArray();
	}

	/// <summary>
	/// Retrieves an archive from, or adds an archive to, the archive cache.
	/// </summary>
	private IArchive? GetArchive( string relativePath )
	{
		if ( !TryGetArchiveType( relativePath, out var archiveType ) )
			return null;

		if ( !ArchiveCache.TryGetValue( relativePath, out var archive ) )
		{
			archive = (Activator.CreateInstance( archiveType, new[] { relativePath } ) as IArchive)!;
			ArchiveCache[relativePath] = archive;
		}

		return archive;
	}

	private bool TryGetArchiveType( string path, [NotNullWhen( true )] out Type? archiveType )
	{
		return ArchiveHandlers.TryGetValue( Path.GetExtension( path ), out archiveType );
	}

	/// <summary>
	/// Gets the archive path and internal path for a particular path, checking each part for registered archive extensions.
	/// </summary>
	private (string ArchivePath, string InternalPath) DissectPath( string path )
	{
		var parts = path.Split( Path.DirectorySeparatorChar );
		var archivePath = new StringBuilder();
		var internalPath = new StringBuilder();

		bool archiveFound = false;

		foreach ( var part in parts )
		{
			if ( !archiveFound )
			{
				// Construct the current path segment
				if ( archivePath.Length > 0 )
				{
					archivePath.Append( "/" );
				}

				archivePath.Append( part );

				foreach ( var handler in ArchiveHandlers )
				{
					var extension = handler.Key;
					var archiveFilePath = $"{archivePath}{extension}";

					// Check if an archive file exists with the current handler's extension
					if ( File.Exists( archiveFilePath ) )
					{
						archiveFound = true;
						archivePath.Append( $"{extension}" );
					}
				}
			}
			else
			{
				// Once an archive is found, the rest of the path is considered internal.
				if ( internalPath.Length > 0 )
				{
					internalPath.Append( "/" );
				}

				internalPath.Append( part );
			}
		}

		return archiveFound ? (archivePath.ToString(), internalPath.ToString()) : ("", "");
	}

	/// <summary>
	/// Handles enumerating through directories based on whether they're part of a WAD or not
	/// </summary>
	private string[] InternalGetDirectories( string absolutePath )
	{
		if ( TryGetArchiveType( absolutePath, out _ ) )
		{
			var (archivePath, internalPath) = DissectPath( absolutePath );
			var archive = GetArchive( archivePath );

			return archive.GetDirectories( internalPath ).Select( x => absolutePath + "\\" + x ).ToArray();
		}
		else
		{
			return Directory.GetDirectories( absolutePath );
		}
	}

	/// <summary>
	/// Handles enumerating through files based on whether they're part of a WAD or not
	/// </summary>
	private string[] InternalGetFiles( string absolutePath )
	{
		var (archivePath, internalPath) = DissectPath( absolutePath );

		if ( string.IsNullOrEmpty( archivePath ) )
		{
			return Directory.GetFiles( absolutePath );
		}
		else
		{
			var archive = GetArchive( archivePath );
			return archive.GetFiles( internalPath ).Select( x => absolutePath + "\\" + x ).ToArray();
		}
	}

	/// <summary>
	/// Handles opening a file based on whether it's part of a WAD or not
	/// </summary>
	private Stream InternalOpenFile( string absolutePath )
	{
		var (archivePath, internalPath) = DissectPath( absolutePath );

		if ( string.IsNullOrEmpty( archivePath ) )
		{
			return File.OpenRead( absolutePath );
		}
		else
		{
			var archive = GetArchive( archivePath );
			var file = archive.GetFile( internalPath );

			return new MemoryStream( file.GetData() );
		}
	}
}
