namespace OpenTPW;

public sealed class StringFile : BaseFormat
{
	public string[] Entries { get; private set; }

	public StringFile( string path )
	{
		ReadFromFile( path );
	}

	public StringFile( Stream stream )
	{
		ReadFromStream( stream );
	}

	public string this[int val] => Entries[val];

	protected override void ReadFromStream( Stream stream )
	{
		var reader = new BFSTReader( stream );
		Entries = reader.ReadFile();
	}
}
