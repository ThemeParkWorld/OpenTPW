namespace OpenTPW;

public class Ride : Entity
{
	private WadFileSystem fileSystem;

	public RideVM VM { get; private set; }

	public Ride( string rideArchive )
	{
		fileSystem = new WadFileSystem( rideArchive );
		var rideName = Path.GetFileNameWithoutExtension( rideArchive );

		Log.Trace( $"Loading ride {rideName}" );
		// VM = new RideVM( fileSystem.OpenRead( rideName + ".rse" ) );

		using var testScriptStream = File.OpenRead( "content/testscripts/test.rse" );
		VM = new RideVM( testScriptStream );
	}

	public override void Render()
	{
	}

	public override void Update()
	{
	}
}
