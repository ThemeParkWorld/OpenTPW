using ECSEngine;
using ECSEngine.Entities;
using ECSEngine.Managers;
using ECSEngine.MathUtils;
using System.Collections.Generic;
using OpenTPW.Entities.UI;

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
                new ImageEntity($"{GameSettings.Default.gameDir}/data/Init/{GameSettings.Default.res}/Welcome.tga", new Vector2(0, 0), new Vector2(1280, 720))
            };

            foreach (IEntity entity in entities)
                SceneManager.instance.AddEntity(entity);
        }
    }
}
