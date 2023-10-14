using Veldrid;
using Veldrid.StartupUtilities;

namespace OpenTPW;

internal class Renderer
{
	public Window window;

	private Editor? editor;

	private ImGuiRenderer imguiRenderer;
	private World world;
	private DateTime lastFrame;

	private CommandList commandList;

	public Renderer()
	{
		Event.Register( this );
	}

	public void Run()
	{
		Init();

		lastFrame = DateTime.Now;
		MainLoop();
	}

	private void Init()
	{
		window = new();

		CreateGraphicsDevice();

		commandList = Device.ResourceFactory.CreateCommandList();

		imguiRenderer = new( Device,
			  Device.SwapchainFramebuffer.OutputDescription,
			  window.SdlWindow.Width,
			  window.SdlWindow.Height );

		editor = new Editor( imguiRenderer );
		world = new();
	}

	private void MainLoop()
	{
		while ( window.SdlWindow.Exists )
		{
			Update();

			PreRender();
			Render();
			PostRender();
		}
	}

	private void PreRender()
	{
		commandList.Begin();
		commandList.SetFramebuffer( Device.SwapchainFramebuffer );
		commandList.ClearColorTarget( 0, RgbaFloat.Black );
		commandList.ClearDepthStencil( 1 );
	}

	private void PostRender()
	{
		commandList.End();
		Device.SubmitCommands( commandList );
		Device.SwapBuffers();
	}

	private void Render()
	{
		world.Render( commandList );
		imguiRenderer?.Render( Device, commandList );
	}

	private void Update()
	{
		float deltaTime = (float)(DateTime.Now - lastFrame).TotalSeconds;
		lastFrame = DateTime.Now;

		InputSnapshot inputSnapshot = window.SdlWindow.PumpEvents();

		Time.UpdateFrom( deltaTime );
		Input.UpdateFrom( inputSnapshot );

		world.Update();
		editor?.UpdateFrom( inputSnapshot );
	}

	private void CreateGraphicsDevice()
	{
		var options = new GraphicsDeviceOptions()
		{
			PreferStandardClipSpaceYDirection = true,
			PreferDepthRangeZeroToOne = true,
			SwapchainDepthFormat = PixelFormat.D24_UNorm_S8_UInt,
			Debug = true
		};

		var preferredBackend = GraphicsBackend.Vulkan;
		var preferredBackendStr = Settings.Default.PreferredBackend;

		if ( !string.IsNullOrEmpty( preferredBackendStr ) )
		{
			preferredBackend = (GraphicsBackend)Enum.Parse(
				typeof( GraphicsBackend ),
				preferredBackendStr,
				true );
		}

		Device = VeldridStartup.CreateGraphicsDevice( Window.Current.SdlWindow, options, preferredBackend );
		Device.SyncToVerticalBlank = true;

		var windowTitle = $"OpenTPW | {Device.BackendType}";
		Window.Current.SdlWindow.Title = windowTitle;
	}

	[Event.Window.Resized]
	public void OnWindowResized( Point2 newSize )
	{
		imguiRenderer.WindowResized( newSize.X, newSize.Y );
		Device.MainSwapchain.Resize( (uint)newSize.X, (uint)newSize.Y );
	}

	public void ImmediateSubmit( Action<CommandList> action )
	{
		var commandList = Device.ResourceFactory.CreateCommandList();
		commandList.Begin();

		action( commandList );

		commandList.End();
		Device.SubmitCommands( commandList );
	}
}
