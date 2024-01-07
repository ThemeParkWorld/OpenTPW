using System.Diagnostics;
using Veldrid;

namespace OpenTPW;

public partial class Material : Asset
{
	public Shader Shader { get; set; }

	public Type UniformBufferType { get; } = typeof( ObjectUniformBuffer );
	public Pipeline Pipeline { get; private set; } = null!;

	private Dictionary<string, BindableResource> _boundResources = new();

	private ResourceLayout _resourceLayout;
	private ResourceSet _resourceSet;

	public Material( string shaderPath )
	{
		Shader = new Shader( shaderPath );

		All.Add( this );
		SetupResources();
	}

	protected Material( string shaderPath, Type uniformBufferType )
	{
		Shader = new Shader( shaderPath );
		UniformBufferType = uniformBufferType;

		All.Add( this );
		SetupResources();
	}

	private DeviceBuffer UniformBuffer;

	public void Set<T>( string name, T obj ) where T : unmanaged
	{
		Render.ImmediateSubmit( cmd =>
		{
			cmd.UpdateBuffer( UniformBuffer, 0, [obj] );
			_boundResources[name] = UniformBuffer;
		} );
	}

	private static Sampler DefaultSampler = CreateSampler();

	private static Sampler CreateSampler()
	{
		var samplerDescription = new SamplerDescription(
			SamplerAddressMode.Clamp,
			SamplerAddressMode.Clamp,
			SamplerAddressMode.Clamp,
			SamplerFilter.MinLinear_MagLinear_MipLinear,
			ComparisonKind.Always,
			0,
			0,
			0,
			0,
			SamplerBorderColor.TransparentBlack
		);

		return Device.ResourceFactory.CreateSampler( samplerDescription );
	}

	public void Set( string name, Texture texture )
	{
		_boundResources[name] = texture.NativeTexture;
		_boundResources["s_" + name] = DefaultSampler;
	}

	internal ResourceLayout CreateResourceLayout()
	{
		return Device.ResourceFactory.CreateResourceLayout( Shader.ResourceLayout );
	}

	internal ResourceSet CreateResourceSet()
	{
		Debug.Assert( _resourceLayout != null );

		var sortedBoundResources = new List<BindableResource>();

		foreach ( var resource in Shader.ResourceLayout.Elements )
		{
			if ( _boundResources.TryGetValue( resource.Name, out var boundResource ) )
			{
				sortedBoundResources.Add( boundResource );
			}
			else
			{
				throw new Exception( $"{resource.Name} wasn't bound at draw time!" );
			}
		}

		var resourceSetDescription = new ResourceSetDescription()
		{
			Layout = _resourceLayout,
			BoundResources = [.. sortedBoundResources]
		};

		return Device.ResourceFactory.CreateResourceSet( resourceSetDescription );
	}

	internal void CreateResources( out ResourceSet resourceSet, out ResourceLayout resourceLayout )
	{
		// Resource layout doesn't change, but resource set does
		_resourceLayout ??= CreateResourceLayout();
		resourceLayout = _resourceLayout;

		_resourceSet = CreateResourceSet();
		resourceSet = _resourceSet;
	}

	private void SetupResources()
	{
		var vertexLayout = new VertexLayoutDescription( Vertex.VertexElementDescriptions );

		//
		// Create resource layout - but only from what we're using/need
		//
		_resourceLayout ??= CreateResourceLayout();

		//
		// Create pipeline
		//
		var pipelineDescription = new GraphicsPipelineDescription()
		{
			BlendState = BlendStateDescription.SingleAlphaBlend,

			DepthStencilState = new DepthStencilStateDescription(
				true,
				true,
				ComparisonKind.Less
			),

			RasterizerState = new RasterizerStateDescription(
				FaceCullMode.Back,
				PolygonFillMode.Solid,
				FrontFace.Clockwise,
				true,
				false
			),

			PrimitiveTopology = PrimitiveTopology.TriangleList,
			ResourceLayouts = [_resourceLayout],
			ShaderSet = new ShaderSetDescription( [vertexLayout], Shader.ShaderProgram ),
			Outputs = Device.SwapchainFramebuffer.OutputDescription
		};

		Pipeline = Device.ResourceFactory.CreateGraphicsPipeline( pipelineDescription );

		//
		// Create single uniform buffer
		// TODO: dynamic 'scratch' buffer system
		//
		var bufferDescription = new BufferDescription( 16 * 200, BufferUsage.UniformBuffer | BufferUsage.Dynamic );
		UniformBuffer = Device.ResourceFactory.CreateBuffer( bufferDescription );
	}
}

public class Material<T>( string shaderPath ) : Material( shaderPath, typeof( T ) );
