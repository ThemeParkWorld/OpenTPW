﻿using Engine;
using Engine.ECS.Entities;
using Engine.Utils.DebugUtils;
using Engine.Utils.MathUtils;
using ImGuiNET;
using OpenTPW.Entities;
using OpenTPW.Entities.UI;
using OpenTPW.Files;
using Quincy.Managers;
using System.Collections.Generic;

namespace OpenTPW
{
    internal sealed class TPWGame : Game
    {
        public TPWGame(string gamePropertyPath) : base(gamePropertyPath) { }

        protected override void InitScene()
        {
            base.InitScene();
            var entities = new List<IEntity>
            {
                new TGAImageEntity($"{TPWSettings.Default.gameDir}/data/Init/{TPWSettings.Default.res}/Welcome.tga", new Vector2d(0, 0), new Vector2d(1280, 720)),
                new WCTImageEntity(@$"{TPWSettings.Default.gameDir}/data/ui.wad", "b_ardown.wct", new Vector2d(0, 0), new Vector2d(1280, 720)),
                new RideEntity($"{TPWSettings.Default.gameDir}/data/levels/space/rides/Wateride.wad")
            };

            var uiText = FileManager.Instance.ReadFile<List<string>>($"{TPWSettings.Default.gameDir}/data/Language/English/UITEXT.str");
            foreach (var textElement in uiText.Data)
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
