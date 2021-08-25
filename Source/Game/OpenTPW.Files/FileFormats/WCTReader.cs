using Engine.Utils.DebugUtils;
using Ionic.Zlib;
using OpenGL;
using Quincy;
using System;
using System.IO;
using System.Text;

// This is a mess, sorry.
// TODO: REWRITE THIS!
namespace OpenTPW.Files.FileFormats
{
    /*
     * https://tools.ietf.org/html/rfc1950 - ZLIB Compressed Data Format Specification (RFC 1950)
     * https://www.w3.org/Graphics/PNG/RFC-1951 - DEFLATE Compressed Data Format Specification version 1.3 (RFC 1951)
     * 
     * Bits are numbered with the most significant bit on the left.
     * +--------+
     * |76543210|
     * +--------+
     * To maintain readability, the bit numbers are taken from the documentation
     * but are basically converted within these functions.
     */

    // world compressed texture
    public class WCTReader : IAssetReader
    {
        public string[] Extensions => new[] { ".wct" };
        public string AssetName => "WCT (Texture)";

        public IAssetContainer LoadAsset(byte[] data)
        {
            // WCT File header - little endian
            // Version? (1 byte) - usually 0x12 or 0x13, links with BILZ/ZLIB
            // Version minor? (1 byte) - usually 0x00 or 0x01, also seemingly links with bits per pixel?
            // Bits per pixel (1 byte) - usually 24 or 32.
            // Unknown (1 byte) - always 0x04
            // Texture width (2 bytes) - this is usually POT
            // Texture height (2 bytes) - also usually POT
            //
            // 8 bytes of "i have no clue"
            // File 1 length (4 bytes) - Includes zlib header
            // File 2 length (4 bytes) - Includes zlib header
            // 4 bytes of "????"
            //
            // interestingly, if its a bilz file, the bytes at 0x00000010 will repeat again at 0x00000024, so these must play a part in compression
            // Update: the bytes at 0x00000010 are the file's length
            //
            // If the next 4 bytes are "BILZ", then we have to work out more data.  This is probably some sort of compression.
            // Magic number? (4 bytes) - Always "BILZ" - zlib maybe?
            // 28 more bytes of "???"
            int width, height, file1Len, file2Len;
            using var memoryStream = new MemoryStream(data);
            using var binaryReader = new BinaryReader(memoryStream);
            int versionMajor = binaryReader.ReadByte();
            int versionMinor = binaryReader.ReadByte();
            int bpp = binaryReader.ReadByte();
            bpp = 24;
            int unknown0 = binaryReader.ReadByte();
            width = (binaryReader.ReadInt16());
            height = (binaryReader.ReadInt16());

            width = 32;
            height = 32;

            binaryReader.BaseStream.Seek(8, SeekOrigin.Current);
            file1Len = binaryReader.ReadInt32();
            file2Len = binaryReader.ReadInt32();
            binaryReader.BaseStream.Seek(4, SeekOrigin.Current);

            var zlibFile = Encoding.ASCII.GetString(binaryReader.ReadBytes(4)) == "BILZ";
            var decmpMemoryStream = new MemoryStream();
            if (zlibFile)
            {
                // 4 bytes - decompressed size
                // 4 bytes - compressed size
                // rest unknown

                binaryReader.BaseStream.Seek(24, SeekOrigin.Current);
                var cmpMemoryStream = new MemoryStream();
                cmpMemoryStream.Write(binaryReader.ReadBytes(file1Len), 0, file1Len);
                cmpMemoryStream.Seek(0, SeekOrigin.Begin); // Rewind

                using (var decompressionStream = new ZlibStream(cmpMemoryStream, CompressionMode.Decompress, true))
                    decompressionStream.CopyTo(decmpMemoryStream);
                
                // Dump file 1
                decmpMemoryStream.Seek(0, SeekOrigin.Begin);
                if (File.Exists("file1Dump"))
                    File.Delete("file1Dump");
                var file1Contents = new byte[decmpMemoryStream.Length];
                decmpMemoryStream.Read(file1Contents, 0, (int)decmpMemoryStream.Length);
                using (var fileStream = new FileStream("file1Dump", FileMode.OpenOrCreate))
                    fileStream.Write(file1Contents, 0, file1Contents.Length);
            }
            else
            {
                throw new Exception("Not yet implemented");
            }

            Logging.Log($"WCT file has a width of {width} and a height of {height}, and uses {bpp} bits per pixel.  Compressed: {zlibFile.ToString()} - decompressed data length: {decmpMemoryStream.Length} (should be ~{(width * height * bpp) / 8})");

            var imageData = new ColorRGBA32[decmpMemoryStream.Length];

            decmpMemoryStream.Seek(0, SeekOrigin.Begin);
            var dataBuffer = new byte[decmpMemoryStream.Length];
            decmpMemoryStream.Read(dataBuffer, 0, (int)decmpMemoryStream.Length);
            decmpMemoryStream.Close();

            int stride = 0;
            var convertedBuffer = ConvertTo8Bit(dataBuffer, width, height, 0, 1, false, ref stride);
            var dataPos = imageData.Length - 1;
            
            for (int i = 0; i < convertedBuffer.Length; ++i)
            {
                var b = convertedBuffer[i];
                // Console.WriteLine(b);

                if (b == 1)
                {
                    imageData[dataPos] = new ColorRGBA32(255, 255, 255, 255);    
                }
                else
                {
                    imageData[dataPos] = new ColorRGBA32(0, 0, 0, 255);
                }
                    
                if (dataPos > 0)
                    dataPos--;
            }

            return new AssetContainer<Texture>(Texture.LoadFromColorData(imageData, width, height, "texture_diffuse"));
        }
        
        // Code below stolen from https://stackoverflow.com/a/51124131.
        /// <summary>
        /// Converts given raw image data for a paletted image to 8-bit, so we have a simple one-byte-per-pixel format to work with.
        /// </summary>
        /// <param name="fileData">The file data.</param>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <param name="start">Start offset of the image data in the fileData parameter.</param>
        /// <param name="bitsLength">Amount of bits used by one pixel.</param>
        /// <param name="bigEndian">True if the bits in the original image data are stored as big-endian.</param>
        /// <param name="stride">Stride used in the original image data. Will be adjusted to the new stride value.</param>
        /// <returns>The image data in a 1-byte-per-pixel format, with a stride exactly the same as the width.</returns>
        public static Byte[] ConvertTo8Bit(byte[] fileData, int width, int height, int start, int bitsLength, bool bigEndian, ref int stride)
        {
            if (bitsLength != 1 && bitsLength != 2 && bitsLength != 4 && bitsLength != 8)
                throw new ArgumentOutOfRangeException("Cannot handle image data with " + bitsLength + "bits per pixel.");
            // Full array
            Byte[] data8bit = new Byte[width * height];
            // Amount of pixels that end up on the same byte
            int parts = 8 / bitsLength;
            // Amount of bytes to write per line
            int newStride = width;
            // Bit mask for reducing read and shifted data to actual bits length
            int bitmask = (1 << bitsLength) - 1;
            int size = stride * height;
            // File check, and getting actual data.
            if (start + size > fileData.Length)
                throw new IndexOutOfRangeException("Data exceeds array bounds!");
            // Actual conversion process.
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // This will hit the same byte multiple times
                    int indexXbit = start + y * stride + x / parts;
                    // This will always get a new index
                    int index8bit = y * newStride + x;
                    // Amount of bits to shift the data to get to the current pixel data
                    int shift = (x % parts) * bitsLength;
                    // Reversed for big-endian
                    if (bigEndian)
                        shift = 8 - shift - bitsLength;
                    // Get data and store it.
                    data8bit[index8bit] = (Byte)((fileData[indexXbit] >> shift) & bitmask);
                }
            }
            stride = newStride;
            return data8bit;
        }
    }
}
