namespace OpenTPW;

public class ShaderBuilder
{
	public enum ShaderType
	{
		Fragment,
		Vertex
	};

	private uint programId;

	internal ShaderBuilder()
	{
	}

	public ShaderBuilder WithFragment( string fragPath )
	{
		return this;
	}

	public ShaderBuilder WithVertex( string vertPath )
	{
		return this;
	}

	private void CompileShader( string path, ShaderType shaderType )
	{

	}

	private void CheckForErrors( uint shader )
	{

	}

	public Shader Build()
	{
		return new Shader();
	}
}
