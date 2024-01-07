using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Veldrid;

namespace OpenTPW.UI;

public class Cursor : Panel
{
	public static Cursor Current { get; set; }

	private Texture texture;

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

		return "C" + cursorType.ToString()[..3].ToLower();
	}

	private void LoadTexture()
	{
		var cursorPath = $"/data/ui/cursors/{GetImageName( cursorType )}.tga";
		texture = new Texture( GameDir.GetPath( cursorPath ), TextureCreationFlags.PinkChromaKey );
	}

	protected override void OnRender()
	{
		var material = Material.UI;
		material.Set( "Color", texture );

		var size = new Vector2( 32 );

		var position = Input.Mouse.Position;
		position.Y = Screen.Height - position.Y;
		position.Y -= size.Y;

		var frame = ((Time.Now * 3).CeilToInt()) % 4;
		var uv0 = new Vector2( frame * 0.25f, 0f );
		var uv1 = uv0 + new Vector2( 0.25f, 1f );

		var rect = new Rectangle( position, size );
		var uvRect = new Rectangle( uv0, uv1 - uv0 );

		ImDraw.Quad( rect, uvRect, material );
	}
}
