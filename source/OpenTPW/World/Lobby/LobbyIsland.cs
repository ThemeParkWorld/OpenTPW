using System.Numerics;

namespace OpenTPW;

public sealed class LobbyIsland : Entity
{
	public LobbyIsland( Vector3 _position, string themeName )
	{
		Position = _position;

		var modelPrefix = themeName[0..3];
		var modelFile = new ModelFile( $"lobby/terrain/{modelPrefix}_isle.md2" );

		foreach ( var mesh in modelFile.Meshes )
		{
			var material = new Material<ObjectUniformBuffer>( "content/shaders/test.shader" );
			var textures = new List<Texture>();

			for ( int i = 0; i < 16; ++i )
			{
				if ( mesh.Materials.Length <= i )
				{
					textures.Add( Texture.Missing );
				}
				else
				{
					var j = mesh.Materials[i];
					textures.Add( new Texture( $"lobby/terrain/textures/{j.Name}.wct", TextureFlags.Repeat ) );
				}
			}

			material.Set( $"Color", [.. textures] );

			var vertices = new List<Vertex>();
			for ( int i = 0; i < mesh.Vertices.Length; ++i )
			{
				vertices.Add( new Vertex()
				{
					Position = new Vector3( mesh.Vertices[i].Position.X, mesh.Vertices[i].Position.Z, mesh.Vertices[i].Position.Y ),
					Normal = mesh.Normals[i],
					TexCoords = mesh.TexCoords[i],
					TexIndex = (int)mesh.Vertices[i].TextureIndex,
					MatFlags = mesh.Materials[(int)mesh.Vertices[i].TextureIndex].Flags
				} );
			}

			var model = new Model( [.. vertices], mesh.Indices, material );
			Matrix4x4.Decompose( mesh.TransformMatrix, out var scl, out var rot, out var pos );

			var position = new Vector3( pos.X, pos.Z, pos.Y - 2.5f );
			var rotation = new Quaternion( rot.X, rot.Z, rot.Y, -rot.W );
			var scale = new Vector3( scl.X, scl.Z, scl.Y );

			_ = new ModelEntity()
			{
				Model = model,
				Scale = scale,
				Rotation = rotation,
				Position = position + Position,
			};
		}
	}
}
