using ImGuiNET;

namespace OpenTPW;

public class Camera : Entity
{
	public Matrix4X4 ViewMatrix { get; set; }
	public Matrix4X4 ProjMatrix { get; set; }

	private void CalcViewProjMatrix()
	{
		var cameraPos = new Vector3( position.X, position.Y, 4 );
		var cameraFront = new Vector3( 0, 0, -1 );
		var cameraUp = new Vector3( 0, 1, 0 );

		ViewMatrix = Silk.NET.Maths.Matrix4X4.CreateLookAt<float>( cameraPos, cameraPos + cameraFront, cameraUp );
		ProjMatrix = Silk.NET.Maths.Matrix4X4.CreatePerspectiveFieldOfView<float>(
			90.0f.DegreesToRadians(),
			Screen.Aspect,
			0.1f,
			1000.0f
		);
	}

	public override void Update()
	{
		base.Update();

		position += Vector3.UnitX * Input.Right * Time.Delta;
		position += Vector3.UnitY * Input.Forward * Time.Delta;

		CalcViewProjMatrix();
	}
}
