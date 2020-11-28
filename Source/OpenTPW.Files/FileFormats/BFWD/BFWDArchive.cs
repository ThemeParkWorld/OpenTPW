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
    public class BFWDArchive : IArchive
    {
        private BFWDMemoryStream memoryStream;
        private int version;
        public byte[] buffer { get; internal set; }
        public List<ArchiveFile> files { get; internal set; }

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
            version = memoryStream.ReadInt32();
            memoryStream.Seek(64, SeekOrigin.Current); // Skip padding
            var fileCount = memoryStream.ReadInt32();

            var fileDirectoryOffset = memoryStream.ReadInt32();
            var fileDirectoryLength = memoryStream.ReadInt32();

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

                newFile.compressed = memoryStream.ReadUInt32() == 4;
                newFile.decompressedSize = memoryStream.ReadUInt32();

                // Set file's name name
                memoryStream.Seek(filenameOffset, SeekOrigin.Begin);
                newFile.name = memoryStream.ReadString((int)filenameLength);

                // Get file's raw data
                memoryStream.Seek(dataOffset, SeekOrigin.Begin);
                newFile.data = memoryStream.ReadBytes((int)dataLength);

                newFile.archiveOffset = (int)dataOffset;
                newFile.parentArchive = this;

                files.Add(newFile);
                memoryStream.Seek(initialPos + 40, SeekOrigin.Begin); // Return to initial position, skip to the next file's data
            }
        }

        public void LoadArchive(string path)
        {
            // Set up read buffer
            var tempStreamReader = new StreamReader(path);
            var fileLength = (int)tempStreamReader.BaseStream.Length;
            buffer = new byte[fileLength];
            tempStreamReader.BaseStream.Read(buffer, 0, fileLength);
            tempStreamReader.Close();

            memoryStream = new BFWDMemoryStream(buffer);
            files = new List<ArchiveFile>();

            ReadArchive();
        }
    }
}
