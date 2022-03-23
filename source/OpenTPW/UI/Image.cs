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

	public override void Update()
	{
		base.Update();

		var aspect = 4f / 3f;
		position = new Vector2( Screen.Size.X / 2, Screen.Size.Y / 2 );
		size = new Vector2( Screen.Size.Y * aspect, Screen.Size.Y );
	}

	public override void Draw()
	{
		base.Draw();

		shader.SetMatrix( "g_mModel", modelMatrix );
		plane.Draw( shader, texture );
	}
}
