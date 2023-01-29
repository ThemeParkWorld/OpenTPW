namespace OpenTPW;

partial class TextureFile
{
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
			Array.Copy( pSrc, 1, pSrc, 0, pSrc.Length - 1 );

			if ( val == sbyte.MinValue )
			{
				val = pSrc[0] | (pSrc[1] << 8);
				Array.Copy( pSrc, 2, pSrc, 0, pSrc.Length - 2 );
			}

			state.DequantizationBuffer[i * 2] = (float)val;

			val = pSrc[0];
			Array.Copy( pSrc, 1, pSrc, 0, pSrc.Length - 1 );

			if ( val == sbyte.MinValue )
			{
				val = pSrc[0] | (pSrc[1] << 8);
				Array.Copy( pSrc, 2, pSrc, 0, pSrc.Length - 2 );
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
}
