using Engine;
using Engine.ECS.Entities;
using Engine.Gui.Managers;
using Engine.Gui.Managers.ImGuiWindows.Theming;
using Engine.Renderer.GL.Managers;
using ImGuiNET;
using OpenTPW.Entities;
using OpenTPW.Files;
using System.Collections.Generic;

namespace OpenTPW
{
    internal sealed class TPWGame : Game
    {
        public TPWGame(string gamePropertyPath) : base(gamePropertyPath) { }

        protected override void InitScene()
        {
            // TODO: Set up an archive opening system instead of doing it per-entity
            base.InitScene();
            var entities = new List<IEntity>
            {
                // new WCTImageEntity($"{GameSettings.Default.gameDir}/data/ui.wad", $"hilight.wct", new Vector2(0, 0), new Vector2(1280, 720)),
                // new TGAImageEntity($"{GameSettings.Default.gameDir}/data/Init/{GameSettings.Default.res}/Welcome.tga", new Vector2(0, 0), new Vector2(1280, 720)),
                new RideEntity($"{GameSettings.Default.gameDir}/data/levels/space/rides/GoKarts.wad"),
                //new CefEntity()
                //{
                //    Name = "CEF HUD Entity"
                //}
            };

            foreach (var entity in entities)
                SceneManager.Instance.AddEntity(entity);

            ImGuiManager.Instance.Theme = ImGuiTheme.LoadFromFile("Content/Themes/light.json");
            ImGui.GetIO().ConfigWindowsMoveFromTitleBarOnly = true;
            FileManager.CreateInstance();
        }
    }
}
