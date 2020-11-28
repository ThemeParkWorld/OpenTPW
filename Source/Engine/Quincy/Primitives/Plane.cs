using OpenGL;
using System;
using System.Collections.Generic;

namespace Quincy.Primitives
{
    internal class Plane
    {
        public static Vertex[] Vertices { get; } = new[]
        {
            // TODO: Tangent, bi-tangent
            new Vertex()
            {
                Position = new MathUtils.Vector3f(-1.0f, -1.0f, 0.0f),
                TexCoords = new MathUtils.Vector2f(0.0f, 0.0f),
                Normal = new MathUtils.Vector3f(0.0f, 0.0f, 1.0f),
                Tangent = new MathUtils.Vector3f(0, 0, 0),
                BiTangent = new MathUtils.Vector3f(0, 0, 0)
            },
            new Vertex()
            {
                Position = new MathUtils.Vector3f(1.0f, -1.0f, 0.0f),
                TexCoords = new MathUtils.Vector2f(1.0f, 0.0f),
                Normal = new MathUtils.Vector3f(0.0f, 0.0f, 1.0f),
                Tangent = new MathUtils.Vector3f(0, 0, 0),
                BiTangent = new MathUtils.Vector3f(0, 0, 0)
            },
            new Vertex()
            {
                Position = new MathUtils.Vector3f(-1.0f, 1.0f, 0.0f),
                TexCoords = new MathUtils.Vector2f(0.0f, 1.0f),
                Normal = new MathUtils.Vector3f(0.0f, 0.0f, 1.0f),
                Tangent = new MathUtils.Vector3f(0, 0, 0),
                BiTangent = new MathUtils.Vector3f(0, 0, 0)
            },
            new Vertex()
            {
                Position = new MathUtils.Vector3f(1.0f, 1.0f, 0.0f),
                TexCoords = new MathUtils.Vector2f(1.0f, 1.0f),
                Normal = new MathUtils.Vector3f(0.0f, 0.0f, 1.0f),
                Tangent = new MathUtils.Vector3f(0, 0, 0),
                BiTangent = new MathUtils.Vector3f(0, 0, 0)
            }
        };

        public static uint[] Indices { get; } = new uint[]
        {
            0, 1, 3,
            3, 2, 0
            // 1, 2, 3
        };

        private uint vao, vbo, ebo;

        public Plane()
        {
            SetupMesh();
        }

        public void SetupMesh()
        {
            var vertexStructSize = 14 * sizeof(float);

            vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            vbo = Gl.GenBuffer();
            ebo = Gl.GenBuffer();

            var glVertices = new List<float>();
            foreach (var vertex in Vertices)
            {
                glVertices.AddRange(new[] {
                    vertex.Position.x,
                    vertex.Position.y,
                    vertex.Position.z,

                    vertex.Normal.x,
                    vertex.Normal.y,
                    vertex.Normal.z,

                    vertex.Tangent.x,
                    vertex.Tangent.y,
                    vertex.Tangent.z,

                    vertex.BiTangent.x,
                    vertex.BiTangent.y,
                    vertex.BiTangent.z,

                    vertex.TexCoords.x,
                    vertex.TexCoords.y
                });
            }

            Gl.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)glVertices.Count * sizeof(float), glVertices.ToArray(), BufferUsage.StaticDraw);

            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)Indices.Length * sizeof(uint), Indices, BufferUsage.StaticDraw);

            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 3, VertexAttribType.Float, false, vertexStructSize, (IntPtr)0);

            Gl.EnableVertexAttribArray(1);
            Gl.VertexAttribPointer(1, 3, VertexAttribType.Float, false, vertexStructSize, (IntPtr)(3 * sizeof(float)));

            Gl.EnableVertexAttribArray(2);
            Gl.VertexAttribPointer(2, 3, VertexAttribType.Float, false, vertexStructSize, (IntPtr)(6 * sizeof(float)));

            Gl.EnableVertexAttribArray(3);
            Gl.VertexAttribPointer(3, 3, VertexAttribType.Float, false, vertexStructSize, (IntPtr)(9 * sizeof(float)));

            Gl.EnableVertexAttribArray(4);
            Gl.VertexAttribPointer(4, 2, VertexAttribType.Float, false, vertexStructSize, (IntPtr)(12 * sizeof(float)));

            Gl.BindVertexArray(0);
        }

        public void Draw(Shader shader, uint diffuseTexture)
        {
            shader.Use();

            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2d, diffuseTexture);
            shader.SetInt("diffuseTexture", 0);

            Gl.ActiveTexture(TextureUnit.Texture0);

            DrawRaw();
        }

        public void DrawRaw()
        {
            Gl.BindVertexArray(vao);
            Gl.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindVertexArray(0);
        }
    }
}
