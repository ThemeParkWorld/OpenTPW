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
		// TODO: Move this to some sort of renderer class
		Gl.Disable( Silk.NET.OpenGL.EnableCap.DepthTest );

		Children.ForEach( child => child.Draw() );

		// TODO: Move this to some sort of renderer class
		Gl.Enable( Silk.NET.OpenGL.EnableCap.DepthTest );
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
