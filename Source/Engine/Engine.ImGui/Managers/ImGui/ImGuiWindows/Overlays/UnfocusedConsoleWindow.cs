using Engine.Assets;
using Engine.Utils;
using Engine.Utils.DebugUtils;
using ImGuiNET;
using System;
using System.Linq;
using System.Numerics;

namespace Engine.Gui.Managers.ImGuiWindows.Overlays
{
    class UnfocusedConsoleWindow : ImGuiWindow
    {
        public override bool Render { get; set; } = true;
        public override string IconGlyph { get; } = FontAwesome5.Question;
        public override string Title { get; } = "Console Overlay";
        public override ImGuiWindowFlags Flags => ImGuiWindowFlags.NoTitleBar
                                                  | ImGuiWindowFlags.NoResize
                                                  | ImGuiWindowFlags.NoMove
                                                  | ImGuiWindowFlags.NoDocking
                                                  | ImGuiWindowFlags.NoDecoration
                                                  | ImGuiWindowFlags.NoBackground;
        private const int logLimitSeconds = 5;
        private const int logLimit = 5;

        public override void Draw()
        {
            ImGui.PushFont(ImGuiManager.Instance.MonospacedFont);
            ImGui.SetWindowPos(new Vector2(0, 0));
            ImGui.SetWindowSize(new Vector2(-1, Math.Min(GameSettings.GameResolutionX - 250, 500)));

            // This isn't 100% correct (TakeLast and Where should really be swapped), but it's faster than having linq loop 
            // through every single log entry every frame...
            foreach (var logEntry in Logging.LogEntries.TakeLast(logLimit).Where(entry => (DateTime.Now - entry.timestamp).TotalSeconds <= logLimitSeconds))
            {
                DrawShadowLabel(logEntry.ToString());
            }

            ImGui.PopFont();
        }
    }
}
