using System.Numerics;

namespace OpenTPW;

public class Camera : Entity
{
	public Matrix4x4 ViewMatrix { get; set; }
	public Matrix4x4 ProjMatrix { get; set; }

	public Vector3 Forward => ViewMatrix.Forward();
	public Vector3 Right => ViewMatrix.Right();
	public Vector3 Up => ViewMatrix.Up();

	private Vector3 velocity = new();
	private Vector3 wishVelocity = new();

	private float wishYaw = 0f;
	private float yaw = 0f;
	private float wishHeight = 8f;
	private float cameraSpeed = 100f;

	private void CalcViewProjMatrix()
	{
		var lookAt = new Vector3( position.X, position.Y + 4, 0 );

		var cameraPos = lookAt + new Vector3(
			MathF.Cos( yaw.DegreesToRadians() ) * position.Z,
			MathF.Sin( yaw.DegreesToRadians() ) * position.Z,
			position.Z
		);

		var cameraUp = new Vector3( 0, 0, 1 );

		ViewMatrix = Matrix4x4.CreateLookAt( cameraPos, lookAt, cameraUp );
		ProjMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
			90.0f.DegreesToRadians(),
			Screen.Aspect,
			0.1f,
			1000.0f
		);
	}

	private Vector2 mouseAnchor;
	private bool wasPressed = false;

	public override void Update()
	{
		base.Update();

		//
		// Get user input
		//

		var wishDir = new Vector3( Input.Forward, Input.Right, 0 ).Normal;

		if ( Input.Mouse.Right && !wasPressed )
		{
			mouseAnchor = Input.Mouse.Position;
		}

		if ( Input.Mouse.Right )
		{
			var delta = mouseAnchor - Input.Mouse.Position;
			wishDir = new Vector3( delta.Y, -delta.X, 0 ) / 512f;
		}

		wasPressed = Input.Mouse.Right;

		wishVelocity = Forward * wishDir * Time.Delta * cameraSpeed;
		wishVelocity += Right * wishDir * Time.Delta * cameraSpeed;
		wishVelocity.Z = 0;

		wishHeight += -Input.Mouse.Wheel;
		wishHeight = wishHeight.Clamp( 1, 10 );

		if ( Input.Pressed( InputButton.RotateLeft ) )
			wishYaw -= 90;
		if ( Input.Pressed( InputButton.RotateRight ) )
			wishYaw += 90;

		//
		// Apply everything
		//

		// Apply velocity
		velocity += wishVelocity;

		// Apply drag
		velocity *= 1 - Time.Delta * 10f;

		// Rotate camera
		yaw = yaw.LerpTo( wishYaw, 10f * Time.Delta );

		// Move camera
		position += velocity * Time.Delta;
		position.Z = position.Z.LerpTo( wishHeight, 10f * Time.Delta );

		// Run view/proj matrix calculations
		CalcViewProjMatrix();
	}
}
