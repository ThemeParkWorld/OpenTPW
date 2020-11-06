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

        protected Vector2d position;
        protected Vector2d scale;

        public BaseImageEntity(Vector2d position, Vector2d scale)
        {
            // Add components
            AddComponent(new ShaderComponent(new Shader("Content/Shaders/2D/2D.frag", Shader.Type.FragmentShader),
                new Shader("Content/Shaders/2D/2D.vert", Shader.Type.VertexShader)));
            AddComponent(new TransformComponent(new Vector3d(0, 0, 0.1f), Quaternion.FromEulerAngles(new Vector3f(90f, 0, 0)), new Vector3d(1, 1, 1)));
            AddComponent(new MaterialComponent(material));
            AddComponent(new MeshComponent("Content/plane.obj"));
        }

        protected void SetupMaterial()
        {
            // Setup material
            material = new Material("Content/plane.mtl");
        }
    }
}