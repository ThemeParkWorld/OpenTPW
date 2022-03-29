using Veldrid;

namespace OpenTPW;

public class Texture : Asset
{
	public int Width { get; set; }
	public int Height { get; set; }
	public string Type { get; set; }

	public Veldrid.Texture VeldridTexture { get; }
	public Veldrid.TextureView VeldridTextureView { get; }

	public static TextureBuilder Builder => new();

	internal Texture( string path, Veldrid.Texture texture, Veldrid.TextureView textureView, string type, int width, int height )
	{
		Path = path;
		VeldridTexture = texture;
		VeldridTextureView = textureView;
		Type = type;
		Width = width;
		Height = height;

		All.Add( this );
	}

	public void Bind()
	{

	}
}
