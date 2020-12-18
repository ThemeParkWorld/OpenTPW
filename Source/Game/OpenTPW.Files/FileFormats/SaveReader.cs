using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenTPW.Files.FileFormats
{
    // https://github.com/xezno/OpenTPW/wiki/TPWS,-INTS,-LAYS
    // TODO: INTS, LAYS implementation
    // TODO: Read serialized data
    public class SaveFile
    {
        public int FileType { get; set; }
        public int FileVersion { get; set; }
        public bool IsOnline { get; set; }
        public byte[] SerializedData { get; set; }
    }
    public class SaveReader : IAssetReader
    {
        public string[] Extensions => new[] { ".tpws", ".ints", ".lays" };

        private SaveFile saveFile;
        public string AssetName => "TPWS / INTS / LAYS (Save files)";

        public IAssetContainer LoadAsset(byte[] data)
        {
            using var memoryStream = new MemoryStream(data);
            using var binaryReader = new BinaryReader(memoryStream);

            ReadHeader(binaryReader);
            ReadFileInfo(binaryReader);
            ReadData(binaryReader);

            return new AssetContainer<SaveFile>(saveFile);
        }

        private void ReadHeader(BinaryReader binaryReader)
        {
            /* Header
             * Magic number (TPWS): F4 01 00 00
             * Copyright notice: 0x4 to 0x33B
             * Padding: 0x33C to 0x603
             */
            if (!Equals(new[] { 0xF4, 0x01, 0x00, 0x00 }, binaryReader.ReadBytes(4)))
                throw new Exception("Not a valid TPWS file.");

            binaryReader.BaseStream.Seek(0x603, SeekOrigin.Begin);
        }

        private void ReadFileInfo(BinaryReader binaryReader)
        {
            /* File info
             * 4 bytes: File type (00 01 22 19)
             * 1 byte: File version (85)
             * 1 byte: Online flag (00 = offline save, 01 = upload.LAYS)
             * 2 bytes: Padding
             * 
             * If online flag set:
             *     Unknown data - 0x060C to 0x0846
             */

            saveFile.FileType = binaryReader.ReadInt32();
            saveFile.FileVersion = binaryReader.ReadByte();
            saveFile.IsOnline = binaryReader.ReadByte() == 1;

            binaryReader.ReadBytes(2); // Padding
        }

        private void ReadData(BinaryReader binaryReader)
        {
            /* Data
             * ZLIB Header:
             * 4 bytes: Magic number - BILZ
             * 4 bytes: Unknown
             * 4 bytes: Compressed length
             * 16 bytes: Unknown
             * 
             * ZLIB stream begins after this point, continues to end of file 
             */
            
            if (!Equals(new[] { 'B', 'I', 'L', 'Z' }, binaryReader.ReadBytes(4)))
                throw new Exception("Not a valid BILZ stream.");

            binaryReader.ReadBytes(4);

            var compressedLength = binaryReader.ReadInt32();
            binaryReader.ReadBytes(16);
            
            // Read all data (from here til the end of the file)
            var data = binaryReader.ReadBytes((int)(binaryReader.BaseStream.Length - binaryReader.BaseStream.Position));

            using var memoryStream = new MemoryStream(data);
            using var decmpMemoryStream = new MemoryStream();
            using var zlibStream = new ZlibStream(memoryStream, CompressionMode.Decompress);

            zlibStream.CopyTo(decmpMemoryStream);

            // Read to Data
            saveFile.SerializedData = new byte[(int)decmpMemoryStream.Length];
            decmpMemoryStream.Seek(0, SeekOrigin.Begin);
            decmpMemoryStream.Read(data , 0, (int)decmpMemoryStream.Length);
        }
    }
}
