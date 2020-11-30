using Engine.ECS.Managers;
using Quincy.Managers;
using System;
using System.Threading;

namespace Engine.Managers
{
    public class UpdateManager : Manager<UpdateManager>
    {
        private DateTime lastUpdate;
        private int minimumUpdateDelay = 8;
        public override void Run()
        {
            var updateTime = (DateTime.Now - lastUpdate).Milliseconds;
            var deltaTime = Math.Max(updateTime, 0.1f) / 1000.0f;
            foreach (var entity in SceneManager.Instance.Entities)
            {
                if (entity.Enabled)
                    entity.Update(deltaTime);
            }
            lastUpdate = DateTime.Now;
            Thread.Sleep(Math.Max(minimumUpdateDelay - updateTime, 0));
        }
    }
}
