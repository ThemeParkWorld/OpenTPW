using OpenTPW.UI;
using System.Numerics;
using Veldrid;
using static OpenTPW.ModelFile;

namespace OpenTPW;

public class Level
{
	internal static Level Current { get; set; }

	public RootPanel Hud { get; set; }
	public Sun SunLight { get; set; }

	public SettingsFile Global { get; private init; }

	// private int fCount = 0;
	// private Image loadingTexture;

	public Level( string levelName )
	{
		Global = new SettingsFile( $"/levels/{levelName}/global.sam" );
		Current = this;

		SetupEntities();
		SetupHud();

		Event.Register( this );
		Event.Run( Event.Game.LoadAttribute.Name );
	}

	private void SetupEntities()
	{
		SunLight = new Sun() { Position = new( 0, 100, 100 ) };

		_ = new Water() { Scale = new Vector3( 10000f ) };
		_ = new Sky();

		var modelFile = new ModelFile( "lobby/terrain/Fan_isle.md2" );
		foreach ( var mesh in modelFile.Meshes )
		{
			var material = new Material<ObjectUniformBuffer>( "content/shaders/test.shader" );
			var textures = new List<Texture>();

			Log.Info( $"{mesh.Name}: " );

			for ( int i = 0; i < 16; ++i )
			{
				if ( mesh.Materials.Length <= i )
				{
					textures.Add( Texture.Missing );
				}
				else
				{
					var j = mesh.Materials[i];
					Log.Info( $"\tMaterial:" );
					Log.Info( $"\tName: {j.Name}" );
					Log.Info( $"\tFlags: {j.Flags}" );
					textures.Add( new Texture( $"lobby/terrain/textures/{j.Name}.wct", TextureFlags.Repeat ) );
				}
			}

			material.Set( $"Color", textures.ToArray() );

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
			Matrix4x4.Decompose( mesh.TransformMatrix, out var scale, out var rot, out var pos );

			pos = new System.Numerics.Vector3( pos.X, pos.Z, pos.Y );
			rot = new Quaternion( rot.X, rot.Z, rot.Y, -rot.W );
			scale = new System.Numerics.Vector3( scale.X, scale.Z, scale.Y );

			var m = new ModelEntity
			{
				Model = model,
				Scale = scale,
				Rotation = rot,
				Position = pos
			};
		}
	}

	private void SetupHud()
	{
		Hud = new();

		var layout = new LobbyLayout() { Hud = Hud };
		layout.OnInit();

		Hud.AddChild( new Cursor() );
	}

	public void Update()
	{
		Entity.All.ForEach( entity => entity.Update() );
	}

	public void Render()
	{
		Camera.Update();

		Entity.All.ForEach( entity => entity.Render() );
	}

	[Event.Game.Load]
	public void OnLoad()
	{
		Entity.All.OfType<Ride>().ToList().ForEach( x => x.VM.Run() );
	}
}
