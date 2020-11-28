using OpenGL;
using Quincy.MathUtils;
using System;

namespace Quincy
{
    internal class Camera
    {
        private Vector3f position;
        public Vector3f Position { get => position; set => position = value; }
        public Vector3f Rotation { get; set; }

        private float angle;

        public float FieldOfView { get; set; } = 70f;
        public float NearPlane { get; set; } = 0.1f;

        private Matrix4x4f viewMatrix;
        public Matrix4x4f ViewMatrix { get => viewMatrix; set => viewMatrix = value; }

        private Matrix4x4f projMatrix;
        public Matrix4x4f ProjMatrix { get => projMatrix; set => projMatrix = value; }

        public Camera(Vector3f position)
        {
            this.position = position;
            ProjMatrix = CreateInfReversedZProj(FieldOfView,
                Constants.renderWidth / (float)Constants.renderHeight,
                NearPlane);
        }

        private Matrix4x4f CreateInfReversedZProj(float fov, float aspectRatio, float nearPlane)
        {
            float f = 1.0f / (float)Math.Tan(Angle.ToRadians(fov) / 2.0f);
            return new Matrix4x4f(f / aspectRatio, 0f, 0f, 0f,
                0f, f, 0f, 0f,
                0f, 0f, 0f, -1f,
                0f, 0f, nearPlane, 0f);
        }

        public void Render()
        { }

        public void Update(float deltaTime)
        {
            viewMatrix = Matrix4x4f.Identity;

            position.x = MathF.Cos(angle) * 15f;
            position.y = 3f;
            position.z = MathF.Sin(angle) * 15f;

            angle += deltaTime;
            angle %= 360;

            viewMatrix *= Matrix4x4f.LookAt(new Vertex3f(position.x, position.y, position.z), new Vertex3f(0f, position.y, 0f), new Vertex3f(0f, 1f, 0f));
        }
    }
}
