using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace OpenTPW;

internal class Window
{
	private IWindow window;

	private Shader testShader;
	private Texture testTexture;
	private Primitives.Plane testPlane;

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
		Gl.ClearColor( 1, 0, 1, 1 );
		Gl.Clear( ClearBufferMask.ColorBufferBit );

		testPlane.Draw( testShader, testTexture );
	}

	private void Window_Load()
	{
		Global.Gl = window.CreateOpenGL();

		testPlane = new();
		testShader = Shader.Builder.WithVertex( "content/shaders/test.vert" ).WithFragment( "content/shaders/test.frag" ).Build();
		testTexture = TextureBuilder.FromPath( GameDir.GetPath( "data/Init/1024/Splash_English.tga" ) )
							  .UseSrgbFormat( false )
							  .Build();
	}
}
