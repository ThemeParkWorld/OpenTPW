namespace OpenTPW;

partial class Material
{
	/// <summary>
	/// Default shader for 2D UI objects
	/// </summary>
	public static Material UI = new( "content/shaders/ui.shader" );

	/// <summary>
	/// Default shader for 3D objects
	/// </summary>
	public static Material Default = new Material<ObjectUniformBuffer>( "content/shaders/3d.shader" );
}
