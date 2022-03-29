namespace OpenTPW;

public struct Material
{
	public Shader Shader { get; set; }
	public Texture DiffuseTexture { get; set; }

	public Material( Texture diffuseTexture, Shader shader )
	{
		DiffuseTexture = diffuseTexture;
		Shader = shader;
	}
}
