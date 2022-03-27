using Veldrid;
using Veldrid.StartupUtilities;

namespace OpenTPW;

internal class Renderer
{
	public Window window;

	private Editor? editor;

	private ImGuiRenderer imgui;
	private World world;
	private DateTime lastFrame;

	private GraphicsDevice graphicsDevice;
	private CommandList commandList;
	private Pipeline pipeline;

	private ResourceLayout rsrcLayout;
	private Texture missingTexture;

	private ImGuiRenderer imguiRenderer;

	public Renderer()
	{
		window = new();

		lastFrame = DateTime.Now;
		Init();

		MainLoop();
	}

	private void Init()
	{
		CreateGraphicsDevice();
		CreateResources();

		imgui = new( graphicsDevice,
			  graphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
			  window.SdlWindow.Width,
			  window.SdlWindow.Height );
		editor = new Editor( imgui );

		world = new();
	}

	public void CreateResources()
	{
		var factory = graphicsDevice.ResourceFactory;

		//var layout = new VertexLayoutDescription(
		//	new VertexElementDescription( "Position", VertexElementSemantic.TextureCoordinate,
		//		VertexElementFormat.Float3 ),
		//	new VertexElementDescription( "TexCoords", VertexElementSemantic.TextureCoordinate,
		//		VertexElementFormat.Float2 ) );

		// CreateShaders( factory );

		// var pipelineDescription = CreatePipelineDescription();

		// pipelineDescription.ShaderSet = new ShaderSetDescription( new[] { layout }, shaders );

		// pipeline = factory.CreateGraphicsPipeline( pipelineDescription );
		commandList = factory.CreateCommandList();
	}

	private GraphicsPipelineDescription CreatePipelineDescription()
	{
		var rsrcLayoutDesc = new ResourceLayoutDescription()
		{
			Elements = new[]
			{
					new ResourceLayoutElementDescription()
					{
						Kind = ResourceKind.TextureReadOnly,
						Name = "mainTexture",
						Options = ResourceLayoutElementOptions.None,
						Stages = ShaderStages.Fragment
					},
					new ResourceLayoutElementDescription()
					{
						Kind = ResourceKind.Sampler,
						Name = "mainTextureSampler",
						Options = ResourceLayoutElementOptions.None,
						Stages = ShaderStages.Fragment
					},
					new ResourceLayoutElementDescription()
					{
						Kind = ResourceKind.UniformBuffer,
						Name = "ubo",
						Options = ResourceLayoutElementOptions.None,
						Stages = ShaderStages.Vertex
					}
				}
		};

		rsrcLayout = graphicsDevice.ResourceFactory.CreateResourceLayout( rsrcLayoutDesc );

		var pipelineDescription = new GraphicsPipelineDescription();

		pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;

		pipelineDescription.DepthStencilState =
			new DepthStencilStateDescription( true, true, ComparisonKind.Greater );

		pipelineDescription.RasterizerState = new RasterizerStateDescription( FaceCullMode.Back,
			PolygonFillMode.Solid, FrontFace.CounterClockwise, true, false );

		pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleList;

		pipelineDescription.ResourceLayouts = new[] { rsrcLayout };

		pipelineDescription.Outputs = graphicsDevice.SwapchainFramebuffer.OutputDescription;

		return pipelineDescription;
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
		List<ResourceSet> disposableResourceSets = new List<ResourceSet>();

		commandList.Begin();
		commandList.SetFramebuffer( graphicsDevice.SwapchainFramebuffer );
		commandList.ClearColorTarget( 0, RgbaFloat.CornflowerBlue );
		commandList.ClearDepthStencil( 0 );
	}

	private void PostRender()
	{
		commandList.End();
		graphicsDevice.SubmitCommands( commandList );
		graphicsDevice.SwapBuffers();
	}

	private void Render()
	{
		world.Render();
		imgui?.Render( graphicsDevice, commandList );
	}

	private void Update()
	{
		float deltaTime = (float)((DateTime.Now - lastFrame).TotalSeconds);
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
			SwapchainDepthFormat = PixelFormat.D32_Float_S8_UInt
		};

		graphicsDevice = VeldridStartup.CreateGraphicsDevice( Window.Current.SdlWindow,
													   options,
													   GraphicsBackend.Direct3D11 );
	}
}
