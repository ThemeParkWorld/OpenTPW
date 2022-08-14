using Veldrid;

namespace OpenTPW;

public partial class ModelEntity : Entity
{
	public Model? Model { get; set; }

	public ModelEntity()
	{
		Spawn();
	}

	public virtual void Spawn()
	{

	}

	public override void Render( CommandList commandList )
	{
		base.Render( commandList );

		if ( Model == null )
			return;

		var uniformBuffer = new ObjectUniformBuffer
		{
			g_mModel = ModelMatrix,
			g_mView = World.Current.Camera.ViewMatrix,
			g_mProj = World.Current.Camera.ProjMatrix,
			g_vLightPos = World.Current.Sun.position,
			g_vLightColor = World.Current.Sun.Color,
			g_vCameraPos = World.Current.Camera.position,

			_padding0 = 0,
			_padding1 = 0,
			_padding2 = 0
		};

		Model.Draw( uniformBuffer, commandList );
	}
}
