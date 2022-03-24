namespace OpenTPW.UI;

public class RootPanel : Entity
{
	public List<Panel> Children { get; set; } = new();
	public static RootPanel? Instance { get; set; }

	public RootPanel()
	{
		Instance ??= this;
	}

	public override void Render()
	{
		Gl.Disable( Silk.NET.OpenGL.EnableCap.DepthTest ); // Move this to some sort of renderer class

		Children.ForEach( child => child.Draw() );

		Gl.Enable( Silk.NET.OpenGL.EnableCap.DepthTest ); // Move this to some sort of renderer class
	}

	public override void Update()
	{
		Children.ForEach( child => child.Update() );
	}

	public void AddChild<T>( T? obj = null ) where T : Panel
	{
		if ( obj is null )
			obj = Activator.CreateInstance( typeof( T ) ) as T;

		Children.Add( obj );
	}
}
