namespace OpenTPW;

public class Terrain : ModelEntity
{
	public override void Spawn()
	{
		var textureFile = new TextureFile( @"/levels/jungle/terrain/textures/jgr_bas1.wct" );
		var texture = new Texture( textureFile );

		var material = new Material( texture );
		Model = Primitives.Plane.GenerateModel( material, new Point2( 32, 32 ) );
	}
}
