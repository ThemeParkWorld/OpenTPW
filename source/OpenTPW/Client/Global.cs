global using static Global;
using OpenTPW;
using Veldrid;

public static class Global
{
	internal static GraphicsDevice Device { get; set; } = null!;
	internal static Renderer Render { get; set; } = null!;
}
