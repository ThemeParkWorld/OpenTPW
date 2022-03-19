namespace OpenTPW;

public struct TimeSince : IEquatable<TimeSince>
{
	private float time;

	public float Absolute => time;
	public float Relative => this;

	public static implicit operator float( TimeSince other )
	{
		return Time.Now - other.time;
	}

	public static implicit operator TimeSince( float other )
	{
		TimeSince result = new();
		result.time = Time.Now - other;

		return result;
	}

	public override bool Equals( object? obj )
	{
		if ( obj is TimeSince o )
			return Equals( o );

		return false;
	}

	public bool Equals( TimeSince o ) => time == o.time;

	public static bool operator ==( TimeSince a, TimeSince b ) => a.Equals( b );
	public static bool operator !=( TimeSince a, TimeSince b ) => !a.Equals( b );

	public override int GetHashCode() => base.GetHashCode();
	public override string ToString() => Relative.ToString();
}
