using Engine.Assets;
using Engine.Gui.Managers.ImGuiWindows.Theming;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.Gui.Managers.ImGuiWindows.Editor
{
    [ImGuiMenuPath(ImGuiMenus.Menu.Engine)]
    class EngineConfigWindow : ImGuiWindow
    {
        public override bool Render { get; set; }
        public override string IconGlyph { get; } = FontAwesome5.Cog;
        public override string Title { get; } = "Engine Config";

        private List<ImGuiTheme> themes = new List<ImGuiTheme>();
        private string[] themeNames;
        private ImGuiTheme defaultTheme;
        private int currentTheme;

        public EngineConfigWindow()
        {
            SetupDefaultTheme();
            LoadThemes();
        }

        private void LoadThemes()
        {
            //foreach (var file in Directory.GetFiles("Content/Themes"))
            //{
            //    if (file.EndsWith(".json", StringComparison.CurrentCultureIgnoreCase))
            //    {
            //        themes.Add(ImGuiTheme.LoadFromFile(file));
            //    }
            //}

            themeNames = new string[themes.Count];
            for (int i = 0; i < themes.Count; ++i)
            {
                themeNames[i] = themes[i].Meta.Name;
            }
        }

        /// <summary>
        /// Sets up the default / base spacing (no colors) for theming
        /// </summary>
        private void SetupDefaultTheme()
        {
            defaultTheme = new ImGuiTheme();
            defaultTheme.Style = new ImGuiStyleProperties()
            {
                WindowPadding = "<8, 8>",
                FramePadding = "<4, 3>",
                ItemSpacing = "<8, 4>",
                ItemInnerSpacing = "<4, 4>",
                IndentSpacing = 21,
                ScrollbarSize = 14,
                GrabMinSize = 10,
                WindowBorderSize = 1,
                ChildBorderSize = 1,
                PopupBorderSize = 1,
                FrameBorderSize = 0,
                WindowRounding = 7,
                ChildRounding = 0,
                FrameRounding = 0,
                PopupRounding = 0,
                ScrollbarRounding = 9,
                GrabRounding = 0,
                TabRounding = 4,
                WindowTitleAlign = "<0.00, 0.50>",
                ButtonTextAlign = "<0.50, 0.50>",
                SelectableTextAlign = "<0, 0>",
                Alpha = 1,
                WindowMinSize = "<0, 0>"
            };
            defaultTheme.Colors = new ImGuiThemeColors();
            defaultTheme.Meta = new ImGuiThemeMeta();
        }

        private void UnsetTheme()
        {
            defaultTheme.SetTheme();
        }

        private void UpdateTheme()
        {
            ImGuiManager.Instance.Theme = themes[currentTheme];
        }

        public override void Draw()
        {
            ImGui.Text("Editor Themes");
            ImGui.SetNextItemWidth(0);
            if (ImGui.ListBox("##hidelabel", ref currentTheme, themeNames, themeNames.Length))
            {
                UnsetTheme();
                UpdateTheme();
            }
        }
    }
}
