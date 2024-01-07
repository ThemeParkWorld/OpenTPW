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

	protected override void OnRender()
	{
		if ( Model == null )
			return;

		var uniformBuffer = new ObjectUniformBuffer
		{
			g_mModel = ModelMatrix,
			g_mView = Camera.ViewMatrix,
			g_mProj = Camera.ProjMatrix,
			g_vLightPos = Level.SunLight.Position,
			g_vLightColor = Level.SunLight.Color,
			g_vCameraPos = Camera.Position,

			_padding0 = 0,
			_padding1 = 0,
			_padding2 = 0
		};

		Model.Draw( uniformBuffer );
	}
}
