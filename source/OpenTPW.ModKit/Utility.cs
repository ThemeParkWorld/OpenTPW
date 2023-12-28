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
}
