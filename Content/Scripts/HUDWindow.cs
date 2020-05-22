using ECSEngine.Assets;
using ECSEngine.DebugUtils;
using ECSEngine.Managers;
using ECSEngine.Managers.ImGuiWindows;
using ECSEngine.Managers.Scripting;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;

namespace UlaidGame.Scripts
{
    class PackageHook
    {
        public void Run()
        {
            Logging.Log("Hello, world!");
            ImGuiManager.Instance.Menus.Add(new ImGuiMenu(FontAwesome5.FileCode, "Custom", new List<IImGuiWindow>()
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

    class HUDWindow : IImGuiWindow
    {
        public bool Render { get; set; }
        public string Title { get; } = "Scripts";
        public string IconGlyph { get; } = FontAwesome5.FileCode;

        private Player Player { get; } = new Player();

        private void DrawScriptsWindow()
        {
            for (int i = 0; i < ScriptManager.Instance.ScriptList.Count; ++i)
            {
                var scriptName = ScriptManager.Instance.ScriptList.Keys.ToArray()[i];
                ImGui.LabelText("##hidelabel", scriptName);
                ImGui.SameLine();
                if (ImGui.Button("Reload"))
                {
                    ScriptManager.Instance.ReloadScript(scriptName);
                }
            }
        }

        public void Draw()
        {
            DrawScriptsWindow();
        }
    }
}