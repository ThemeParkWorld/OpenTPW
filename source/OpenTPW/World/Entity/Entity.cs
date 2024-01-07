using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;

namespace OpenTPW;

public class Entity
{
	public Level Level { get; set; }
	public static List<Entity> All { get; set; } = Assembly.GetCallingAssembly().GetTypes().OfType<Entity>().ToList();

	/// <summary>
	/// Right, Up, Forward (FLU)
	/// </summary>
	public Vector3 Position;

	/// <summary>
	/// Pitch, Yaw, Roll (PYR)
	/// </summary>
	public Vector3 Rotation;

	public Vector3 Scale = Vector3.One;

	public string Name { get; set; }

	public Matrix4x4 ModelMatrix
	{
		get
		{
			var matrix = Matrix4x4.CreateScale( Scale );
			matrix *= Matrix4x4.CreateTranslation( Position );
			matrix *= Matrix4x4.CreateFromYawPitchRoll(
				Rotation.Y.DegreesToRadians(),
				Rotation.X.DegreesToRadians(),
				Rotation.Z.DegreesToRadians()
			);

			return matrix;
		}
	}

	public Entity()
	{
		Level = Level.Current;
		All.Add( this );
		Name = $"{this.GetType().Name} {All.Count}";
	}

	public void Render()
	{
		OnRender();
	}

	public void Update()
	{
		OnUpdate();
	}
	public void Delete()
	{
		OnDelete();
	}

	protected virtual void OnRender() { }
	protected virtual void OnUpdate() { }
	protected virtual void OnDelete() { }

	public bool Equals( Entity x, Entity y ) => x.GetHashCode() == y.GetHashCode();
	public int GetHashCode( [DisallowNull] Entity obj ) => base.GetHashCode();
}
