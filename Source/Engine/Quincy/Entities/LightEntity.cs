using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Utils;
using Engine.Utils.MathUtils;
using Quincy.Components;

namespace Quincy.Entities
{
    public sealed class LightEntity : Entity<LightEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.Lightbulb;

        public LightEntity()
        {
            var fs = ServiceLocator.FileSystem;
            AddComponent(new ShaderComponent(fs.GetAsset("Shaders/PBR/pbr.frag"), fs.GetAsset("Shaders/PBR/pbr.vert")));
            AddComponent(new TransformComponent(new Vector3d(0, 5f, 0f), new Vector3d(90, 0, 0), new Vector3d(1, 1, 1)));
            AddComponent(new LightComponent());
            // AddComponent(new ModelComponent("Content/Models/arrow/scene.gltf"));
        }
    }
}