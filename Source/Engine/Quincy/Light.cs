using OpenGL;
using Quincy.MathUtils;
using System.Runtime.InteropServices;

namespace Quincy
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Light
    {
        public Light(Vector3f position)
        {
            Position = position;
            var size = 20f;
            var farPlane = 200f;
            ProjMatrix = Matrix4x4f.Ortho(-size, size, -size, size, 0.1f, farPlane);
            ViewMatrix = Matrix4x4f.Identity;
            ShadowMap = new ShadowMap(4096, 4096);
        }

        public Vector3f Position { get; set; }
        public Matrix4x4f ViewMatrix { get; set; }
        public Matrix4x4f ProjMatrix { get; set; }
        public ShadowMap ShadowMap { get; set; }

        public void Render()
        {
            ViewMatrix = Matrix4x4f.LookAt(new Vertex3f(Position.x, Position.y, Position.z), new Vertex3f(0f, 0f, 0f), new Vertex3f(0f, 1f, 0f));
        }
    }
}
