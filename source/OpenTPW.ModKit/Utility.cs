using System.Diagnostics;

namespace OpenTPW.ModKit;

internal static class Utility
{
	public static void LaunchImHex( string targetFile )
	{
		var executableName = "C:\\Program Files\\ImHex\\imhex.exe";

		var process = new Process
		{
			StartInfo = new ProcessStartInfo( executableName, $"\"{targetFile}\"" )
		};

		process.Start();
	}

	public static void LaunchExplorer( string targetFile )
	{
		var process = new Process
		{
			StartInfo = new ProcessStartInfo( "explorer.exe", $"/select,\"{targetFile}\"" )
		};

		process.Start();
	}

	public static void DumpFile( string targetFile )
	{
		var fileContent = FileSystem.ReadAllBytes( targetFile );

		var fileName = Path.GetFileName( targetFile );
		var dest = "dumps/" + fileName + ".dump";

		System.IO.Directory.CreateDirectory( "dumps" );
		System.IO.File.WriteAllBytes( dest, fileContent );

		LaunchExplorer( dest );
	}
}
