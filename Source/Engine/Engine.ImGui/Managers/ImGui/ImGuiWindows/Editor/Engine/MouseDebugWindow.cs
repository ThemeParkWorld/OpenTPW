using Engine.Assets;
using ImGuiNET;

namespace Engine.Gui.Managers.ImGuiWindows.Editor.Engine
{
    [ImGuiMenuPath(ImGuiMenus.Menu.Engine)]
    public class MouseDebugWindow : ImGuiWindow
    {
        public override bool Render { get; set; }
        public override string Title => "Mouse Debug";
        public override string IconGlyph => FontAwesome5.Mouse;

        public override void Draw()
        {
            ImGui.Text($"LMB: {ImGui.IsMouseDown(ImGuiMouseButton.Left)}");
            ImGui.Text($"RMB: {ImGui.IsMouseDown(ImGuiMouseButton.Right)}");
            ImGui.Text($"MMB: {ImGui.IsMouseDown(ImGuiMouseButton.Middle)}");
            
            ImGui.Text($"LMB Drag: {ImGui.IsMouseDragging(ImGuiMouseButton.Left)}");
            ImGui.Text($"RMB Drag: {ImGui.IsMouseDragging(ImGuiMouseButton.Right)}");
            ImGui.Text($"MMB Drag: {ImGui.IsMouseDragging(ImGuiMouseButton.Middle)}");

            ImGui.Text($"Position: {ImGui.GetMousePos()}");
        }
    }
}
