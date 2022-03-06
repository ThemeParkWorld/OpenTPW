using System.Runtime.CompilerServices;

namespace OpenTPW;

public class Shader
{
	public uint Id { get; set; }

	public static ShaderBuilder Builder => new();

	internal Shader( uint id )
	{
		Id = id;
	}

	public void Use()
	{
		Gl.UseProgram( Id );
	}

	public void SetFloat( string name, float value )
	{
		if ( TryGetUniformLocation( name, out int loc ) )
		{
			Gl.ProgramUniform1( Id, loc, value );
		}
	}

	public void SetInt( string name, int value )
	{
		if ( TryGetUniformLocation( name, out int loc ) )
		{
			Gl.ProgramUniform1( Id, loc, value );
		}
	}

	public unsafe void SetMatrix( string name, Matrix4X4 value )
	{
		if ( TryGetUniformLocation( name, out int loc ) )
		{
			Gl.ProgramUniformMatrix4( Id, loc, 1, false, (float*)Unsafe.AsPointer( ref value ) );
		}
	}

	public void SetVector3( string name, Vector3 value )
	{
		if ( TryGetUniformLocation( name, out int loc ) )
		{
			Gl.ProgramUniform3( Id, loc, value.X, value.Y, value.Z );
		}
	}

	internal void SetBool( string name, bool value )
	{
		if ( TryGetUniformLocation( name, out int loc ) )
		{
			Gl.ProgramUniform1( Id, loc, value ? 1 : 0 );
		}
	}

	private bool TryGetUniformLocation( string name, out int loc )
	{
		loc = Gl.GetUniformLocation( Id, name );

		if ( loc < 0 )
		{
			Log.Warning( $"No variable {name}" );
			return false;
		}

		return true;
	}
}
