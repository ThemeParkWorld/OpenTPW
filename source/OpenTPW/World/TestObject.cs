using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid;

namespace OpenTPW;

public class TestObject : Entity
{
	private Model model;

	[StructLayout( LayoutKind.Sequential )]
	struct ObjectUniformBuffer
	{
		public Matrix4x4 g_mModel;
		public Matrix4x4 g_mView;
		public Matrix4x4 g_mProj;

		public System.Numerics.Vector3 g_vLightPos;
		public float _padding0;

		public System.Numerics.Vector3 g_vLightColor;
		public float _padding1;

		public System.Numerics.Vector3 g_vCameraPos;
		public float _padding2;
	}

	public TestObject()
	{
		var material = new Material(
			TextureBuilder.WorldTexture.FromPath( "content/textures/test.png" ).Build(),
			ShaderBuilder.Default.WithVertex( "content/shaders/3d/3d.vert" )
							 .WithFragment( "content/shaders/3d/3d.frag" )
							 .Build(),
			typeof( ObjectUniformBuffer )
		);

		model = Primitives.Cube.GenerateModel( material );
	}

	public override void Render( CommandList commandList )
	{
		base.Render( commandList );

		var uniformBuffer = new ObjectUniformBuffer
		{
			g_mModel = ModelMatrix,
			g_mView = World.Current.Camera.ViewMatrix,
			g_mProj = World.Current.Camera.ProjMatrix,
			g_vLightPos = World.Current.Sun.position,
			g_vLightColor = new( 1, 1, 0 ),
			g_vCameraPos = World.Current.Camera.position,

			_padding0 = 0,
			_padding1 = 0,
			_padding2 = 0,
		};

		model.Draw( uniformBuffer, commandList );
	}
}
