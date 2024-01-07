using Veldrid;
using Veldrid.StartupUtilities;

namespace OpenTPW;

internal class Renderer
{
	private DateTime _lastFrame;

	//
	// Objects
	//
	public Window Window { get; private set; }
	public CommandList CommandList { get; private set; }

	//
	// State
	//
	public bool IsRendering { get; private set; }

	private Editor Editor { get; set; }
	private ImGuiRenderer ImGuiRenderer { get; set; }

	public Renderer()
	{
		Event.Register( this );
	}

	public void Run()
	{
		Init();

		_lastFrame = DateTime.Now;
		MainLoop();
	}

	private void Init()
	{
		Window = new( Settings.Default.GameWindowSize.X, Settings.Default.GameWindowSize.Y, "Theme Park World (OpenTPW)", true );

		CreateGraphicsDevice();
		CommandList = Device.ResourceFactory.CreateCommandList();

		var level = new Level( "fantasy" );
		Log.Info( $"This level costs {level.Global["Keys.CostToEnter"]} keys to enter." );

		Window.Visible = true;

		ImGuiRenderer = new ImGuiRenderer( Device, Device.MainSwapchain.Framebuffer.OutputDescription, Window.Size.X, Window.Size.Y );
		Editor = new Editor( ImGuiRenderer, Device );
	}

	private void MainLoop()
	{
		while ( Window.SdlWindow.Exists )
		{
			Update();

			PreRender();
			Render();
			PostRender();
		}
	}

	private void PreRender()
	{
		CommandList.Begin();
		CommandList.SetFramebuffer( Device.SwapchainFramebuffer );
		CommandList.ClearColorTarget( 0, RgbaFloat.Black );
		CommandList.ClearDepthStencil( 1 );

		IsRendering = true;
	}

	private void PostRender()
	{
		IsRendering = false;

		ImGuiRenderer.Render( Device, CommandList );

		CommandList.End();
		Device.SubmitCommands( CommandList );
		Device.SwapBuffers();
	}

	private void Render()
	{
		Level.Current.Render();
	}

	private void Update()
	{
		float deltaTime = (float)(DateTime.Now - _lastFrame).TotalSeconds;
		_lastFrame = DateTime.Now;

		InputSnapshot inputSnapshot = Window.SdlWindow.PumpEvents();

		Time.UpdateFrom( deltaTime );
		Input.UpdateFrom( inputSnapshot );
		Editor.UpdateFrom( inputSnapshot );

		Level.Current.Update();
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
	}

	[Event.Window.Resized]
	public void OnWindowResized( Point2 newSize )
	{
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
