using System.Collections.Generic;

namespace OpenTPW.Files.FileFormats.BFWD.Refpack
{
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
}
