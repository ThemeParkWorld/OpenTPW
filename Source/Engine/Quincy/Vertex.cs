using Quincy.MathUtils;
using System.Runtime.InteropServices;

namespace Quincy
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct Vertex
    {
        public Vector3f Position { get; set; }
        public Vector3f Normal { get; set; }
        public Vector2f TexCoords { get; set; }
        public Vector3f Tangent { get; set; }
        public Vector3f BiTangent { get; set; }
    }
}
