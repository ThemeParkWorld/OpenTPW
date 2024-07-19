using System.Text;

namespace OpenTPW;

public record struct SettingsPair( string Key, string Value );

public sealed class SettingsFile : BaseFormat
{
	public List<SettingsPair> Entries { get; set; } = new();

	public SettingsFile( string path )
	{
		ReadFromFile( path );
	}

	public SettingsFile( Stream stream )
	{
		ReadFromStream( stream );
	}

	public string this[string val] => Entries.FirstOrDefault( x => x.Key.Equals( val, StringComparison.OrdinalIgnoreCase ) ).Value ?? null!;

	protected override void ReadFromStream( Stream stream )
	{
		using var binaryReader = new BinaryReader( stream );
		var text = Encoding.ASCII.GetString( binaryReader.ReadBytes( (int)stream.Length ) );

		var parser = new SAMParser( text );
		Entries = parser.Parse();
	}
}
