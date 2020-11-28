using OpenGL;
using Quincy.Primitives;
using System;

namespace Quincy
{
    internal class EquirectangularToCubemap
    {
        public static (Cubemap, Cubemap, Cubemap) Convert(string hdriPath)
        {
            Gl.Disable(EnableCap.CullFace);
            var equirectangularToCubemapShader = new Shader("Content/Shaders/EquirectangularToCubemap/EquirectangularToCubemap.frag", "Content/Shaders/EquirectangularToCubemap/EquirectangularToCubemap.vert");
            var convolutionShader = new Shader("Content/Shaders/Convolution/convolution.frag", "Content/Shaders/Convolution/convolution.vert");
            var prefilterShader = new Shader("Content/Shaders/Prefilter/prefilter.frag", "Content/Shaders/Prefilter/prefilter.vert");
            var skyHdri = HdriTexture.LoadFromFile(hdriPath);

            var envMap = new Cubemap(RenderToCubemap(equirectangularToCubemapShader, 512, () =>
            {
                equirectangularToCubemapShader.SetInt("equirectangularMap", 0);
                Gl.ActiveTexture(TextureUnit.Texture0);
                Gl.BindTexture(TextureTarget.Texture2d, skyHdri.Id);
            }));
            var convMap = new Cubemap(RenderToCubemap(convolutionShader, 64, () =>
            {
                convolutionShader.SetInt("environmentMap", 0);
                Gl.ActiveTexture(TextureUnit.Texture0);
                Gl.BindTexture(TextureTarget.TextureCubeMap, envMap.Id);
            }));
            var prefMap = new Cubemap(CreatePrefilteredEnvironmentMap(prefilterShader, () =>
            {
                prefilterShader.SetInt("environmentMap", 0);
                Gl.ActiveTexture(TextureUnit.Texture0);
                Gl.BindTexture(TextureTarget.TextureCubeMap, envMap.Id);
            }));
            Gl.Enable(EnableCap.CullFace);

            return (
                envMap,
                convMap,
                prefMap
            );
        }

        public static uint RenderToCubemap(Shader shader, int resolution, Action preRender)
        {
            var cube = new Cube();
            var captureFbo = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, captureFbo);

            var colorTexture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, colorTexture);
            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Srgb, Constants.renderWidth, Constants.renderHeight, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);

            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, colorTexture, 0);

            var envCubemap = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.TextureCubeMap, envCubemap);
            for (int i = 0; i < 6; ++i)
            {
                Gl.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, InternalFormat.Srgb, resolution, resolution, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            }

            Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            var projMatrix = Matrix4x4f.Perspective(90f, 1.0f, 0.1f, 10.0f);
            var viewMatrices = new Matrix4x4f[]
            {
                Matrix4x4f.LookAt(new Vertex3f(0f, 0f, 0f), new Vertex3f(1f, 0f, 0f), new Vertex3f(0f, -1f, 0f)),
                Matrix4x4f.LookAt(new Vertex3f(0f, 0f, 0f), new Vertex3f(-1f, 0f, 0f), new Vertex3f(0f, -1f, 0f)),
                Matrix4x4f.LookAt(new Vertex3f(0f, 0f, 0f), new Vertex3f(0f, 1f, 0f), new Vertex3f(0f, 0f, 1f)),
                Matrix4x4f.LookAt(new Vertex3f(0f, 0f, 0f), new Vertex3f(0f, -1f, 0f), new Vertex3f(0f, 0f, -1f)),
                Matrix4x4f.LookAt(new Vertex3f(0f, 0f, 0f), new Vertex3f(0f, 0f, 1f), new Vertex3f(0f, -1f, 0f)),
                Matrix4x4f.LookAt(new Vertex3f(0f, 0f, 0f), new Vertex3f(0f, 0f, -1f), new Vertex3f(0f, -1f, 0f)),
            };

            shader.Use();
            preRender.Invoke();
            shader.SetMatrix("projectionMatrix", projMatrix);

            Gl.Viewport(0, 0, resolution, resolution);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, captureFbo);
            for (int i = 0; i < 6; ++i)
            {
                shader.SetMatrix("viewMatrix", viewMatrices[i]);
                Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.TextureCubeMapPositiveX + i, envCubemap, 0);
                Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                cube.Draw();
            }
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            return envCubemap;
        }

        public static uint CreateBrdfLut()
        {
            var shader = new Shader("Content/Shaders/BrdfLut/brdfLut.frag", "Content/Shaders/BrdfLut/brdfLut.vert");
            var brdfLutTexture = Gl.GenTexture();
            var plane = new Plane();
            Gl.BindTexture(TextureTarget.Texture2d, brdfLutTexture);
            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rg16f, 512, 512, 0, PixelFormat.Rg, PixelType.Float, IntPtr.Zero);

            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            var captureFbo = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, captureFbo);
            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, brdfLutTexture, 0);

            Gl.Viewport(0, 0, 512, 512);
            shader.Use();
            // Gl.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Gl.Disable(EnableCap.DepthTest);
            Gl.Disable(EnableCap.CullFace);
            plane.DrawRaw();
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.CullFace);

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            return brdfLutTexture;
        }

        private static uint CreatePrefilteredEnvironmentMap(Shader shader, Action preRender)
        {
            var cube = new Cube();
            var captureFbo = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, captureFbo);

            var captureRbo = Gl.GenRenderbuffer();
            Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, captureRbo);
            Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent24, 128, 128);
            Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, captureRbo);

            var prefilterMap = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.TextureCubeMap, prefilterMap);
            for (int i = 0; i < 6; ++i)
            {
                Gl.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, InternalFormat.Rgb16f, 128, 128, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            }

            Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            Gl.GenerateMipmap(TextureTarget.TextureCubeMap);

            var projMatrix = Matrix4x4f.Perspective(90f, 1.0f, 0.1f, 10.0f);
            var viewMatrices = new Matrix4x4f[]
            {
                Matrix4x4f.LookAt(new Vertex3f(0f, 0f, 0f), new Vertex3f(1f, 0f, 0f), new Vertex3f(0f, -1f, 0f)),
                Matrix4x4f.LookAt(new Vertex3f(0f, 0f, 0f), new Vertex3f(-1f, 0f, 0f), new Vertex3f(0f, -1f, 0f)),
                Matrix4x4f.LookAt(new Vertex3f(0f, 0f, 0f), new Vertex3f(0f, 1f, 0f), new Vertex3f(0f, 0f, 1f)),
                Matrix4x4f.LookAt(new Vertex3f(0f, 0f, 0f), new Vertex3f(0f, -1f, 0f), new Vertex3f(0f, 0f, -1f)),
                Matrix4x4f.LookAt(new Vertex3f(0f, 0f, 0f), new Vertex3f(0f, 0f, 1f), new Vertex3f(0f, -1f, 0f)),
                Matrix4x4f.LookAt(new Vertex3f(0f, 0f, 0f), new Vertex3f(0f, 0f, -1f), new Vertex3f(0f, -1f, 0f)),
            };

            shader.Use();
            preRender.Invoke();
            shader.SetMatrix("projectionMatrix", projMatrix);

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, captureFbo);
            int mipLevels = 5;
            for (int mips = 0; mips < mipLevels; ++mips)
            {
                int mipSize = (int)(128 * (float)Math.Pow(0.5, mips));
                Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, captureRbo);
                Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent24, mipSize, mipSize);
                Gl.Viewport(0, 0, mipSize, mipSize);
                float roughness = mips / (float)(mipLevels - 1);
                shader.SetFloat("roughness", roughness);
                for (int i = 0; i < 6; ++i)
                {
                    shader.SetMatrix("viewMatrix", viewMatrices[i]);
                    Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.TextureCubeMapPositiveX + i, prefilterMap, mips);
                    Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    cube.Draw();
                }
            }
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            return prefilterMap;
        }
    }
}
