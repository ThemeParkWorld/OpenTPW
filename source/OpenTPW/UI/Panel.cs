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

	private Vector2 SizeToNDC( Vector2 size )
	{
		var p = size / (Vector2)Screen.Size;
		return p;
	}

	private Vector2 PositionToNDC( Vector2 position )
	{
		var p = position / (Vector2)Screen.Size;
		p += new Vector2( -0.5f, -0.5f );
		p *= 2.0f;
		return p;
	}

	public virtual void Update()
	{
	}

	public virtual void Draw()
	{
		var positionNDC = PositionToNDC( position );
		var sizeNDC = SizeToNDC( size );

		modelMatrix = Silk.NET.Maths.Matrix4X4.CreateScale( sizeNDC.X, sizeNDC.Y, 1 ) *
			Silk.NET.Maths.Matrix4X4.CreateTranslation( positionNDC.X, positionNDC.Y, 1 );
	}
}
