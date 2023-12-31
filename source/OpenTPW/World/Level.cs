using OpenTPW.UI;
using Veldrid;

namespace OpenTPW;

public class Level
{
	internal static Level Current { get; set; }

	public RootPanel Hud { get; set; }
	public Sun SunLight { get; set; }

	public SettingsFile Global { get; private init; }

	// private int fCount = 0;
	// private Image loadingTexture;

	public Level( string levelName )
	{
		Global = new SettingsFile( $"/levels/{levelName}/global.sam" );
		Current = this;

		SetupHud();
		// SetupEntities();

		Event.Register( this );
		Event.Run( Event.Game.LoadAttribute.Name );
	}

	private void SetupEntities()
	{
		SunLight = new Sun() { Position = new( 0, 10, 10 ) };

		_ = new Terrain();
		_ = new Sky();
	}

	private void SetupHud()
	{
		Hud = new();
		Hud.AddChild( new Cursor() );

		Hud.AddChild( new Image( "/data/ui/textures/tpw_logo.wct" ) );
	}

	public void Update()
	{
		Entity.All.ForEach( entity => entity.Update() );
	}

	public void Render( CommandList commandList )
	{	
		Camera.Update();

		Entity.All.ForEach( entity => entity.Render( commandList ) );
	}

	[Event.Game.Load]
	public void OnLoad()
	{
		Entity.All.OfType<Ride>().ToList().ForEach( x => x.VM.Run() );
	}
}
