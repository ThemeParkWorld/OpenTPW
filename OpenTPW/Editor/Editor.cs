using ImGuiNET;
using Silk.NET.OpenGL.Extensions.ImGui;
using System.Runtime.InteropServices;

namespace OpenTPW;
internal partial class Editor
{
	ImGuiController ImGuiController;

	List<BaseTab> tabs = new();

	Texture defaultFontTexture;

	public Editor( ImGuiController imGuiController )
	{
		ImGuiController = imGuiController;
		InitIO();
		SetTheme();

		tabs.AddRange( new BaseTab[]{
				new TexturesTab(),
				new ConsoleTab(),
			} );
	}

	private void InitIO()
	{
		var io = ImGui.GetIO();

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

		ImGui.DockSpaceOverViewport( ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode );
		ImGui.ShowDemoWindow();

		tabs.ForEach( tab => tab.Draw() );
	}
}

