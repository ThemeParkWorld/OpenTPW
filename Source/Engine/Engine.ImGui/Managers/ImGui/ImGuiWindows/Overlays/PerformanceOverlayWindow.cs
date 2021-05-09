using Engine.Assets;
using Engine.Utils;
using ImGuiNET;
using Quincy.Managers;
using System.Numerics;

namespace Engine.Gui.Managers.ImGuiWindows.Overlays
{
    class PerformanceOverlayWindow : ImGuiWindow
    {
        public override bool Render { get; set; } = true;
        public override string IconGlyph { get; } = FontAwesome5.Question;
        public override string Title { get; } = "Performance Overlay";
        public override ImGuiWindowFlags Flags => ImGuiWindowFlags.NoTitleBar
                                                  | ImGuiWindowFlags.NoResize
                                                  | ImGuiWindowFlags.NoMove
                                                  | ImGuiWindowFlags.NoDocking
                                                  | ImGuiWindowFlags.NoDecoration
                                                  | ImGuiWindowFlags.NoBackground;

        public override void Draw()
        {
            ImGui.SetWindowSize(new Vector2(0, 0));
            ImGui.SetWindowPos(new Vector2(EngineSettings.GameResolutionX - 150 - ImGui.GetStyle().WindowPadding.X - ImGui.GetStyle().FramePadding.X, 0));

            var debugText = FontAwesome5.SpaceShuttle + " Engine\n" +
                            "F1 for editor\n" +
                            "F2 for cursor lock toggle\n" +
                            "F3 for console toggle";

            DrawShadowLabel(debugText);

            ImGui.PlotHistogram(
                $"{RenderManager.Instance.LastFrameTime}ms",
                ref RenderManager.Instance.FrametimeHistory[0],
                RenderManager.Instance.FrametimeHistory.Length,
                0,
                "",
                0
            );

            ImGui.PlotLines(
                $"{RenderManager.Instance.CalculatedFramerate}fps",
                ref RenderManager.Instance.FramerateHistory[0],
                RenderManager.Instance.FramerateHistory.Length,
                0,
                "",
                0
            );
        }
    }
}
