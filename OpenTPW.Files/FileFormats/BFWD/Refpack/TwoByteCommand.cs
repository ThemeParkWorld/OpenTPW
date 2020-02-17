using System.Collections.Generic;

namespace OpenTPW.Files.FileFormats.BFWD.Refpack
{
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
}
