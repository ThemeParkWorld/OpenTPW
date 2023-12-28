using Veldrid;

namespace OpenTPW.UI;

public class RootPanel : Entity
{
	public List<Panel> Children { get; set; } = new();
	public static RootPanel? Instance { get; set; }

	public RootPanel()
	{
		Instance ??= this;
	}

	public override void Render( CommandList commandList )
	{
		Children.ForEach( child => child.Draw( commandList ) );
	}

	public override void Update()
	{
		Children.ForEach( child => child.Update() );
	}

	public T AddChild<T>( T? obj = null ) where T : Panel
	{
		if ( obj is null )
			obj = Activator.CreateInstance( typeof( T ) ) as T;

		Children.Add( obj );

		return obj;
	}
}
