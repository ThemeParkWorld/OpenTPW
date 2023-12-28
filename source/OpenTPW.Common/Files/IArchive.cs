namespace OpenTPW;

public interface IArchive : IFileFormat, IDisposable
{
	/// <summary>
	/// Disposes of the resources used by the archive
	/// </summary>
	void Dispose();

	/// <summary>
	/// Retrieves the names of all files within a specified internal path in the archive
	/// </summary>
	/// <param name="internalPath"></param>
	/// <returns></returns>
	string[] GetFiles( string internalPath );

	/// <summary>
	/// Retrieves the names of all directories within a specified internal path in the archive
	/// </summary>
	/// <param name="internalPath"></param>
	/// <returns></returns>
	string[] GetDirectories( string internalPath );

	/// <summary>
	/// Retrieves a specific file from the archive based on its name or internal path
	/// </summary>
	/// <param name="internalPath"></param>
	/// <returns></returns>
	ArchiveFile GetFile( string internalPath );

	/// <summary>
	/// 
	/// </summary>
	/// <param name="internalPath"></param>
	/// <returns></returns>
	byte[] GetData( int offset, int length );

	/// <summary>
	/// 
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	Stream OpenFile( string path );

	long GetFileSize( string path );

	DateTime GetModifiedTime();
}
