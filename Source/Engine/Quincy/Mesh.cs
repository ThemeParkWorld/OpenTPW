using OpenGL;
using Quincy.Components;
using Quincy.Entities;
using System;
using System.Collections.Generic;

namespace Quincy
{
    public class Mesh
    {
        public List<Vertex> Vertices { get; set; }
        public List<uint> Indices { get; set; }
        public List<Texture> Textures { get; set; }

        public int VertexCount { get; private set; }
        public int IndexCount { get; private set; }
        public int TextureCount { get; private set; }

        private Matrix4x4f localModelMatrix;

        private uint vao, vbo, ebo;

        public Mesh(List<Vertex> vertices, List<uint> indices, List<Texture> textures, Matrix4x4f oglTransform)
        {
            Vertices = vertices;
            Indices = indices;
            Textures = textures;
            localModelMatrix = oglTransform;

            SetupMesh();
        }

        private void SetupMesh()
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
            Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)Indices.Count * sizeof(uint), Indices.ToArray(), BufferUsage.StaticDraw);

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

            // Buffered indices & vertices to GPU
            // Keep counts, remove CPU copies
            IndexCount = Indices.Count;
            VertexCount = Vertices.Count;

            Indices.Clear();
            Vertices.Clear();

            Indices = null;
            Vertices = null;
        }

        public void Draw(CameraEntity camera, ShaderComponent shader, LightEntity light, (Cubemap, Cubemap, Cubemap) pbrCubemaps, Texture brdfLut, Matrix4x4f modelMatrix, Texture holoTexture)
        {
            Dictionary<string, uint> counts = new Dictionary<string, uint>();
            List<string> expected = new List<string>() { "texture_diffuse", "texture_emissive", "texture_unknown", "texture_normal" };

            shader.Use();

            for (int i = 0; i < Textures.Count; ++i)
            {
                var texture = Textures[i];

                Gl.ActiveTexture(TextureUnit.Texture0 + i);
                Gl.BindTexture(TextureTarget.Texture2d, texture.Id);

                string name = texture.Type;

                if (!counts.ContainsKey(name))
                {
                    counts.Add(name, 0);
                }

                string number = (++counts[name]).ToString();

                shader.SetBool($"material.{name}{number}.exists", true);
                shader.SetInt($"material.{name}{number}.texture", i);
            }

            foreach (var expectedVar in expected)
            {
                if (!counts.ContainsKey(expectedVar))
                {
                    shader.SetBool($"material.{expectedVar}1.exists", false);
                }
            }
            var cameraComponent = camera.GetComponent<CameraComponent>();
            var lightComponent = light.GetComponent<LightComponent>();

            shader.SetMatrix("projectionMatrix", cameraComponent.ProjMatrix);
            shader.SetMatrix("viewMatrix", cameraComponent.ViewMatrix);
            shader.SetMatrix("modelMatrix", modelMatrix * localModelMatrix);

            shader.SetVector3d("camPos", camera.GetComponent<TransformComponent>().Position);
            shader.SetVector3d("lightPos", light.GetComponent<TransformComponent>().Position);

            shader.SetMatrix("lightProjectionMatrix", lightComponent.ProjMatrix);
            shader.SetMatrix("lightViewMatrix", lightComponent.ViewMatrix);

            Gl.ActiveTexture(TextureUnit.Texture0 + Textures.Count);
            Gl.BindTexture(TextureTarget.Texture2d, lightComponent.ShadowMap.DepthMap);
            shader.SetInt("shadowMap", Textures.Count);

            Gl.ActiveTexture(TextureUnit.Texture0 + Textures.Count + 1);
            Gl.BindTexture(TextureTarget.TextureCubeMap, pbrCubemaps.Item2.Id);
            shader.SetInt("irradianceMap", Textures.Count + 1);

            Gl.ActiveTexture(TextureUnit.Texture0 + Textures.Count + 2);
            Gl.BindTexture(TextureTarget.Texture2d, brdfLut.Id);
            shader.SetInt("brdfLut", Textures.Count + 2);

            Gl.ActiveTexture(TextureUnit.Texture0 + Textures.Count + 3);
            Gl.BindTexture(TextureTarget.TextureCubeMap, pbrCubemaps.Item3.Id);
            shader.SetInt("prefilterMap", Textures.Count + 3);

            Gl.ActiveTexture(TextureUnit.Texture0 + Textures.Count + 4);
            Gl.BindTexture(TextureTarget.Texture2d, holoTexture.Id);
            shader.SetInt("holoMap", Textures.Count + 4);

            Gl.ActiveTexture(TextureUnit.Texture0);

            Gl.BindVertexArray(vao);
            Gl.DrawElements(PrimitiveType.Triangles, IndexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindVertexArray(0);
        }

        public void DrawShadows(LightComponent light, ShaderComponent depthShader, Matrix4x4f modelMatrix)
        {
            depthShader.Use();

            depthShader.SetMatrix("projectionMatrix", light.ProjMatrix);
            depthShader.SetMatrix("viewMatrix", light.ViewMatrix);
            depthShader.SetMatrix("modelMatrix", modelMatrix * localModelMatrix);

            Gl.ActiveTexture(TextureUnit.Texture0);

            Gl.BindVertexArray(vao);
            Gl.DrawElements(PrimitiveType.Triangles, IndexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindVertexArray(0);
        }
    }
}
