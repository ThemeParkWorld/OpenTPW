namespace OpenTPW;

public class BaseFormat
{
	/// <summary>
	/// Read the format from a file within the TPW's internal file system
	/// </summary>
	protected virtual void ReadFromFile( string path )
	{
		using var fileStream = FileSystem.Game.OpenRead( path );
		ReadFromStream( fileStream );
	}

	/// <summary>
	/// Read the format from a stream.
	/// </summary>
	protected virtual void ReadFromStream( Stream stream )
	{
	}
}
