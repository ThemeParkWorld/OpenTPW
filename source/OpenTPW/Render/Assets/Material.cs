namespace OpenTPW;

public class Material : Asset
{
	public Shader Shader { get; set; }
	public Type UniformBufferType { get; }
	public Texture DiffuseTexture { get; set; }

	public static Material Default => new Material(
			new Texture( "content/textures/test.png" ),
			ShaderBuilder.Default.WithVertex( "content/shaders/3d/3d.vert" )
							 .WithFragment( "content/shaders/3d/3d.frag" )
							 .Build(),
			typeof( ObjectUniformBuffer )
		);

	/// <summary>
	/// Create a material with the default shader and uniform buffer type.
	/// </summary>
	public Material( Texture diffuseTexture )
	{
		DiffuseTexture = diffuseTexture;
		Shader = ShaderBuilder.Default.WithVertex( "content/shaders/3d/3d.vert" )
								.WithFragment( "content/shaders/3d/3d.frag" )
								.Build();
		UniformBufferType = typeof( ObjectUniformBuffer );

		All.Add( this );
	}

	public Material( Texture diffuseTexture, Shader shader, Type uniformBufferType )
	{
		DiffuseTexture = diffuseTexture;
		Shader = shader;
		UniformBufferType = uniformBufferType;

		All.Add( this );
	}
}
