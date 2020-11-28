using OpenGL;
using System;

namespace Quincy
{
    internal class Framebuffer
    {
        public uint Fbo { get; }
        public uint DepthTexture { get; }
        public uint ColorTexture { get; }

        public Framebuffer()
        {
            Fbo = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, Fbo);

            ColorTexture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, ColorTexture);
            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Srgb, Constants.renderWidth, Constants.renderHeight, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);

            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, ColorTexture, 0);

            DepthTexture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, DepthTexture);
            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.DepthComponent, Constants.renderWidth, Constants.renderHeight, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);

            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2d, DepthTexture, 0);

            Gl.BindTexture(TextureTarget.Texture2d, 0);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }
}
