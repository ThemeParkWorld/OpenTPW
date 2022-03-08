namespace OpenTPW;

public static class Screen
{
	public static Point2 Size { get; set; } = new( 1, 1 );

	public static float Aspect => (float)Size.Y / (float)Size.X;

	public static void UpdateFrom( Point2 size )
	{
		Size = size;
	}
}
