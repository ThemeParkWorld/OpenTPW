namespace OpenTPW;

public class Ride : Entity
{
	private WadFileSystem fileSystem;
	private FileSystemWatcher watcher;

	public RideVM VM { get; private set; }

	public Ride( string rideArchive )
	{
		fileSystem = new WadFileSystem( rideArchive );
		var rideName = Path.GetFileNameWithoutExtension( rideArchive );

		Log.Trace( $"Loading ride {rideName}" );

		watcher = new FileSystemWatcher( "content/testscripts", "test.rse" );

		watcher.NotifyFilter = NotifyFilters.Attributes
							 | NotifyFilters.CreationTime
							 | NotifyFilters.DirectoryName
							 | NotifyFilters.FileName
							 | NotifyFilters.LastAccess
							 | NotifyFilters.LastWrite
							 | NotifyFilters.Size;
		watcher.EnableRaisingEvents = true;
		watcher.Changed += Watcher_Changed;

		CreateVM();

		// VM = new RideVM( fileSystem.OpenRead( rideName + ".rse" ) );
	}

	private void Watcher_Changed( object sender, FileSystemEventArgs e )
	{
		Log.Trace( $"File {e.Name} changed" );
		CreateVM();
	}
	public static bool IsFileReady( string path )
	{
		try
		{
			using ( FileStream inputStream = File.Open( path, FileMode.Open, FileAccess.Read, FileShare.None ) )
				return inputStream.Length > 0;
		}
		catch ( Exception )
		{
			return false;
		}
	}

	private void CreateVM()
	{
		var path = "content/testscripts/test.rse";
		while ( !IsFileReady( path ) ) ;

		using var testScriptStream = File.OpenRead( path );
		VM = new RideVM( testScriptStream );
	}

	public override void Render()
	{
	}

	public override void Update()
	{
	}
}
