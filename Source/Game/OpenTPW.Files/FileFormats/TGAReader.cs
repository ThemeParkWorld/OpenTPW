using Quincy;
using Engine.Utils.DebugUtils;
using OpenGL;
using System;
using System.Collections.Generic;
using System.IO;

namespace OpenTPW.Files.FileFormats
{
    public class TGAReader : IAssetReader
    {
        public string[] Extensions => new[] { ".tga" };

        public IAssetContainer LoadAsset(byte[] data)
        {
            // I'm really lazy, and this is really basic.
            // This only supports like, a quarter of the available TGA features,
            // however TPW doesn't actually use any other features.
            using var memoryReader = new MemoryStream(data);
            using var binaryReader = new BinaryReader(memoryReader);

            int width, height;
            int idLength = binaryReader.ReadByte();
            int colorMapType = binaryReader.ReadByte();
            int dataTypeCode = binaryReader.ReadByte();
            int colorMapOrigin = binaryReader.ReadInt16();
            int colorMapLength = binaryReader.ReadInt16();
            int colorMapDepth = binaryReader.ReadByte();

            int xOrigin = binaryReader.ReadInt16();
            int yOrigin = binaryReader.ReadInt16();
            width = binaryReader.ReadInt16();
            height = binaryReader.ReadInt16() + 1;
            int bpp = binaryReader.ReadByte();
            int descriptor = binaryReader.ReadByte();
            
            var colorData = new ColorRGBA32[width * height];

            Logging.Log($"Image has a width of {width} and a height of {height}, and uses {dataTypeCode} data type code. Image uses {bpp} bits per pixel");

            if (dataTypeCode != 2 || colorMapType != 0)
                throw new Exception("uhh");

            int i = 0;

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length - (bpp / 8))
            {
                switch (bpp)
                {
                    case 16:
                        // ARRRRRGG GGGBBBBB - but its big endian, so in reality its GGGBBBBB ARRRRRGG
                        throw new NotImplementedException();
                    case 24:
                        {
                            // 1 byte of each B, G, R (no alpha)
                            var byte0 = binaryReader.ReadByte();
                            var byte1 = binaryReader.ReadByte();
                            var byte2 = binaryReader.ReadByte();
                            colorData[i] = new ColorRGBA32(byte2, byte1, byte0, 255);
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
                            colorData[i] = new ColorRGBA32(byte3, byte2, byte1, byte0);
                        }
                        break;
                    default:
                        throw new Exception($"{bpp} is an invalid number of bits per pixel.");
                }
                i++;
            }

            return new AssetContainer<Texture>(Texture.LoadFromColorData(colorData, width, height, "texture_diffuse"));
        }
    }
}
