using System.Drawing;
using System.Numerics;

namespace OpenTPW;

public static class ByteExtension
{
	public static bool GetBit( this byte self, int index )
	{
		var value = ((self >> (7 - index)) & 1) != 0;
		return value;
	}
	public static bool[] GetBits( this byte self, params int[] indices )
	{
		var bits = new bool[indices.Length];
		for ( var i = 0; i < indices.Length; ++i )
		{
			bits[i] = self.GetBit( indices[i] );
		}

		return bits;
	}
}

public static class BoolArrayExtension
{
	public static bool ValuesEqual( this bool[] self, bool[] other )
	{
		for ( var i = 0; i < Math.Min( self.Length, other.Length ); ++i )
		{
			if ( self[i] != other[i] )
				return false;
		}

		return true;
	}
}

public static class MathExtensions
{
	public static int CeilToInt( this float x ) => (int)Math.Ceiling( x );
	public static int FloorToInt( this float x ) => (int)Math.Floor( x );
	public static int RoundToInt( this float x ) => (int)Math.Round( x );

	public static int NearestPowerOf2( this int x ) => NearestPowerOf2( (uint)x );
	public static int NearestPowerOf2( this uint x ) => 1 << (sizeof( uint ) * 8 - BitOperations.LeadingZeroCount( x - 1 ));

	public static float DegreesToRadians( this float degrees ) => degrees * (MathF.PI / 180f);
	public static float RadiansToDegrees( this float radians ) => radians * (180f / MathF.PI);

	public static float Clamp( this float v, float min, float max )
	{
		if ( min > max )
			return max;
		if ( v < min )
			return min;
		return v > max ? max : v;
	}

	public static float LerpTo( this float a, float b, float t ) => a * (1 - t) + b * t;

	public static Vector3 Normalize( this Vector3 vector ) => vector / vector.Length;

	public static Vector3 RandomVector3( float min = 0.0f, float max = 1.0f )
	{
		float x = Random.Shared.NextSingle() * (max - min) + min;
		float y = Random.Shared.NextSingle() * (max - min) + min;
		float z = Random.Shared.NextSingle() * (max - min) + min;

		return new Vector3( x, y, z );
	}

	public static System.Numerics.Vector4 GetColor( string hex )
	{
		var color = ColorTranslator.FromHtml( hex );

		return new System.Numerics.Vector4(
				(color.R / 255f),
				(color.G / 255f),
				(color.B / 255f),
				(color.A / 255f)
		);
	}

	public static Vector4 Column1( this Matrix4x4 matrix ) => new Vector4( matrix.M11, matrix.M21, matrix.M31, matrix.M41 );
	public static Vector4 Column2( this Matrix4x4 matrix ) => new Vector4( matrix.M12, matrix.M22, matrix.M32, matrix.M42 );
	public static Vector4 Column3( this Matrix4x4 matrix ) => new Vector4( matrix.M13, matrix.M23, matrix.M33, matrix.M43 );
	public static Vector4 Column4( this Matrix4x4 matrix ) => new Vector4( matrix.M14, matrix.M24, matrix.M34, matrix.M44 );

	public static Vector3 Right( this Matrix4x4 matrix ) => new Vector3( matrix.M11, matrix.M21, matrix.M31 );
	public static Vector3 Up( this Matrix4x4 matrix ) => new Vector3( matrix.M12, matrix.M22, matrix.M32 );
	public static Vector3 Forward( this Matrix4x4 matrix ) => -new Vector3( matrix.M13, matrix.M23, matrix.M33 );

	// https://stackoverflow.com/a/22733709/8176082
	public enum SizeUnits
	{
		Byte, KB, MB, GB, TB, PB, EB, ZB, YB
	}

	public static string ToSize( this long value, SizeUnits unit ) => (value / (double)Math.Pow( 1024, (long)unit )).ToString( "0.00" ) + unit.ToString();
}

public static class ListExtension
{
	public static T Pop<T>( this IList<T> list )
	{
		T item = list.First();
		list.RemoveAt( 0 );
		return item;
	}

	public static void Push<T>( this IList<T> list, T item )
	{
		list.Add( item );
	}
}
