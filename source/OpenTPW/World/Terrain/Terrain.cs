namespace OpenTPW;

public class Terrain : ModelEntity
{
	public override void Spawn()
	{
		base.Spawn();

		// TODO: File System
		var texture = new TextureFile( @"C:\Users\Alex\BLACKHOLE\monkey.wad\m_face4 (1).wct" );
		var material = new Material( texture.Texture );

		Model = Primitives.Plane.GenerateModel( material );
		scale = new Vector3( 100f );
	}
}
