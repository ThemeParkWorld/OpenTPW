namespace OpenTPW;

public class Terrain : ModelEntity
{
	public override void Spawn()
	{
		base.Spawn();

		var texture = new TextureFile( @"/levels/jungle/terrain/textures/jgr_bas1.wct" );
		// var material = new Material( texture.Texture );

		// Model = Primitives.Plane.GenerateModel( material, new Point2( 32, 32 ) );
	}
}
