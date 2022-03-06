﻿using Silk.NET.Windowing;

namespace OpenTPW;

internal class Window
{
	private IWindow window;

	public Window()
	{
		var windowOptions = WindowOptions.Default;
		windowOptions.Position = new Point2( 32, 32 );

		window = Silk.NET.Windowing.Window.Create( windowOptions );
		window.Size = new Point2( 1280, 720 );
		window.Title = "OpenTPW";

		window.Load += Window_Load;
		window.Render += Window_Render;
		window.Closing += Window_Closing;
		window.Resize += Window_Resize;

		window.Run();
	}

	private void Window_Resize( Silk.NET.Maths.Vector2D<int> newSize )
	{
	}

	private void Window_Closing()
	{
		window.Dispose();
	}

	private void Window_Render( double deltaTime )
	{
	}

	private void Window_Load()
	{
	}
}
