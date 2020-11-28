using OpenGL;
using Quincy.DebugUtils;
using StbiSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Quincy
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Texture
    {
        public uint Id { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }

        public static Texture LoadFromFile(string filePath, string typeName)
        {
            // Check if already loaded
            if (TextureContainer.Textures.Any(t => t.Path == filePath))
            {
                // Already loaded, we'll just use that
                return TextureContainer.Textures.First(t => t.Path == filePath);
            }

            // Not loaded, load from scratch
            var texturePtr = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, texturePtr);
            var fileData = File.ReadAllBytes(filePath);
            var image = Stbi.LoadFromMemory(fileData, 4);

            var imageFormat = PixelFormat.Rgb;
            if (image.NumChannels == 4)
            {
                imageFormat = PixelFormat.Rgba;
            }

            var textureDataPtr = Marshal.AllocHGlobal(image.Data.Length);
            Marshal.Copy(image.Data.ToArray(), 0, textureDataPtr, image.Data.Length);

            var internalFormat = InternalFormat.Rgba;
            if (typeName == "texture_diffuse")
            {
                internalFormat = InternalFormat.SrgbAlpha;
            }
            else if (typeName == "texture_normal")
            {
                internalFormat = InternalFormat.Rgba8;
            }

            Gl.TexImage2D(TextureTarget.Texture2d, 0, internalFormat, image.Width, image.Height, 0, imageFormat, PixelType.UnsignedByte, textureDataPtr);

            Gl.TexParameterf(TextureTarget.Texture2d, (TextureParameterName)Gl.TEXTURE_MAX_ANISOTROPY, 16.0f); // (should be) 16x anisotropic filtering
            Gl.GenerateMipmap(TextureTarget.Texture2d);

            image.Dispose();
            Marshal.FreeHGlobal(textureDataPtr);

            Logging.Log($"Loaded texture {filePath}, ptr {texturePtr}");
            Gl.BindTexture(TextureTarget.Texture2d, 0);

            return new Texture()
            {
                Id = texturePtr,
                Path = filePath,
                Type = typeName
            };
        }

        public static Texture LoadFromData(byte[] data, int width, int height, int bpp, string typeName)
        {
            var texturePtr = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, texturePtr);

            var imageFormat = PixelFormat.Rgb;
            if (bpp == 4)
            {
                imageFormat = PixelFormat.Rgba;
            }

            var textureDataPtr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, textureDataPtr, data.Length);

            var internalFormat = InternalFormat.Rgba;
            if (typeName == "texture_diffuse")
            {
                internalFormat = InternalFormat.SrgbAlpha;
            }
            else if (typeName == "texture_normal")
            {
                internalFormat = InternalFormat.Rgba8;
            }

            Gl.TexImage2D(TextureTarget.Texture2d, 0, internalFormat, width, height, 0, imageFormat, PixelType.UnsignedByte, textureDataPtr);

            Gl.TexParameterf(TextureTarget.Texture2d, (TextureParameterName)Gl.TEXTURE_MAX_ANISOTROPY, 16.0f); // (should be) 16x anisotropic filtering
            Gl.GenerateMipmap(TextureTarget.Texture2d);

            Marshal.FreeHGlobal(textureDataPtr);

            Logging.Log($"Loaded texture from bytes, ptr {texturePtr}");
            Gl.BindTexture(TextureTarget.Texture2d, 0);

            return new Texture()
            {
                Id = texturePtr,
                Path = $"Bytes{data.Length}",
                Type = typeName
            };
        }

        public static Texture LoadFromPtr(IntPtr pixels, int width, int height, int bytesPerPixel, string typeName)
        {
            var texturePtr = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, texturePtr);

            var imageFormat = PixelFormat.Rgb;
            if (bytesPerPixel == 4)
            {
                imageFormat = PixelFormat.Rgba;
            }

            var internalFormat = InternalFormat.Rgba;
            if (typeName == "texture_diffuse")
            {
                internalFormat = InternalFormat.SrgbAlpha;
            }

            Gl.TexImage2D(TextureTarget.Texture2d, 0, internalFormat, width, height, 0, imageFormat, PixelType.UnsignedByte, pixels);
            Gl.GenerateMipmap(TextureTarget.Texture2d);

            Logging.Log($"Loaded texture from ptr {pixels}, ptr {texturePtr}");
            Gl.BindTexture(TextureTarget.Texture2d, 0);

            return new Texture()
            {
                Id = texturePtr,
                Path = $"{pixels}",
                Type = typeName
            };
        }
    }

    internal class TextureContainer
    {
        public static List<Texture> Textures { get; } = new List<Texture>();
    }
}
