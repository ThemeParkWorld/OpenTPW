using Engine.Assets;
using Engine.Gui.Managers;
using Engine.Gui.Managers.ImGuiWindows;
using Engine.Utils.DebugUtils;
using System.Collections.Generic;

namespace Example.Scripts
{
    class PackageHook
    {
        public void Run()
        {
            Logging.Log("Hello, world!");
            ImGuiManager.Instance.Menus.Add(new ImGuiMenu(FontAwesome5.FileCode, "Custom", new List<ImGuiWindow>()
            {
                new HUDWindow()
            }));
        }

        public void Unload() { }
    }

    class Player
    {
        public int Health { get; } = 100;
        public int Shields { get; } = 100;
    }

    class HUDWindow : ImGuiWindow
    {
        public override bool Render { get; set; }
        public override string Title { get; } = "Scripts";
        public override string IconGlyph { get; } = FontAwesome5.FileCode;

        private Player Player { get; } = new Player();

        private void DrawScriptsWindow()
        {
            //for (int i = 0; i < ScriptManager.Instance.ScriptList.Count; ++i)
            //{
            //    var scriptName = ScriptManager.Instance.ScriptList.Keys.ToArray()[i];
            //    ImGui.LabelText("##hidelabel", scriptName);
            //    ImGui.SameLine();
            //    if (ImGui.Button("Reload"))
            //    {
            //        ScriptManager.Instance.ReloadScript(scriptName);
            //    }
            //}
        }

        public override void Draw()
        {
            DrawScriptsWindow();
        }
    }
}