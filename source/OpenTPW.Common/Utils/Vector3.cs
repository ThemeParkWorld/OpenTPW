using System.Text;

namespace OpenTPW;

public struct Vector3 : IEquatable<Vector3>
{
	private System.Numerics.Vector3 _internalVector;

	public float X
	{
		readonly get => _internalVector.X;
		set => _internalVector.X = value;
	}

	public float Y
	{
		readonly get => _internalVector.Y;
		set => _internalVector.Y = value;
	}

	public float Z
	{
		readonly get => _internalVector.Z;
		set => _internalVector.Z = value;
	}

	public static readonly Vector3 One = new( 1f );

	public static readonly Vector3 Zero = new( 0f );

	public static readonly Vector3 Forward = new( 1f, 0f, 0f );

	public static readonly Vector3 Backward = -Forward;

	public static readonly Vector3 Up = new( 0f, 0f, 1f );

	public static readonly Vector3 Down = -Up;

	public static readonly Vector3 Right = new( 0f, -1f, 0f );

	public static readonly Vector3 Left = -Right;

	public static readonly Vector3 OneX = new( 1f, 0f, 0f );

	public static readonly Vector3 OneY = new( 0f, 1f, 0f );

	public static readonly Vector3 OneZ = new( 0f, 0f, 1f );

	public readonly float Length => _internalVector.Length();
	public readonly float LengthSquared => _internalVector.LengthSquared();

	public readonly Vector3 Normal => (Length == 0) ? new( 0 ) : (this / Length);
	public void Normalize()
	{
		this = Normal;
	}

	public Vector3( Vector2 v2 )
	{
		_internalVector = new System.Numerics.Vector3( v2.X, v2.Y, 0 );
	}

	public Vector3( float x, float y, float z )
	{
		_internalVector = new System.Numerics.Vector3( x, y, z );
	}

	public Vector3( Vector3 other ) : this( other.X, other.Y, other.Z )
	{

	}

	public Vector3( float all = 0.0f ) : this( all, all, all )
	{

	}

	public static implicit operator Vector3( System.Numerics.Vector3 value ) => new Vector3( value.X, value.Y, value.Z );

	public static Vector3 operator +( Vector3 a, Vector3 b ) => new Vector3( a.X + b.X, a.Y + b.Y, a.Z + b.Z );

	public static Vector3 operator +( Vector3 a, float b ) => new Vector3( a.X + b, a.Y + b, a.Z + b );

	public static Vector3 operator -( Vector3 a, Vector3 b ) => new Vector3( a.X - b.X, a.Y - b.Y, a.Z - b.Z );

	public static Vector3 operator -( Vector3 a, float b ) => new Vector3( a.X - b, a.Y - b, a.Z - b );

	public static Vector3 operator *( Vector3 a, float f ) => new Vector3( a.X * f, a.Y * f, a.Z * f );

	public static Vector3 operator *( Vector3 a, Vector3 b ) => new Vector3( a.X * b.X, a.Y * b.Y, a.Z * b.Z );

	public static Vector3 operator *( float f, Vector3 a ) => new Vector3( a.X * f, a.Y * f, a.Z * f );

	public static Vector3 operator *( Vector3 a, System.Numerics.Matrix4x4 transform ) => System.Numerics.Vector3.Transform( a.GetSystemVector3(), transform );

	public static Vector3 operator /( Vector3 a, float f ) => new Vector3( a.X / f, a.Y / f, a.Z / f );

	public static Vector3 operator -( Vector3 value ) => new Vector3( 0f - value.X, 0f - value.Y, 0f - value.Z );

	public static bool operator ==( Vector3 left, Vector3 right ) => left.Equals( right );

	public static bool operator !=( Vector3 left, Vector3 right ) => !(left == right);

	public override bool Equals( object? obj )
	{
		if ( obj is Vector3 vec )
		{
			return vec.X == X && vec.Y == Y && vec.Z == Z;
		}

		return false;
	}

	public bool Equals( Vector3 other ) => _internalVector.Equals( other._internalVector );

	public static Vector3 Cross( Vector3 a, Vector3 b ) => new Vector3( a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X );

	public readonly Vector3 Cross( Vector3 b ) => new Vector3( Y * b.Z - Z * b.Y, Z * b.X - X * b.Z, X * b.Y - Y * b.X );

	public static float Dot( Vector3 a, Vector3 b ) => System.Numerics.Vector3.Dot( a._internalVector, b._internalVector );

	public readonly float Dot( Vector3 b ) => Dot( this, b );

	public readonly float Distance( Vector3 target ) => DistanceBetween( this, target );

	public static float DistanceBetween( Vector3 a, Vector3 b ) => (b - a).Length;

	public static Vector3 Reflect( Vector3 direction, Vector3 normal ) => direction - 2f * Dot( direction, normal ) * normal;

	public readonly Vector3 WithX( float x ) => new Vector3( x, Y, Z );
	public readonly Vector3 WithY( float y ) => new Vector3( X, y, Z );
	public readonly Vector3 WithZ( float z ) => new Vector3( X, Y, z );

	public static float GetAngle( Vector3 a, Vector3 b )
	{
		float dot = a.Dot( b );
		dot /= (a.Length * b.Length);

		float rad = MathF.Acos( dot );
		float angle = MathExtensions.RadiansToDegrees( rad );

		return angle;
	}

	public override int GetHashCode() => HashCode.Combine( _internalVector );

	public System.Numerics.Vector3 GetSystemVector3() => _internalVector;

	public static Vector3 OrthoNormalize( Vector3 normal, Vector3 tangent )
	{
		normal = normal.Normal;
		var right = normal.Cross( tangent ).Normal;
		return right.Cross( normal ).Normal;
	}

	public Vector3 LerpTo( Vector3 b, float delta )
	{
		return new(
				this.X.LerpTo( b.X, delta ),
				this.Y.LerpTo( b.Y, delta ),
				this.Z.LerpTo( b.Z, delta )
		);
	}

	public override string ToString()
	{
		var sb = new StringBuilder();

		sb.Append( "( " );

		sb.Append( $"X: {X}, " );
		sb.Append( $"Y: {Y}, " );
		sb.Append( $"Z: {Z}" );

		sb.Append( " )" );

		return sb.ToString();
	}
}
