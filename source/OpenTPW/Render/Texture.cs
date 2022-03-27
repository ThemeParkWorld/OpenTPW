namespace OpenTPW;

public class Texture : Asset
{
	public int Width { get; set; }
	public int Height { get; set; }

	internal uint Id { get; set; }

	public string Type { get; set; }

	public static TextureBuilder Builder => new();

	internal Texture( string path, uint id, string type, int width, int height )
	{
		Path = path;
		Id = id;
		Type = type;
		Width = width;
		Height = height;

		All.Add( this );
	}

	public void Bind()
	{

	}
}
