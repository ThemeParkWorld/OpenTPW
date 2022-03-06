namespace OpenTPW;

internal class Game
{
	Window window;

	public Game()
	{
		Log.Trace( "Initializing game" );
		window = new Window();
	}
}
