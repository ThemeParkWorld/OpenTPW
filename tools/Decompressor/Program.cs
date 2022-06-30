using CommandLine;
using OpenTPW;

namespace Decompressor;

public static class Program
{
	public enum CompressionMethod
	{
		LZSS,
		Refpack
	};

	public class Options
	{
		[Option( 'm', "method", Required = true )]
		public CompressionMethod Method { get; set; }

		[Option( 'f', "file", Required = true )]
		public string File { get; set; }
	}

	public static void Main( string[] args )
	{
		Parser.Default.ParseArguments<Options>( args )
			.WithParsed<Options>( o =>
			{
				using var outputStream = File.OpenWrite( "output.bin" );
				var data = File.ReadAllBytes( o.File );
				switch ( o.Method )
				{
					case CompressionMethod.LZSS:
						{
							using var binaryWriter = new BinaryWriter( outputStream );
							var bitReader = new BitReader( data );
							LZSS.Decompress( bitReader, binaryWriter );
							Console.WriteLine( "Decompressed using LZSS." );
							break;
						}
					case CompressionMethod.Refpack:
						{
							var refpack = new Refpack( data );
							outputStream.Write( refpack.Decompress().ToArray() );
							Console.WriteLine( "Decompressed using refpack." );
							break;
						}
				}
			} );
	}
}
