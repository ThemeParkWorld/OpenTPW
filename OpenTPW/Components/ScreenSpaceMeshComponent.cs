using ECSEngine.Attributes;
using ECSEngine.Components;
using ECSEngine.Entities;
using ECSEngine.Managers;
using ECSEngine.Render;
using OpenGL;

namespace TPWEngine.Components
{
    /// <summary>
    /// Contains the component-based code used in order to render
    /// a mesh on-screen.
    /// </summary>
    [Requires(typeof(MaterialComponent))]
    [Requires(typeof(ShaderComponent))]
    [Requires(typeof(TransformComponent))]
    public class MeshComponent : Component<MeshComponent>
    {
        /// <summary>
        /// The <see cref="Mesh"/> to draw.
        /// </summary>
        private readonly Mesh mesh;

        private ShaderComponent shaderComponent;
        private TransformComponent transformComponent;

        /// <summary>
        /// Construct an instance of MeshComponent, loading the mesh from the path specified.
        /// </summary>
        /// <param name="path">The path to load the <see cref="Mesh"/> from.</param>
        public MeshComponent(string path)
        {
            mesh = new Mesh(path);
        }

        /// <summary>
        /// Draw the <see cref="Mesh"/> on-screen using the attached <see cref="ShaderComponent"/> and <see cref="MaterialComponent"/>.
        /// </summary>
        public override void Render()
        {
            shaderComponent = ((IEntity)parent).GetComponent<ShaderComponent>();
            transformComponent = ((IEntity)parent).GetComponent<TransformComponent>();

            shaderComponent.UseShader(); // TODO: Attach GetComponent function to IComponent

            Gl.BindVertexArray(mesh.vao);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, mesh.vbo);

            CameraEntity camera = ((SceneManager)parent.parent).mainCamera;
            shaderComponent.SetVariable("projMatrix", camera.projMatrix);
            shaderComponent.SetVariable("viewMatrix", camera.viewMatrix);
            shaderComponent.SetVariable("cameraPos", camera.position);
            shaderComponent.SetVariable("modelMatrix", transformComponent.matrix);

            GetComponent<MaterialComponent>().BindAll(shaderComponent);

            SceneManager.instance.lights[0].Bind(shaderComponent);

            Gl.DrawArrays(PrimitiveType.Triangles, 0, mesh.elementCount * sizeof(float));

            Gl.BindVertexArray(0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
