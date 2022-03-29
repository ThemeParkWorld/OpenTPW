using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid;

namespace OpenTPW.UI;

public class Image : Panel
{
	private Shader shader;
	private Texture texture;
	private Primitives.Plane plane;

	public Image( Texture texture )
	{
		shader = Shader.Builder.WithVertex( "content/shaders/test.vert" )
							 .WithFragment( "content/shaders/test.frag" )
							 .Build();

		this.texture = texture;
		plane = new();

		var vertexLayout = new VertexLayoutDescription( Vertex.VertexElementDescriptions );

		var rsrcLayoutDesc = new ResourceLayoutDescription()
		{
			Elements = new[]
			{
				//new ResourceLayoutElementDescription()
				//{
				//	Kind = ResourceKind.TextureReadOnly,
				//	Name = "g_tDiffuse",
				//	Options = ResourceLayoutElementOptions.None,
				//	Stages = ShaderStages.Fragment
				//},
				//new ResourceLayoutElementDescription()
				//{
				//	Kind = ResourceKind.Sampler,
				//	Name = "g_sDiffuse",
				//	Options = ResourceLayoutElementOptions.None,
				//	Stages = ShaderStages.Fragment
				//},
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

		var resourceSetDescription = new ResourceSetDescription( rsrcLayout, uniformBuffer );
		resourceSet = Device.ResourceFactory.CreateResourceSet( resourceSetDescription );
	}

	DeviceBuffer uniformBuffer;
	Pipeline pipeline;
	ResourceSet resourceSet;

	public override void Update()
	{
		base.Update();

		var aspect = 4f / 3f;
		position = new Vector2( Screen.Size.X / 2, Screen.Size.Y / 2 );
		size = new Vector2( Screen.Size.Y * aspect, Screen.Size.Y );
	}

	struct ObjectUniformBuffer
	{
		public Matrix4x4 g_mModel;
	}

	public override void Draw( CommandList commandList )
	{
		base.Draw( commandList );

		commandList.SetVertexBuffer( 0, plane.VertexBuffer );
		commandList.SetIndexBuffer( plane.IndexBuffer, IndexFormat.UInt32 );
		commandList.SetPipeline( pipeline );

		var uniformBufferContents = new ObjectUniformBuffer
		{
			g_mModel = modelMatrix
		};

		Device.UpdateBuffer( uniformBuffer, 0, new[] { uniformBufferContents } );

		commandList.SetGraphicsResourceSet( 0, resourceSet );

		// shader.SetMatrix( "g_mModel", modelMatrix );

		commandList.DrawIndexed(
			indexCount: (uint)Primitives.Plane.Indices.Length,
			instanceCount: 1,
			indexStart: 0,
			vertexOffset: 0,
			instanceStart: 0
		);

		plane.Draw( shader, texture );
	}
}
