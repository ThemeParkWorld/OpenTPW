namespace OpenTPW;

public class Ride : Entity
{
	private MeshFile mesh;
	private Shader shader;
	private Texture texture;

	public Ride( string meshPath )
	{
		shader = Shader.Builder.WithVertex( "content/shaders/test.vert" )
							 .WithFragment( "content/shaders/test.frag" )
							 .Build();

		texture = TextureBuilder.FromPath( "content/textures/test.png" ).UseSrgbFormat( false ).Build();
		mesh = new( meshPath );
	}

	public override void Render()
	{
		base.Render();

		var modelMatrix = Silk.NET.Maths.Matrix4X4.CreateScale( scale.X, scale.Y, scale.Z ) *
			Silk.NET.Maths.Matrix4X4.CreateTranslation( position.X, position.Y, position.Z );

		shader.SetMatrix( "g_mModel", modelMatrix );
		mesh.Draw( shader, texture );
	}

	public override void Update()
	{
		base.Update();
	}
}
