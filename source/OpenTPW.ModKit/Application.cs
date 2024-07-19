using OpenTPW;
using Veldrid;
using Veldrid.StartupUtilities;

internal class Application
{
	private Editor editor;
	private GraphicsDevice gd;

	public Application()
	{
		Event.Register( this );
	}

	public void Run()
	{
		if ( !Path.Exists( "C:\\Program Files (x86)\\Bullfrog\\Theme Park World\\Data" ) )
			throw new DirectoryNotFoundException( "Theme Park World not found" );

		FileSystem = new BaseFileSystem( "C:\\Program Files (x86)\\Bullfrog\\Theme Park World\\Data" );
		FileSystem.RegisterArchiveHandler<WadArchive>( ".wad" );
		FileSystem.RegisterArchiveHandler<SdtArchive>( ".sdt" );

		CacheFileSystem = new BaseFileSystem( "C:\\Program Files (x86)\\Bullfrog\\Theme Park World\\Data\\.opentpw" );

		Log = new();

		var window = new Window( 1280, 720, "OpenTPW ModKit" );

		gd = VeldridStartup.CreateGraphicsDevice( window.SdlWindow, GraphicsBackend.Vulkan );

		ImGuiManager = new ImGuiRenderer(
			gd, gd.MainSwapchain.Framebuffer.OutputDescription,
			(int)gd.MainSwapchain.Framebuffer.Width, (int)gd.MainSwapchain.Framebuffer.Height );

		var cl = gd.ResourceFactory.CreateCommandList();

		editor = new( ImGuiManager, gd );

		while ( window.SdlWindow.Exists )
		{
			var input = window.SdlWindow.PumpEvents();

			cl.Begin();
			cl.SetFramebuffer( gd.MainSwapchain.Framebuffer );
			cl.ClearColorTarget( 0, RgbaFloat.Black );

			editor?.UpdateFrom( input );
			ImGuiManager.Render( gd, cl );

			cl.End();
			gd.SubmitCommands( cl );
			gd.SwapBuffers( gd.MainSwapchain );
		}
	}

	[Event.Window.Resized]
	public void OnWindowResized( Point2 newSize )
	{
		ImGuiManager.WindowResized( newSize.X, newSize.Y );
		gd.MainSwapchain.Resize( (uint)newSize.X, (uint)newSize.Y );
	}
}
