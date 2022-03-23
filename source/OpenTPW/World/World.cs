using OpenTPW.UI;

namespace OpenTPW;

public class World
{
	public RootPanel Hud { get; set; }

	public World()
	{
		Hud = new();
		Hud.AddChild( new Image( TextureBuilder.FromPath( GameDir.GetPath( "data/Init/1024/Welcome.tga" ) ).Build() ) );

		var ride = new Ride( GameDir.GetPath( "data/levels/jungle/rides/monkey.wad" ) );

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
