using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;

namespace OpenTPW;

/// <summary>
/// Contains code for the instantiation and management of a window, the game editor,
/// ImGUI, inputs, the renderer, and the world itself.
/// </summary>
internal class Window
{
	private IWindow window;

	private Editor? editor;

	private ImGuiController? imgui;
	private IInputContext inputContext;

	private World world;

	public Window()
	{
		var windowOptions = WindowOptions.Default;
		windowOptions.Position = new Point2( 32, 32 );

		window = Silk.NET.Windowing.Window.Create( windowOptions );
		window.Size = new Point2( Settings.Default.GameWindowSize.X, Settings.Default.GameWindowSize.Y );
		window.Title = "OpenTPW";

		Screen.UpdateFrom( window.Size );

		window.Load += Window_Load;
		window.Render += Window_Render;
		window.Closing += Window_Closing;
		window.Resize += Window_Resize;

		window.Run();
	}

	private void Window_Resize( Point2 newSize )
	{
		Gl.Viewport( newSize );

		Screen.UpdateFrom( newSize );
	}

	private void Window_Closing()
	{
		window.Dispose();
	}

	private void Window_Render( double deltaTime )
	{
		Time.UpdateFrom( (float)deltaTime );
		Input.UpdateFrom( inputContext );

		Gl.ClearColor( 0, 0, 0, 1 );
		Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

		world.Update();
		world.Render();

		editor?.Update();
		imgui?.Render();
	}

	private void Window_Load()
	{
		window.SetWindowIcon( IconLoader.LoadIcon( "content/icon-sm.tga" ) );

		Global.Gl = window.CreateOpenGL();
		inputContext = window.CreateInput();
		imgui = new( Global.Gl, window, inputContext );

		world = new();

		editor = new Editor( imgui );

		Gl.Enable( EnableCap.Blend );
		Gl.Enable( EnableCap.DepthTest );
		Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
	}
}
