using System.Runtime.InteropServices;

namespace OpenTPW;

[StructLayout( LayoutKind.Sequential )]
public struct Vertex
{
	public Vector3 Position { get; set; }
	public Vector3 Normal { get; set; }
	public Vector2 TexCoords { get; set; }
}
