using StbImageSharp;
using System.Runtime.InteropServices;
using Veldrid;

namespace OpenTPW;

public partial class TextureBuilder
{
	private string type = "texture_diffuse";

	private byte[] data;
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
		if ( TryGetExistingTexture( path, out var existingTexture ) )
			return existingTexture;

		var textureDataPtr = Marshal.AllocHGlobal( data.Length );
		Marshal.Copy( data, 0, textureDataPtr, data.Length );

		var textureDescription = TextureDescription.Texture2D(
			width,
			height,
			1,
			1,
			PixelFormat.R8_G8_B8_A8_UNorm,
			TextureUsage.Sampled
		);

		var texture = Device.ResourceFactory.CreateTexture( textureDescription );
		Device.UpdateTexture( texture, textureDataPtr, (uint)data.Length, 0, 0, 0, width, height, 1, 0, 0 );

		var textureView = Device.ResourceFactory.CreateTextureView( texture );
		Marshal.FreeHGlobal( textureDataPtr );

		return new Texture( path, texture, textureView, type, (int)width, (int)height );
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

	public static TextureBuilder FromPath( string path, bool flipY = true )
	{
		if ( TryGetExistingTexture( path, out _ ) )
			return new TextureBuilder() { path = path };

		var textureBuilder = new TextureBuilder();

		// shit-tier hack
		if ( flipY )
			StbImage.stbi_set_flip_vertically_on_load( 1 );

		var fileData = File.ReadAllBytes( path );
		var image = ImageResult.FromMemory( fileData, ColorComponents.RedGreenBlueAlpha );

		StbImage.stbi_set_flip_vertically_on_load( 0 );

		textureBuilder.data = image.Data;
		textureBuilder.width = (uint)image.Width;
		textureBuilder.height = (uint)image.Height;
		textureBuilder.path = path;

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

		// shit-tier hack
		if ( flipY )
			StbImage.stbi_set_flip_vertically_on_load( 1 );

		var fileData = new byte[stream.Length];
		stream.Read( fileData, 0, fileData.Length );

		var image = ImageResult.FromMemory( fileData, ColorComponents.RedGreenBlueAlpha );

		StbImage.stbi_set_flip_vertically_on_load( 0 );

		textureBuilder.data = image.Data;
		textureBuilder.width = (uint)image.Width;
		textureBuilder.height = (uint)image.Height;
		textureBuilder.path = $"Stream {stream.GetHashCode()}";

		return textureBuilder;
	}
}
