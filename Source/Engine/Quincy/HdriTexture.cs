using OpenGL;
using Quincy.DebugUtils;
using StbiSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Quincy
{
    internal struct HdriTexture
    {
        public uint Id { get; }

        public HdriTexture(uint id)
        {
            Id = id;
        }

        public static HdriTexture LoadFromFile(string filePath)
        {
            var fileData = File.ReadAllBytes(filePath);
            var image = Stbi.LoadfFromMemory(fileData, 3);
            var imageBytes = new List<byte>();
            foreach (var fl in image.Data)
            {
                var imageDataByteArray = BitConverter.GetBytes(fl);
                imageBytes.AddRange(imageDataByteArray);
            }

            var textureDataPtr = Marshal.AllocHGlobal(imageBytes.Count);
            Marshal.Copy(imageBytes.ToArray(), 0, textureDataPtr, imageBytes.Count);

            uint texturePtr = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, texturePtr);

            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgb16f, image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.Float, textureDataPtr);

            Marshal.FreeHGlobal(textureDataPtr);

            Logging.Log($"Loaded cubemap texture {filePath}, ptr {texturePtr}");

            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            Gl.BindTexture(TextureTarget.Texture2d, 0);
            image.Dispose();

            return new HdriTexture(texturePtr);
        }
    }
}
