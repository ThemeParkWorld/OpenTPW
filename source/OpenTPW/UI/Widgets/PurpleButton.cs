using OpenTPW.UI;
using Veldrid;

namespace OpenTPW;

internal class PurpleButton : Panel
{
	private Texture ButtonText1;
	private Texture ButtonText2;

	public string Text { get; set; }

	public PurpleButton( string text )
	{
		ButtonText1 = new Texture( "ui/textures/button_text1.wct" );
		ButtonText2 = new Texture( "ui/textures/button_text2.wct" );

		Text = Localization.Parse( text );
		Log.Info( $"Parsed '{text}' as '{Text}'" );
	}

	protected override void OnRender()
	{
		var material = Material.UI;
		var size = new Vector2( 200, 100 );
		var rect = new Rectangle( position, size );

		var uvs = new Rectangle( 0, 0, 1, 0.5f );
		rect.Y -= 130;
		rect.X += 40;

		//
		// Start
		//
		rect.X -= 200;
		material.Set( "Color", ButtonText2 );
		ImDraw.Quad( rect, uvs.Shift( new Vector2( 0, 0.5f ) ), material );

		//
		// Middle
		//
		material.Set( "Color", ButtonText1 );
		rect.X -= 200;
		ImDraw.Quad( rect.Shift( new Vector2( 0, 28 ) ), uvs, material );

		//
		// End
		//
		rect.X -= 200;
		ImDraw.Quad( rect, uvs.Shift( new Vector2( 0, 0.5f ) ), material );
	}
}
