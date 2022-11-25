namespace OpenTPW;

public class Ride : Entity
{
	public RideVM VM { get; private set; }

	public Ride( string rideArchive )
	{
		var rideName = Path.GetFileNameWithoutExtension( rideArchive );

		VM = new RideVM( FileSystem.Game.OpenRead( rideArchive + "\\" + rideName + ".rse" ) );
		var settingsFile = new SettingsFile( FileSystem.Game.OpenRead( rideArchive + "\\" + rideName + ".sam" ) );

		Log.Trace( $"Loaded ride {settingsFile.Entries.First( x => x.Key == "Info.Name" ).Value}" );
	}

	public override void Update()
	{
		VM.Update();
	}
}
