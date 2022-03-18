using Silk.NET.OpenGL;

namespace OpenTPW;

public class ShaderBuilder
{
	private uint programId;

	internal ShaderBuilder()
	{
		programId = Gl.CreateProgram();
	}

	public ShaderBuilder WithFragment( string fragPath )
	{
		CompileShader( fragPath, ShaderType.FragmentShader );

		return this;
	}

	public ShaderBuilder WithVertex( string vertPath )
	{
		CompileShader( vertPath, ShaderType.VertexShader );

		return this;
	}

	private void CompileShader( string path, ShaderType shaderType )
	{
		var vertContents = File.ReadAllText( path );

		var shaderId = Gl.CreateShader( shaderType );
		Gl.ShaderSource( shaderId, vertContents );
		Gl.CompileShader( shaderId );

		CheckForErrors( shaderId );

		Gl.AttachShader( programId, shaderId );
	}

	private void CheckForErrors( uint shader )
	{
		Gl.GetShader( shader, ShaderParameterName.CompileStatus, out int isCompiled );
		if ( isCompiled == 0 )
		{
			Gl.GetShader( shader, ShaderParameterName.InfoLogLength, out int maxLength );

			string str;
			Gl.GetShaderInfoLog( shader, (uint)maxLength, out uint _, out str );

			Log.Error( str );
		}
	}

	public Shader Build()
	{
		Gl.LinkProgram( programId );

		return new Shader( programId );
	}
}
