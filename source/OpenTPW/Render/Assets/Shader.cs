using System.Numerics;

namespace OpenTPW;

public class Shader : Asset
{
	public Veldrid.Shader[] ShaderProgram { get; }

	internal Shader( Veldrid.Shader[] shaderProgram )
	{
		ShaderProgram = shaderProgram;

		All.Add( this );
	}

	public void Use()
	{
	}

	public void SetFloat( string name, float value )
	{
		if ( TryGetUniformLocation( name, out int loc ) )
		{
		}
	}

	public void SetInt( string name, int value )
	{
		if ( TryGetUniformLocation( name, out int loc ) )
		{
		}
	}

	public unsafe void SetMatrix( string name, Matrix4x4 value )
	{
		if ( TryGetUniformLocation( name, out int loc ) )
		{
		}
	}

	public void SetVector2( string name, Vector2 value )
	{
		if ( TryGetUniformLocation( name, out int loc ) )
		{
		}
	}

	public void SetVector3( string name, Vector3 value )
	{
		if ( TryGetUniformLocation( name, out int loc ) )
		{
		}
	}

	internal void SetBool( string name, bool value )
	{
		if ( TryGetUniformLocation( name, out int loc ) )
		{
		}
	}

	private bool TryGetUniformLocation( string name, out int loc )
	{
		loc = -1;

		if ( loc < 0 )
		{
			Log.Warning( $"No variable {name}" );
			return false;
		}

		return true;
	}
}
