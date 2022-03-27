namespace OpenTPW;

public class TestObject : Entity
{
	private Shader shader;
	private Texture texture;
	private Primitives.Cube cube;

	public TestObject()
	{
		shader = Shader.Builder.WithVertex( "content/shaders/3d/3d.vert" )
							 .WithFragment( "content/shaders/3d/3d.frag" )
							 .Build();

		this.texture = TextureBuilder.FromPath( "content/textures/test.png" ).Build();
		cube = new();
	}

	public override void Render()
	{
		base.Render();

		shader.SetMatrix( "g_mModel", ModelMatrix );
		shader.SetMatrix( "g_mView", World.Current.Camera.ViewMatrix );
		shader.SetMatrix( "g_mProj", World.Current.Camera.ProjMatrix );

		shader.SetVector3( "g_vLightPos", World.Current.Sun.position );
		shader.SetVector3( "g_vLightColor", World.Current.Sun.Color );

		shader.SetVector3( "g_vCameraPos", World.Current.Camera.position );
		shader.SetBool( "g_bHighlighted", Time.Now.CeilToInt() % 2 == 0 );

		cube.Draw( shader, texture );
	}
}
