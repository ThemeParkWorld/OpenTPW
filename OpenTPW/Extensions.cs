public static class ByteExtension
{
	public static bool GetBit( this byte @byte, int index )
	{
		var value = ((@byte >> (7 - index)) & 1) != 0;
		return value;
	}
	public static bool[] GetBits( this byte @byte, params int[] indices )
	{
		var bits = new bool[indices.Length];
		for ( var i = 0; i < indices.Length; ++i )
		{
			bits[i] = @byte.GetBit( indices[i] );
		}
		return bits;
	}
}

public static class BoolArrayExtension
{
	public static bool ValuesEqual( this bool[] a, bool[] b )
	{
		for ( var i = 0; i < Math.Min( a.Length, b.Length ); ++i )
		{
			if ( a[i] != b[i] ) return false;
		}
		return true;
	}
}
