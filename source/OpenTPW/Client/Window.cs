using Veldrid;
using Veldrid.StartupUtilities;
using Veldrid.Sdl2;

namespace OpenTPW;

/// <summary>
/// Contains code for the instantiation and management of a window, the game editor,
/// ImGUI, inputs, the renderer, and the world itself.
/// </summary>
internal class Window
{
	public static Window Current { get; set; }

	public Sdl2Window SdlWindow { get; private set; }

	public Point2 Size => new Point2( SdlWindow.Width, SdlWindow.Height );

	public Window()
	{
		Current ??= this;

		var windowCreateInfo = new WindowCreateInfo()
		{
			WindowWidth = Settings.Default.GameWindowSize.X,
			WindowHeight = Settings.Default.GameWindowSize.Y,
			WindowTitle = "OpenTPW",
			X = 32,
			Y = 32
		};

		SdlWindow = VeldridStartup.CreateWindow( windowCreateInfo );

		Screen.UpdateFrom( Size );

		SdlWindow.Resized += SdlWindow_Resized;
	}

	private void SdlWindow_Resized()
	{
		Screen.UpdateFrom( Size );
		Event.Run( Event.Window.ResizedAttribute.Name, Size );
	}
}
