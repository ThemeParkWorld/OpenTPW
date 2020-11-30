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

        public override void Draw()
        {
            var debugText = FontAwesome5.SpaceShuttle + " Engine\n" +
                            "F1 for editor\n" +
                            "F2 for cursor lock toggle\n" +
                            "F3 for console toggle";

            ImGui.Begin("perfOverlayGraphs##hidelabel", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoInputs);

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

            ImGui.End();

            var labelPosition = new Vector2(GameSettings.GameResolutionX - 192, ImGui.GetStyle().WindowPadding.Y);
            DrawShadowLabel(debugText, labelPosition);
            var labelEnd = ImGui.CalcTextSize(debugText) + labelPosition;

            ImGui.SetWindowSize("perfOverlayGraphs##hidelabel", new Vector2(0, 0));
            ImGui.SetWindowPos("perfOverlayGraphs##hidelabel", new Vector2(labelPosition.X - ImGui.GetStyle().WindowPadding.X - ImGui.GetStyle().FramePadding.X, labelEnd.Y));
        }
    }
}
