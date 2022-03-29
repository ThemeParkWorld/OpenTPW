using System.Numerics;
using Veldrid;

namespace OpenTPW.UI;

public class Cursor : Panel
{
	public static Cursor Current { get; set; }

	private Model model;

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

	public Texture Texture => model.Material.DiffuseTexture;

	struct ObjectUniformBuffer
	{
		public Matrix4x4 g_mModel;
		public int g_iFrame;
	}

	public Cursor()
	{
		Current ??= this;

		// TODO: Move this to Input
		CursorType = Input.CursorTypes.Normal;
	}

	private string GetImageName( Input.CursorTypes cursorType )
	{
		// bullfrog PLEASE...
		if ( cursorType == Input.CursorTypes.Cash )
			return "Cash";

		return "C" + (cursorType.ToString().Substring( 0, 3 ).ToLower());
	}

	private void LoadTexture()
	{
		// TODO: clean this up
		var material = new Material(
			TextureBuilder.FromPath( GameDir.GetPath( $"data/ui/cursors/{GetImageName( cursorType )}.tga" ) ).UsePointFiltering().Build(),
			Shader.Builder.WithVertex( "content/shaders/cursor/cursor.vert" )
							 .WithFragment( "content/shaders/cursor/cursor.frag" )
							 .Build(),
			typeof( ObjectUniformBuffer )
		);

		model = Primitives.Plane.GenerateModel( material );
	}

	public override void Update()
	{
		base.Update();

		size = new Vector2( 32, 32 );

		var center = size / 2;
		position = Input.Mouse.Position + center;
		position.Y = Screen.Size.Y - position.Y;
	}

	public override void Draw( CommandList commandList )
	{
		base.Draw( commandList );

		if ( this.model == null )
			return;

		var uniformBuffer = new ObjectUniformBuffer()
		{
			g_mModel = modelMatrix,
			g_iFrame = ((Time.Now * 3).CeilToInt()) % 4
		};

		model.Draw( uniformBuffer, commandList );
	}
}
