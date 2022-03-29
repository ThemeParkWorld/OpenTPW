using ImGuiNET;
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

			lastFrame = DateTime.Now;
		}
	}

	private void PreRender()
	{
		commandList.Begin();
		commandList.SetFramebuffer( Device.SwapchainFramebuffer );
		commandList.ClearColorTarget( 0, RgbaFloat.CornflowerBlue );
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
		InputSnapshot inputSnapshot = window.SdlWindow.PumpEvents();

		Time.UpdateFrom( deltaTime );
		Input.UpdateFrom( inputSnapshot );

		world.Update();
		editor?.UpdateFrom( inputSnapshot );
	}

	private void CreateGraphicsDevice()
	{
		Device = VeldridStartup.CreateGraphicsDevice( Window.Current.SdlWindow, GraphicsBackend.Vulkan );
		Window.Current.SdlWindow.Title += Device.BackendType.ToString();
	}

	[Event.Window.Resized]
	public void OnWindowResized( Point2 newSize )
	{
		imguiRenderer.WindowResized( newSize.X, newSize.Y );
		Device.MainSwapchain.Resize( (uint)newSize.X, (uint)newSize.Y );
	}
}
