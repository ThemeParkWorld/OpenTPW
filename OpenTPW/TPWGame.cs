using Engine;
using Engine.ECS.Entities;
using Engine.Entities;
using Engine.Renderer.GL.Managers;
using OpenTPW.Entities;
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
                new RideEntity($"{GameSettings.Default.gameDir}/data/levels/jungle/rides/bouncy.wad"),
                new CefEntity()
                {
                    Name = "CEF HUD Entity"
                }
            };

            foreach (var entity in entities)
                SceneManager.Instance.AddEntity(entity);
        }
    }
}
