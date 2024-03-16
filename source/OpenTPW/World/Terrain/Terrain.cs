namespace OpenTPW;

public class Terrain : ModelEntity
{
	public override void Spawn()
	{
		base.Spawn();

		var texture = new TextureFile( @"data/levels/jungle/terrain.wad/textures/jgr_bas1.wct" );
		var material = new Material( texture.Texture );

		Model = Primitives.Plane.GenerateModel( material, new Point2( 32, 32 ) );
	}
}
