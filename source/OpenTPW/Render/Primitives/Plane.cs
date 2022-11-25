namespace OpenTPW;

partial class Primitives
{
	internal static class Plane
	{
		public static Vertex[] Vertices = new[]
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
			0, 2, 3,
			3, 1, 0,
		};

		public static Model GenerateModel( Material material, Point2? _size = null )
		{
			var size = _size ?? new Point2( 1, 1 );

			var vertices = new Vertex[4 * size.X * size.Y];
			var indices = new uint[6 * size.X * size.Y];

			for ( int y = 0; y < size.Y; y++ )
			{
				for ( int x = 0; x < size.X; x++ )
				{
					var offset = (y * size.X + x) * 4;
					var indexOffset = (y * size.X + x) * 6;

					for ( int i = 0; i < 4; i++ )
					{
						var vertex = Vertices[i];
						vertex.Position += new Vector3( x * 2.0f, y * 2.0f, 0 );

						vertices[offset + i] = vertex;
					}

					for ( int i = 0; i < 6; i++ )
					{
						indices[indexOffset + i] = Indices[i] + (uint)offset;
					}
				}
			}

			// Shift to center
			if ( size.X > 1 && size.Y > 1 )
			{
				var center = new Point2( size.X / 2, size.Y / 2 );

				for ( int i = 0; i < vertices.Length; i++ )
				{
					vertices[i].Position -= new Vector3( center.X * 2.0f, center.Y * 2.0f, 0 );
				}
			}

			var model = new Model( vertices, indices, material );
			return model;
		}
	}
}
