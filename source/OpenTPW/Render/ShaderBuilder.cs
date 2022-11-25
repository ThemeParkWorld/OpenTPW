using Veldrid;
using Veldrid.SPIRV;

namespace OpenTPW;

public class ShaderBuilder
{
	private ShaderDescription VertexShaderDescription;
	private ShaderDescription FragmentShaderDescription;

	public static ShaderBuilder Default => new ShaderBuilder();

	internal ShaderBuilder()
	{

	}

	private ShaderDescription CreateShaderDescription( string path, ShaderStages shaderStage )
	{
		var shaderBytes = File.ReadAllBytes( path );
		var shaderDescription = new ShaderDescription( shaderStage, shaderBytes, "main" );

		return shaderDescription;
	}

	public ShaderBuilder WithFragment( string fragPath )
	{
		FragmentShaderDescription = CreateShaderDescription( fragPath, ShaderStages.Fragment );
		return this;
	}

	public ShaderBuilder WithVertex( string vertPath )
	{
		VertexShaderDescription = CreateShaderDescription( vertPath, ShaderStages.Vertex );
		return this;
	}

	public Shader Build()
	{
		try
		{
			var shaderProgram = Device.ResourceFactory.CreateFromSpirv( VertexShaderDescription, FragmentShaderDescription );
			return new Shader( shaderProgram );
		}
		catch ( Exception ex )
		{
			Log.Error( ex.ToString() );
			return default;
		}
	}
}
