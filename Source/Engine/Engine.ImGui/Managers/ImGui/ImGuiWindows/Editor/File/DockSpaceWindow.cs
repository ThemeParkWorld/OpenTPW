using Engine.Assets;
using Engine.Utils;
using ImGuiNET;
using System.Numerics;

namespace Engine.Gui.Managers.ImGuiWindows.Editor.Engine
{
    [ImGuiMenuPath(ImGuiMenus.Menu.File)]
    class DockSpaceWindow : ImGuiWindow
    {
        public override bool Render { get; set; } = true;

        public override string Title => "Main DockSpace Window";

        public override string IconGlyph => FontAwesome5.Question;

        public override ImGuiWindowFlags Flags => ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBackground;

        public override void Draw()
        {
            var dockSpaceId = ImGui.GetID("mainDockSpace");
            ImGui.DockSpace(dockSpaceId, new Vector2(-1, -1), ImGuiDockNodeFlags.PassthruCentralNode);

            ImGui.SetWindowSize(new Vector2(GameSettings.GameResolutionX, GameSettings.GameResolutionY));
            ImGui.SetWindowPos(new Vector2(0, 18 /* Menu bar hack */));
        }
    }
}
