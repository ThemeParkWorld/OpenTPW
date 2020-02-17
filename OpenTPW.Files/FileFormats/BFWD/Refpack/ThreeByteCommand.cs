using System.Collections.Generic;

namespace OpenTPW.Files.FileFormats.BFWD.Refpack
{
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
}
