using Silk.NET.Input;

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

	internal static List<InputButton> LastKeysDown { get; set; } = new();
	internal static List<InputButton> KeysDown { get; set; } = new();

	public static bool Pressed( InputButton button )
	{
		return KeysDown.Contains( button ) && !LastKeysDown.Contains( button );
	}

	public static bool Down( InputButton button )
	{
		return KeysDown.Contains( button );
	}

	public static bool Released( InputButton button )
	{
		return !KeysDown.Contains( button ) && LastKeysDown.Contains( button );
	}
}

public enum InputButton
{
	ConsoleToggle
}
