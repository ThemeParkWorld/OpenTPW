using Engine.ECS.Entities;
using Engine.Utils;
using Engine.Utils.FileUtils;
using Engine.Utils.MathUtils;
using Quincy.Components;
using Quincy.Managers;

namespace Engine.Entities
{
    public sealed class SkyboxEntity : Entity<SkyboxEntity>
    {
        private TransformComponent transform;

        public SkyboxEntity()
        {
            var fs = ServiceLocator.FileSystem;

            transform = new TransformComponent(new Vector3d(0, 2f, -2f), new Vector3d(0, 0, 0), new Vector3d(1, 1, 1));
            AddComponent(transform);
            AddComponent(new ShaderComponent(fs.GetAsset("Shaders/Skybox/skybox.frag"),
                                             fs.GetAsset("Shaders/Skybox/skybox.vert")));
            AddComponent(new ModelComponent(fs.GetAsset("Models/Skybox.obj")));
        }
    }
}