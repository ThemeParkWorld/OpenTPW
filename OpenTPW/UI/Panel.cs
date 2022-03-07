namespace OpenTPW.UI;

public class Panel
{
	public Vector2 position;
	public Vector2 size;

	public Matrix4X4 modelMatrix;

	public Panel()
	{
		position = new();
		size = new( 1.0f, 1.0f );
	}

	public virtual void Update()
	{
	}

	public virtual void Draw()
	{
	}
}
