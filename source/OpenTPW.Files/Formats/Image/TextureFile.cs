using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace OpenTPW;

public record struct TextureData( int Width, int Height, byte[] Data );

public partial class TextureFile : BaseFormat
{
	public TextureData Data { get; set; }
	public Header FileHeader { get; private set; }

	static float ComputeDequantizationScaleY( int n ) => 1.0f - ((float)n * -0.5f);
	static float ComputeDequantizationScaleCbCr( int n ) => 1.0f - ((float)n * -0.25f);
	static float ComputeDequantizationScaleA( int n ) => (float)n + 1.0f;

	public TextureFile( Stream stream )
	{
		ReadFromStream( stream );
	}

	public TextureFile( string path )
	{
		try
		{
			ReadFromFile( path );
		}
		catch ( Exception ex )
		{
			Log.Error( $"TextureFile failed: {ex}" );
		}
	}

	protected override void ReadFromStream( Stream stream )
	{
		using var binaryReader = new BinaryReader( stream );

		//
		// Read header
		//
		Header header = new();

		header.CompressionType = (CompressionType)binaryReader.ReadByte();
		header.Version = binaryReader.ReadByte();
		header.BitCount = binaryReader.ReadByte();
		header.Unknown0 = binaryReader.ReadByte();
		header.Width = binaryReader.ReadInt16();
		header.Height = binaryReader.ReadInt16();
		header.YChannelQuantizationScale = binaryReader.ReadInt16();
		header.CbChannelQuantizationScale = binaryReader.ReadInt16();
		header.CrChannelQuantizationScale = binaryReader.ReadInt16();
		header.AChannelQuantizationScale = binaryReader.ReadInt16();

		header.BlockSize0 = binaryReader.ReadInt32();
		header.BlockSize1 = binaryReader.ReadInt32();

		header.Unknown1 = binaryReader.ReadInt32();

		header.Block0 = binaryReader.ReadBytes( header.BlockSize0 );

		if ( header.BlockSize1 > 0 )
		{
			header.Block1 = binaryReader.ReadBytes( header.BlockSize1 );
		}

		//
		// Decode image
		//
		if ( header.CompressionType != CompressionType.LZSS && header.CompressionType != CompressionType.ZLIB )
			throw new Exception();

		if ( header.BlockSize0 == 0 )
			throw new Exception();

		var blockData = header.Block0;
		FileHeader = header;

		byte[] DecompressLZSS( byte[] data )
		{
			const int maxSize = 256 * 256 * 3 + 128 * 128 * 3 + 128 * 128 * 3 + 256 * 256 * 3;
			using var outputStream = new MemoryStream( maxSize );
			using var outputWriter = new BinaryWriter( outputStream );

			var blockReader = new BitReader( data );
			if ( !LZSS.Decompress( blockReader, outputWriter ) )
			{
				throw new Exception( "LZSS failed to decompress" );
			}

			return outputStream.ToArray();
		}

		byte[] DecompressZLIB( byte[] data )
		{
			/*
			 * Theme Park World uses ZLIB 1.1.3
			 * We can therefore deduce that the custom ZLIB header is as follows:
			 * - 4 bytes: "BILZ" magic number
			 * - 4 bytes: decompressed size
			 * - 4 bytes: stream size
			 * - 4 bytes: Constant - MAX_WBITS (15) - from zlib
			 * - 4 bytes: Constant - DEF_MEM_LEVEL (9) - from zlib
			 * - 4 bytes: Unused
			 * - 4 bytes: Unused 
			 */

			// We don't need to use any of this data - so we'll just skip it entirely
			data = data[0x1C..]; // Unused data

			const int maxSize = 256 * 256 * 3 + 128 * 128 * 3 + 128 * 128 * 3 + 256 * 256 * 3;
			using var outputStream = new MemoryStream( maxSize );
			using var outputWriter = new BinaryWriter( outputStream );
			{
				using var inputStream = new MemoryStream( data );
				using var compressed = new InflaterInputStream( inputStream );
				compressed.CopyTo( outputStream );
			}

			return outputStream.ToArray();
		}

		sbyte[] decompressedBlock0 = Array.Empty<sbyte>();
		sbyte[] decompressedBlock1 = Array.Empty<sbyte>();

		if ( header.CompressionType == CompressionType.LZSS )
		{
			decompressedBlock0 = DecompressLZSS( blockData ).Select( x => (sbyte)x ).ToArray();

			if ( header.BlockSize1 > 0 )
				decompressedBlock1 = DecompressLZSS( header.Block1 ).Select( x => (sbyte)x ).ToArray();
		}
		else
		{
			decompressedBlock0 = DecompressZLIB( blockData ).Select( x => (sbyte)x ).ToArray();

			if ( header.BlockSize1 > 0 )
				decompressedBlock1 = DecompressZLIB( header.Block1 ).Select( x => (sbyte)x ).ToArray();
		}

		int size = (int)Math.Max( GetAlignedSize( (uint)Math.Max( header.Height, header.Width ) ), 8 );

		List<float> outputY = Enumerable.Repeat( 0f, size * size ).ToList();
		List<float> outputCb = Enumerable.Repeat( 0f, size * size ).ToList();
		List<float> outputCr = Enumerable.Repeat( 0f, size * size ).ToList();
		List<float> outputA = Enumerable.Repeat( 255f, size * size ).ToList();

		ImageDecodeState state = new( size );

		List<float> output = Enumerable.Repeat( 0f, header.Width * header.Height * 4 ).ToList();

		var decompressedBlockCopy = decompressedBlock0.ToArray();

		bool isHalfScale = FileHeader.CompressionType == CompressionType.ZLIB;

		DecodeChannel( ref state, size, ref decompressedBlock0, ref outputY, ComputeDequantizationScaleY( header.YChannelQuantizationScale ), isHalfScale );
		DecodeChannel( ref state, size / 2, ref decompressedBlock0, ref outputCb, ComputeDequantizationScaleCbCr( header.CbChannelQuantizationScale ), isHalfScale );
		DecodeChannel( ref state, size / 2, ref decompressedBlock0, ref outputCr, ComputeDequantizationScaleCbCr( header.CrChannelQuantizationScale ), isHalfScale );

		if ( header.BlockSize1 > 0 )
			DecodeChannel( ref state, size, ref decompressedBlock1, ref outputA, ComputeDequantizationScaleA( header.AChannelQuantizationScale ), isHalfScale, true );

		uint alignedWidth = GetAlignedSize( (uint)header.Width );
		uint alignedHeight = GetAlignedSize( (uint)header.Height );

		int maxSize = (int)(alignedWidth < alignedHeight ? alignedHeight : alignedWidth);
		for ( int y = 0; y < header.Height; y++ )
		{
			for ( int x = 0; x < header.Width; x++ )
			{
				float cy = outputY[y * maxSize + x];
				float cb = outputCb[((y / 2) * (maxSize / 2) + (x / 2))];
				float cr = outputCr[((y / 2) * (maxSize / 2) + (x / 2))];

				float r = cy + 1.402f * (cr);
				float g = cy - 0.344136f * (cb) - 0.714136f * (cr);
				float b = cy + 1.772f * (cb);
				float a = outputA[y * maxSize + x];

				output[((y * header.Width + x) * 4)] = r;
				output[((y * header.Width + x) * 4) + 1] = g;
				output[((y * header.Width + x) * 4) + 2] = b;

				output[((y * header.Width + x) * 4) + 3] = a;
			}
		}

		List<byte> textureData = Enumerable.Repeat( (byte)0, header.Width * header.Height * 4 ).ToList();
		for ( int i = 0; i < output.Count; i++ )
		{
			textureData[i] = (byte)output[i].Clamp( 0, 255 );
		}

		Data = new TextureData( header.Width, header.Height, textureData.ToArray() );
	}
}
