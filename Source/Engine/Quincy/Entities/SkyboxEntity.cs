using Engine.ECS.Entities;
using Quincy.Components;

namespace Quincy.Entities
{
    public class SkyboxEntity : Entity<SkyboxEntity>
    {
        public SkyboxEntity(string hdriPath)
        {
            AddComponent(new SkyboxComponent(hdriPath));
        }
    }
}
