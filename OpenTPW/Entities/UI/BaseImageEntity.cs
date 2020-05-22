using Engine.ECS.Entities;
using Engine.Renderer.GL.Components;
using Engine.Renderer.GL.Render;
using Engine.Utils.MathUtils;

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
            AddComponent(new ShaderComponent(new Shader("Content/Shaders/2D/main.frag", Shader.Type.FragmentShader),
                new Shader("Content/Shaders/2D/main.vert", Shader.Type.VertexShader)));
            AddComponent(new TransformComponent(new Vector3(0, 0, -0.1f), Quaternion.FromEulerAngles(new Vector3(90f, 0, 0)), new Vector3(1, 1, 1)));
            AddComponent(new MaterialComponent(material));
            AddComponent(new MeshComponent("Content/plane.obj"));
        }
    }
}