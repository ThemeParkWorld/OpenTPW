using Engine.ECS.Entities;
using Engine.Utils;
using Engine.Utils.MathUtils;
using Quincy;
using Quincy.Components;

namespace OpenTPW.Entities.UI
{
    public class BaseImageEntity : Entity<BaseImageEntity>
    {
        protected Texture texture;

        protected Vector2d position;
        protected Vector2d scale;

        public BaseImageEntity(Vector2d position, Vector2d scale)
        {
            // Add components
            var fileSystem = ServiceLocator.FileSystem;
            AddComponent(new ShaderComponent(fileSystem.GetAsset("Shaders/2D/2D.frag"), fileSystem.GetAsset("Shaders/2D/2D.vert")));
            AddComponent(new TransformComponent(new Vector3d(0, 0, 0.1f), Quaternion.FromEulerAngles(new Vector3f(90f, 0, 0)), new Vector3d(1, 1, 1)));
            AddComponent(new ModelComponent(fileSystem.GetAsset("plane.obj")));
        }
    }
}