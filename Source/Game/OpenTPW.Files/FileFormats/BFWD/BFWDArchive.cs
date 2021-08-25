using System;
using System.Collections.Generic;
using System.IO;

namespace OpenTPW.Files.FileFormats.BFWD
{
    /*
     * Special thanks to Fatbag: http://wiki.niotso.org/RefPack, 
     * WATTO: http://wiki.xentax.com/index.php/WAD_DWFB, 
     * and Rhys: https://github.com/RHY3756547/FreeSO/blob/master/TSOClient/tso.files/FAR3/Decompresser.cs.
     */
    public class BFWDArchive
    {
        private BFWDMemoryStream memoryStream;
        public byte[] Buffer { get; internal set; }
        public List<ArchiveFile> Files { get; internal set; }

        public BFWDArchive(string path)
        {
            LoadArchive(path);
        }

        public void Dispose()
        {
            memoryStream.Dispose();
        }

        private void ReadArchive()
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            /* Header:
             * 4 - magic number ('DWFB')
             * 4 - version
             * 64 - padding??
             * 4 - file count
             * 4 - file list offset
             * 4 - file list length
             * 4 - null
             */

            var magicNumber = memoryStream.ReadString(4);
            if (magicNumber != "DWFB")
                throw new Exception($"Magic number did not match: {magicNumber}");
            _ = memoryStream.ReadInt32(); // version
            memoryStream.Seek(64, SeekOrigin.Current); // Skip padding
            var fileCount = memoryStream.ReadInt32();

            _ = memoryStream.ReadInt32(); // fileDirectoryOffset
            _ = memoryStream.ReadInt32(); // fileDirectoryLength

            memoryStream.Seek(4, SeekOrigin.Current); // Skip 'null'

            // Details directory
            // See DWFBFile for more info
            for (var i = 0; i < fileCount; ++i)
            {
                GC.Collect();
                // Save the current position so that we can go back to it later
                var initialPos = memoryStream.Position;

                var newFile = new ArchiveFile();
                memoryStream.Seek(4, SeekOrigin.Current); // Skip 'null'
                var filenameOffset = memoryStream.ReadUInt32();
                var filenameLength = memoryStream.ReadUInt32();
                var dataOffset = memoryStream.ReadUInt32();
                var dataLength = memoryStream.ReadUInt32();

                newFile.Compressed = memoryStream.ReadUInt32() == 4;
                newFile.DecompressedSize = memoryStream.ReadUInt32();

                // Set file's name name
                memoryStream.Seek(filenameOffset, SeekOrigin.Begin);
                newFile.Name = memoryStream.ReadString((int)filenameLength);

                // Get file's raw data
                memoryStream.Seek(dataOffset, SeekOrigin.Begin);
                newFile.CompressedData = memoryStream.ReadBytes((int)dataLength);

                newFile.ArchiveOffset = (int)dataOffset;
                newFile.ParentArchive = this;

                Files.Add(newFile);
                memoryStream.Seek(initialPos + 40, SeekOrigin.Begin); // Return to initial position, skip to the next file's data
            }
        }

        public void LoadArchive(string path)
        {
            // Set up read buffer
            var tempStreamReader = new StreamReader(path);
            var fileLength = (int)tempStreamReader.BaseStream.Length;
            Buffer = new byte[fileLength];
            tempStreamReader.BaseStream.Read(Buffer, 0, fileLength);
            tempStreamReader.Close();

            memoryStream = new BFWDMemoryStream(Buffer);
            Files = new List<ArchiveFile>();

            ReadArchive();
        }
    }
}
