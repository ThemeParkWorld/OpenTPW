namespace OpenTPW;

public struct Material
{
	public Shader Shader { get; set; }
	public Type UniformBufferType { get; }
	public Texture DiffuseTexture { get; set; }

	public Material( Texture diffuseTexture, Shader shader, Type uniformBufferType )
	{
		DiffuseTexture = diffuseTexture;
		Shader = shader;
		UniformBufferType = uniformBufferType;
	}
}
