namespace OpenTPW;

public class BaseFileManager
{
	protected byte[] FileData { get; set; }

	public BaseFileManager( byte[] fileData )
	{
		this.FileData = fileData;
	}

	public virtual void Draw() { }
}
