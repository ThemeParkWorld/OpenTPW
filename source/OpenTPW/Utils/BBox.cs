namespace OpenTPW;

public struct BBox
{
	public Vector3 Mins { get; set; }
	public Vector3 Maxs { get; set; }

	public BBox()
	{
		Mins = default;
		Maxs = default;
	}

	public BBox( Vector3 mins, Vector3 maxs )
	{
		Mins = mins;
		Maxs = maxs;
	}
}
