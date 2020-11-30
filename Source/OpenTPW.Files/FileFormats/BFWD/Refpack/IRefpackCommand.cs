using System.Collections.Generic;

namespace OpenTPW.Files.FileFormats.BFWD.Refpack
{
    public interface IRefpackCommand
    {
        bool stopAfterFound { get; }
        int length { get; }
        void Decompress(byte[] data, ref List<byte> decompressedData, int offset, out uint skipAhead);
        bool OpcodeMatches(byte firstByte);
    }

}
