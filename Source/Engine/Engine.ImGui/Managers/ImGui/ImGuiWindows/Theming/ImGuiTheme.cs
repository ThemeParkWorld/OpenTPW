using Engine.Utils.DebugUtils;
using Engine.Utils.FileUtils;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Engine.Gui.Managers.ImGuiWindows.Theming
{
    public class ImGuiThemeColors
    {
        public string Text { get; set; }
        public string TextDisabled { get; set; }
        public string WindowBg { get; set; }
        public string ChildBg { get; set; }
        public string PopupBg { get; set; }
        public string Border { get; set; }
        public string BorderShadow { get; set; }
        public string FrameBg { get; set; }
        public string FrameBgHovered { get; set; }
        public string FrameBgActive { get; set; }
        public string TitleBg { get; set; }
        public string TitleBgActive { get; set; }
        public string TitleBgCollapsed { get; set; }
        public string MenuBarBg { get; set; }
        public string ScrollbarBg { get; set; }
        public string ScrollbarGrab { get; set; }
        public string ScrollbarGrabHovered { get; set; }
        public string ScrollbarGrabActive { get; set; }
        public string CheckMark { get; set; }
        public string SliderGrab { get; set; }
        public string SliderGrabActive { get; set; }
        public string Button { get; set; }
        public string ButtonHovered { get; set; }
        public string ButtonActive { get; set; }
        public string Header { get; set; }
        public string HeaderHovered { get; set; }
        public string HeaderActive { get; set; }
        public string Separator { get; set; }
        public string SeparatorHovered { get; set; }
        public string SeparatorActive { get; set; }
        public string ResizeGrip { get; set; }
        public string ResizeGripHovered { get; set; }
        public string ResizeGripActive { get; set; }
        public string Tab { get; set; }
        public string TabHovered { get; set; }
        public string TabActive { get; set; }
        public string TabUnfocused { get; set; }
        public string TabUnfocusedActive { get; set; }
        public string PlotLines { get; set; }
        public string PlotLinesHovered { get; set; }
        public string PlotHistogram { get; set; }
        public string PlotHistogramHovered { get; set; }
        public string TextSelectedBg { get; set; }
        public string DragDropTarget { get; set; }
        public string NavHighlight { get; set; }
        public string NavWindowingHighlight { get; set; }
        public string NavWindowingDimBg { get; set; }
        public string ModalWindowDimBg { get; set; }
    }

    public class ImGuiStyleProperties
    {
        public float Alpha { get; set; } = -1;
        public float WindowRounding { get; set; } = -1;
        public float ChildRounding { get; set; } = -1;
        public float PopupRounding { get; set; } = -1;
        public float FrameRounding { get; set; } = -1;
        public float ScrollbarRounding { get; set; } = -1;
        public float GrabRounding { get; set; } = -1;
        public float TabRounding { get; set; } = -1;
        public float ChildBorderSize { get; set; } = -1;
        public float PopupBorderSize { get; set; } = -1;
        public float FrameBorderSize { get; set; } = -1;
        public float WindowBorderSize { get; set; } = -1;
        public float IndentSpacing { get; set; } = -1;
        public float ScrollbarSize { get; set; } = -1;
        public float GrabMinSize { get; set; } = -1;

        public string FramePadding { get; set; }
        public string ButtonTextAlign { get; set; }
        public string SelectableTextAlign { get; set; }
        public string ItemInnerSpacing { get; set; }
        public string ItemSpacing { get; set; }
        public string WindowPadding { get; set; }
        public string WindowMinSize { get; set; }
        public string WindowTitleAlign { get; set; }
    }

    public class ImGuiThemeMeta
    {
        public string Author { get; set; }
        public string Name { get; set; }
    }

    public static class Vector4Extensions
    {
        public static Vector4 Parse(this Vector4 a, string str)
        {
            return new Vector4(0, 0, 0, 0);
        }
    }

    public struct ImGuiTheme
    {
        public ImGuiStyleProperties Style { get; set; }
        public ImGuiThemeColors Colors { get; set; }
        public ImGuiThemeMeta Meta { get; set; }

        private const string numRegex = @"(\d+\.\d+|\d+)";

        public static ImGuiTheme Load(Asset asset)
        {
            var str = asset.AsString();
            return JsonConvert.DeserializeObject<ImGuiTheme>(str);
        }

        private static Vector2 ParseVector2String(string str)
        {
            var vec2 = new Vector2();
            // <x, y> - same as Vector2.ToString()
            var matches = Regex.Matches(str, numRegex);

            vec2.X = float.Parse(matches[0].Value);
            vec2.Y = float.Parse(matches[1].Value);

            return vec2;
        }
        private static Vector4 ParseVector4String(string str)
        {
            var vec4 = new Vector4();
            // <x, y, z, w> - same as Vector4.ToString()
            var matches = Regex.Matches(str, numRegex);

            vec4.X = float.Parse(matches[0].Value);
            vec4.Y = float.Parse(matches[1].Value);
            vec4.Z = float.Parse(matches[2].Value);
            vec4.W = float.Parse(matches[3].Value);

            return vec4;
        }

        private void SetColors()
        {
            var colors = ImGui.GetStyle().Colors;
            foreach (var property in Colors.GetType().GetProperties())
            {
                var colorEnum = Enum.Parse<ImGuiCol>(property.Name);

                var propertyValue = (string)property.GetValue(Colors);

                if (string.IsNullOrEmpty(propertyValue))
                    continue;

                colors[(int)colorEnum] = ParseVector4String(propertyValue);
            }
        }

        private void SetStyle()
        {
            foreach (var property in Style.GetType().GetProperties())
            {
                var styleEnum = Enum.Parse<ImGuiStyleVar>(property.Name);
                var propertyValue = property.GetValue(Style);

                if (propertyValue == null)
                    continue;

                if (propertyValue.GetType() == typeof(string))
                {
                    if (string.IsNullOrEmpty((string)propertyValue))
                        continue;

                    ImGui.PushStyleVar(styleEnum, ParseVector2String((string)propertyValue));
                }
                else if (propertyValue.GetType() == typeof(float))
                {
                    if ((float)propertyValue < 0)
                        continue;

                    ImGui.PushStyleVar(styleEnum, (float)propertyValue);
                }
            }
        }

        public void SetTheme()
        {
            SetColors();
            SetStyle();
        }
    }
}
