using OpenTPW.Files.BFWD.Refpack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OpenTPW.Files.BFWD
{
    public class ArchiveFile
    {
        /*for each file (40 bytes per entry)
        4 - unused
        4 - filename offset
        4 - filename length
        4 - data offset
        4 - file length
        4 - compression type ('4' for refpack)
        4 - decompressed size
        12 - null*/

        private byte[] _data;
        private bool _hasBeenDecompressed;

        public string name { get; set; }
        public bool compressed { get; set; }
        public uint decompressedSize { get; set; }
        public IArchive parentArchive { get; set; }
        public int archiveOffset { get; set; }
        public byte[] data
        {
            get
            {
                // Refpack is big-endian, unlike the rest of the DWFB format
                // Therefore all memorystream operations have bigEndian set to true
                if (compressed && !_hasBeenDecompressed)
                {
                    List<byte> decompressedData = new List<byte>();
                    using (var memoryStream = new BFWDMemoryStream(_data))
                    {
                        var refpackHeader = memoryStream.ReadBytes(2, bigEndian: true);
                        if (refpackHeader[0] != 0xFB || refpackHeader[1] != 0x10) // 0x10: LU01000C - 00010000 - large files & compressed size are not supported.
                        {
                            throw new Exception("Data was not compressed using refpack (header does not match) - possibly corrupted?");
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
