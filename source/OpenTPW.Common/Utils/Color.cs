namespace OpenTPW
{
	public struct Color : IEquatable<Color>
	{
		private Vector4 internalColor;

		public float R
		{
			readonly get => internalColor.X;
			set => internalColor.X = value;
		}

		public float G
		{
			readonly get => internalColor.Y;
			set => internalColor.Y = value;
		}

		public float B
		{
			readonly get => internalColor.Z;
			set => internalColor.Z = value;
		}

		public float A
		{
			readonly get => internalColor.W;
			set => internalColor.W = value;
		}

		public static readonly Color Black = new( 0f, 0f, 0f, 1f );
		public static readonly Color White = new( 1f, 1f, 1f, 1f );
		public static readonly Color Red = new( 1f, 0f, 0f, 1f );
		public static readonly Color Green = new( 0f, 1f, 0f, 1f );
		public static readonly Color Blue = new( 0f, 0f, 1f, 1f );

		public readonly float Length => internalColor.Length;
		public readonly float LengthSquared => internalColor.LengthSquared;

		public Color( float r, float g, float b, float a )
		{
			internalColor = new Vector4( r, g, b, a );
		}

		public Color( Color other ) : this( other.R, other.G, other.B, other.A )
		{

		}

		public Color( float all = 0.0f ) : this( all, all, all, 1f )
		{

		}

		internal Color( Vector4 other ) : this( other.X, other.Y, other.Z, other.W )
		{

		}

		public static implicit operator Color( Vector4 value ) => new Color( value.X, value.Y, value.Z, value.W );
		public static implicit operator Vector4( Color value ) => new Vector4( value.R, value.G, value.B, value.A );

		public static Color operator +( Color a, Color b ) => new Color( a.R + b.R, a.G + b.G, a.B + b.B, a.A + b.A );
		public static Color operator -( Color a, Color b ) => new Color( a.R - b.R, a.G - b.G, a.B - b.B, a.A - b.A );

		public static bool operator ==( Color left, Color right ) => left.Equals( right );
		public static bool operator !=( Color left, Color right ) => !(left == right);

		public override bool Equals( object? obj )
		{
			if ( obj is Color col )
			{
				return col.R == R && col.G == G && col.B == B && col.A == A;
			}

			return false;
		}

		public bool Equals( Color other ) => Equals( other );

		public override int GetHashCode() => HashCode.Combine( internalColor );
		public override string ToString() => internalColor.ToString();

		public Vector4 GetSystemVector4() => internalColor;

		public static Color Lerp( Color a, Color b, float t )
		{
			return new Color(
				a.R.LerpTo( b.R, t ),
				a.G.LerpTo( b.G, t ),
				a.B.LerpTo( b.B, t ),
				a.A.LerpTo( b.A, t )
			);
		}
	}
}
