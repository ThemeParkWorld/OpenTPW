namespace OpenTPW;

public class BaseFormat
{
	protected virtual void ReadFromFile( string path )
	{
		using var fileStream = FileSystem.Game.OpenRead( path );
		ReadFromStream( fileStream );
	}

	protected virtual void ReadFromStream( Stream stream )
	{
	}
}
