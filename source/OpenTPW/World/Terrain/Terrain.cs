namespace OpenTPW;

public class Terrain : ModelEntity
{
	public override void Spawn()
	{
		base.Spawn();

		Model = Primitives.Plane.GenerateModel( Material.Default );
		scale = new Vector3( 100f );
	}
}
