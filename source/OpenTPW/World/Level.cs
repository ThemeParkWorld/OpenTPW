using OpenTPW.UI;
using Veldrid;

namespace OpenTPW;

public class Level
{
	internal static Level Current { get; set; }

	public RootPanel Hud { get; set; }
	public Sun SunLight { get; set; }

	public SettingsFile Global { get; private init; }

	public Level( string levelName )
	{
		Global = new SettingsFile( $"data/levels/{levelName}/global.sam" );
		Current = this;

		SetupEntities();
		SetupHud();

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
