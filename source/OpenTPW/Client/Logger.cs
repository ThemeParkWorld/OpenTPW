namespace OpenTPW;

/// <summary>
/// Handles all debug logging functionality.
/// </summary>
public class Logger
{
	public enum Level
	{
		Trace,
		Info,
		Warning,
		Error
	};

	public void Trace( object obj ) => Log( obj?.ToString(), Level.Trace );
	public void Info( object obj ) => Log( obj?.ToString(), Level.Info );
	public void Warning( object obj ) => Log( obj?.ToString(), Level.Warning );
	public void Error( object obj ) => Log( obj?.ToString(), Level.Error );

	public delegate void LogDelegate( Level severity, string logText );
	public static LogDelegate? OnLog;

	private static void Log( string? str, Level severity = Level.Trace )
	{
		if ( str == null )
			return;

#if RELEASE
		if ( severity == Level.Error )
			throw new Exception( str );
#endif

		Console.ForegroundColor = SeverityToConsoleColor( severity );
		Console.WriteLine( $"[{DateTime.Now.ToLongTimeString()}] {str}" );

		OnLog?.Invoke( severity, str );
	}

	private static ConsoleColor SeverityToConsoleColor( Level severity )
	{
		switch ( severity )
		{
			case Level.Error:
				return ConsoleColor.DarkRed;
			case Level.Warning:
				return ConsoleColor.Red;
			case Level.Trace:
				return ConsoleColor.DarkGray;
			case Level.Info:
				return ConsoleColor.White;
		}

		return ConsoleColor.White;
	}
}
