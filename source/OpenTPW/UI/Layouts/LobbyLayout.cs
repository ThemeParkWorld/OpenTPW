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

		List<string> saves = new();

		if ( Directory.Exists( GameDir.GetPath( "save/users" ) ) )
			saves = Directory.EnumerateDirectories( GameDir.GetPath( "save/users" ) ).ToList();

		for ( int i = 0; i < 4; ++i )
		{
			var offset = new Vector2( 0, i * 100 );
			var buttonText = "#CreateNewPlayer";

			if ( saves.Count > i )
			{
				buttonText = Path.GetFileName( saves[i] )[1..] ?? "#CreateNewPlayer";
			}

			Hud.AddChild( new PurpleButton( buttonText )
			{
				position = topRight - offset
			} );
		}

		Hud.AddChild( new PurpleButton( "#QuitGame" )
		{
			position = topRight + new Vector2( 200, -425 )
		} );
	}
}
