using OpenTPW.UI;
using Veldrid;

namespace OpenTPW;

public class Level
{
	internal static Level Current { get; set; }

	public RootPanel Hud { get; set; }
	public Sun SunLight { get; set; }

	public SettingsFile Global { get; private init; }

	private int fCount = 0;
	private Image loadingTexture;

	public Level( string levelName )
	{
		Global = new SettingsFile( $"/levels/{levelName}/global.sam" );
		Current = this;

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

		var texture = new TextureFile( GameDir.GetPath( "/data/ui/textures/text_grad.wct" ) );
		loadingTexture = Hud.AddChild( new Image( new Texture( texture ) ) );
	}

	public void Update()
	{
		Entity.All.ForEach( entity => entity.Update() );
	}

	public void Render( CommandList commandList )
	{
		if ( fCount == 1 )
		{
			SetupEntities();
			loadingTexture.Delete();
		}

		Camera.Update();

		Entity.All.ForEach( entity => entity.Render( commandList ) );

		fCount++;
	}

	[Event.Game.Load]
	public void OnLoad()
	{
		Entity.All.OfType<Ride>().ToList().ForEach( x => x.VM.Run() );
	}
}
