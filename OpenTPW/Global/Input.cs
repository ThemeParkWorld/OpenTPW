namespace OpenTPW;

public static class Input
{
	public struct MouseInfo
	{
		public Vector2 Delta;
		public Vector2 Position;

		public bool Left;
		public bool Right;

		public float Wheel;
	}

	public static MouseInfo Mouse { get; internal set; }

	public static float Forward { get; set; }
	public static float Right { get; set; }
}
