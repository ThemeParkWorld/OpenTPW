using Engine.Utils.DebugUtils;
using OpenGL;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using StbImageSharp;

namespace Quincy
{
    public struct Cubemap
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
                var image = ImageResult.FromMemory(fileData, ColorComponents.RedGreenBlueAlpha);

                var imageFormat = PixelFormat.Rgb;
                if (image.Comp == ColorComponents.RedGreenBlueAlpha)
                {
                    imageFormat = PixelFormat.Rgba;
                }

                var textureDataPtr = Marshal.AllocHGlobal(image.Data.Length);
                Marshal.Copy(image.Data, 0, textureDataPtr, image.Data.Length);

                var internalFormat = InternalFormat.Rgb;

                Gl.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, internalFormat, image.Width, image.Height, 0, imageFormat, PixelType.UnsignedByte, textureDataPtr);
                Marshal.FreeHGlobal(textureDataPtr);

                Logging.Log($"Loaded cubemap texture {faces[i]}, ptr {texturePtr}");

                Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

                Gl.BindTexture(TextureTarget.TextureCubeMap, 0);
            }

            return new Cubemap(texturePtr);
        }
    }
}
