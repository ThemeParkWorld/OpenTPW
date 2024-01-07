namespace OpenTPW;

[Flags]
public enum TextureFlags
{
	None,

	/// <summary>
	/// If this is set, then the pixel value (255, 0, 255) will be treated as a transparent pixel.
	/// </summary>
	PinkChromaKey,

	/// <summary>
	/// Force point filtering for this texture's sampler
	/// </summary>
	PointFilter
}
