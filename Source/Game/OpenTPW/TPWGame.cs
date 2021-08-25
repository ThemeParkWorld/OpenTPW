using Engine;
using Engine.ECS.Entities;
using Engine.Utils.DebugUtils;
using Engine.Utils.MathUtils;
using ImGuiNET;
using OpenTPW.Entities;
using OpenTPW.Entities.UI;
using OpenTPW.Files;
using Quincy.Managers;
using System.Collections.Generic;
using System.Linq;
using Engine.ECS.Observer;
using OpenGL.CoreUI;

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
                // new WCTImageEntity(@$"{TPWSettings.Default.gameDir}/data/ui.wad", "b_ardown.wct", new Vector2d(0, 0), new Vector2d(1280, 720)),
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

        public override void OnNotify(NotifyType eventType, INotifyArgs notifyArgs)
        {
            base.OnNotify(eventType, notifyArgs);

            if (eventType != NotifyType.KeyUp) 
                return;
            
            var eventArgs = notifyArgs as KeyboardNotifyArgs;
            
            if ((KeyCode) eventArgs.KeyboardKey != KeyCode.R) 
                return;
            
            // Reload wct file
            var wctFiles = SceneManager.Instance.Entities.Where(e => e is WCTImageEntity);
            var file = wctFiles.FirstOrDefault();
            if (file != null)
            {
                SceneManager.Instance.Entities.Remove(file);
                SceneManager.Instance.Entities.Add(new WCTImageEntity(
                    @$"{TPWSettings.Default.gameDir}/data/ui.wad", "b_ardown.wct", new Vector2d(0, 0),
                    new Vector2d(1280, 720)));
            }
        }
    }
}
