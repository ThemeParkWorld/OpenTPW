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

	static float g_Scale0 = (float)(((MathF.Sqrt( 3.0f ) + 1.0f) / 4.0f) / MathF.Sqrt( 2 ));
	static float g_Scale1 = (float)(((MathF.Sqrt( 3.0f ) + 3.0f) / 4.0f) / MathF.Sqrt( 2 ));
	static float g_Scale2 = (float)(((3.0 - MathF.Sqrt( 3.0f )) / 4.0f) / MathF.Sqrt( 2 ));
	static float g_Scale3 = (float)(((1.0 - MathF.Sqrt( 3.0f )) / 4.0f) / MathF.Sqrt( 2 ));

	static float g_Scale0_ = (float)(((MathF.Sqrt( 3.0f ) + 1.0f) / 4.0f));
	static float g_Scale1_ = (float)(((MathF.Sqrt( 3.0f ) + 3.0f) / 4.0f));
	static float g_Scale2_ = (float)(((3.0 - MathF.Sqrt( 3.0f )) / 4.0f));
	static float g_Scale3_ = (float)(((1.0 - MathF.Sqrt( 3.0f )) / 4.0f));

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

	static float ApplyScalingCoefficientsInv( float valueA, float valueB, float prevValueA, float prevValueB, D4Coefficients coefs )
	{
		return prevValueA * coefs.IH0 + prevValueB * coefs.IH1 + valueA * coefs.IH2 + valueB * coefs.IH3;
	}

	static float ApplyWaveCoefficientsInv( float valueA, float valueB, float prevValueA, float prevValueB, D4Coefficients coefs )
	{
		return prevValueA * coefs.IG0 + prevValueB * coefs.IG1 + valueA * coefs.IG2 + valueB * coefs.IG3;
	}

	static float ApplyScalingCoefficientsInv2( float smoothVal, float coef, float previousSmoothVal, float previousCoef, D4Coefficients coefs )
	{
		return previousSmoothVal * coefs.IH0 + previousCoef * coefs.IH1 +
			smoothVal * coefs.IH2 + coef * coefs.IH3;
	}

	static float ApplyWaveCoefficientsInv2( float smoothVal, float coef, float previousSmoothVal, float previousCoef, D4Coefficients coefs )
	{
		return previousSmoothVal * coefs.IG0 + previousCoef * coefs.IG1 +
			smoothVal * coefs.IG2 + coef * coefs.IG3;
	}

	static float ApplyScalingCoefficientsInv3( float smooth0, float coef0, float smooth1, float coef1, D4Coefficients coefs )
	{
		return smooth0 * coefs.IH0 + coef0 * coefs.IH1 +
			smooth1 * coefs.IH2 + coef1 * coefs.IH3;
	}

	static float ApplyWaveCoefficientsInv3( float smooth0, float coef0, float smooth1, float coef1, D4Coefficients coefs )
	{
		return smooth0 * coefs.IG0 + coef0 * coefs.IG1 +
			smooth1 * coefs.IG2 + coef1 * coefs.IG3;
	}

	private void D4InverseTransform( Func<float> fetchFn, int size, D4Coefficients coefs, ref List<float> pOutput )
	{
		int i, j, outIndex = 0;
		int halfSize = size / 2;

		for ( i = 0; i < size; i++ )
		{
			float valueA = fetchFn.Invoke();
			float valueB = fetchFn.Invoke();

			float previousA = valueA;
			float previousB = valueB;

			for ( j = 0; j < halfSize; j++ )
			{
				valueA = fetchFn.Invoke();
				valueB = fetchFn.Invoke();

				pOutput[outIndex + j * 2] = ApplyScalingCoefficientsInv( valueA, valueB, previousA, previousB, coefs );
				pOutput[outIndex + j * 2 + 1] = ApplyWaveCoefficientsInv( valueA, valueB, previousA, previousB, coefs );

				previousA = valueA;
				previousB = valueB;
			}

			pOutput[outIndex + j * 2] = ApplyScalingCoefficientsInv( valueA, valueB, previousA, previousB, coefs );
			pOutput[outIndex + j * 2 + 1] = ApplyWaveCoefficientsInv( valueA, valueB, previousA, previousB, coefs );

			outIndex += size;
		}
	}

	void D4InverseTransform2( int size, ref List<float> pSrc, ref List<float> pDest, D4Coefficients coefs )
	{
		int halfSize = size / 2;
		int i = 0, j = 0;

		int offsetA = 0;
		int offsetB = size;
		int offsetC = size * halfSize;
		int offsetD = size + size * halfSize;

		int outputIndexStart = size * 2;
		int inputIndexStart = 0;

		for ( i = 0; i < halfSize - 1; i++ )
		{
			for ( j = 0; j < size - 1; j++ )
			{
				pDest[outputIndexStart + j * 2] = ApplyScalingCoefficientsInv(
					pSrc[inputIndexStart + j + offsetB], pSrc[inputIndexStart + j + offsetD],
					pSrc[inputIndexStart + j + offsetA], pSrc[inputIndexStart + j + offsetC],
					coefs );

				pDest[outputIndexStart + j * 2 + 1] = ApplyWaveCoefficientsInv(
					pSrc[inputIndexStart + j + offsetB], pSrc[inputIndexStart + j + offsetD],
					pSrc[inputIndexStart + j + offsetA], pSrc[inputIndexStart + j + offsetC],
					coefs );
			}
		}

		pDest[outputIndexStart + j * 2] = ApplyScalingCoefficientsInv(
			pSrc[inputIndexStart + j + offsetB], pSrc[inputIndexStart + j + offsetD],
			pSrc[inputIndexStart + j + offsetA], pSrc[inputIndexStart + j + offsetC],
			coefs );

		pDest[outputIndexStart + j * 2 + 1] = ApplyWaveCoefficientsInv(
			pSrc[inputIndexStart + j + offsetB], pSrc[inputIndexStart + j + offsetD],
			pSrc[inputIndexStart + j + offsetA], pSrc[inputIndexStart + j + offsetC],
			coefs );
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

	void D4InverseTransform3( Func<int, D4Component, int> indexLookup, Func<int, D4Component, int> outputIndexLookup, ref List<float> pSrc, ref List<float> pDest, int offset, int size, D4Coefficients coefs )
	{
		int i;

		for ( i = 0; i < size; i++ )
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

			pDest[s0d + offset] = ApplyScalingCoefficientsInv3( s0, w0, s1, w1, coefs );
			pDest[w0d + offset] = ApplyWaveCoefficientsInv3( s0, w0, s1, w1, coefs );
		}
	}

	private bool DecodeChannel( ref ImageDecodeState state, int size, ref int[] pSrc, ref List<float> outputBuffer, float dequantizationScale )
	{
		int i = 0, count;
		count = size * (size / 2);

		int pSrcIndex = 0;

		//
		// Step 1: Dequantize
		//
		for ( i = 0; i < count; i++ )
		{
			//if ( pSrcEnd - pSrc < sizeof( int8_t ) )
			//{
			//	return false;
			//}

			//int32_t val = *(pSrc++);
			int val = pSrc[pSrcIndex++];

			if ( val == char.MinValue )
			{
				//	if ( pSrcEnd - pSrc < sizeof( int16_t ) )
				//	{
				//		return false;
				//	}

				//	val = *(int16_t*)pSrc;
				//	pSrc += sizeof( int16_t );
				val = pSrc[pSrcIndex++];
			}

			state.DequantizationBuffer[i * 2] = (float)val;

			//if ( pSrcEnd - pSrc < sizeof( int8_t ) )
			//{
			//	return false;
			//}

			val = pSrc[pSrcIndex++];

			if ( val == char.MinValue )
			{
				//	if ( pSrcEnd - pSrc < sizeof( int16_t ) )
				//	{
				//		return false;
				//	}

				//	val = *(int16_t*)pSrc;
				//	pSrc += sizeof( int16_t );
				val = pSrc[pSrcIndex++];
			}

			state.DequantizationBuffer[i * 2 + 1] = (float)val;
		}

		//
		// Step 2: decode rows
		//
		D4Coefficients coefs = new( dequantizationScale );
		for ( i = 0; i < size; i++ )
		{
			D4InverseTransform3(

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
		for ( i = 0; i < size; ++i )
		{
			int sOffset = 0;
			int wOffset = size * (size / 2);
			int colOffset = i;
			D4InverseTransform3(

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

		Log.Trace( header );

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

		byte[] Decompress()
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

				Log.Trace( BitConverter.ToString( outputStream.ToArray() ).Replace( "-", " " ) );
				return outputStream.ToArray();
			}
			else
			{
				throw new Exception();
			}
		}

		var decompressedBlock0 = Decompress().Select( x => (int)x ).ToArray();
		// var decompressedBlock1 = Decompress().Select( x => (int)x ).ToArray();

		int size = (int)Math.Max( GetAlignedSize( (uint)Math.Max( header.Height, header.Width ) ), 8 );

		List<float> outputY = Enumerable.Repeat( 0f, size * size ).ToList();
		List<float> outputCb = Enumerable.Repeat( 0f, size * size ).ToList();
		List<float> outputCr = Enumerable.Repeat( 0f, size * size ).ToList();
		List<float> outputA = Enumerable.Repeat( 1f, size * size ).ToList();

		float float8 = 1.0f - ((float)header.YChannelQuantizationScale * -0.5f);
		float floatA = 1.0f - ((float)header.CbChannelQuantizationScale * -0.25f);
		float floatC = 1.0f - ((float)header.CrChannelQuantizationScale * -0.25f);
		float floatE = (float)header.AChannelQuantizationScale + 1.0f;

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

				float r = cy + 1.402f * (cr - 128.0f);
				float g = cy - 0.344136f * (cb - 128.0f) - 0.714136f * (cr - 128.0f);
				float b = cy + 1.772f * (cb - 128.0f);

				Log.Trace( $"YCbCr: {cy},{cb},{cr} :: RGB: {r},{g},{b}" );

				output[((y * header.Width + x) * 4)] = r;
				output[((y * header.Width + x) * 4) + 1] = g;
				output[((y * header.Width + x) * 4) + 2] = b;

				output[((y * header.Width + x) * 4) + 3] = 1.0f;
			}
		}

		List<byte> textureData = Enumerable.Repeat( (byte)0, header.Width * header.Height * 4 ).ToList();
		for ( int i = 0; i < output.Count; i++ )
		{
			textureData[i] = (byte)decompressedBlock0[i];
		}

		var texture = TextureBuilder.FromBytes( textureData.ToArray(), (uint)header.Width, (uint)header.Height ).Build();
		this.Texture = texture;
	}
}
