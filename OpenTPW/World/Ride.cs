namespace OpenTPW;

public class Ride : Entity
{
	private WadFileSystem fileSystem;

	private RideVM vm;

	public Ride( string rideArchive )
	{
		fileSystem = new WadFileSystem( rideArchive );
		var rideName = Path.GetFileNameWithoutExtension( rideArchive );

		Log.Trace( $"Loading ride {rideName}" );
		vm = new RideVM( fileSystem.OpenRead( rideName + ".rse" ) );
	}

	public override void Render()
	{
	}

	public override void Update()
	{
	}
}
