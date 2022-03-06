global using Matrix4X4 = Silk.NET.Maths.Matrix4X4<float>;
global using Vector2 = Silk.NET.Maths.Vector2D<float>;
global using Point2 = Silk.NET.Maths.Vector2D<int>;
global using Vector3 = Silk.NET.Maths.Vector3D<float>;
using OpenTPW;
using Silk.NET.OpenGL;

public static class Global
{
	public static Logger Log { get; } = new();
	public static GL Gl { get; internal set; }
}
