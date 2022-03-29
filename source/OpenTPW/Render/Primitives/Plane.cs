using System.Runtime.InteropServices;
using Veldrid;

namespace OpenTPW;

partial class Primitives
{
	internal class Plane
	{
		public static Vertex[] Vertices { get; } = new[]
		{
			new Vertex()
			{
				Position = new Vector3(-1.0f, -1.0f, 0.0f),
				TexCoords = new Vector2(0.0f, 0.0f),
				Normal = new Vector3(0.0f, 0.0f, 1.0f),
			},
			new Vertex()
			{
				Position = new Vector3(1.0f, -1.0f, 0.0f),
				TexCoords = new Vector2(1.0f, 0.0f),
				Normal = new Vector3(0.0f, 0.0f, 1.0f),
			},
			new Vertex()
			{
				Position = new Vector3(-1.0f, 1.0f, 0.0f),
				TexCoords = new Vector2(0.0f, 1.0f),
				Normal = new Vector3(0.0f, 0.0f, 1.0f),
			},
			new Vertex()
			{
				Position = new Vector3(1.0f, 1.0f, 0.0f),
				TexCoords = new Vector2(1.0f, 1.0f),
				Normal = new Vector3(0.0f, 0.0f, 1.0f),
			}
		};

		public static uint[] Indices { get; } = new uint[]
		{
			0, 1, 3,
			3, 2, 0
		};

		public Plane()
		{
			SetupMesh();
		}

		public DeviceBuffer VertexBuffer { get; private set; }
		public DeviceBuffer IndexBuffer { get; private set; }

		public void SetupMesh()
		{
			var factory = Device.ResourceFactory;

			var vertexStructSize = (uint)Marshal.SizeOf( typeof( Vertex ) );

			VertexBuffer = factory.CreateBuffer(
				new Veldrid.BufferDescription( (uint)Vertices.Length * vertexStructSize, Veldrid.BufferUsage.VertexBuffer )
			);
			IndexBuffer = factory.CreateBuffer(
				new Veldrid.BufferDescription( (uint)Indices.Length * sizeof( uint ), Veldrid.BufferUsage.IndexBuffer )
			);

			Device.UpdateBuffer( VertexBuffer, 0, Vertices );
			Device.UpdateBuffer( IndexBuffer, 0, Indices );
		}

		public void Draw( Shader shader, Texture diffuseTexture )
		{
			shader.Use();

			diffuseTexture.Bind();
			// shader.SetInt( "g_tDiffuse", 0 );

			InternalDraw();
		}

		internal unsafe void InternalDraw()
		{
		}
	}
}
