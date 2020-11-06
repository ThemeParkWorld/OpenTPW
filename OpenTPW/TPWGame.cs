using Engine;
using Engine.ECS.Entities;
using Engine.Gui.Managers;
using Engine.Gui.Managers.ImGuiWindows.Theming;
using Engine.Renderer.GL.Managers;
using Engine.Utils.DebugUtils;
using Engine.Utils.MathUtils;
using ImGuiNET;
using OpenTPW.Entities;
using OpenTPW.Entities.UI;
using OpenTPW.Files;
using System.Collections.Generic;

namespace OpenTPW
{
    internal sealed class TPWGame : Game
    {
        public TPWGame(string gamePropertyPath) : base(gamePropertyPath) { }

        protected override void InitScene()
        {
            // FileManager.Instance.ReadFile($"{GameSettings.Default.gameDir}/data/generic/dynamic/textures/red.wct");

            base.InitScene();
            var entities = new List<IEntity>
            {
                // new WCTImageEntity($"{GameSettings.Default.gameDir}/data/ui.wad", $"b_upgrade.wct", new Vector2d(0, 0), new Vector2d(1280, 720)),
                new TGAImageEntity($"{GameSettings.Default.gameDir}/data/Init/{GameSettings.Default.res}/Welcome.tga", new Vector2d(0, 0), new Vector2d(1280, 720)),
                new RideEntity($"{GameSettings.Default.gameDir}/data/levels/jungle/rides/Bouncy.wad")
            };

            var uiText = FileManager.Instance.ReadFile($"{GameSettings.Default.gameDir}/data/Language/English/UITEXT.str");
            foreach (var textElement in uiText.Data as List<string>)
            {
                Logging.Log(textElement);
            }

            foreach (var entity in entities)
                SceneManager.Instance.AddEntity(entity);

            ImGui.GetIO().ConfigWindowsMoveFromTitleBarOnly = true;
            FileManager.CreateInstance();
        }
    }
}
