using ECSEngine;
using ECSEngine.Render;
using OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

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
    public static class WCTReader
    {
        public static Texture2D LoadAsset(string path)
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
            StreamReader streamReader = new StreamReader(path);
            BinaryReader binaryReader = new BinaryReader(streamReader.BaseStream);
            int versionMajor = binaryReader.ReadByte();
            int versionMinor = binaryReader.ReadByte();
            int bpp = binaryReader.ReadByte();
            bpp = 24;
            int unknown0 = binaryReader.ReadByte();
            width = (binaryReader.ReadInt16());
            height = (binaryReader.ReadInt16());

            binaryReader.BaseStream.Seek(8, SeekOrigin.Current);
            file1Len = binaryReader.ReadInt32();
            file2Len = binaryReader.ReadInt32();
            binaryReader.BaseStream.Seek(4, SeekOrigin.Current);

            bool zlibFile = Encoding.ASCII.GetString(binaryReader.ReadBytes(4)) == "BILZ";
            List<byte> decompressedData = new List<byte>();
            List<byte> decompressedDataFile2 = new List<byte>();
            if (zlibFile)
            {
                binaryReader.BaseStream.Seek(24, SeekOrigin.Current);
                // epic zlib time
                // first byte is CMF - compression method and flags.
                // if the compression method is 8, then DEFLATE is being used.
                // if the compression method is 15, then the reserved method is being used.

                // if we're using DEFLATE:
                //      compression info becomes the base-2 logarithm of the deflate window size minus eight

                byte cmf = binaryReader.ReadByte();
                int compressionMethod = cmf.GetHighNibble();
                int compressionInfo = cmf.GetLowNibble();

                int deflateWindowSize = (int)Math.Pow(2, 8 + compressionInfo); // (we reverse the base-2 log - 8)

                // second byte is the flags:
                // 

                byte flg = binaryReader.ReadByte();
                int fcheck = flg.GetBitsAsInt(0, 4);

                if ((256 * (flg + (cmf << 8))) % 31 != 0) // checksum style thing
                    throw new Exception("FCHECK was set incorrectly");

                bool fdict = flg.GetBit(5);
                int flevel = flg.GetBitsAsInt(6, 2);

                if (fdict)
                {
                    byte[] dict = binaryReader.ReadBytes(4);
                    // adler-32 checksum for this block
                }


                Debug.Log($"zlib compressed file:\n\tCMF: {BitConverter.ToString(new[] { cmf })}\n\tFLG: {BitConverter.ToString(new[] { flg })}\n\tCompression method: {compressionMethod}\n\tDeflate window size: {deflateWindowSize}\n\tHas dictionary? {fdict}\n\tCompression level: {flevel}");

                var memoryStream = new MemoryStream();
                using (DeflateStream decompressionStream = new DeflateStream(streamReader.BaseStream, CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(memoryStream);
                }
                memoryStream.Seek(0, SeekOrigin.Begin);
                for (int i = 0; i < file1Len; ++i)
                {
                    decompressedData.Add((byte)memoryStream.ReadByte());
                }
                memoryStream.Close();

                binaryReader.BaseStream.Seek(file1Len + 28, SeekOrigin.Current);

                var memoryStreamFile2 = new MemoryStream();
                using (DeflateStream decompressionStream = new DeflateStream(streamReader.BaseStream, CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(memoryStreamFile2);
                }
                memoryStreamFile2.Seek(0, SeekOrigin.Begin);
                for (int i = 0; i < file2Len; ++i)
                {
                    decompressedDataFile2.Add((byte)memoryStreamFile2.ReadByte());
                }
                memoryStreamFile2.Close();
            }
            else
            {
                throw new Exception("i dont know how to do this yet :(");
            }

            Debug.Log($"WCT {path} has a width of {width} and a height of {height}, and uses {bpp} bits per pixel.  Compressed: {zlibFile.ToString()} - decompressed data length: {decompressedData.Count} (should be ~{(width * height * bpp) / 8})");

            if (File.Exists("test.wct.file1"))
                File.Delete("test.wct.file1");
            using (FileStream fileStream = new FileStream("test.wct.file1", FileMode.OpenOrCreate))
            {
                fileStream.Write(decompressedData.ToArray(), 0, decompressedData.Count);
                Debug.Log($"Wrote file 1 to test.wct.file1");
            }
            if (File.Exists("test.wct.file2"))
                File.Delete("test.wct.file2");
            using (FileStream fileStream = new FileStream("test.wct.file2", FileMode.OpenOrCreate))
            {
                fileStream.Write(decompressedDataFile2.ToArray(), 0, decompressedDataFile2.Count);
                Debug.Log($"Wrote file 2 to test.wct.file2");
            }

            ColorRGB24[] data = new ColorRGB24[width * height * bpp];

            for (int i = 0; i < width * height; ++i)
            {
                data[i] = new ColorRGB24(255, 0, 255);
            }

            int dataPos = 0;
            for (int y = height - 1; y >= 0; --y)
            {
                for (int x = 0; x < width; ++x)
                {
                    var bytes = new byte[bpp / 8];
                    for (int i = 0; i < bytes.Length; ++i)
                    {
                        // Each byte appears to refer to a color
                        // In the test file, the palette starts at 0x0100 (8 * 8 * 4)
                        // So we seek to 0x100 + the byte's value, and then read (bpp / 8) bytes
                        // in order to get the colour

                        //bytes[i] = decompressedData[(width * height * 4) + decompressedData[x + (y * width)] + i];
                        try
                        {
                            bytes[i] = decompressedData[x + (y * width) + (i * width)];
                        }
                        catch
                        {
                            bytes[i] = 0xff;
                        }
                    }

                    // var bytes = decompressedReader.ReadBytes(bpp / 8);
                    switch (bpp)
                    {
                        case 32: // R G B A?
                            {
                                // TODO: Transparency
                                data[dataPos] = new ColorRGB24(bytes[0], bytes[3], bytes[2]);
                            }
                            break;
                        case 24: // R G B? (no alpha)
                            {
                                data[dataPos] = new ColorRGB24(bytes[2], bytes[1], bytes[0]);
                            }
                            break;
                        default:
                            throw new Exception($"wtf is {bpp} bits per pixel????");
                    }
                    dataPos++;
                }
            }

            binaryReader.Close();
            streamReader.Close();
            return new Texture2D(data, width, height);
        }
    }
}
