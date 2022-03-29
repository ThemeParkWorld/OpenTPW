using OpenTPW;
using Veldrid;

public static class Global
{
	public static Logger Log { get; } = new();
	public static GraphicsDevice Device { get; internal set; }
}
