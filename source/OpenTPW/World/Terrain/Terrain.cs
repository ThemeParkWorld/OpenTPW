namespace OpenTPW;

public class Terrain : ModelEntity
{
	public override void Spawn()
	{
		base.Spawn();

		var textureFile = new TextureFile( @"/levels/jungle/terrain/textures/jgr_bas1.wct" ).Data;
		var texture = new Texture( textureFile.Data, textureFile.Width, textureFile.Height );

		var material = new Material( texture );
		Model = Primitives.Plane.GenerateModel( material, new Point2( 32, 32 ) );
	}
}
