using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

// TODO: clean up, this file is the biggest mess
namespace OpenTPW.Files.FileFormats
{
    /*
     * Special thanks to Fatbag: http://wiki.niotso.org/RefPack, 
     * WATTO: http://wiki.xentax.com/index.php/WAD_BFWD, 
     * and Rhys: https://github.com/RHY3756547/FreeSO/blob/master/TSOClient/tso.files/FAR3/Decompresser.cs.
     */
    public class BFWDMemoryStream : MemoryStream
    {
        public BFWDMemoryStream(byte[] buffer) : base(buffer) { }

        public byte[] ReadBytes(int length, bool bigEndian = false)
        {
            byte[] bytes = new byte[length];
            Read(bytes, 0, length);

            if (bigEndian) Array.Reverse(bytes);
            return bytes;
        }

        public char[] ReadChars(int length, bool bigEndian = false)
        {
            return Encoding.ASCII.GetChars(ReadBytes(length, bigEndian));
        }

        public string ReadString(int length, bool bigEndian = false)
        {
            return Encoding.ASCII.GetString(ReadBytes(length, bigEndian));
        }

        public int ReadInt32(bool bigEndian = false)
        {
            return BitConverter.ToInt32(ReadBytes(4, bigEndian), 0);
        }

        public uint ReadUInt32(bool bigEndian = false)
        {
            return BitConverter.ToUInt32(ReadBytes(4, bigEndian), 0);
        }

        public uint ReadUIntN(int n, bool bigEndian = false)
        {
            return BitConverter.ToUInt32(ReadBytes(n, bigEndian), 0);
        }
    }

    public static class BoolArrayExtension
    {
        public static bool ValuesEqual(this bool[] a, bool[] b)
        {
            for (int i = 0; i < Math.Min(a.Length, b.Length); ++i)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }
    }

    public class NotBFWDException : Exception
    {
        public NotBFWDException(string message) : base(message) { }
    }

    public class NotRefpackException : Exception
    {
        public NotRefpackException(string message) : base(message) { }
    }

    public class RefpackCorruptException : Exception
    {
        public RefpackCorruptException(string message) : base(message) { }
    }

    public class BFWDArchive : IDisposable
    {
        public byte[] buffer { get; internal set; }
        private BFWDMemoryStream _memoryStream;
        private int _version;

        public List<BFWDFile> files { get; }

        public BFWDArchive(string path)
        {
            // Set up read buffer
            var tempStreamReader = new StreamReader(path);
            var fileLength = (int)tempStreamReader.BaseStream.Length;
            buffer = new byte[fileLength];
            tempStreamReader.BaseStream.Read(buffer, 0, fileLength);
            tempStreamReader.Close();

            _memoryStream = new BFWDMemoryStream(buffer);
            files = new List<BFWDFile>();

            _ReadArchive();
        }

        public void Dispose()
        {
            _memoryStream.Dispose();
        }

        private void _ReadArchive()
        {
            _memoryStream.Seek(0, SeekOrigin.Begin);
            // Header
            /*4 - magic number ('BFWD')
              4 - version
              64 - padding??
              4 - file count
              4 - file list offset
              4 - file list length
              4 - null*/

            var magicNumber = _memoryStream.ReadString(4);
            if (magicNumber != "BFWD") throw new NotBFWDException($"Magic number did not match: {magicNumber} vs BFWD");
            _version = _memoryStream.ReadInt32();
            _memoryStream.Seek(64, SeekOrigin.Current); // Skip padding
            var fileCount = _memoryStream.ReadInt32();

            var fileDirectoryOffset = _memoryStream.ReadInt32();
            var fileDirectoryLength = _memoryStream.ReadInt32();

            _memoryStream.Seek(4, SeekOrigin.Current); // Skip 'null'

            // Details directory
            // See BFWDFile for more info
            for (int i = 0; i < fileCount; ++i)
            {
                GC.Collect();
                // Save the current position so that we can go back to it later
                var initialPos = _memoryStream.Position;

                BFWDFile newFile = new BFWDFile();
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
    }

    public interface IRefpackCommand
    {
        bool stopAfterFound { get; }
        int length { get; }
        void Decompress(byte[] data, ref List<byte> decompressedData, int offset, out uint skipAhead);
        bool OpcodeMatches(byte firstByte);
    }

    public static class RefpackUtils
    {
        public static void DecompressData(byte[] data, ref List<byte> outputData, int offset, int opcodeLength, uint proceedingDataLength, uint referencedDataLength, uint referencedDataDistance)
        {

            for (int i = 0; i < proceedingDataLength; ++i) // Proceeding data comes from the source buffer (compressed data)
            {
                uint pos = (uint)(offset + opcodeLength + i);
                if (pos < 0 || pos >= data.Length) break;  // Prevent any overflowing
                outputData.Add(data[pos]);
            }

            int outputDataLen = outputData.Count;
            for (int i = 0; i < referencedDataLength; ++i) // Referenced data comes from the output buffer (decompressed data)
            {
                int pos = (int)(outputDataLen - referencedDataDistance);
                if (pos < 0 || pos >= outputData.Count) break; // Prevent any overflowing
                outputData.Add(outputData[pos + i]);
            }
        }
    }

    public class OneByteCommand : IRefpackCommand
    {
        public int length => 1;
        public bool stopAfterFound => false;

        public void Decompress(byte[] data, ref List<byte> decompressedData, int offset, out uint skipAhead)
        {
            var dataAtOffset = data[offset];
            uint proceedingDataLength = (uint)(((dataAtOffset & 0x1F) + 1) << 2);

            skipAhead = proceedingDataLength;
            RefpackUtils.DecompressData(data, ref decompressedData, offset, length, proceedingDataLength, 0, 0);
        }
        public bool OpcodeMatches(byte firstByte) => ((firstByte & 0x1F) + 1) << 2 <= 0x70 && firstByte.GetBits(0, 1, 2).ValuesEqual(new[] { true, true, true });
    }

    public class StopCommand : IRefpackCommand
    {
        public int length => 1;
        public bool stopAfterFound => true;
        public void Decompress(byte[] data, ref List<byte> decompressedData, int offset, out uint skipAhead)
        {
            uint proceedingDataLength = (uint)((data[offset] & 0x03));
            skipAhead = proceedingDataLength;
            RefpackUtils.DecompressData(data, ref decompressedData, offset, length, proceedingDataLength, 0, 0);
        }
        public bool OpcodeMatches(byte firstByte) => ((firstByte & 0x1F) + 1) << 2 > 0x70 && firstByte.GetBits(0, 1, 2).ValuesEqual(new[] { true, true, true });
    }


    public class TwoByteCommand : IRefpackCommand
    {
        public int length => 2;
        public bool stopAfterFound => false;
        public void Decompress(byte[] data, ref List<byte> decompressedData, int offset, out uint skipAhead)
        {
            uint proceedingDataLength = (uint)((data[offset] & 0x03));
            uint referencedDataLength = (uint)(((data[offset] & 0x1C) >> 2) + 3);
            uint referencedDataDistance = (uint)(((data[offset] & 0x60) << 3) + data[offset + 1] + 1);
            skipAhead = proceedingDataLength;

            RefpackUtils.DecompressData(data, ref decompressedData, offset, length, proceedingDataLength, referencedDataLength, referencedDataDistance);
        }
        public bool OpcodeMatches(byte firstByte) => !firstByte.GetBit(0);
    }

    public class ThreeByteCommand : IRefpackCommand
    {
        public int length => 3;
        public bool stopAfterFound => false;
        public void Decompress(byte[] data, ref List<byte> decompressedData, int offset, out uint skipAhead)
        {
            uint proceedingDataLength = (uint)((data[offset + 1] & 0xC0) >> 6);
            uint referencedDataLength = (uint)((data[offset] & 0x3F) + 4);
            uint referencedDataDistance = (uint)(((data[offset + 1] & 0x3F) << 8) + data[offset + 2] + 1);
            skipAhead = proceedingDataLength;

            RefpackUtils.DecompressData(data, ref decompressedData, offset, length, proceedingDataLength, referencedDataLength, referencedDataDistance);
        }
        public bool OpcodeMatches(byte firstByte) => firstByte.GetBits(0, 1).ValuesEqual(new[] { true, false });
    }

    public class FourByteCommand : IRefpackCommand
    {
        public int length => 4;
        public bool stopAfterFound => false;
        public void Decompress(byte[] data, ref List<byte> decompressedData, int offset, out uint skipAhead)
        {
            uint proceedingDataLength = (uint)((data[offset] & 0x03));
            uint referencedDataLength = (uint)(((data[offset] & 0x0C) << 6) + data[offset + 3] + 5);
            uint referencedDataDistance = (uint)(((data[offset] & 0x10) << 12) + (data[offset + 1] << 8) + data[offset + 2] + 1);
            skipAhead = proceedingDataLength;

            RefpackUtils.DecompressData(data, ref decompressedData, offset, length, proceedingDataLength, referencedDataLength, referencedDataDistance);
        }
        public bool OpcodeMatches(byte firstByte) => firstByte.GetBits(0, 1, 2).ValuesEqual(new[] { true, true, false });
    }

    public class BFWDFile
    {
        /*for each file (40 bytes per entry)
        4 - unused
        4 - filename offset
        4 - filename length
        4 - data offset
        4 - file length
        4 - compression type s('4' for refpack)
        4 - decompressed size
        12 - null*/

        private byte[] _data;
        private bool _hasBeenDecompressed;

        public string name { get; set; }
        public bool compressed { get; set; }
        public uint decompressedSize { get; set; }
        public BFWDArchive parentArchive { get; set; }
        public int archiveOffset { get; set; }
        public byte[] data
        {
            get
            {
                // Refpack is big-endian, unlike the rest of the BFWD format
                // Therefore all memorystream operations have bigEndian set to true
                if (compressed && !_hasBeenDecompressed)
                {
                    List<byte> decompressedData = new List<byte>();
                    using (var memoryStream = new BFWDMemoryStream(_data))
                    {
                        var refpackHeader = memoryStream.ReadBytes(2, bigEndian: true);
                        if (refpackHeader[0] != 0xFB || refpackHeader[1] != 0x10) // 0x10: LU01000C - 00010000 - large files & compressed size are not supported.
                        {
                            throw new NotRefpackException("Data was not compressed using refpack (header does not match) - possibly corrupted?");
                        }

                        memoryStream.Seek(3, SeekOrigin.Current); // Skip decompressed size

                        byte[] currentByte = memoryStream.ReadBytes(1, bigEndian: true);

                        var commands = new List<IRefpackCommand>();
                        var commandCount = new Dictionary<Type, int>();

                        foreach (var class_ in Assembly.GetExecutingAssembly().GetTypes())
                        {
                            if (class_.GetInterfaces().Contains(typeof(IRefpackCommand)))
                            {
                                commands.Add((IRefpackCommand)Activator.CreateInstance(class_));
                            }
                        }

                        while (memoryStream.Position < _data.Length)
                        {
                            foreach (var command in commands)
                            {
                                if (command.OpcodeMatches(currentByte[0]))
                                {
                                    var commandType = command.GetType();
                                    if (commandCount.ContainsKey(commandType))
                                        commandCount[commandType]++;
                                    else
                                        commandCount.Add(commandType, 1);
                                    try
                                    {
                                        command.Decompress(_data, ref decompressedData, (int)memoryStream.Position - 1, out uint skipAhead);
                                        memoryStream.Seek(command.length + skipAhead - 1, SeekOrigin.Current);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"oh fuck:\n{ex}");
                                    }


                                    if (command.stopAfterFound) memoryStream.Seek(_data.Length, SeekOrigin.Current); // stop
                                }
                            }
                            currentByte = memoryStream.ReadBytes(1, bigEndian: true);
                        }
                    }

                    // Avoid decompressing after we've already done it once! Store the result in case its used later.
                    _hasBeenDecompressed = true;
                    _data = decompressedData.ToArray();
                }
                return _data;
            }
            set
            {
                _data = value;
            }
        }
    }
}
