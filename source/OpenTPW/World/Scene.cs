using OpenTPW.UI;
using Veldrid;

namespace OpenTPW;

public class Scene
{
	internal static Scene Current { get; set; }

	public RootPanel Hud { get; set; }
	public Sun SunLight { get; set; }

	public static Scene InitFromLevel( string levelName )
	{
		// todo: load info from levelName sam files
		return new Scene();
	}

	private Scene()
	{
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
