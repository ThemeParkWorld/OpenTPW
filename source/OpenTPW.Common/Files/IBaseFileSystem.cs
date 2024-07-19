
namespace OpenTPW;

public interface IBaseFileSystem
{
	string GetAbsolutePath( string relativePath );
	string[] GetDirectories( string relativePath );
	string[] GetFiles( string relativePath );
	Stream OpenRead( string relativePath );
	byte[] ReadAllBytes( string relativePath );
	string ReadAllText( string relativePath );
	void RegisterArchiveHandler<T>( string extension ) where T : IArchive;
}