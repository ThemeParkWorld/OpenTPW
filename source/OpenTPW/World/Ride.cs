namespace OpenTPW;

public class Ride : Entity
{
	private WadFileSystem fileSystem;
	private FileSystemWatcher watcher;
	private const string testScriptPath = "content/testscripts/test.rse";

	public RideVM VM { get; private set; }

	public Ride( string rideArchive )
	{
		var rideName = Path.GetFileNameWithoutExtension( rideArchive );

		VM = new RideVM( FileSystem.Game.OpenRead( rideArchive + "\\" + rideName + ".rse" ) );
		var settingsFile = new SettingsFile( FileSystem.Game.OpenRead( rideArchive + "\\" + rideName + ".sam" ) );

		Log.Trace( $"Loaded ride {settingsFile.Entries.First( x => x.Item1 == "Info.Name" ).Item2}" );
	}

	public void LoadDebugScript()
	{
		watcher = new FileSystemWatcher( Path.GetDirectoryName( testScriptPath ), Path.GetFileName( testScriptPath ) );

		watcher.NotifyFilter = NotifyFilters.Attributes
							 | NotifyFilters.CreationTime
							 | NotifyFilters.DirectoryName
							 | NotifyFilters.FileName
							 | NotifyFilters.LastAccess
							 | NotifyFilters.LastWrite
							 | NotifyFilters.Size;

		watcher.EnableRaisingEvents = true;
		watcher.Changed += Watcher_Changed;

		OnHotLoad();
	}

	private void Watcher_Changed( object sender, FileSystemEventArgs e )
	{
		Log.Trace( $"File {e.Name} changed" );

		while ( !IsFileReady( e.Name ) ) ;
		Event.Run( Event.FileSystem.HotLoadAttribute.Name );
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

	public override void Update()
	{
		base.Update();

		VM.Update();
	}

	[Event.FileSystem.HotLoad]
	private void OnHotLoad()
	{
		Log.Trace( $"File {testScriptPath} was changed. Reloading VM" );

		using var testScriptStream = File.OpenRead( testScriptPath );
		VM = new RideVM( testScriptStream );
	}
}
