using OpenTPW.UI;

namespace OpenTPW;

internal sealed class LobbyLayout : Layout
{
	public override void OnInit()
	{
		Hud.AddChild( new Image( "/ui/textures/tpw_logo.wct" )
		{
			Uv0 = new Vector2( 0, 0.6f ),
			Uv1 = new Vector2( 1, 1 ),

			Position = new Vector2( 25 ),
			Size = new Vector2( 180f, 77.4f )
		} );

		var topRight = new Vector2( Screen.Width, Screen.Height );

		for ( int i = 0; i < 4; ++i )
		{
			var offset = new Vector2( 0, i * 100 );

			Hud.AddChild( new PurpleButton( "Create New Player" )
			{
				position = topRight - offset
			} );
		}

		Hud.AddChild( new PurpleButton( "Quit Game" )
		{
			position = topRight + new Vector2( 200, -425 )
		} );
	}
}
