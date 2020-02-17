using ECSEngine.Components;
using ECSEngine.Entities;
using ECSEngine.MathUtils;
using ECSEngine.Render;
using OpenTPW.Files.FileFormats;

namespace OpenTPW.Entities.UI
{
    public sealed class ImageEntity : Entity<ImageEntity>
    {
        private Material material;

        private Mesh imageMesh;
        private Texture2D texture;

        private Vector2 position;
        private Vector2 scale;

        public ImageEntity(string path, Vector2 position, Vector2 scale)
        {
            // Load image
            texture = TGAReader.LoadAsset(path);

            // Setup material
            material = new Material("Content/plane.mtl");
            material.diffuseTexture = texture;

            // Add components
            AddComponent(new ShaderComponent(new Shader("Content/2D/main.frag", OpenGL.ShaderType.FragmentShader),
                new Shader("Content/2D/main.vert", OpenGL.ShaderType.VertexShader)));
            AddComponent(new TransformComponent(new Vector3(0, 0, -0.1f), Quaternion.FromEulerAngles(new Vector3(90f, 0, 0)), new Vector3(1, 1, 1)));
            AddComponent(new MaterialComponent(material));
            AddComponent(new MeshComponent("Content/plane.obj"));
        }
    }
}
