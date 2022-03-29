using System.Runtime.InteropServices;
using Veldrid;

namespace OpenTPW;

public class Model
{
	private Shader shader;
	private DeviceBuffer uniformBuffer;
	private Pipeline pipeline;
	private ResourceSet resourceSet;

	public DeviceBuffer VertexBuffer { get; private set; }
	public DeviceBuffer IndexBuffer { get; private set; }

	public Material Material { get; private set; }

	public Model( Vertex[] vertices, uint[] indices, Material material )
	{
		SetupMesh( vertices, indices );
		SetupResources( material );

		Material = material;
	}

	private void SetupMesh( Vertex[] vertices, uint[] indices )
	{
		var factory = Device.ResourceFactory;

		var vertexStructSize = (uint)Marshal.SizeOf( typeof( Vertex ) );

		VertexBuffer = factory.CreateBuffer(
			new Veldrid.BufferDescription( (uint)vertices.Length * vertexStructSize, Veldrid.BufferUsage.VertexBuffer )
		);
		IndexBuffer = factory.CreateBuffer(
			new Veldrid.BufferDescription( (uint)indices.Length * sizeof( uint ), Veldrid.BufferUsage.IndexBuffer )
		);

		Device.UpdateBuffer( VertexBuffer, 0, vertices );
		Device.UpdateBuffer( IndexBuffer, 0, indices );
	}

	private void SetupResources( Material material )
	{
		shader = Shader.Builder.WithVertex( "content/shaders/test.vert" )
							 .WithFragment( "content/shaders/test.frag" )
							 .Build();

		var vertexLayout = new VertexLayoutDescription( Vertex.VertexElementDescriptions );

		var rsrcLayoutDesc = new ResourceLayoutDescription()
		{
			Elements = new[]
			{
				new ResourceLayoutElementDescription()
				{
					Kind = ResourceKind.TextureReadOnly,
					Name = "g_tDiffuse",
					Options = ResourceLayoutElementOptions.None,
					Stages = ShaderStages.Fragment
				},
				new ResourceLayoutElementDescription()
				{
					Kind = ResourceKind.Sampler,
					Name = "g_sDiffuse",
					Options = ResourceLayoutElementOptions.None,
					Stages = ShaderStages.Fragment
				},
				new ResourceLayoutElementDescription()
				{
					Kind = ResourceKind.UniformBuffer,
					Name = "g_oUbo",
					Options = ResourceLayoutElementOptions.None,
					Stages = ShaderStages.Vertex
				}
			}
		};

		var rsrcLayout = Device.ResourceFactory.CreateResourceLayout( rsrcLayoutDesc );

		var pipelineDescription = new GraphicsPipelineDescription()
		{
			BlendState = BlendStateDescription.SingleOverrideBlend,
			DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
			RasterizerState = RasterizerStateDescription.Default,
			PrimitiveTopology = PrimitiveTopology.TriangleList,
			ResourceLayouts = new[] { rsrcLayout },
			ShaderSet = new ShaderSetDescription( new[] { vertexLayout }, shader.ShaderProgram ),
			Outputs = Device.SwapchainFramebuffer.OutputDescription
		};

		pipeline = Device.ResourceFactory.CreateGraphicsPipeline( pipelineDescription );

		uniformBuffer = Device.ResourceFactory.CreateBuffer(
			new BufferDescription( 4 * (uint)Marshal.SizeOf( typeof( ObjectUniformBuffer ) ),
				BufferUsage.UniformBuffer ) );

		var resourceSetDescription = new ResourceSetDescription( rsrcLayout, material.DiffuseTexture.VeldridTexture, Device.Aniso4xSampler, uniformBuffer );
		resourceSet = Device.ResourceFactory.CreateResourceSet( resourceSetDescription );
	}

	public void Draw( ObjectUniformBuffer uniformBufferContents, CommandList commandList )
	{
		commandList.SetVertexBuffer( 0, VertexBuffer );
		commandList.SetIndexBuffer( IndexBuffer, IndexFormat.UInt32 );
		commandList.SetPipeline( pipeline );

		Device.UpdateBuffer( uniformBuffer, 0, new[] { uniformBufferContents } );

		commandList.SetGraphicsResourceSet( 0, resourceSet );
		commandList.DrawIndexed(
			indexCount: (uint)Primitives.Plane.Indices.Length,
			instanceCount: 1,
			indexStart: 0,
			vertexOffset: 0,
			instanceStart: 0
		);
	}
}
