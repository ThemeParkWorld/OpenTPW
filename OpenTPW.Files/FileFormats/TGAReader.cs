using ECSEngine;
using ECSEngine.Render;
using OpenGL;
using System;
using System.Collections.Generic;
using System.IO;

namespace OpenTPW.Files.FileFormats
{
    public static class TGAReader
    {
        public static Texture2D LoadAsset(string path)
        {
            // I'm really lazy, and this is really basic.
            // This only supports like, a quarter of the available TGA features,
            // however TPW doesnt't actually use any other features.
            List<ColorRGBA32> colorData = new List<ColorRGBA32>();
            int width, height;
            StreamReader streamReader = new StreamReader(path);
            BinaryReader binaryReader = new BinaryReader(streamReader.BaseStream);
            int idLength = binaryReader.ReadByte();
            int colorMapType = binaryReader.ReadByte();
            int dataTypeCode = binaryReader.ReadByte();
            int colorMapOrigin = binaryReader.ReadInt16();
            int colorMapLength = binaryReader.ReadInt16();
            int colorMapDepth = binaryReader.ReadByte();

            int xOrigin = binaryReader.ReadInt16();
            int yOrigin = binaryReader.ReadInt16();
            width = binaryReader.ReadInt16();
            height = binaryReader.ReadInt16();
            int bpp = binaryReader.ReadByte();
            int descriptor = binaryReader.ReadByte();

            Debug.Log($"Image {path} has a width of {width} and a height of {height}, and uses {dataTypeCode} data type code.  Image uses {bpp} bits per pixel");

            if (dataTypeCode != 2 || colorMapType != 0) throw new Exception("uhh");

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length - (bpp / 8))
            {
                switch (bpp)
                {
                    case 16:
                        {
                            // ARRRRRGG GGGBBBBB - but its big endian, so in reality its GGGBBBBB ARRRRRGG
                            var byte0 = binaryReader.ReadByte();
                            var byte1 = binaryReader.ReadByte();
                            colorData.Add(new ColorRGBA32(byte0, byte1, 255, 255));
                        }
                        break;
                    case 24:
                        {
                            // 1 byte of each B, G, R (no alpha)
                            var byte0 = binaryReader.ReadByte();
                            var byte1 = binaryReader.ReadByte();
                            var byte2 = binaryReader.ReadByte();
                            colorData.Add(new ColorRGBA32(byte2, byte1, byte0, 255));
                        }
                        break;
                    case 32:
                        {
                            // 1 byte of each B, G, R, A
                            var byte0 = binaryReader.ReadByte();
                            var byte1 = binaryReader.ReadByte();
                            var byte2 = binaryReader.ReadByte();
                            var byte3 = binaryReader.ReadByte();
                            // TODO: handle transparency
                            colorData.Add(new ColorRGBA32(byte3, byte2, byte1, byte0));
                        }
                        break;
                    default:
                        throw new Exception($"{bpp} bits per pixel????");
                }
            }
            binaryReader.Close();
            streamReader.Close();

            return new Texture2D(colorData.ToArray(), width, height);
        }
    }
}
