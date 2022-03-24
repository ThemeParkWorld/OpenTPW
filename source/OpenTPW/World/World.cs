using OpenTPW.UI;

namespace OpenTPW;

public class World
{
	public static World Current { get; set; }

	public RootPanel Hud { get; set; }
	public Camera Camera { get; set; }

	public World()
	{
		Current = this;

		Camera = new Camera();
		_ = new TestObject();

		Hud = new();
		// Hud.AddChild( new Image( TextureBuilder.FromPath( GameDir.GetPath( "data/Init/1024/Welcome.tga" ) ).Build() ) );
		Hud.AddChild( new Cursor() );

		Event.Register( this );
		Event.Run( Event.Game.LoadAttribute.Name );
	}

	public void Update()
	{
		Entity.All.ForEach( entity => entity.Update() );
	}

	public void Render()
	{
		Entity.All.ForEach( entity => entity.Render() );
	}

	[Event.Game.Load]
	public void OnLoad()
	{
		Entity.All.OfType<Ride>().ToList().ForEach( x => x.VM.Run() );
	}
}
