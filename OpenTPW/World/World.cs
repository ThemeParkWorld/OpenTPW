using OpenTPW.UI;

namespace OpenTPW;

public class World
{
	public RootPanel Hud { get; set; }

	public World()
	{
		Hud = new();
		Hud.AddChild( new Image( GameDir.GetPath( "data/Init/1024/Welcome.tga" ) ) );
	}

	public void Update()
	{
		Entity.All.ForEach( entity => entity.Update() );
	}

	public void Render()
	{
		Entity.All.ForEach( entity => entity.Render() );
	}
}
