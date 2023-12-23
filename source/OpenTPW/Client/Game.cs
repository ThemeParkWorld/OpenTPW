namespace OpenTPW;

/// <summary>
/// Handles the creation and management of various systems, including the game
/// window.
/// </summary>
internal static class Game
{
	public static Renderer renderer;

	[Flags]
	public enum InitFlags
	{
		None,
		NoVid
	}

	private static InitFlags ParseInitFlagsFromArgs( string[] args )
	{
		var initFlags = InitFlags.None;

		foreach ( var arg in args )
		{
			if ( arg.StartsWith( "-" ) )
			{
				var flag = arg[1..];
				if ( Enum.TryParse<InitFlags>( flag, true, out var flagBit ) )
				{
					initFlags |= flagBit;
				}
			}
		}

		return initFlags;
	}

	public static void Run( string[] args )
	{
		var initFlags = ParseInitFlagsFromArgs( args );
		Run( initFlags );
	}

	public static void Run( InitFlags initFlags = InitFlags.None )
	{
		Log = new();

		FileSystem = new BaseFileSystem( $"{Settings.Default.GamePath}/data/" );
		FileSystem.RegisterArchiveHandler<WadArchive>( ".wad" );
		FileSystem.RegisterArchiveHandler<SdtArchive>( ".sdt" );

		renderer = new();
		renderer.Run();
	}
}
