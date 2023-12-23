namespace OpenTPW;

public interface IFileFormat
{
	/// <summary>
	/// Reads the format's data from a given stream
	/// </summary>
	/// <param name="stream"></param>
	void ReadFromStream( Stream stream );
}
