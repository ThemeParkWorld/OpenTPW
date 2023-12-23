namespace OpenTPW;

public class BaseParser
{
	protected int Position { get; set; }
	protected string Input { get; set; }

	public BaseParser( string input )
	{
		Input = input;
		Position = 0;
	}

	protected void Assert( bool condition )
	{
		if ( !condition )
			throw new Exception( "Assert failed" );
	}

	protected char NextChar()
	{
		return Input[Position];
	}

	protected bool StartsWith( string str )
	{
		return string.Join( "", Input.Skip( Position ).Take( str.Length ) ).StartsWith( str );
	}

	protected bool EndOfFile()
	{
		return Position >= Input.Length;
	}

	protected char ConsumeChar()
	{
		return Input[Position++];
	}

	protected string ConsumeWhile( Func<char, bool> condition )
	{
		var result = "";

		while ( !EndOfFile() && condition.Invoke( NextChar() ) )
		{
			result += ConsumeChar();
		}

		return result;
	}

	protected void ConsumeWhitespace()
	{
		ConsumeWhile( x => char.IsWhiteSpace( x ) );
	}
}
