namespace OpenTPW;

public static class GameDir
{
	/// <summary>
	/// Joins <see cref="Settings.Default.GamePath"/> to <param name="path">path</param>.
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public static string GetPath( string path )
	{
		return Path.Join( Settings.Default.GamePath, path );
	}
}
