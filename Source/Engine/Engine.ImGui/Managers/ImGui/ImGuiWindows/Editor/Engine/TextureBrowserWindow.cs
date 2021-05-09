using Engine.Assets;
using ImGuiNET;
using System;
using System.Numerics;
using Engine.Gui.Managers;
using Engine.Gui.Managers.ImGuiWindows;
using Quincy;

namespace Engine.GUI.Managers.ImGuiWindows.Editor
{
    [ImGuiMenuPath(ImGuiMenus.Menu.Engine)]
    class TextureBrowserWindow : ImGuiWindow
    {
        public override string Title { get; } = "Texture Browser";
        public override string IconGlyph { get; } = FontAwesome5.Square;
        public override bool Render { get; set; }

        private int selectedTexture;

        public override void Draw()
        {
            var textureList = TextureContainer.Textures;
            var texturePaths = TextureContainer.TexturePaths;

            ImGui.Combo("Texture", ref selectedTexture, texturePaths.ToArray(), texturePaths.Count);

            if (selectedTexture > textureList.Count - 1)
                selectedTexture = textureList.Count - 1;

            if (selectedTexture < 0)
                selectedTexture = 0;

            var texture = textureList[selectedTexture];

            var windowWidth = ImGui.GetWindowSize().X;
            var ratio = 1f;

            ImGui.Text($"Texture selected: {texture.Id}");
            ImGui.Image((IntPtr)texture.Id, new Vector2(windowWidth, windowWidth * ratio), new Vector2(0, 1), new Vector2(1, 0));
        }
    }
}