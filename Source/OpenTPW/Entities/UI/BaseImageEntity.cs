using Engine.ECS.Entities;
using Engine.Utils;
using Engine.Utils.MathUtils;
using Quincy.Components;

namespace OpenTPW.Entities.UI
{
    public class BaseImageEntity : Entity<BaseImageEntity>
    {
        protected Vector2d position;
        protected Vector2d scale;

        public BaseImageEntity(Vector2d position, Vector2d scale)
        {
            // Add components
            AddComponent(new ShaderComponent(ServiceLocator.FileSystem.GetAsset("Content/Shaders/2D/2D.frag"),
                ServiceLocator.FileSystem.GetAsset("Content/Shaders/2D/2D.vert")));
            AddComponent(new TransformComponent(new Vector3d(0, 0, 0.1f), Quaternion.FromEulerAngles(new Vector3f(90f, 0, 0)), new Vector3d(1, 1, 1)));
            AddComponent(new ModelComponent(ServiceLocator.FileSystem.GetAsset("Content/plane.obj")));
        }
    }
}