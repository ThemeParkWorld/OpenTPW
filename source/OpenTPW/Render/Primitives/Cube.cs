using Silk.NET.OpenGL;

namespace OpenTPW;

partial class Primitives
{
	internal class Cube
	{
		private float[] cubeVertices = new[] {
			-1.0f,  1.0f, 1.0f,
			-1.0f, -1.0f, 1.0f,
			 1.0f, -1.0f, 1.0f,
			 1.0f, -1.0f, 1.0f,
			 1.0f,  1.0f, 1.0f,
			-1.0f,  1.0f, 1.0f,

			-1.0f, -1.0f, -1.0f,
			-1.0f, -1.0f, 1.0f,
			-1.0f,  1.0f, 1.0f,
			-1.0f,  1.0f, 1.0f,
			-1.0f,  1.0f, -1.0f,
			-1.0f, -1.0f, -1.0f,

			 1.0f, -1.0f, 1.0f,
			 1.0f, -1.0f, -1.0f,
			 1.0f,  1.0f, -1.0f,
			 1.0f,  1.0f, -1.0f,
			 1.0f,  1.0f, 1.0f,
			 1.0f, -1.0f, 1.0f,

			-1.0f, -1.0f, -1.0f,
			-1.0f,  1.0f, -1.0f,
			 1.0f,  1.0f, -1.0f,
			 1.0f,  1.0f, -1.0f,
			 1.0f, -1.0f, -1.0f,
			-1.0f, -1.0f, -1.0f,

			-1.0f,  1.0f, 1.0f,
			 1.0f,  1.0f, 1.0f,
			 1.0f,  1.0f, -1.0f,
			 1.0f,  1.0f, -1.0f,
			-1.0f,  1.0f, -1.0f,
			-1.0f,  1.0f, 1.0f,

			-1.0f, -1.0f, 1.0f,
			-1.0f, -1.0f, -1.0f,
			 1.0f, -1.0f, 1.0f,
			 1.0f, -1.0f, 1.0f,
			-1.0f, -1.0f, -1.0f,
			 1.0f, -1.0f, -1.0f
		};

		public List<Vertex> Vertices
		{
			get
			{
				List<Vertex> tmp = new List<Vertex>();

				for ( int i = 0; i < cubeVertices.Length; i += 3 )
				{
					var x = cubeVertices[i];
					var y = cubeVertices[i + 1];
					var z = cubeVertices[i + 2];

					tmp.Add( new Vertex()
					{
						Position = new Vector3( x, y, z ),

						// TODO:
						TexCoords = new Vector2( 0, 0 ),
						Normal = new Vector3( 0, 0, 0 ),
					} );
				}

				return tmp;
			}
		}

		private uint vao, vbo;

		public Cube()
		{
			SetupMesh();
		}

		public void SetupMesh()
		{
			var vertexStructSize = 8 * sizeof( float );

			vao = Gl.GenVertexArray();
			Gl.BindVertexArray( vao );

			vbo = Gl.GenBuffer();

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
			Gl.DrawArrays( GLEnum.Triangles, 0, (uint)Vertices.Count );
			Gl.BindVertexArray( 0 );
		}
	}
}
