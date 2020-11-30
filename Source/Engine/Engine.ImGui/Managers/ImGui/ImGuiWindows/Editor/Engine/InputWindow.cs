using Engine.Assets;
using ImGuiNET;

namespace Engine.Gui.Managers.ImGuiWindows.Editor
{
    [ImGuiMenuPath(ImGuiMenus.Menu.Engine)]
    public sealed class InputWindow : ImGuiWindow
    {
        public override bool Render { get; set; }
        public override string Title => "Input Manager";
        public override string IconGlyph => FontAwesome5.Gamepad;

        public override void Draw()
        {
            ImGui.Text("Input Manager");

            // Action Maps
            ImGui.Columns(3, "inputColumns", true);

            ImGui.BeginChild("actionMaps");

            ImGui.Text("Action Maps");
            ImGui.SameLine();
            ImGui.Button(FontAwesome5.Plus);

            ImGui.Separator();

            ImGui.EndChild();
            ImGui.NextColumn();

            // Actions
            ImGui.BeginChild("actions");

            ImGui.Text("Actions");
            ImGui.SameLine();
            ImGui.Button(FontAwesome5.Plus);

            ImGui.Separator();

            ImGui.EndChild();
            ImGui.NextColumn();

            // Properties
            ImGui.BeginChild("properties");

            ImGui.Text("Select an item to view its properties");

            ImGui.EndChild();
            ImGui.NextColumn();

            ImGui.Columns(1);
        }
    }
}
