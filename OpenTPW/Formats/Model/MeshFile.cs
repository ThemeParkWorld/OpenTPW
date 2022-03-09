using Silk.NET.OpenGL;

namespace OpenTPW;

public class MeshFile
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

	private uint vao, vbo, ebo;

	public MeshFile( string path )
	{
		SetupMesh();
	}

	public void SetupMesh()
	{
		var vertexStructSize = 8 * sizeof( float );

		vao = Gl.GenVertexArray();
		Gl.BindVertexArray( vao );

		vbo = Gl.GenBuffer();
		ebo = Gl.GenBuffer();

		var glVertices = new List<float>();
		foreach ( var vertex in Vertices )
		{
			glVertices.AddRange( new[] {
					vertex.Position.X,
					vertex.Position.Y,
					vertex.Position.Z,

					vertex.Normal.X,
					vertex.Normal.Y,
					vertex.Normal.Z,

					vertex.TexCoords.X,
					vertex.TexCoords.Y
				} );
		}

		Gl.BindBuffer( GLEnum.ArrayBuffer, vbo );
		Gl.BufferData<float>( GLEnum.ArrayBuffer, (uint)glVertices.Count * sizeof( float ), glVertices.ToArray(), GLEnum.StaticDraw );

		Gl.BindBuffer( GLEnum.ElementArrayBuffer, ebo );
		Gl.BufferData<uint>( GLEnum.ElementArrayBuffer, (uint)Indices.Length * sizeof( uint ), Indices, GLEnum.StaticDraw );

		unsafe
		{
			Gl.EnableVertexAttribArray( 0 );
			Gl.VertexAttribPointer( 0, 3, GLEnum.Float, false, (uint)vertexStructSize, (void*)0 );

			Gl.EnableVertexAttribArray( 1 );
			Gl.VertexAttribPointer( 1, 3, GLEnum.Float, false, (uint)vertexStructSize, (void*)(3 * sizeof( float )) );

			Gl.EnableVertexAttribArray( 2 );
			Gl.VertexAttribPointer( 2, 2, GLEnum.Float, false, (uint)vertexStructSize, (void*)(6 * sizeof( float )) );
		}

		Gl.BindVertexArray( 0 );
	}

	public void Draw( Shader shader, Texture diffuseTexture )
	{
		shader.Use();

		Gl.ActiveTexture( TextureUnit.Texture0 );

		diffuseTexture.Bind();
		shader.SetInt( "g_tDiffuse", 0 );

		Gl.ActiveTexture( TextureUnit.Texture0 );

		InternalDraw();
	}

	internal unsafe void InternalDraw()
	{
		Gl.BindVertexArray( vao );
		Gl.DrawElements( PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null );
		Gl.BindVertexArray( 0 );
	}
}
