using Engine.Utils.DebugUtils;
using Engine.Utils.FileUtils;
using OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using StbImageSharp;

namespace Quincy
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Texture
    {
        public uint Id { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }

        public static Texture LoadFromAsset(Asset asset, string typeName)
        {
            // Check if already loaded
            if (TextureContainer.Textures.Any(t => t.Path == asset.MountPath))
            {
                // Already loaded, we'll just use that
                return TextureContainer.Textures.First(t => t.Path == asset.MountPath);
            }

            // Not loaded, load from scratch
            var start = DateTime.Now;
            var texturePtr = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, texturePtr);
            var image = ImageResult.FromMemory(asset.Data, ColorComponents.RedGreenBlueAlpha);
            var end = DateTime.Now;

            Logging.Log($"Stbi load took {(end - start).TotalSeconds:F2}s");

            var imageFormat = PixelFormat.Rgb;
            if (image.Comp == ColorComponents.RedGreenBlueAlpha)
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

            // Gl.TexParameterf(TextureTarget.Texture2d, (TextureParameterName)Gl.TEXTURE_MAX_ANISOTROPY, 16.0f); // (should be) 16x anisotropic filtering
            Gl.GenerateMipmap(TextureTarget.Texture2d);
            Marshal.FreeHGlobal(textureDataPtr);

            Logging.Log($"Loaded texture {asset.MountPath}, ptr {texturePtr}");
            Gl.BindTexture(TextureTarget.Texture2d, 0);

            var texture = new Texture()
            {
                Id = texturePtr,
                Path = asset.MountPath,
                Type = typeName
            };
            
            TextureContainer.Textures.Add(texture);
            TextureContainer.TexturePaths.Add(texture.Path);
            return texture;
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

            var texture = new Texture()
            {
                Id = texturePtr,
                Path = $"Bytes{data.Length}",
                Type = typeName
            };
            
            TextureContainer.Textures.Add(texture);
            TextureContainer.TexturePaths.Add(texture.Path);
            return texture;
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
            //else if (typeName == "texture_gui")
            //{
            //    internalFormat = InternalFormat.Rgba;
            //}

            Gl.TexImage2D(TextureTarget.Texture2d, 0, internalFormat, width, height, 0, imageFormat, PixelType.UnsignedByte, pixels);
            Gl.GenerateMipmap(TextureTarget.Texture2d);

            Logging.Log($"Loaded texture from ptr {pixels}, ptr {texturePtr}");
            Gl.BindTexture(TextureTarget.Texture2d, 0);

            var texture = new Texture()
            {
                Id = texturePtr,
                Path = $"{pixels}",
                Type = typeName
            };
            
            TextureContainer.Textures.Add(texture);
            TextureContainer.TexturePaths.Add(texture.Path);
            return texture;
        }

        public void Bind(TextureTarget target = TextureTarget.Texture2d)
        {
            Gl.BindTexture(target, Id);
        }

        public static Texture LoadFromColorData(ColorRGBA32[] imageData, int width, int height, string typeName)
        {
            var data = new byte[width * height * 4];
            var dataIndex = 0;

            foreach (var color in imageData)
            {
                data[dataIndex++] = color.r;
                data[dataIndex++] = color.g;
                data[dataIndex++] = color.b;
                data[dataIndex++] = color.a;
            }

            return LoadFromData(data, width, height, 4, typeName);
        }

        public static Texture LoadFromFloatData(byte[] data, int width, int height, string typeName)
        {
            var textureDataPtr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, textureDataPtr, data.Length);

            uint texturePtr = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, texturePtr);

            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba16f, width, height, 0, PixelFormat.Rgba, PixelType.Float, textureDataPtr);

            Marshal.FreeHGlobal(textureDataPtr);
            
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            Gl.BindTexture(TextureTarget.Texture2d, 0);

            var texture = new Texture()
            {
                Id = texturePtr,
                Path = $"Bytes{data.Length}",
                Type = typeName
            };
            
            TextureContainer.Textures.Add(texture);
            TextureContainer.TexturePaths.Add(texture.Path);
            return texture;
        }

        public static Texture LoadFromFloatData(float[] data, int width, int height, string typeName)
        {
            var imageBytes = new List<byte>();
            // Convert floats into bytes - so we can hand them over to ogl
            foreach (var fl in data)
            {
                var imageDataByteArray = BitConverter.GetBytes(fl);
                imageBytes.AddRange(imageDataByteArray);
            }

            return LoadFromFloatData(imageBytes.ToArray(), width, height, typeName);
        }
    }
    
    public class TextureContainer
    {
        public static List<string> TexturePaths { get; } = new List<string>();
        public static List<Texture> Textures { get; } = new List<Texture>();
    }
}
