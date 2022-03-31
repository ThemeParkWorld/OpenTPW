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
		/*
		 * These fields are padded so that they're
		 * aligned (as blocks) to multiples of 16.
		 */

		public Matrix4x4 g_mModel; // 64
		public Matrix4x4 g_mView; // 64
		public Matrix4x4 g_mProj; // 64

		public System.Numerics.Vector3 g_vLightPos; // 12
		public float _padding0; // 4

		public System.Numerics.Vector3 g_vLightColor; // 12
		public float _padding1; // 4

		public System.Numerics.Vector3 g_vCameraPos; // 12
		public float _padding2; // 4
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
			g_vLightColor = World.Current.Sun.Color,
			g_vCameraPos = World.Current.Camera.position,

			_padding0 = 0,
			_padding1 = 0,
			_padding2 = 0
		};

		model.Draw( uniformBuffer, commandList );
	}
}
