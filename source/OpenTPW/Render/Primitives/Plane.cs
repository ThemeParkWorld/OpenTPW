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
				TexCoords = new Vector2(0.0f, 1.0f),
				Normal = new Vector3(0.0f, 0.0f, 1.0f),
			},
			new Vertex()
			{
				Position = new Vector3(1.0f, -1.0f, 0.0f),
				TexCoords = new Vector2(1.0f, 1.0f),
				Normal = new Vector3(0.0f, 0.0f, 1.0f),
			},
			new Vertex()
			{
				Position = new Vector3(-1.0f, 1.0f, 0.0f),
				TexCoords = new Vector2(0.0f, 0.0f),
				Normal = new Vector3(0.0f, 0.0f, 1.0f),
			},
			new Vertex()
			{
				Position = new Vector3(1.0f, 1.0f, 0.0f),
				TexCoords = new Vector2(1.0f, 0.0f),
				Normal = new Vector3(0.0f, 0.0f, 1.0f),
			}
		};

		public static uint[] Indices { get; } = new uint[]
		{
			0, 1, 3,
			3, 2, 0
		};

		public static Model GenerateModel( Material material )
		{
			var model = new Model( Vertices, Indices, material );
			return model;
		}
	}
}
