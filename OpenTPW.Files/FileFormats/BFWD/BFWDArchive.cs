using System;
using System.Collections.Generic;
using System.IO;

namespace OpenTPW.Files.BFWD
{
    /*
     * Special thanks to Fatbag: http://wiki.niotso.org/RefPack, 
     * WATTO: http://wiki.xentax.com/index.php/WAD_DWFB, 
     * and Rhys: https://github.com/RHY3756547/FreeSO/blob/master/TSOClient/tso.files/FAR3/Decompresser.cs.
     */
    public class BFWDArchive : IArchive
    {
        private BFWDMemoryStream _memoryStream;
        private int _version;
        public byte[] buffer { get; internal set; }
        public List<ArchiveFile> files { get; internal set; }

        public BFWDArchive(string path)
        {
            LoadArchive(path);
        }

        public void Dispose()
        {
            _memoryStream.Dispose();
        }

        private void _ReadArchive()
        {
            _memoryStream.Seek(0, SeekOrigin.Begin);
            /* Header:
             * 4 - magic number ('DWFB')
             * 4 - version
             * 64 - padding??
             * 4 - file count
             * 4 - file list offset
             * 4 - file list length
             * 4 - null
             */

            var magicNumber = _memoryStream.ReadString(4);
            if (magicNumber != "DWFB")
                throw new Exception($"Magic number did not match: {magicNumber}");
            _version = _memoryStream.ReadInt32();
            _memoryStream.Seek(64, SeekOrigin.Current); // Skip padding
            var fileCount = _memoryStream.ReadInt32();

            var fileDirectoryOffset = _memoryStream.ReadInt32();
            var fileDirectoryLength = _memoryStream.ReadInt32();

            _memoryStream.Seek(4, SeekOrigin.Current); // Skip 'null'

            // Details directory
            // See DWFBFile for more info
            for (int i = 0; i < fileCount; ++i)
            {
                GC.Collect();
                // Save the current position so that we can go back to it later
                var initialPos = _memoryStream.Position;

                ArchiveFile newFile = new ArchiveFile();
                _memoryStream.Seek(4, SeekOrigin.Current); // Skip 'null'
                var filenameOffset = _memoryStream.ReadUInt32();
                var filenameLength = _memoryStream.ReadUInt32();
                var dataOffset = _memoryStream.ReadUInt32();
                var dataLength = _memoryStream.ReadUInt32();

                newFile.compressed = _memoryStream.ReadUInt32() == 4;
                newFile.decompressedSize = _memoryStream.ReadUInt32();

                // Set file's name name
                _memoryStream.Seek(filenameOffset, SeekOrigin.Begin);
                newFile.name = _memoryStream.ReadString((int)filenameLength);

                // Get file's raw data
                _memoryStream.Seek(dataOffset, SeekOrigin.Begin);
                newFile.data = _memoryStream.ReadBytes((int)dataLength);

                newFile.archiveOffset = (int)dataOffset;
                newFile.parentArchive = this;

                files.Add(newFile);
                _memoryStream.Seek(initialPos + 40, SeekOrigin.Begin); // Return to initial position, skip to the next file's data
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

            _memoryStream = new BFWDMemoryStream(buffer);
            files = new List<ArchiveFile>();

            _ReadArchive();
        }
    }
}
