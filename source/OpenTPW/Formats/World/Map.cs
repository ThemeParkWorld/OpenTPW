namespace OpenTPW;

public sealed class MapFile : BaseFormat
{
	// Guesswork / placeholders
	public enum TileType
	{
		Ground = 0,
		Wall = 1,
		River = 3,
		NotWalkable = 17,
		BrickPath = 144,
	}

	public MapFile( string path )
	{
		ReadFromFile( path );
	}

	public MapFile( Stream stream )
	{
		ReadFromStream( stream );
	}

	protected override void ReadFromStream( Stream stream )
	{
	}
}
