using OpenTPW.UI;

namespace OpenTPW;

public class World
{
	public RootPanel Hud { get; set; }

	public World()
	{
		Hud = new();

		// var wctTest = new WctFile( "F:\\TP\\red.wct" );
		// Hud.AddChild( new Image( wctTest.Texture ?? default ) );
		Hud.AddChild( new Image( TextureBuilder.FromPath( GameDir.GetPath( "data/Init/1024/Welcome.tga" ) ).Build() ) );
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
