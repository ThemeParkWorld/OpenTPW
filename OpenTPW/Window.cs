using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;

namespace OpenTPW;

internal class Window
{
	private IWindow window;

	private Shader? testShader;
	private Texture? testTexture;
	private Primitives.Plane? testPlane;

	private Editor? editor;

	private ImGuiController? imgui;
	private IInputContext inputContext;

	public Window()
	{
		var windowOptions = WindowOptions.Default;
		windowOptions.Position = new Point2( 32, 32 );

		window = Silk.NET.Windowing.Window.Create( windowOptions );
		window.Size = new Point2( Settings.Default.GameWindowSize.X, Settings.Default.GameWindowSize.Y );
		window.Title = "OpenTPW";

		window.Load += Window_Load;
		window.Render += Window_Render;
		window.Closing += Window_Closing;
		window.Resize += Window_Resize;

		window.Run();
	}

	private void Window_Resize( Silk.NET.Maths.Vector2D<int> newSize )
	{
		Gl.Viewport( newSize );
	}

	private void Window_Closing()
	{
		window.Dispose();
	}

	private void Window_Render( double deltaTime )
	{
		Time.Delta = (float)deltaTime;
		Time.Now += (float)deltaTime;

		var io = ImGui.GetIO();
		if ( !io.WantCaptureMouse )
		{
			var mouse = inputContext.Mice.First();
			var mousePos = new Vector2( mouse.Position.X, mouse.Position.Y );

			var mouseInfo = new Input.MouseInfo
			{
				Delta = Input.Mouse.Position - mousePos,
				Position = mousePos,
				Left = mouse.IsButtonPressed( MouseButton.Left ),
				Right = mouse.IsButtonPressed( MouseButton.Right ),
				Wheel = mouse.ScrollWheels.First().Y
			};
			Input.Mouse = mouseInfo;

			var keyboard = inputContext.Keyboards.First();

			Input.Right = 0;
			Input.Forward = 0;

			if ( keyboard.IsKeyPressed( Key.A ) )
				Input.Right -= 1;
			if ( keyboard.IsKeyPressed( Key.D ) )
				Input.Right += 1;
			if ( keyboard.IsKeyPressed( Key.W ) )
				Input.Forward += 1;
			if ( keyboard.IsKeyPressed( Key.S ) )
				Input.Forward -= 1;

			Input.LastKeysDown = Input.KeysDown.ToList();
			Input.KeysDown.Clear();
			if ( keyboard.IsKeyPressed( Key.F1 ) )
				Input.KeysDown.Add( InputButton.ConsoleToggle );
		}
		else
		{
			Input.Mouse = new Input.MouseInfo();
		}

		Gl.ClearColor( 1, 0, 1, 1 );
		Gl.Clear( ClearBufferMask.ColorBufferBit );

		testPlane?.Draw( testShader, testTexture );

		editor?.Update();

		{
			ImGui.Text( $"Time: {Time.Now}" );

			ImGui.Text( $"Last keys down:" );
			foreach ( var key in Input.LastKeysDown )
			{
				ImGui.Text( $"\t{key}" );
			}

			ImGui.Text( $"\n\nKeys down:" );
			foreach ( var key in Input.KeysDown )
			{
				ImGui.Text( $"\t{key}" );
			}
		}

		imgui?.Render();
	}

	private void Window_Load()
	{
		Global.Gl = window.CreateOpenGL();
		inputContext = window.CreateInput();
		imgui = new( Global.Gl, window, inputContext );

		testPlane = new();
		testShader = Shader.Builder.WithVertex( "content/shaders/test.vert" )
							 .WithFragment( "content/shaders/test.frag" )
							 .Build();

		testTexture = TextureBuilder.FromPath( GameDir.GetPath( "data/Init/1024/Splash_English.tga" ) )
							  .UseSrgbFormat( false )
							  .Build();

		var testArchive = new WadArchive( GameDir.GetPath( "data/lobby.wad" ) );
		testArchive.Files.ForEach( f => Log.Trace( f.Name ?? "Unnamed file" ) );

		var testSettings = new SettingsFile( GameDir.GetPath( "data/Online.sam" ) );
		testSettings.Entries.ForEach( e => Log.Trace( $"{e.Item1}: {e.Item2}" ) );

		editor = new Editor( imgui );
	}
}
