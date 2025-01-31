namespace OpenTPW;

public class Time
{
	public static float Delta { get; internal set; }
	public static float Now { get; internal set; }

	public static void Update( float deltaTime )
	{
		Delta = deltaTime;
		Now += deltaTime;
	}
}
