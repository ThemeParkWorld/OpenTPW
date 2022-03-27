namespace OpenTPW;

public partial class TextureBuilder
{
	private uint id;
	private string type = "texture_diffuse";

	private byte[]? data;
	private uint width;
	private uint height;

	private string path;

	public TextureBuilder()
	{
		path = GetHashCode().ToString();
	}

	private static bool TryGetExistingTexture( string path, out Texture texture )
	{
		var existingTexture = Asset.All.OfType<Texture>().ToList().FirstOrDefault( t => t.Path == path );
		if ( existingTexture != null )
		{
			texture = existingTexture;
			return true;
		}

		texture = default;
		return false;
	}

	public Texture Build()
	{
		if ( TryGetExistingTexture( path, out var texture ) )
			return texture;

		return new Texture( path, id, type, (int)width, (int)height );
	}

	public TextureBuilder UsePointFiltering( bool usePointFiltering = true )
	{
		if ( !usePointFiltering )
			return this;


		return this;
	}

	public TextureBuilder UseNormalFormat( bool useNormal = true )
	{
		if ( !useNormal )
			return this;

		return this;
	}

	public TextureBuilder UseSrgbFormat( bool useSrgb = true )
	{
		if ( !useSrgb )
			return this;

		return this;
	}

	public TextureBuilder WithType( string type )
	{
		this.type = type;
		return this;
	}

	public static TextureBuilder FromHdri( string path )
	{
		if ( TryGetExistingTexture( path, out _ ) )
			return new TextureBuilder() { path = path };

		var textureBuilder = new TextureBuilder();


		return textureBuilder;
	}

	public static TextureBuilder FromPath( string path, bool flipY = true )
	{
		if ( TryGetExistingTexture( path, out _ ) )
			return new TextureBuilder() { path = path };

		var textureBuilder = new TextureBuilder();

		return textureBuilder;
	}

	public static TextureBuilder FromBytes( byte[] bytes, uint width, uint height )
	{
		var textureBuilder = new TextureBuilder
		{
			data = bytes,
			width = width,
			height = height
		};

		return textureBuilder;
	}

	public static TextureBuilder FromStream( Stream stream, bool flipY = true )
	{
		var textureBuilder = new TextureBuilder();

		return textureBuilder;
	}
}
