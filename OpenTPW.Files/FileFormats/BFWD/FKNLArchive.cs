using System;
using System.Collections.Generic;
using System.IO;

namespace OpenTPW.Files.FileFormats.BFWD
{
    // TODO: This is incredibly incomplete and does not work.
    // Also, is there actually any need for it?
    /*
     * The 'FKNL' WAD format isn't actually used in the PC game,
     * but it's used within the PS2 (and probably PS1) ports of
     * the game.  It's quite similar to DWFB in many ways (it
     * uses refpack, for example) which meas that FKNLArchive
     * and BFWDArchive are technically compatible with each
     * other - as long as they use their separate loaders.
     */
    public class FKNLArchive : IArchive
    {
        private BFWDMemoryStream _memoryStream;
        private int _version;
        public byte[] buffer { get; internal set; }
        public List<ArchiveFile> files { get; internal set; }

        public FKNLArchive(string path)
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
             * 4 - magic number ('FKNL')
             * 4 - flags
             * 4 - data offset
             *  if flags & 2
             *      4 - size
             *      4 - zsize
             *      4 - files
             *      4 - dummy
             * 4 - dummy
             * 4 - filename dictionary offset
             * 4 - ??
             * 4 - ??
             * 4 - ??
             */

            /* Details directory
             * for each file:
             * 4 - filename size?
             * 4 - filename offset
             * 4 - file offset (relative to file data start)
             * 4 - file length
             */

            var magicNumber = _memoryStream.ReadString(4);
            if (magicNumber != "FKNL")
                throw new Exception($"Magic number did not match: {magicNumber}");
            _version = _memoryStream.ReadInt32();

            var firstFileOffset = _memoryStream.ReadInt32();

            var fileDirectoryOffset = _memoryStream.ReadInt32();
            var fileDirectoryLength = _memoryStream.ReadInt32();

            _memoryStream.Seek(4, SeekOrigin.Current); // Skip 'null'

            // Details directory
            // See DWFBFile for more info
            for (var i = 0; i < 1; ++i)
            {
                GC.Collect();
                // Save the current position so that we can go back to it later
                var initialPos = _memoryStream.Position;

                var newFile = new ArchiveFile();
                var filenameLength = _memoryStream.ReadUInt32();
                var filenameOffset = _memoryStream.ReadUInt32();
                var dataOffset = _memoryStream.ReadUInt32();
                var dataLength = _memoryStream.ReadUInt32();

                newFile.compressed = _memoryStream.ReadUInt32() == 4;
                newFile.decompressedSize = _memoryStream.ReadUInt32();

                // Set file's name
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
