using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace OpenTPW;

public class Entity
{
	public static List<Entity> All { get; set; } = Assembly.GetCallingAssembly().GetTypes().OfType<Entity>().ToList();

	//
	// Transform
	// These aren't properties because we want to be able to add to them
	//

	/// <summary>
	/// Right, Up, Forward (RUF)
	/// </summary>
	public Vector3 position;

	/// <summary>
	/// Pitch, Yaw, Roll (PYR)
	/// </summary>
	public Vector3 rotation;
	public Vector3 scale = Vector3.One;

	public string Name { get; set; }

	public Matrix4X4 ModelMatrix
	{
		get
		{
			var matrix = Silk.NET.Maths.Matrix4X4.CreateScale( scale );
			matrix *= Silk.NET.Maths.Matrix4X4.CreateTranslation( position );
			matrix *= Silk.NET.Maths.Matrix4X4.CreateFromYawPitchRoll(
				rotation.Y.DegreesToRadians(),
				rotation.X.DegreesToRadians(),
				rotation.Z.DegreesToRadians() );

			return matrix;
		}
	}

	public Entity()
	{
		All.Add( this );
		Name = $"{this.GetType().Name} {All.Count}";
	}

	public virtual void Render() { }
	public virtual void Update() { }

	public virtual void Delete() { }

	public bool Equals( Entity x, Entity y ) => x.GetHashCode() == y.GetHashCode();
	public int GetHashCode( [DisallowNull] Entity obj ) => base.GetHashCode();
}
