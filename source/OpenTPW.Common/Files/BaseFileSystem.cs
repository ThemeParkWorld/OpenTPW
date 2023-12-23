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

	public string GetAbsolutePath( string relativePath )
	{
		var path = Path.Combine( BasePath, relativePath );
		return path;
	}

	public string ReadAllText( string relativePath )
	{
		return Encoding.ASCII.GetString( ReadAllBytes( relativePath ) );
	}

	public byte[] ReadAllBytes( string relativePath )
	{
		var stream = InternalOpenFile( relativePath );
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

		if ( name.EndsWith($".{extension}", StringComparison.OrdinalIgnoreCase ) )
			return true;

		return false;
	}

	public string[] GetDirectories( string relativePath )
	{
		var absolutePath = GetAbsolutePath( relativePath );

		var dirs = InternalGetDirectories( absolutePath );
		var archives = InternalGetFiles( absolutePath ).Where( x => HasExtension( x, ".wad" ) || HasExtension( x, ".sdt" ));
		
		if( dirs == null )
		{
			return archives.ToArray();
		}

		return dirs.Concat( archives ).ToArray();
	}

	public string[] GetFiles( string relativePath )
	{
		var dirs = InternalGetFiles( GetAbsolutePath( relativePath ) );

		return dirs.Where( x => !HasExtension( x, ".wad" ) && !HasExtension( x, ".sdt" ) ).ToArray();
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
			archive = (Activator.CreateInstance( archiveType ) as IArchive)!;
			ArchiveCache[relativePath] = archive;
		}

		return archive;
	}

	private bool TryGetArchiveType( string path, [NotNullWhen(true)] out Type? archiveType )
	{
		foreach ( var handler in ArchiveHandlers )
		{
			if ( HasExtension( path, handler.Key ) )
			{
				archiveType = handler.Value;
				return true;
			}
		}

		archiveType = null;
		return false;
	}

	/// <summary>
	/// Gets the archive path and internal path for a particular path
	/// </summary>
	private (string WadPath, string InternalPath) DissectPath( string path )
	{
		if ( !path.Contains( ".wad", StringComparison.OrdinalIgnoreCase ) )
			return ("", "");

		// Find archive in path
		var archivePath = path[..(path.IndexOf( ".wad", StringComparison.OrdinalIgnoreCase ) + 4)];

		if ( HasExtension( path, ".wad" ) )
			return (archivePath, "");

		// Find file in path
		var internalPath = path[(path.IndexOf( ".wad", StringComparison.OrdinalIgnoreCase ) + 5)..];

		return (archivePath, internalPath);
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
		if ( TryGetArchiveType( absolutePath, out _ ) )
		{
			var (archivePath, internalPath) = DissectPath( absolutePath );
			var archive = GetArchive( archivePath );

			return archive.GetFiles( internalPath ).Select( x => absolutePath + "\\" + x ).ToArray();
		}
		else
		{
			return Directory.GetFiles( absolutePath );
		}
	}

	/// <summary>
	/// Handles opening a file based on whether it's part of a WAD or not
	/// </summary>
	private Stream InternalOpenFile( string absolutePath )
	{
		if ( TryGetArchiveType( absolutePath, out _ ) )
		{
			var (archivePath, internalPath) = DissectPath( absolutePath );
			var archive = GetArchive( archivePath );

			var file = archive.GetFile( internalPath );

			return new MemoryStream( file.Data );
		}
		else
		{
			return File.OpenRead( absolutePath );
		}
	}
}
