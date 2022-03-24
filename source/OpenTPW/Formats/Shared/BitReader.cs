namespace OpenTPW;

/// <summary>
/// Allows reading from a byte array on a bit-by-bit basis.
/// Mainly used for LZSS (de-)compression.
/// </summary>
public class BitReader
{
	private byte[] buffer;
	private int offset;
	private int bitsRemaining;
	private int value;

	public BitReader( byte[] buffer )
	{
		this.buffer = buffer;
		this.offset = 0;
		this.bitsRemaining = 0;
		this.value = 0;
	}

	public int GetBits( int length )
	{
		if ( bitsRemaining < length )
		{
			int fetch = (buffer[offset] | (buffer[offset + 1] << 8)) & 0xFFFF;
			offset += 2;
			Log.Trace( $"Fetch {fetch:X4}" );
			value = value | (fetch << bitsRemaining);
			bitsRemaining += 16;
		}

		int returnValue = value & ((1 << length) - 1);

		bitsRemaining -= length;
		value >>= length;

		return returnValue;
	}

	public int GetOffset()
	{
		return this.offset;
	}
}
