namespace OpenTPW;

partial class Primitives
{
	internal static class Cube
	{
		private static float[] cubeVertices = new[] {
			-0.5f, +0.5f, -0.5f, 0, 0,
			+0.5f, +0.5f, -0.5f, 1, 0,
			+0.5f, +0.5f, +0.5f, 1, 1,
			-0.5f, +0.5f, +0.5f, 0, 1,
			-0.5f,-0.5f, +0.5f, 0, 0,
			+0.5f,-0.5f, +0.5f, 1, 0,
			+0.5f,-0.5f, -0.5f, 1, 1,
			-0.5f,-0.5f, -0.5f, 0, 1,
			-0.5f, +0.5f, -0.5f, 0, 0,
			-0.5f, +0.5f, +0.5f, 1, 0,
			-0.5f, -0.5f, +0.5f, 1, 1,
			-0.5f, -0.5f, -0.5f, 0, 1,
			+0.5f, +0.5f, +0.5f, 0, 0,
			+0.5f, +0.5f, -0.5f, 1, 0,
			+0.5f, -0.5f, -0.5f, 1, 1,
			+0.5f, -0.5f, +0.5f, 0, 1,
			+0.5f, +0.5f, -0.5f, 0, 0,
			-0.5f, +0.5f, -0.5f, 1, 0,
			-0.5f, -0.5f, -0.5f, 1, 1,
			+0.5f, -0.5f, -0.5f, 0, 1,
			-0.5f, +0.5f, +0.5f, 0, 0,
			+0.5f, +0.5f, +0.5f, 1, 0,
			+0.5f, -0.5f, +0.5f, 1, 1,
			-0.5f, -0.5f, +0.5f, 0, 1,
		};


		private static uint[] indices =
		{
			0,1,2, 0,2,3,
			4,5,6, 4,6,7,
			8,9,10, 8,10,11,
			12,13,14, 12,14,15,
			16,17,18, 16,18,19,
			20,21,22, 20,22,23,
		};

		public static List<Vertex> Vertices
		{
			get
			{
				List<Vertex> tmp = new List<Vertex>();

				for ( int i = 0; i < cubeVertices.Length; i += 5 )
				{
					var x = cubeVertices[i];
					var y = cubeVertices[i + 1];
					var z = cubeVertices[i + 2];

					var u = cubeVertices[i + 3];
					var v = cubeVertices[i + 4];

					// var nX = cubeVertices[i + 5];
					// var nY = cubeVertices[i + 6];
					// var nZ = cubeVertices[i + 7];

					tmp.Add( new Vertex()
					{
						Position = new Vector3( x, y, z ),
						TexCoords = new Vector2( u, v ),
						// Normal = new Vector3( nX, nY, nZ ),
					} );
				}

				return tmp;
			}
		}

		public static Model GenerateModel( Material material )
		{
			var model = new Model( Vertices.ToArray(), indices, material );
			return model;
		}
	}
}
