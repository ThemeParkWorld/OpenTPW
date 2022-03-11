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
	}

	public override void Draw()
	{
		modelMatrix = Silk.NET.Maths.Matrix4X4.CreateScale( size.X, size.Y, 1 ) *
			Silk.NET.Maths.Matrix4X4.CreateTranslation( position.X, position.Y, 1 );

		shader.SetMatrix( "g_mModel", modelMatrix );
		plane.Draw( shader, texture );
	}
}
