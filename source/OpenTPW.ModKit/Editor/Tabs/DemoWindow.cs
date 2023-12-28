using ImGuiNET;

namespace OpenTPW;

[EditorMenu( "Editor/ImGUI Demo Window" )]
internal class DemoWindow : BaseTab
{
	public override void Draw()
	{
		ImGui.ShowDemoWindow( ref visible );
	}
}
