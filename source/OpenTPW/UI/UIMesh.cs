using OpenTPW.Files;
using System.Numerics;
using Veldrid;

namespace OpenTPW.UI;

public partial class UIMesh : Panel
{
	private Model model;

	struct ObjectUniformBuffer
	{
		public Matrix4x4 g_mModel;
	}

	public UIMesh()
	{
		var textureFile = new TextureFile( "ui/textures/tpw_logo.wct" );
		var texture = new Texture( textureFile );

		var material = new Material(
			texture,
			ShaderBuilder.Default.WithVertex( "content/shaders/test.vert" )
						  .WithFragment( "content/shaders/test.frag" )
						  .Build(),
			typeof( ObjectUniformBuffer )
		);

		var modelFile = new ModelFile( "ui/tpwlogo.MD2" );

		var vertices = modelFile.Data.Vertices.Select( x => new Vertex()
		{
			Normal = new Vector3( 0, 1, 0 ),
			Position = new Vector3( x.Position.X, x.Position.Z, x.Position.Y ) / 800f,
			TexCoords = x.TexCoords * new Vector2( 1, -1 )
		} ).ToArray();

		var indices = modelFile.Data.Indices.ToArray();
		indices = indices.Reverse().ToArray();

		indices = new uint[] { 3, 1, 2, 0, 3, 1 }.Reverse().ToArray();

		model = new Model( vertices, indices, material );
	}

	public override void Update()
	{
		base.Update();

		position = new Vector2( Screen.Size.X / 2, Screen.Size.Y / 2 );
		size = new Vector2( Screen.Size.X, Screen.Size.Y );
	}

	public override void Draw( CommandList commandList )
	{
		base.Draw( commandList );

		var uniformBufferContents = new ObjectUniformBuffer
		{
			g_mModel = modelMatrix
		};

		model.Draw( uniformBufferContents, commandList );
	}
}
