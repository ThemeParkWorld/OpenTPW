using Silk.NET.OpenGL;
using StbImageSharp;

namespace OpenTPW;

public partial class TextureBuilder
{
	private uint id;
	private string type = "texture_diffuse";

	private InternalFormat internalFormat = InternalFormat.Rgba;
	private PixelFormat pixelFormat = PixelFormat.Rgba;
	private PixelType pixelType = PixelType.UnsignedByte;

	private byte[]? data;
	private uint width;
	private uint height;

	private GLEnum minFilter = GLEnum.LinearMipmapLinear;
	private GLEnum magFilter = GLEnum.Linear;

	private string path;

	public TextureBuilder()
	{
		path = GetHashCode().ToString();
	}

	private static bool TryGetExistingTexture( string path, out Texture? texture )
	{
		var existingTexture = Asset.All.OfType<Texture>().ToList().FirstOrDefault( t => t.Path == path );
		if ( existingTexture != null )
		{
			texture = existingTexture;
			return true;
		}

		texture = null;
		return false;
	}

	public Texture Build()
	{
		if ( TryGetExistingTexture( path, out var texture ) )
			return texture;

		if ( id <= 0 )
		{
			id = Gl.GenTexture();
			Gl.BindTexture( TextureTarget.Texture2D, id );

			Gl.TexImage2D<byte>(
				TextureTarget.Texture2D,
				0,
				internalFormat,
				width,
				height,
				0,
				pixelFormat,
				pixelType,
				data );

			Gl.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter );
			Gl.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter );
			Gl.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMaxAnisotropy, 4.0f );
			Gl.GenerateMipmap( TextureTarget.Texture2D );

			Gl.BindTexture( TextureTarget.Texture2D, 0 );
		}

		return new Texture( path, id, type, (int)width, (int)height );
	}

	public TextureBuilder UsePointFiltering( bool usePointFiltering = true )
	{
		if ( !usePointFiltering )
			return this;

		minFilter = GLEnum.Nearest;
		magFilter = GLEnum.Nearest;

		return this;
	}

	public TextureBuilder UseNormalFormat( bool useNormal = true )
	{
		if ( !useNormal )
			return this;

		internalFormat = InternalFormat.Rgba8;
		return this;
	}

	public TextureBuilder UseSrgbFormat( bool useSrgb = true )
	{
		if ( !useSrgb )
			return this;

		internalFormat = InternalFormat.SrgbAlpha;
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

		var fileData = File.ReadAllBytes( path );
		var image = ImageResultFloat.FromMemory( fileData, ColorComponents.RedGreenBlue );

		var imageBytes = new byte[image.Data.Length * sizeof( float )];
		System.Buffer.BlockCopy( image.Data, 0, imageBytes, 0, imageBytes.Length );

		textureBuilder.data = imageBytes;
		textureBuilder.width = (uint)image.Width;
		textureBuilder.height = (uint)image.Height;
		textureBuilder.pixelType = PixelType.Float;
		textureBuilder.pixelFormat = PixelFormat.Rgb;
		textureBuilder.internalFormat = InternalFormat.Rgb16f;
		textureBuilder.path = path;

		return textureBuilder;
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
		var textureBuilder = new TextureBuilder();

		textureBuilder.data = bytes;
		textureBuilder.width = width;
		textureBuilder.height = height;

		return textureBuilder;
	}
}
