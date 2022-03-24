namespace OpenTPW;

public class BaseFileHandler
{
	protected byte[] FileData { get; set; }

	public BaseFileHandler( byte[] fileData )
	{
		this.FileData = fileData;
	}

	public virtual void Draw() { }
}
