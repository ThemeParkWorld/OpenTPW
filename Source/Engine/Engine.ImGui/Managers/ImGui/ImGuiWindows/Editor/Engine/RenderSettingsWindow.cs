using Engine.Assets;
using ImGuiNET;
using Quincy.Managers;
using System;
using System.Collections.Generic;
using static Quincy.Managers.RenderManager;

namespace Engine.Gui.Managers.ImGuiWindows.Editor
{
    [ImGuiMenuPath(ImGuiMenus.Menu.Engine)]
    class RenderSettingsWindow : ImGuiWindow
    {
        public override bool Render { get; set; }
        public override string IconGlyph { get; } = FontAwesome5.DrawPolygon;
        public override string Title { get; } = "Render Settings";

        public override void Draw()
        {
            ImGui.SliderFloat("Exposure", ref RenderManager.Instance.exposure, 0.0f, 10.0f, "%.2f", ImGuiSliderFlags.Logarithmic);
            ImGui.InputText("HDRI", ref RenderManager.Instance.hdri, 256, ImGuiInputTextFlags.None);

            var tonemapOperator = (int)RenderManager.Instance.tonemapOperator;
            var values = new List<string>();
            foreach (var val in Enum.GetValues(typeof(TonemapOperator)))
            {
                values.Add(val.ToString());
            }

            ImGui.Combo("Tonemap Operator", ref tonemapOperator, values.ToArray(), values.Count, 3);
            RenderManager.Instance.tonemapOperator = (TonemapOperator)tonemapOperator;
        }
    }
}
