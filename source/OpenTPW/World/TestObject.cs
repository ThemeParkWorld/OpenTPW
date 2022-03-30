using System.Numerics;
using Veldrid;

namespace OpenTPW;

public class TestObject : Entity
{
	private Model model;

	struct ObjectUniformBuffer
	{
		public Matrix4x4 g_mModel;
		public Matrix4x4 g_mView;
		public Matrix4x4 g_mProj;
		public Vector3 g_vLightPos;
		public Vector3 g_vLightColor;
		public Vector3 g_vCameraPos;
		public int g_bHighlighted;
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
			g_bHighlighted = (Time.Now.CeilToInt() % 2 == 0) ? 1 : 0
		};

		model.Draw( uniformBuffer, commandList );
	}
}
