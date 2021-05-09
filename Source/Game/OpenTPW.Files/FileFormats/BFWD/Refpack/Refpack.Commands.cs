using System.Collections.Generic;

namespace OpenTPW.Files.FileFormats.BFWD.Refpack
{
    public class FourByteCommand : IRefpackCommand
    {
        public int length => 4;
        public bool stopAfterFound => false;
        public void Decompress(byte[] data, ref List<byte> decompressedData, int offset, out uint skipAhead)
        {
            var proceedingDataLength = (uint)((data[offset] & 0x03));
            var referencedDataLength = (uint)(((data[offset] & 0x0C) << 6) + data[offset + 3] + 5);
            var referencedDataDistance = (uint)(((data[offset] & 0x10) << 12) + (data[offset + 1] << 8) + data[offset + 2] + 1);
            skipAhead = proceedingDataLength;

            Refpack.DecompressData(data, ref decompressedData, offset, length, proceedingDataLength, referencedDataLength, referencedDataDistance);
        }
        public bool OpcodeMatches(byte firstByte) => firstByte.GetBits(0, 1, 2).ValuesEqual(new[] { true, true, false });
    }
    
    public class ThreeByteCommand : IRefpackCommand
    {
        public int length => 3;
        public bool stopAfterFound => false;
        public void Decompress(byte[] data, ref List<byte> decompressedData, int offset, out uint skipAhead)
        {
            var proceedingDataLength = (uint)((data[offset + 1] & 0xC0) >> 6);
            var referencedDataLength = (uint)((data[offset] & 0x3F) + 4);
            var referencedDataDistance = (uint)(((data[offset + 1] & 0x3F) << 8) + data[offset + 2] + 1);
            skipAhead = proceedingDataLength;

            Refpack.DecompressData(data, ref decompressedData, offset, length, proceedingDataLength, referencedDataLength, referencedDataDistance);
        }
        public bool OpcodeMatches(byte firstByte) => firstByte.GetBits(0, 1).ValuesEqual(new[] { true, false });
    }
    
    public class TwoByteCommand : IRefpackCommand
    {
        public int length => 2;
        public bool stopAfterFound => false;
        public void Decompress(byte[] data, ref List<byte> decompressedData, int offset, out uint skipAhead)
        {
            var proceedingDataLength = (uint)((data[offset] & 0x03));
            var referencedDataLength = (uint)(((data[offset] & 0x1C) >> 2) + 3);
            var referencedDataDistance = (uint)(((data[offset] & 0x60) << 3) + data[offset + 1] + 1);
            skipAhead = proceedingDataLength;

            Refpack.DecompressData(data, ref decompressedData, offset, length, proceedingDataLength, referencedDataLength, referencedDataDistance);
        }
        public bool OpcodeMatches(byte firstByte) => !firstByte.GetBit(0);
    }
    
    public class OneByteCommand : IRefpackCommand
    {
        public int length => 1;
        public bool stopAfterFound => false;

        public void Decompress(byte[] data, ref List<byte> decompressedData, int offset, out uint skipAhead)
        {
            var dataAtOffset = data[offset];
            var proceedingDataLength = (uint)(((dataAtOffset & 0x1F) + 1) << 2);

            skipAhead = proceedingDataLength;
            Refpack.DecompressData(data, ref decompressedData, offset, length, proceedingDataLength, 0, 0);
        }
        public bool OpcodeMatches(byte firstByte) => ((firstByte & 0x1F) + 1) << 2 <= 0x70 && firstByte.GetBits(0, 1, 2).ValuesEqual(new[] { true, true, true });
    }
    
    public class StopCommand : IRefpackCommand
    {
        public int length => 1;
        public bool stopAfterFound => true;
        public void Decompress(byte[] data, ref List<byte> decompressedData, int offset, out uint skipAhead)
        {
            var proceedingDataLength = (uint)((data[offset] & 0x03));
            skipAhead = proceedingDataLength;
            Refpack.DecompressData(data, ref decompressedData, offset, length, proceedingDataLength, 0, 0);
        }
        public bool OpcodeMatches(byte firstByte) => ((firstByte & 0x1F) + 1) << 2 > 0x70 && firstByte.GetBits(0, 1, 2).ValuesEqual(new[] { true, true, true });
    }
    
    public interface IRefpackCommand
    {
        bool stopAfterFound { get; }
        int length { get; }
        void Decompress(byte[] data, ref List<byte> decompressedData, int offset, out uint skipAhead);
        bool OpcodeMatches(byte firstByte);
    }
}