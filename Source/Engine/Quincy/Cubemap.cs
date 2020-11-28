using OpenGL;
using Quincy.DebugUtils;
using StbiSharp;
using System.IO;
using System.Runtime.InteropServices;

namespace Quincy
{
    internal struct Cubemap
    {
        public uint Id { get; }

        public Cubemap(uint id)
        {
            Id = id;
        }

        public static Cubemap LoadFromFiles(string[] faces)
        {
            uint texturePtr = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.TextureCubeMap, texturePtr);

            for (int i = 0; i < faces.Length; ++i)
            {
                var fileData = File.ReadAllBytes(faces[i]);
                var image = Stbi.LoadFromMemory(fileData, 4);

                var imageFormat = PixelFormat.Rgb;
                if (image.NumChannels == 4)
                {
                    imageFormat = PixelFormat.Rgba;
                }

                var textureDataPtr = Marshal.AllocHGlobal(image.Data.Length);
                Marshal.Copy(image.Data.ToArray(), 0, textureDataPtr, image.Data.Length);

                var internalFormat = InternalFormat.Rgb;

                Gl.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, internalFormat, image.Width, image.Height, 0, imageFormat, PixelType.UnsignedByte, textureDataPtr);

                image.Dispose();
                Marshal.FreeHGlobal(textureDataPtr);

                Logging.Log($"Loaded cubemap texture {faces[i]}, ptr {texturePtr}");

                Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

                Gl.BindTexture(TextureTarget.TextureCubeMap, 0);
                image.Dispose();
            }

            return new Cubemap(texturePtr);
        }
    }
}
