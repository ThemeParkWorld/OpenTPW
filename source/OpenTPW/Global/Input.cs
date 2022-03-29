using ImGuiNET;
using Veldrid;

namespace OpenTPW;

public static partial class Input
{
	public static MouseInfo Mouse { get; internal set; } = new();
	public static KeyboardInfo Keyboard { get; internal set; } = new();

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

	public struct KeyboardInfo
	{
		public List<Key> KeysDown { get; set; }

		public KeyboardInfo()
		{
			KeysDown = new();
		}

		public KeyboardInfo( List<Key> keysDown )
		{
			KeysDown = keysDown;
		}
	}

	public static void UpdateFrom( InputSnapshot inputSnapshot )
	{
		var io = ImGui.GetIO();

		if ( inputSnapshot.MousePosition.X < 0 || inputSnapshot.MousePosition.X > Screen.Size.X
			|| inputSnapshot.MousePosition.Y < 0 || inputSnapshot.MousePosition.Y > Screen.Size.Y )
			return;

		if ( io.WantCaptureMouse )
		{
			// mouse.Cursor.CursorMode = CursorMode.Normal;
			Mouse = new MouseInfo();
		}
		else
		{
			// mouse.Cursor.CursorMode = CursorMode.Hidden;

			var mousePos = new Vector2( inputSnapshot.MousePosition.X, inputSnapshot.MousePosition.Y );
			var mouseInfo = new MouseInfo
			{
				Delta = Mouse.Position - mousePos,
				Position = mousePos,
				Left = inputSnapshot.IsMouseDown( MouseButton.Left ),
				Right = inputSnapshot.IsMouseDown( MouseButton.Right ),
				Wheel = inputSnapshot.WheelDelta
			};

			Mouse = mouseInfo;

			Right = 0;
			Forward = 0;

			var newKeysDown = inputSnapshot.KeyEvents.Where( x => x.Down ).Select( x => x.Key );
			var newKeysUp = inputSnapshot.KeyEvents.Where( x => !x.Down ).Select( x => x.Key );

			Keyboard = new KeyboardInfo(
				Keyboard.KeysDown.Concat( newKeysDown )
					.Distinct()
					.Where( x => !newKeysUp.Contains( x ) )
					.ToList()
			);

			bool IsKeyPressed( Key k ) => Keyboard.KeysDown.Contains( k );

			if ( IsKeyPressed( Key.A ) )
				Right -= 1;
			if ( IsKeyPressed( Key.D ) )
				Right += 1;
			if ( IsKeyPressed( Key.W ) )
				Forward += 1;
			if ( IsKeyPressed( Key.S ) )
				Forward -= 1;

			LastKeysDown = KeysDown.ToList();
			KeysDown.Clear();

			if ( IsKeyPressed( Key.F1 ) )
				KeysDown.Add( InputButton.ConsoleToggle );
			if ( IsKeyPressed( Key.Left ) )
				KeysDown.Add( InputButton.RotateLeft );
			if ( IsKeyPressed( Key.Right ) )
				KeysDown.Add( InputButton.RotateRight );
		}
	}
}
