using System.Diagnostics;
using System.Text;

namespace OpenTPW;

partial class ShaderPreprocessor
{
	/// <summary>
	/// Preprocesses into two segments - Vertex and Fragment - including anything in Common.
	/// </summary>
	internal class ShaderSegmentParser : BaseParser
	{
		public ShaderSegmentParser( string input ) : base( input )
		{
		}

		private string ConsumeBlock()
		{
			var curDepth = 0;
			var sb = new StringBuilder();
			ConsumeWhitespace();

			while ( true )
			{
				var c = ConsumeChar();
				if ( c == '{' )
					curDepth++;

				if ( c == '}' )
					curDepth--;

				sb.Append( c );

				if ( curDepth == 0 )
					return sb.ToString();
			}
		}

		public void Parse( out string vertexShader, out string fragmentShader )
		{
			Dictionary<string, string> blocks = [];

			while ( !EndOfFile() )
			{
				// Find a block
				ConsumeWhitespace();

				if ( EndOfFile() )
					break;

				var name = ConsumeWhile( x => x != '{' || EndOfFile() );
				name = name.Trim();

				var contents = ConsumeBlock();
				contents = contents[1..^1].Trim(); // Remove '{' and '}' along with any whitespace

				blocks.Add( name, contents );
			}

			if ( !blocks.TryGetValue( "vertex", out vertexShader ) )
				throw new Exception("Vertex block is required, but wasn't found!");

			if ( !blocks.TryGetValue( "fragment", out fragmentShader ) )
				throw new Exception( "Fragment block is required, but wasn't found!" );

			if ( blocks.TryGetValue( "common", out var common ) )
			{
				vertexShader = common + "\n\n" + vertexShader;
				fragmentShader = common + "\n\n" + fragmentShader;
			}

			vertexShader = $"#version 450\n\n{vertexShader}";
			fragmentShader = $"#version 450\n\n{fragmentShader}";
		}
	}
}
