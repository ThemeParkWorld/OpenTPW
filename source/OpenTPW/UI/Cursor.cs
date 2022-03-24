namespace OpenTPW.UI;

public class Cursor : Panel
{
	public static Cursor Current { get; set; }

	private string GetImageName( Input.CursorTypes cursorType )
	{
		// bullfrog PLEASE...
		if ( cursorType == Input.CursorTypes.Cash )
			return "Cash";

		return "C" + (cursorType.ToString().Substring( 0, 3 ).ToLower());
	}

	private Shader shader;
	private Texture texture;
	private Primitives.Plane plane;

	private Input.CursorTypes cursorType;
	public Input.CursorTypes CursorType
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
		Current ??= this;

		shader = Shader.Builder.WithVertex( "content/shaders/cursor/cursor.vert" )
							 .WithFragment( "content/shaders/cursor/cursor.frag" )
							 .Build();

		plane = new();

		// TODO: Move this to Input
		CursorType = Input.CursorTypes.Normal;
	}

	private void LoadTexture()
	{
		texture = TextureBuilder.FromPath( GameDir.GetPath( $"data/ui/cursors/{GetImageName( cursorType )}.tga" ) ).UsePointFiltering().Build();
	}

	public override void Update()
	{
		base.Update();

		size = new Vector2( 32, 32 );

		var center = size / 2;
		position = Input.Mouse.Position + center;
		position.Y = Screen.Size.Y - position.Y;
	}

	public override void Draw()
	{
		base.Draw();

		if ( this.texture == null )
			return;

		shader.SetMatrix( "g_mModel", modelMatrix );
		shader.SetInt( "g_iFrame", ((Time.Now * 3).CeilToInt()) % 4 );
		plane.Draw( shader, texture );
	}
}
