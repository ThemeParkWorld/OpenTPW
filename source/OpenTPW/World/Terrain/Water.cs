namespace OpenTPW;

public class Water : ModelEntity
{
	public override void Spawn()
	{
		var texture = new Texture( "lobby/terrain/textures/jri_lak3.wct" );

		var material = Material.Default;
		material.Set( "Color", texture );

		Model = Primitives.Plane.GenerateModel( material, new Point2( 128, 128 ) );
	}
}
