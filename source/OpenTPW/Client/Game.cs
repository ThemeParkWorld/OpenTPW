namespace OpenTPW;

/// <summary>
/// Handles the creation and management of various systems, including the game
/// window.
/// </summary>
internal static class Game
{
	[Flags]
	public enum InitFlags
	{
		None,

		/// <summary>
		/// Don't show intro videos or logos
		/// </summary>
		NoVid,

		/// <summary>
		/// Enables the editor inside the game
		/// </summary>
		Editor
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

		if ( !Path.Exists( $"{Settings.Default.GamePath}/data/" ) )
			throw new DirectoryNotFoundException( "Theme Park World not found" );

		FileSystem = new BaseFileSystem( $"{Settings.Default.GamePath}/data/" );
		FileSystem.RegisterArchiveHandler<WadArchive>( ".wad" );
		FileSystem.RegisterArchiveHandler<SdtArchive>( ".sdt" );

		if ( !Path.Exists( $"{Settings.Default.GamePath}/save/" ) )
			Directory.CreateDirectory( $"{Settings.Default.GamePath}/save/" );

		SaveFileSystem = new BaseFileSystem( $"{Settings.Default.GamePath}/save/" );

		CacheFileSystem = new BaseFileSystem( $"./.opentpw" );

		Render = new();
		Render.Run();
	}
}
