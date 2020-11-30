using Engine.ECS.Components;
using Engine.Utils;
using Engine.Utils.Attributes;
using Engine.Utils.MathUtils;
using OpenGL;
using System;

namespace Quincy.Components
{
    [Requires(typeof(TransformComponent))]
    public class CameraComponent : Component<CameraComponent>
    {
        public Framebuffer Framebuffer { get; private set; }

        private float nearPlane = 0.1f;
        private float farPlane = 2500f;
        private float fieldOfView = 90f;

        private Matrix4x4f viewMatrix;
        private Matrix4x4f projMatrix;

        [Range(0, 180)] public float FieldOfView { get => fieldOfView; set { fieldOfView = value; CreateProjectionMatrix(); } }
        public float NearPlane { get => nearPlane; set { nearPlane = value; CreateProjectionMatrix(); } }
        public float FarPlane { get => farPlane; set { farPlane = value; CreateProjectionMatrix(); } }

        private Vector2f resolution = new Vector2f(GameSettings.GameResolutionX, GameSettings.GameResolutionY);
        public Vector2f Resolution
        {
            get => resolution;
            set
            {
                resolution = value;
                CreateFramebuffer();
                CreateProjectionMatrix();
            }
        }

        public Matrix4x4f ViewMatrix { get => viewMatrix; set => viewMatrix = value; }

        public Matrix4x4f ProjMatrix { get => projMatrix; set => projMatrix = value; }

        public CameraComponent()
        {
            CreateProjectionMatrix();
            CreateFramebuffer();
        }

        private void CreateProjectionMatrix()
        {
            ProjMatrix = CreateInfReversedZProj(FieldOfView,
                Resolution.x / Resolution.y,
                NearPlane);
        }

        private void CreateFramebuffer()
        {
            Framebuffer = new Framebuffer((int)Resolution.x, (int)Resolution.y);
        }

        private Matrix4x4f CreateInfReversedZProj(float fov, float aspectRatio, float nearPlane)
        {
            float f = 1.0f / (float)Math.Tan(Angle.ToRadians(fov) / 2.0f);
            return new Matrix4x4f(f / aspectRatio, 0f, 0f, 0f,
                0f, f, 0f, 0f,
                0f, 0f, 0f, -1f,
                0f, 0f, nearPlane, 0f);
        }

        public override void Update(float deltaTime)
        {
            // TODO: Better double->float conversion
            var transformComponent = GetComponent<TransformComponent>();
            viewMatrix = Matrix4x4f.Identity;
            viewMatrix.RotateX((float)transformComponent.RotationEuler.x);
            viewMatrix.RotateY((float)transformComponent.RotationEuler.y);
            viewMatrix.RotateZ((float)transformComponent.RotationEuler.z);
            viewMatrix *= (Matrix4x4f.LookAtDirection(
                new Vertex3f((float)transformComponent.Position.x, (float)transformComponent.Position.y, (float)transformComponent.Position.z),
                new Vertex3f(0f, 0f, -1f),
                new Vertex3f(0f, 1f, 0f)));
        }
    }
}
