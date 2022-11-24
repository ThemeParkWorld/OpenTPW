namespace OpenTPW;

public class TextureFile
{
	public Texture? Texture { get; set; }

	public enum CompressionType
	{
		ZLIB = 0x12,
		LZSS = 0x13
	};

	struct Header
	{
		public CompressionType CompressionType { get; set; }
		public byte Version { get; set; }
		public byte BitCount { get; set; }
		public byte Unknown0 { get; set; }

		public short Width { get; set; }
		public short Height { get; set; }
		public short YChannelQuantizationScale { get; set; }
		public short CbChannelQuantizationScale { get; set; }
		public short CrChannelQuantizationScale { get; set; }
		public short AChannelQuantizationScale { get; set; }

		public int BlockSize0 { get; set; }
		public int BlockSize1 { get; set; }

		public int Unknown1 { get; set; }

		public byte[] Block0 { get; set; }
		public byte[] Block1 { get; set; }

		public override string ToString()
		{
			var str = "";

			foreach ( var prop in typeof( Header ).GetProperties() )
			{
				if ( prop.PropertyType == typeof( byte[] ) )
				{
					var propValue = prop?.GetValue( this );
					if ( propValue == null )
						str += $"{prop.Name}: null\n";
					else
						str += $"{prop.Name}: {BitConverter.ToString( propValue as byte[] )}\n";
				}
				else
					str += $"{prop.Name}: {prop.GetValue( this )}\n";
			}

			return str;
		}
	}

	private uint GetAlignedSize( uint size )
	{
		uint value = (size - 1);
		uint n = 1;

		for ( ; (value >>= 1) != 0; )
		{
			++n;
		}

		if ( n < 3 )
		{
			n = 3;
		}

		return (uint)(1 << (int)n);
	}

	struct D4Coefficients
	{
		public float H0;
		public float H1;
		public float H2;
		public float H3;

		public float G0;
		public float G1;
		public float G2;
		public float G3;

		public float IH0;
		public float IH1;
		public float IH2;
		public float IH3;

		public float IG0;
		public float IG1;
		public float IG2;
		public float IG3;

		public D4Coefficients( float scale = 1.0f )
		{
			double s3 = Math.Sqrt( 3.0 );
			double denom = 4 * Math.Sqrt( 2.0 );

			H0 = (float)((1 + s3) / denom) * scale;
			H1 = (float)((3 + s3) / denom) * scale;
			H2 = (float)((3 - s3) / denom) * scale;
			H3 = (float)((1 - s3) / denom) * scale;

			G0 = H3;
			G1 = -H2;
			G2 = H1;
			G3 = -H0;

			IH0 = H2;
			IH1 = G2;
			IH2 = H0;
			IH3 = G0;

			IG0 = H3;
			IG1 = G3;
			IG2 = H1;
			IG3 = G1;
		}
	}

	static float ApplyScalingCoefficientsInv( float smooth0, float coef0, float smooth1, float coef1, D4Coefficients coefs )
	{
		return smooth0 * coefs.IH0 + coef0 * coefs.IH1 +
			smooth1 * coefs.IH2 + coef1 * coefs.IH3;
	}

	static float ApplyWaveCoefficientsInv( float smooth0, float coef0, float smooth1, float coef1, D4Coefficients coefs )
	{
		return smooth0 * coefs.IG0 + coef0 * coefs.IG1 +
			smooth1 * coefs.IG2 + coef1 * coefs.IG3;
	}

	enum D4Component
	{
		Scale,
		Wavelet
	}

	struct ImageDecodeState
	{
		public List<float> DequantizationBuffer;
		public List<float> RowDecodeBuffer;

		public ImageDecodeState( int size )
		{
			DequantizationBuffer = Enumerable.Repeat( 0f, size * size ).ToList();
			RowDecodeBuffer = Enumerable.Repeat( 0f, size * size ).ToList();
		}
	}

	void D4InverseTransform( Func<int, D4Component, int> indexLookup, Func<int, D4Component, int> outputIndexLookup, ref List<float> pSrc, ref List<float> pDest, int offset, int size, D4Coefficients coefs )
	{
		for ( int i = 0; i < size; i++ )
		{
			float s0, w0, s1, w1;
			int s0i, w0i, s1i, w1i;

			if ( i == 0 )
			{
				s0i = indexLookup( size - 1, D4Component.Scale );
				w0i = indexLookup( size - 1, D4Component.Wavelet );
			}
			else
			{
				s0i = indexLookup( i - 1, D4Component.Scale );
				w0i = indexLookup( i - 1, D4Component.Wavelet );
			}

			s1i = indexLookup( i, D4Component.Scale );
			w1i = indexLookup( i, D4Component.Wavelet );

			s0 = pSrc[s0i + offset];
			w0 = pSrc[w0i + offset];
			s1 = pSrc[s1i + offset];
			w1 = pSrc[w1i + offset];

			int s0d = outputIndexLookup( i, D4Component.Scale );
			int w0d = outputIndexLookup( i, D4Component.Wavelet );

			pDest[s0d + offset] = ApplyScalingCoefficientsInv( s0, w0, s1, w1, coefs );
			pDest[w0d + offset] = ApplyWaveCoefficientsInv( s0, w0, s1, w1, coefs );
		}
	}

	private bool DecodeChannel( ref ImageDecodeState state, int size, ref sbyte[] pSrc, ref List<float> outputBuffer, float dequantizationScale )
	{
		int count = size * (size / 2);

		int pSrcIndex = 0;

		//
		// Step 1: Dequantize
		//
		for ( int i = 0; i < count; ++i )
		{
			int val = pSrc[0];
			pSrc = pSrc.Skip( 1 ).ToArray();

			if ( val == sbyte.MinValue )
			{
				val = pSrc[0] | (pSrc[1] << 8);
				pSrc = pSrc.Skip( 2 ).ToArray();
			}

			state.DequantizationBuffer[i * 2] = (float)val;

			val = pSrc[0];
			pSrc = pSrc.Skip( 1 ).ToArray();

			if ( val == sbyte.MinValue )
			{
				val = pSrc[0] | (pSrc[1] << 8);
				pSrc = pSrc.Skip( 2 ).ToArray();
			}

			state.DequantizationBuffer[i * 2 + 1] = (float)val;
		}

		//
		// Step 2: decode rows
		//
		D4Coefficients coefs = new( dequantizationScale );
		for ( int i = 0; i < size; i++ )
		{
			D4InverseTransform(

			// Index lookup
			( index, component ) =>
			{
				int baseIndex = index * 2;
				switch ( component )
				{
					case D4Component.Scale:
						return baseIndex + 0;
					case D4Component.Wavelet:
						return baseIndex + 1;
					default:
						throw new NotImplementedException();
				}
			},

			// Output index lookup
			( index, component ) =>
			{
				switch ( component )
				{
					case D4Component.Scale:
						return index * 2 + 0;
					case D4Component.Wavelet:
						return index * 2 + 1;
					default:
						throw new NotImplementedException();
				}
			},
			ref state.DequantizationBuffer, ref state.RowDecodeBuffer, i * size, size / 2, coefs );
		}

		//
		// Step 3: decode columns
		//
		coefs = new( 1.0f );
		for ( int i = 0; i < size; ++i )
		{
			int sOffset = 0;
			int wOffset = size * (size / 2);
			int colOffset = i;
			D4InverseTransform(

			// Index lookup
			( index, component ) =>
			{
				int baseIndex = index * size;
				switch ( component )
				{
					case D4Component.Scale:
						return baseIndex + colOffset + sOffset;
					case D4Component.Wavelet:
						return baseIndex + colOffset + wOffset;
					default:
						throw new NotImplementedException();
				}
			},

			// Output index lookup
			( index, component ) =>
			{
				int baseIndex = index * 2 * size + colOffset;
				switch ( component )
				{
					case D4Component.Scale:
						return baseIndex;
					case D4Component.Wavelet:
						return baseIndex + size;
					default:
						throw new NotImplementedException();
				}
			},
			ref state.RowDecodeBuffer, ref outputBuffer, 0, size / 2, coefs );
		}

		return true;
	}

	static float ComputeDequantizationScaleY( int n ) => 1.0f - ((float)n * -0.5f);
	static float ComputeDequantizationScaleCbCr( int n ) => 1.0f - ((float)n * -0.25f);
	static float ComputeDequantizationScaleA( int n ) => (float)n + 1.0f;

	public TextureFile( string path )
	{
		//
		// Setup readers
		//
		using var fileStream = File.OpenRead( path );
		using var binaryReader = new BinaryReader( fileStream );

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
			_ = binaryReader.ReadInt16();
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

		if ( header.BlockSize1 > 0 )
			blockData = blockData.Concat( header.Block1 ).ToArray();

		var blockReader = new BitReader( blockData );

		sbyte[] Decompress()
		{
			if ( header.CompressionType == CompressionType.LZSS )
			{
				const int maxSize = 256 * 256 * 3 + 128 * 128 * 3 + 128 * 128 * 3;
				using var outputStream = new MemoryStream( maxSize );
				using var outputWriter = new BinaryWriter( outputStream );

				if ( !LZSS.Decompress( blockReader, outputWriter ) )
				{
					// throw new Exception();
				}

				return outputStream.ToArray().Select( x => (sbyte)x ).ToArray();
			}
			else
			{
				throw new Exception();
			}
		}

		var decompressedBlock0 = Decompress();

		int size = (int)Math.Max( GetAlignedSize( (uint)Math.Max( header.Height, header.Width ) ), 8 );

		List<float> outputY = Enumerable.Repeat( 0f, size * size ).ToList();
		List<float> outputCb = Enumerable.Repeat( 0f, size * size ).ToList();
		List<float> outputCr = Enumerable.Repeat( 0f, size * size ).ToList();
		List<float> outputA = Enumerable.Repeat( 1f, size * size ).ToList();

		ImageDecodeState state = new( size );
		DecodeChannel( ref state, size, ref decompressedBlock0, ref outputY, ComputeDequantizationScaleY( header.YChannelQuantizationScale ) );
		DecodeChannel( ref state, size / 2, ref decompressedBlock0, ref outputCb, ComputeDequantizationScaleCbCr( header.CbChannelQuantizationScale ) );
		DecodeChannel( ref state, size / 2, ref decompressedBlock0, ref outputCr, ComputeDequantizationScaleCbCr( header.CrChannelQuantizationScale ) );

		List<float> output = Enumerable.Repeat( 0f, header.Width * header.Height * 4 ).ToList();

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

				output[((y * header.Width + x) * 4)] = r;
				output[((y * header.Width + x) * 4) + 1] = g;
				output[((y * header.Width + x) * 4) + 2] = b;

				output[((y * header.Width + x) * 4) + 3] = 1.0f;
			}
		}

		List<byte> textureData = Enumerable.Repeat( (byte)0, header.Width * header.Height * 4 ).ToList();
		for ( int i = 0; i < output.Count; i++ )
		{
			textureData[i] = (byte)output[i].Clamp( 0f, 255f );
		}

		// Flip texture
		List<byte> flippedTextureData = Enumerable.Repeat( (byte)0, header.Width * header.Height * 4 ).ToList();
		for ( int y = 0; y < header.Height; y++ )
		{
			for ( int x = 0; x < header.Width; x++ )
			{
				for ( int i = 0; i < 4; i++ )
				{
					flippedTextureData[((y * header.Width + x) * 4) + i] = textureData[(((header.Height - y - 1) * header.Width + x) * 4) + i];
				}
			}
		}

		var texture = TextureBuilder.Default.FromData( flippedTextureData.ToArray(), (uint)header.Width, (uint)header.Height ).Build();
		Texture = texture;
	}
}
