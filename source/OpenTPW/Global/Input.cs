using ImGuiNET;
using Silk.NET.Input;

namespace OpenTPW;

public static class Input
{
	public enum CursorTypes
	{
		Cash,
		Busy,
		Camcorder,
		Carry,
		Crosshair,
		End,
		Erase,
		Height,
		Line,
		Move,
		NoGo,
		Normal,
		Path,
		Pick,
		Pivot,
		Place,
		Pylon,
		Queue,
		Rotate,
		Sdn,
		Sup,
		Track
	}

	public struct MouseInfo
	{
		public Vector2 Delta;
		public Vector2 Position;

		public bool Left;
		public bool Right;

		public float Wheel;

		public override string ToString()
		{
			var str = "";

			foreach ( var field in typeof( MouseInfo ).GetFields() )
				str += $"{field.Name}: {field.GetValue( this )}\n";

			return str;
		}
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

	public static void UpdateFrom( IInputContext inputContext )
	{
		var io = ImGui.GetIO();
		var mouse = inputContext.Mice.First();

		if ( io.WantCaptureMouse )
		{
			mouse.Cursor.CursorMode = CursorMode.Normal;
			Mouse = new MouseInfo();
		}
		else
		{
			mouse.Cursor.CursorMode = CursorMode.Hidden;

			var mousePos = new Vector2( mouse.Position.X, mouse.Position.Y );
			var mouseInfo = new MouseInfo
			{
				Delta = Mouse.Position - mousePos,
				Position = mousePos,
				Left = mouse.IsButtonPressed( MouseButton.Left ),
				Right = mouse.IsButtonPressed( MouseButton.Right ),
				Wheel = mouse.ScrollWheels.First().Y
			};

			Mouse = mouseInfo;

			var keyboard = inputContext.Keyboards.First();

			Right = 0;
			Forward = 0;

			if ( keyboard.IsKeyPressed( Key.A ) )
				Right -= 1;
			if ( keyboard.IsKeyPressed( Key.D ) )
				Right += 1;
			if ( keyboard.IsKeyPressed( Key.W ) )
				Forward += 1;
			if ( keyboard.IsKeyPressed( Key.S ) )
				Forward -= 1;

			LastKeysDown = KeysDown.ToList();
			KeysDown.Clear();

			if ( keyboard.IsKeyPressed( Key.F1 ) )
				KeysDown.Add( InputButton.ConsoleToggle );
		}
	}
}

public enum InputButton
{
	ConsoleToggle
}
