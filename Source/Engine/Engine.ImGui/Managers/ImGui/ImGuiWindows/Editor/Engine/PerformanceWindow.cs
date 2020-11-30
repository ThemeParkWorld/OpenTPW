using Engine.Assets;
using ImGuiNET;
using Quincy.Managers;

namespace Engine.Gui.Managers.ImGuiWindows.Editor
{
    [ImGuiMenuPath(ImGuiMenus.Menu.Engine)]
    class PerformanceWindow : ImGuiWindow
    {
        public override bool Render { get; set; }
        public override string IconGlyph { get; } = FontAwesome5.ChartLine;
        public override string Title { get; } = "Performance";

        // this is probably extremely bad in terms of performance
        public float AverageFramerate
        {
            get
            {
                float averageFramerate = 0;
                for (int i = 0; i < RenderManager.Instance.FramerateHistory.Length; ++i)
                {
                    averageFramerate += RenderManager.Instance.FramerateHistory[i];
                }

                averageFramerate /= RenderManager.Instance.FramerateHistory.Length;
                return averageFramerate;
            }
        }
        public float AverageFrameTime

        {
            get
            {
                float averageFrameTime = 0;
                for (int i = 0; i < RenderManager.Instance.FrametimeHistory.Length; ++i)
                {
                    averageFrameTime += RenderManager.Instance.FrametimeHistory[i];
                }

                averageFrameTime /= RenderManager.Instance.FrametimeHistory.Length;
                return averageFrameTime;
            }
        }

        public override void Draw()
        {
            ImGui.LabelText($"{RenderManager.Instance.LastFrameTime}ms", "Current frame time");

            ImGui.PlotHistogram(
                "Frame time / time",
                ref RenderManager.Instance.FrametimeHistory[0],
                RenderManager.Instance.FrametimeHistory.Length,
                0,
                "",
                0
            );
            ImGui.LabelText($"{AverageFrameTime}fps", "Average frame time");

            ImGui.LabelText($"{RenderManager.Instance.CalculatedFramerate}fps", "Current framerate");
            ImGui.PlotLines(
                "Frame rate / time",
                ref RenderManager.Instance.FramerateHistory[0],
                RenderManager.Instance.FramerateHistory.Length,
                0,
                "",
                0
            );

            ImGui.LabelText($"{AverageFramerate}fps", "Average framerate");

            bool paused = RenderManager.Instance.Paused;
            ImGui.Checkbox("Pause", ref paused);
            RenderManager.Instance.Paused = paused;
        }
    }
}
