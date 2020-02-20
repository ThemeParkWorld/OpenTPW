using ECSEngine.Components;
using ECSEngine.Entities;
using ECSEngine.MathUtils;
using ECSEngine.Render;

namespace OpenTPW.Entities.UI
{
    public class BaseImageEntity : Entity<BaseImageEntity>
    {
        protected Material material;

        protected Mesh imageMesh;
        protected Texture2D texture;

        protected Vector2 position;
        protected Vector2 scale;

        public BaseImageEntity(Vector2 position, Vector2 scale)
        {
            // Setup material
            material = new Material("Content/plane.mtl");

            // Add components
            AddComponent(new ShaderComponent(new Shader("Content/2D/main.frag", OpenGL.ShaderType.FragmentShader),
                new Shader("Content/2D/main.vert", OpenGL.ShaderType.VertexShader)));
            AddComponent(new TransformComponent(new Vector3(0, 0, -0.1f), Quaternion.FromEulerAngles(new Vector3(90f, 0, 0)), new Vector3(1, 1, 1)));
            AddComponent(new MaterialComponent(material));
            AddComponent(new MeshComponent("Content/plane.obj"));
        }
    }
}