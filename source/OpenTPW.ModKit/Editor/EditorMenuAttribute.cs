namespace OpenTPW;

[AttributeUsage( AttributeTargets.Class )]
public class EditorMenuAttribute : Attribute
{
	public string Path { get; set; }

	public EditorMenuAttribute( string path )
	{
		Path = path;
	}
}
