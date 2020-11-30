using Engine.Utils.DebugUtils;
using Ionic.Zlib;
using OpenGL;
using Quincy;
using System;
using System.IO;
using System.Text;

// TODO: Re-write this

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
            bpp = 32;
            int unknown0 = binaryReader.ReadByte();
            width = (binaryReader.ReadInt16());
            height = (binaryReader.ReadInt16());

            binaryReader.BaseStream.Seek(8, SeekOrigin.Current);
            file1Len = binaryReader.ReadInt32();
            file2Len = binaryReader.ReadInt32();
            binaryReader.BaseStream.Seek(4, SeekOrigin.Current);

            var zlibFile = Encoding.ASCII.GetString(binaryReader.ReadBytes(4)) == "BILZ";
            var decmpMemoryStream = new MemoryStream();
            var decmpMemoryStreamFile2 = new MemoryStream();
            if (zlibFile)
            {
                // 4 bytes - decompressed size
                // 4 bytes - compressed size
                // rest unknown

                binaryReader.BaseStream.Seek(24, SeekOrigin.Current);

                // File 1
                var cmpMemoryStream = new MemoryStream();
                cmpMemoryStream.Write(binaryReader.ReadBytes(file1Len - 28), 0, file1Len - 28);
                cmpMemoryStream.Seek(0, SeekOrigin.Begin);

                using (var decompressionStream = new ZlibStream(cmpMemoryStream, Ionic.Zlib.CompressionMode.Decompress, true))
                    decompressionStream.CopyTo(decmpMemoryStream);

                // Dump file 1
                decmpMemoryStream.Seek(0, SeekOrigin.Begin);
                if (File.Exists("file1Dump"))
                    File.Delete("file1Dump");
                var file1Contents = new byte[decmpMemoryStream.Length];
                decmpMemoryStream.Read(file1Contents, 0, (int)decmpMemoryStream.Length);
                using (var fileStream = new FileStream("file1Dump", FileMode.OpenOrCreate))
                    fileStream.Write(file1Contents, 0, file1Contents.Length);

                // Fast-forward to file 2 content
                binaryReader.BaseStream.Seek(28, SeekOrigin.Current);

                if (file2Len != 0)
                {
                    // File 2
                    var cmpMemoryStreamFile2 = new MemoryStream();
                    cmpMemoryStreamFile2.Write(binaryReader.ReadBytes(file2Len - 28), 0, file2Len - 28);
                    cmpMemoryStreamFile2.Seek(0, SeekOrigin.Begin);

                    using (var decompressionStream = new ZlibStream(cmpMemoryStreamFile2, Ionic.Zlib.CompressionMode.Decompress, true))
                        decompressionStream.CopyTo(decmpMemoryStreamFile2);

                    // Dump file 2
                    decmpMemoryStreamFile2.Seek(0, SeekOrigin.Begin);
                    if (File.Exists("file2Dump"))
                        File.Delete("file2Dump");
                    var file2Contents = new byte[decmpMemoryStreamFile2.Length];
                    decmpMemoryStreamFile2.Read(file2Contents, 0, (int)decmpMemoryStreamFile2.Length);
                    using (var fileStream = new FileStream("file2Dump", FileMode.OpenOrCreate))
                        fileStream.Write(file2Contents, 0, file2Contents.Length);

                    cmpMemoryStreamFile2.Close();
                }
            }
            else
            {
                throw new Exception("Not yet implemented");
            }

            Logging.Log($"WCT file has a width of {width} and a height of {height}, and uses {bpp} bits per pixel.  Compressed: {zlibFile.ToString()} - decompressed data length: {decmpMemoryStream.Length} (should be ~{(width * height * bpp) / 8})");

            var imageData = new ColorRGBA32[width * height];

            for (var i = 0; i < width * height; ++i)
            {
                imageData[i] = new ColorRGBA32(255, 0, 255, 255);
            }

            decmpMemoryStream.Seek(0, SeekOrigin.Begin);
            var dataPos = 0;
            for (var y = height - 1; y >= 0; --y)
            {
                for (var x = 0; x < width; ++x)
                {
                    var bytes = new byte[1];
                    decmpMemoryStream.Read(bytes, 0, bytes.Length);
                    imageData[dataPos] = new ColorRGBA32(bytes[0], bytes[0], bytes[0], 255);
                    dataPos++;
                }
            }

            return new AssetContainer<Texture>(Texture.LoadFromColorData(imageData, width, height, "texture_diffuse"));
        }
    }
}
