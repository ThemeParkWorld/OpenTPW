namespace OpenTPW;

public class Sky : ModelEntity
{
	public override void Spawn()
	{
		base.Spawn();

		var texture = new Texture( new byte[4] { 42, 205, 244, 255 }, 1, 1 );

		var shader = ShaderBuilder.Default.WithVertex( "content/shaders/3d/3d.vert" )
								.WithFragment( "content/shaders/unlit/unlit.frag" )
								.Build();

		var uniformBufferType = typeof( ObjectUniformBuffer );

		var material = new Material( texture, shader, uniformBufferType );

		Model = Primitives.Cube.GenerateModel( material );
		scale = new Vector3( -100f );
	}
}
