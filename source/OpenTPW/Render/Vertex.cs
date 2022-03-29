using System.Runtime.InteropServices;
using Veldrid;

namespace OpenTPW;

[StructLayout( LayoutKind.Sequential )]
public struct Vertex
{
	public Vector3 Position { get; set; }
	public Vector3 Normal { get; set; }
	public Vector2 TexCoords { get; set; }

	public static VertexElementDescription[] VertexElementDescriptions = new[]
	{
		new VertexElementDescription( "vPosition", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3 ),
		new VertexElementDescription( "vNormal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3 ),
		new VertexElementDescription( "vTexCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2 )
	};
}
