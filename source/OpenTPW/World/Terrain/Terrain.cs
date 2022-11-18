namespace OpenTPW;

public class Terrain : ModelEntity
{
	public override void Spawn()
	{
		base.Spawn();

		// TODO: File System
		var texture = new TextureFile( @"C:\Users\Alex\BLACKHOLE\terrain.wad\grd_ctr1.wct" );
		var material = new Material( texture.Texture );

		Model = Primitives.Plane.GenerateModel( material );
		scale = new Vector3( 100f );
	}
}
