using Engine.Assets;
using Engine.Utils.DebugUtils;
using ImGuiNET;
using System.Linq;
using System.Numerics;

namespace Engine.Gui.Managers.ImGuiWindows.Overlays
{
    class UnfocusedConsoleWindow : ImGuiWindow
    {
        public override bool Render { get; set; } = true;
        public override string IconGlyph { get; } = FontAwesome5.Question;
        public override string Title { get; } = "Console Overlay";

        private int logLimit = 5;

        public override void Draw()
        {
            var currentOffset = new Vector2();
            
            ImGui.PushFont(ImGuiManager.Instance.MonospacedFont);
            foreach (var logEntry in Logging.LogEntries.TakeLast(logLimit))
            {
                DrawShadowLabel(logEntry.ToString(), ImGui.GetStyle().WindowPadding + currentOffset);
                currentOffset += new Vector2(0, ImGui.GetStyle().ItemSpacing.Y + 12);
            }
            ImGui.PopFont();
        }
    }
}
