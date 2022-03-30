using Veldrid;

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

	public bool IsDirty => DiffuseTexture.IsDirty;

	public void GenerateMipmaps( CommandList commandList )
	{
		if ( DiffuseTexture.IsDirty )
			DiffuseTexture.GenerateMipmaps( commandList );
	}
}
