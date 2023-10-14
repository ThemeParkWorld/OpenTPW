namespace OpenTPW;

/// <summary>
/// Handles the creation and management of various systems, including the game
/// window.
/// </summary>
internal class Game
{
	public static Renderer renderer;

	public Game()
	{
		Log.Trace( "Initializing game" );
		renderer = new();

		renderer.Run();
	}
}
