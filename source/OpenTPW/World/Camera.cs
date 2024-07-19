using System.Numerics;

namespace OpenTPW;

public static class Camera
{
	public static Matrix4x4 ViewMatrix { get; set; }
	public static Matrix4x4 ProjMatrix { get; set; }

	public static Vector3 Forward => ViewMatrix.Forward();
	public static Vector3 Right => ViewMatrix.Right();
	public static Vector3 Up => ViewMatrix.Up();

	private static Vector3 velocity = new();
	private static Vector3 wishVelocity = new();

	private static float wishYaw = 0f;
	private static float yaw = 0f;
	private static float wishHeight = 2f;
	private static float cameraSpeed = 128f;

	private static Vector2 mouseAnchor;
	private static bool wasPressed = false;

	public static Vector3 Position;

	private static void CalcViewProjMatrix()
	{
		var lookAt = new Vector3( Position.X, Position.Y, 0 );

		var cameraPos = lookAt + new Vector3(
			MathF.Cos( yaw.DegreesToRadians() ) * Position.Z * 2f,
			MathF.Sin( yaw.DegreesToRadians() ) * Position.Z * 2f,
			Position.Z
		) * 10f;

		var cameraUp = new Vector3( 0, 0, 1 );

		ViewMatrix = Matrix4x4.CreateLookAt( cameraPos, lookAt, cameraUp );
		ProjMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
			90.0f.DegreesToRadians(),
			Screen.Aspect,
			0.1f,
			10000.0f
		);
	}

	public static void Update()
	{
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

		wishVelocity = Forward * wishDir.X * Time.Delta * cameraSpeed;
		wishVelocity += Right * wishDir.Y * Time.Delta * cameraSpeed;
		wishVelocity.Z = 0;

		wishHeight += -Input.Mouse.Wheel;
		wishHeight = wishHeight.Clamp( 1f, 10f );

		if ( Input.Pressed( InputButton.RotateLeft ) )
			wishYaw -= 90;
		if ( Input.Pressed( InputButton.RotateRight ) )
			wishYaw += 90;

		//
		// Apply everything
		//

		// Apply velocity
		velocity += wishVelocity * Time.Delta;

		// Apply drag
		velocity = Vector3.Lerp( velocity, Vector3.Zero, Time.Delta * 5f );

		// Rotate camera
		yaw = yaw.LerpTo( wishYaw, 10f * Time.Delta );

		// Move camera
		Position += velocity;
		Position.Z = Position.Z.LerpTo( wishHeight, 10f * Time.Delta );

		// Run view/proj matrix calculations
		CalcViewProjMatrix();
	}
}
