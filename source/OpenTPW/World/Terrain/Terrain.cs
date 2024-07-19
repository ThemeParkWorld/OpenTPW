namespace OpenTPW;

public class Terrain : ModelEntity
{
	public override void Spawn()
	{
		var texture = new Texture( "levels/jungle/terrain/textures/jgr_bas1.wct" );

		var material = Material.Default;
		material.Set( "Color", texture );

		Model = Primitives.Plane.GenerateModel( material, new Point2( 32, 32 ) );
	}
}
