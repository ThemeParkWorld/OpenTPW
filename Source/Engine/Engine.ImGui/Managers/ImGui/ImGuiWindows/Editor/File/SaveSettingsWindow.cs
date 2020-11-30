using Engine.Assets;
using Engine.Utils;
using ImGuiNET;

namespace Engine.Gui.Managers.ImGuiWindows.Editor
{
    // Definitely a HACK. TODO: Improve
    [ImGuiMenuPath(ImGuiMenus.Menu.File)]
    class SaveSettingsWindow : ImGuiWindow
    {
        public override bool Render { get; set; }
        public override string IconGlyph { get; } = FontAwesome5.Save;
        public override string Title { get; } = "Save Settings";
        public override ImGuiWindowFlags Flags { get; } = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoDecoration;

        public override void Draw()
        {
            GameSettings.SaveValues();
            Render = false;
        }
    }
}
