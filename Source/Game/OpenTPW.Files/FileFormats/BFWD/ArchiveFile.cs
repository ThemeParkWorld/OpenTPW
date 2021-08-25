using OpenTPW.Files.FileFormats.BFWD.Refpack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OpenTPW.Files.FileFormats.BFWD
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

        private byte[] compressedData;
        private bool hasBeenDecompressed;

        public string Name { get; set; }
        public bool Compressed { get; set; }
        public uint DecompressedSize { get; set; }
        public BFWDArchive ParentArchive { get; set; }
        public int ArchiveOffset { get; set; }
        public byte[] CompressedData
        {
            get
            {
                // Refpack is big-endian, unlike the rest of the DWFB format
                // Therefore all memorystream operations have bigEndian set to true
                if (Compressed && !hasBeenDecompressed)
                {
                    var decompressedData = new List<byte>();
                    using (var memoryStream = new BFWDMemoryStream(compressedData))
                    {
                        var refpackHeader = memoryStream.ReadBytes(2, bigEndian: true);
                        if (refpackHeader[0] != 0xFB || refpackHeader[1] != 0x10) // 0x10: LU01000C - 00010000 - large files & compressed size are not supported.
                        {
                            throw new Exception("Data was not compressed using refpack (header does not match) - possibly corrupted?");
                        }

                        memoryStream.Seek(3, SeekOrigin.Current); // Skip decompressed size

                        var currentByte = memoryStream.ReadBytes(1, bigEndian: true);

                        var commands = new List<IRefpackCommand>();
                        var commandCount = new Dictionary<Type, int>();

                        foreach (var class_ in Assembly.GetExecutingAssembly().GetTypes())
                        {
                            if (class_.GetInterfaces().Contains(typeof(IRefpackCommand)))
                            {
                                commands.Add((IRefpackCommand)Activator.CreateInstance(class_));
                            }
                        }

                        while (memoryStream.Position < compressedData.Length)
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
                                        command.Decompress(compressedData, ref decompressedData, (int)memoryStream.Position - 1, out var skipAhead);
                                        memoryStream.Seek(command.Length + skipAhead - 1, SeekOrigin.Current);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"oh fuck:\n{ex}");
                                    }


                                    if (command.StopAfterFound) memoryStream.Seek(compressedData.Length, SeekOrigin.Current); // stop
                                }
                            }
                            currentByte = memoryStream.ReadBytes(1, bigEndian: true);
                        }
                    }

                    // Avoid decompressing after we've already done it once! Store the result in case its used later.
                    hasBeenDecompressed = true;
                    compressedData = decompressedData.ToArray();
                }
                return compressedData;
            }
            set
            {
                compressedData = value;
            }
        }
    }
}
