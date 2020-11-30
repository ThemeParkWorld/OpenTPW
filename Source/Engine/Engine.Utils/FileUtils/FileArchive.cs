using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Zstandard.Net;

/* 
 * Archive header:
 * 4 bytes - magic number: ALEX (Amazingly Lame Engine, Xezno -- thanks dotequals) 
 * 4 bytes - file version maj: 01
 * 4 bytes - file version min: 00
 * 12 bytes - padding
 * --------------------
 * Archive directory
 * 4 bytes - file count
 * for each file:
 * n bytes - length-prefixed file name string (null-terminated)
 * 8 bytes - file location
 * 8 bytes - file length (max 9.2 yottabytes, but technically limited by file location)
 * 4 bytes - compression method (00 for uncompressed, 01 for zstd)
 * 8 bytes - padding
 * --------------------
 * File data:
 * for each file:
 * 4 bytes - padding
 * n bytes - file data
 * 4 bytes - padding
 */

namespace Engine.Utils.FileUtils
{
    public partial class FileArchive
    {
        public ImmutableList<FileArchiveFile> Files { get; set; }

        public FileArchive()
        {
            Files = ImmutableList<FileArchiveFile>.Empty;
        }

        public FileArchive(List<FileArchiveFile> files)
        {
            Files = files.ToImmutableList();
        }

        public void AddFile(FileArchiveFile file)
        {
            throw new NotImplementedException();
        }

        public void WriteToFile(string filePath)
        {
            using var fileStream = File.Open(filePath, FileMode.Create);
            using var binaryWriter = new BinaryWriter(fileStream);

            // Archive header
            binaryWriter.Write(new[] { 'A', 'L', 'E', 'X' }); // Magic number
            binaryWriter.Write(01); // Version maj
            binaryWriter.Write(00); // Version min

            for (int i = 0; i < 12; ++i)
                binaryWriter.Write((byte)0);

            // Archive directory
            binaryWriter.Write(Files.Count);

            List<long> fileLocOffsets = new List<long>();
            foreach (var file in Files)
            {
                binaryWriter.Write(file.FileName); // Length-prefixed automatically
                fileLocOffsets.Add(binaryWriter.BaseStream.Position);
                binaryWriter.Write((long)00);
                binaryWriter.Write((long)file.FileData.Length);
                binaryWriter.Write((int)file.FileCompressionMethod);
                
                for (int i = 0; i < 8; ++i)
                    binaryWriter.Write((byte)0);
            }

            // File data
            for (int fileIndex = 0; fileIndex < Files.Count; fileIndex++)
            {
                FileArchiveFile file = Files[fileIndex];
                var fileDataPos = binaryWriter.BaseStream.Position;

                binaryWriter.BaseStream.Seek(fileLocOffsets[fileIndex], SeekOrigin.Begin);
                binaryWriter.Write(fileDataPos);

                binaryWriter.BaseStream.Seek(fileDataPos, SeekOrigin.Begin);;
                
                for (int i = 0; i < 4; ++i)
                    binaryWriter.Write((byte)0);

                if (file.FileCompressionMethod == CompressionMethod.None)
                {
                    binaryWriter.Write(file.FileData);
                }
                else if (file.FileCompressionMethod == CompressionMethod.Zstd)
                {
                    using var memoryStream = new MemoryStream();
                    using var compressStream = new ZstandardStream(memoryStream, System.IO.Compression.CompressionMode.Compress);
                    compressStream.CompressionLevel = 5;
                    compressStream.Write(file.FileData, 0, file.FileData.Length);
                    compressStream.Close();

                    var compressedData = memoryStream.ToArray();
                    binaryWriter.Write(compressedData);
                }

                for (int i = 0; i < 4; ++i)
                    binaryWriter.Write((byte)0);
            }
        }

        public static FileArchive LoadFromFile(string filePath)
        {
            using var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var binaryReader = new BinaryReader(fileStream);

            // Archive header
            var magicNumber = binaryReader.ReadBytes(4);
            if (!Enumerable.SequenceEqual(magicNumber, new[] { (byte)'A', (byte)'L', (byte)'E', (byte)'X' }))
                throw new Exception("Not a valid ALEX file.");

            var fileMajor = binaryReader.ReadInt32();
            var fileMinor = binaryReader.ReadInt32();

            if (fileMajor != 01 || fileMinor != 00)
                throw new Exception($"Unknown file version {fileMajor}.{fileMinor}");

            binaryReader.ReadBytes(12); // Padding

            // Archive directory
            var files = new List<FileArchiveFile>();
            var fileCount = binaryReader.ReadInt32();
            for (int i = 0; i < fileCount; ++i)
            {
                var fileName = binaryReader.ReadString();

                var fileLocation = binaryReader.ReadInt64();
                var fileLength = binaryReader.ReadInt64();
                var fileCompressionMethod = (CompressionMethod)binaryReader.ReadInt32();

                binaryReader.ReadBytes(8); // Padding

                files.Add(new FileArchiveFile(
                    fileName,
                    fileLocation,
                    fileLength,
                    fileCompressionMethod,
                    new byte[0]
                ));
            }

            // File data
            foreach (var file in files)
            {
                binaryReader.BaseStream.Seek(file.FileLocation, SeekOrigin.Begin);
                binaryReader.ReadBytes(4); // Padding

                var fileData = binaryReader.ReadBytes((int)file.FileLength); // TODO: support long for reading from stream (i.e. remove cast)
                if (file.FileCompressionMethod == CompressionMethod.None)
                {
                    file.FileData = fileData;
                }
                else if (file.FileCompressionMethod == CompressionMethod.Zstd)
                {
                    using var memoryStream = new MemoryStream(fileData);
                    using var dcmpMemoryStream = new MemoryStream();
                    using var compressionStream = new ZstandardStream(memoryStream, System.IO.Compression.CompressionMode.Decompress);
                    compressionStream.CopyTo(dcmpMemoryStream);
                    file.FileData = dcmpMemoryStream.ToArray();
                }

                binaryReader.ReadBytes(4); // Padding
            }

            return new FileArchive(files);
        }

        public static FileArchive LoadFromData()
        {
            throw new NotImplementedException();
        }
    }
}
