using OpenTPW.UI;
using Veldrid;

namespace OpenTPW;

public class World
{
	public static World Current { get; set; }
	public RootPanel Hud { get; set; }

	public Sun Sun { get; set; }

	public World()
	{
		Current = this;

		SetupEntities();
		SetupHud();

		Event.Register( this );
		Event.Run( Event.Game.LoadAttribute.Name );
	}

	private void SetupEntities()
	{
		Sun = new Sun() { position = new( 0, 10, 10 ) };
		_ = new Terrain();
		_ = new Sky();
		_ = new Ride( GameDir.GetPath( "/data/levels/jungle/rides/bouncy.wad" ) );
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
