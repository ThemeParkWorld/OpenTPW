global using static OpenTPW.Common.GlobalNamespace;

namespace OpenTPW.Common;
public static class GlobalNamespace
{
	public static Logger Log { get; set; }
	public static BaseFileSystem FileSystem { get; set; }
	public static BaseFileSystem SaveFileSystem { get; set; }
	public static BaseFileSystem CacheFileSystem { get; set; }
}
