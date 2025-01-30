namespace OpenTPW;

public class LobbyCameraMode : CameraMode
{
	private float Height => 25f;
	private float Distance => 60f;
	private float Speed => 0.3f;

	private Vector3 Target => new Vector3( 0, 0, 10 );

	public override void Update()
	{
		float x = MathF.Sin( Time.Now * Speed ) * Distance;
		float y = MathF.Cos( Time.Now * Speed ) * Distance;

		Position = new Vector3( x, y, Height );
		Rotation = Rotation.LookAt( Target - Position );

		FieldOfView = 60f;
	}
}
