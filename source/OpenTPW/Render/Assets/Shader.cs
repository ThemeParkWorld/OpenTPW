using Veldrid;

namespace OpenTPW;

public class Shader : Asset
{
	private ShaderInfo shaderInfo;

	public VertexElementDescription[] VertexElements => shaderInfo.Reflection.VertexElements;
	public ResourceLayoutDescription[] ResourceLayouts => shaderInfo.Reflection.ResourceLayouts;
	public Veldrid.Shader[] ShaderProgram => shaderInfo.ShaderProgram;

	internal Shader( string path )
	{
		shaderInfo = ShaderCompiler.CompileShader( path );

		Path = path;
		All.Add( this );
	}
}
