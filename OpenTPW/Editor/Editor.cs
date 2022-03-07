using ImGuiNET;
using Silk.NET.OpenGL.Extensions.ImGui;
using System.Runtime.InteropServices;

namespace OpenTPW;
internal partial class Editor
{
	private ImGuiController ImGuiController;
	private Texture defaultFontTexture;

	private List<BaseTab> tabs = new();

	private bool shouldRender;

	public Editor( ImGuiController imGuiController )
	{
		ImGuiController = imGuiController;

		InitIO();
		SetTheme();

		tabs.AddRange( new BaseTab[] {
			new TexturesTab(),
			new ConsoleTab(),
			new InputTab()
		} );
	}

	private void InitIO()
	{
		var io = ImGui.GetIO();

		io.ConfigFlags |= ImGuiConfigFlags.DockingEnable | ImGuiConfigFlags.IsSRGB;

		io.Fonts.Clear();

		io.Fonts.AddFontFromFileTTF( "content/fonts/Roboto-Regular.ttf", 14f );
		io.Fonts.GetTexDataAsRGBA32( out IntPtr pixels, out var width, out var height, out var bpp );

		int size = width * height * bpp;
		byte[] data = new byte[size];
		Marshal.Copy( pixels, data, 0, size );
		defaultFontTexture = TextureBuilder.FromBytes( data, (uint)width, (uint)height ).Build();

		io.Fonts.SetTexID( (IntPtr)defaultFontTexture.Id );
		io.Fonts.ClearTexData();
	}

	public void Update()
	{
		ImGuiController.Update( Time.Delta );

		if ( Input.Pressed( InputButton.ConsoleToggle ) )
			shouldRender = !shouldRender;

		if ( !shouldRender )
			return;

		ImGui.DockSpaceOverViewport( ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode );
		ImGui.ShowDemoWindow();

		tabs.ForEach( tab => tab.Draw() );
	}
}

