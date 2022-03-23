namespace OpenTPW;

/// <summary>
/// Handles the creation and management of various systems, including the game
/// window.
/// </summary>
internal class Game
{
    Window window;

    public Game()
    {
        Log.Trace( "Initializing game" );
        window = new Window();
    }
}
