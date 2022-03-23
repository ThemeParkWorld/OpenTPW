namespace OpenTPW.UI;

public enum CursorTypes
{
	Cash,
	Busy,
	Camcorder,
	Carry,
	Crosshair,
	End,
	Erase,
	Height,
	Line,
	Move,
	NoGo,
	Normal,
	Path,
	Pick,
	Pivot,
	Place,
	Pylon,
	Queue,
	Rotate,
	Sdn,
	Sup,
	Track
}

public class Cursor : Panel
{
	public static Cursor Current { get; set; }

	private string GetImageName( CursorTypes cursorType )
	{
		if ( cursorType == CursorTypes.Cash )
			return "Cash";

		return "C" + (cursorType.ToString().Substring( 0, 3 ).ToLower());
	}

	private Shader shader;
	private Texture texture;
	private Primitives.Plane plane;

	private CursorTypes cursorType;
	public CursorTypes CursorType
	{
		get => cursorType;
		set
		{
			cursorType = value;
			LoadTexture();
		}
	}

	public Texture Texture => texture;

	public Cursor()
	{
		shader = Shader.Builder.WithVertex( "content/shaders/cursor/cursor.vert" )
							 .WithFragment( "content/shaders/cursor/cursor.frag" )
							 .Build();

		plane = new();
		Current = this;

		CursorType = CursorTypes.Cash;
	}

	private void LoadTexture()
	{
		texture = TextureBuilder.FromPath( GameDir.GetPath( $"data/ui/cursors/{GetImageName( cursorType )}.tga" ) ).Build();
	}

	public override void Update()
	{
		base.Update();

		position = PixelsToNDC( Input.Mouse.Position + new Vector2( 16, 16 ) );
		position -= new Vector2( 0.5f, 0.5f );
		position.Y = -position.Y;
		position *= 2.0f;

		size = PixelsToNDC( new Vector2( 32, 32 ) );
	}

	private Vector2 PixelsToNDC( Vector2 pixels )
	{
		return pixels / (Vector2)Screen.Size;
	}

	public override void Draw()
	{
		if ( this.texture == null )
			return;

		modelMatrix = Silk.NET.Maths.Matrix4X4.CreateScale( size.X, size.Y, 1 ) *
			Silk.NET.Maths.Matrix4X4.CreateTranslation( position.X, position.Y, 1 );

		shader.SetMatrix( "g_mModel", modelMatrix );
		shader.SetInt( "g_iFrame", ((Time.Now * 3).CeilToInt()) % 4 );
		plane.Draw( shader, texture );
	}
}
