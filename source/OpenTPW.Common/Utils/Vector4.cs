namespace OpenTPW;

public struct Vector4 : IEquatable<Vector4>
{
	private System.Numerics.Vector4 internalVector;

	public float X
	{
		readonly get => internalVector.X;
		set => internalVector.X = value;
	}

	public float Y
	{
		readonly get => internalVector.Y;
		set => internalVector.Y = value;
	}

	public float Z
	{
		readonly get => internalVector.Z;
		set => internalVector.Z = value;
	}

	public float W
	{
		readonly get => internalVector.W;
		set => internalVector.W = value;
	}

	public static readonly Vector4 One = new( 1f );
	public static readonly Vector4 Zero = new( 0f );
	public static readonly Vector4 UnitX = new( 1f, 0f, 0f, 0f );
	public static readonly Vector4 UnitY = new( 0f, 1f, 0f, 0f );
	public static readonly Vector4 UnitZ = new( 0f, 0f, 1f, 0f );
	public static readonly Vector4 UnitW = new( 0f, 0f, 0f, 1f );

	public readonly float Length => internalVector.Length();
	public readonly float LengthSquared => internalVector.LengthSquared();

	public readonly Vector4 Normal => (Length == 0) ? new( 0 ) : (this / Length);
	public void Normalize()
	{
		this = Normal;
	}

	public Vector4( Vector3 xyz, float w = 0.0f ) : this( xyz.X, xyz.Y, xyz.Z, w )
	{
	}

	public Vector4( float x, float y, float z, float w )
	{
		internalVector = new System.Numerics.Vector4( x, y, z, w );
	}

	public Vector4( Vector4 other ) : this( other.X, other.Y, other.Z, other.W )
	{
	}

	public Vector4( float all = 0.0f ) : this( all, all, all, all )
	{
	}

	internal Vector4( System.Numerics.Vector4 other ) : this( other.X, other.Y, other.Z, other.W )
	{
	}

	public static implicit operator Vector4( System.Numerics.Vector4 value ) => new Vector4( value.X, value.Y, value.Z, value.W );
	public static implicit operator System.Numerics.Vector4( Vector4 value ) => new System.Numerics.Vector4( value.X, value.Y, value.Z, value.W );

	public static Vector4 operator +( Vector4 a, Vector4 b ) => new Vector4( a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W );
	public static Vector4 operator +( Vector4 a, float b ) => new Vector4( a.X + b, a.Y + b, a.Z + b, a.W + b );
	public static Vector4 operator -( Vector4 a, Vector4 b ) => new Vector4( a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W );
	public static Vector4 operator -( Vector4 a, float b ) => new Vector4( a.X - b, a.Y - b, a.Z - b, a.W - b );
	public static Vector4 operator *( Vector4 a, float f ) => new Vector4( a.X * f, a.Y * f, a.Z * f, a.W * f );
	public static Vector4 operator *( Vector4 a, Vector4 b ) => new Vector4( a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W );
	public static Vector4 operator *( float f, Vector4 a ) => new Vector4( a.X * f, a.Y * f, a.Z * f, a.W * f );
	public static Vector4 operator *( Vector4 a, System.Numerics.Matrix4x4 transform ) => System.Numerics.Vector4.Transform( a, transform );
	public static Vector4 operator /( Vector4 a, float f ) => new Vector4( a.X / f, a.Y / f, a.Z / f, a.W / f );
	public static Vector4 operator -( Vector4 value ) => new Vector4( 0f - value.X, 0f - value.Y, 0f - value.Z, 0f - value.W );

	public static bool operator ==( Vector4 left, Vector4 right ) => left.Equals( right );
	public static bool operator !=( Vector4 left, Vector4 right ) => !(left == right);

	public override bool Equals( object? obj )
	{
		if ( obj is Vector4 vec )
		{
			return vec.X == X && vec.Y == Y && vec.Z == Z && vec.W == W;
		}
		return false;
	}

	public bool Equals( Vector4 other ) => Equals( other );

	public static float Dot( Vector4 a, Vector4 b ) => System.Numerics.Vector4.Dot( a.internalVector, b.internalVector );
	public readonly float Dot( Vector4 b ) => Dot( this, b );

	public readonly float Distance( Vector4 target ) => DistanceBetween( this, target );
	public static float DistanceBetween( Vector4 a, Vector4 b ) => (b - a).Length;

	public readonly Vector4 WithX( float x_ ) => new Vector4( x_, Y, Z, W );
	public readonly Vector4 WithY( float y_ ) => new Vector4( X, y_, Z, W );
	public readonly Vector4 WithZ( float z_ ) => new Vector4( X, Y, z_, W );
	public readonly Vector4 WithW( float w_ ) => new Vector4( X, Y, Z, w_ );

	public override int GetHashCode() => HashCode.Combine( internalVector );

	public override string ToString() => internalVector.ToString();

	public System.Numerics.Vector4 GetSystemVector4() => internalVector;

	public static Vector4 Lerp( Vector4 a, Vector4 b, float t )
	{
		return new Vector4(
			a.X.LerpTo( b.X, t ),
			a.Y.LerpTo( b.Y, t ),
			a.Z.LerpTo( b.Z, t ),
			a.W.LerpTo( b.W, t )
		);
	}
}
