using OpenTPW.UI;

namespace OpenTPW;

public class BitmapText : Panel
{
	private char[,] Characters { get; } = new char[,]
	{
		{ ' ', '!', '"', '#', '$', '%', '&', '\'','(', ')', '*', '+', ',', '-', '.', '/', },
		{ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?', },
		{ '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', },
		{ 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '[', '\\',']', '^', '_', },
		{ '`', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', },
		{ 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '{', '|', '}', '~', '€', }
	};

	private int cols = 16;
	private int rows = 10;

	private string Label = "Hello, World!";

	private Shader shader;
	private Texture texture;
	private Primitives.Plane plane;

	public BitmapText( string text )
	{
		shader = Shader.Builder.WithVertex( "content/shaders/text/text.vert" )
							 .WithFragment( "content/shaders/text/text.frag" )
							 .Build();

		texture = TextureBuilder.FromPath( GameDir.GetPath( "data/debug/xfont.tga" ) ).UsePointFiltering().UseSrgbFormat( false ).Build();
		plane = new();

		Label = text;
	}

	public override void Draw()
	{
		base.Draw();

		float characterWidth = 1 / (float)cols;
		float characterHeight = 1 / 10.66667f;

		float cursorPos = 0f;

		float scale = 0.15f;
		float spacing = 2f;

		float aspect = 1024f / 768f;

		position = new Silk.NET.Maths.Vector2D<float>( -0.95f, 0.9f );

		foreach ( var character in Label )
		{
			Point2 index = new Point2( -1, -1 );

			for ( int j = Characters.GetLowerBound( 0 ); j <= Characters.GetUpperBound( 0 ); j++ )
				for ( int k = Characters.GetLowerBound( 1 ); k <= Characters.GetUpperBound( 1 ); k++ )
					if ( Characters[j, k] == character )
						index = new Point2( k, j );

			if ( index.X >= 0 && index.Y >= 0 )
			{
				modelMatrix = Silk.NET.Maths.Matrix4X4.CreateScale( characterWidth * scale, characterHeight * scale * aspect, 1 ) *
					Silk.NET.Maths.Matrix4X4.CreateTranslation( position.X + cursorPos, position.Y, 1 );

				float atlasX = (index.X + 0) * characterWidth;
				float atlasY = (index.Y + 1) * -characterHeight;

				shader.SetVector2( "g_vPos", new Vector2( atlasX, atlasY ) );
				shader.SetVector2( "g_vSize", new Vector2( characterWidth, characterHeight ) );
				shader.SetMatrix( "g_mModel", modelMatrix );
				plane.Draw( shader, texture );

				cursorPos += characterWidth * spacing * scale;
			}
		}
	}
}
