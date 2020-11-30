using System.Collections.Generic;

namespace OpenTPW.Files.FileFormats.BFWD.Refpack
{
    public static class RefpackUtils
    {
        public static void DecompressData(byte[] data, ref List<byte> outputData, int offset, int opcodeLength, uint proceedingDataLength, uint referencedDataLength, uint referencedDataDistance)
        {

            for (var i = 0; i < proceedingDataLength; ++i) // Proceeding data comes from the source buffer (compressed data)
            {
                var pos = (uint)(offset + opcodeLength + i);
                if (pos < 0 || pos >= data.Length) break;  // Prevent any overflowing
                outputData.Add(data[pos]);
            }

            var outputDataLen = outputData.Count;
            for (var i = 0; i < referencedDataLength; ++i) // Referenced data comes from the output buffer (decompressed data)
            {
                var pos = (int)(outputDataLen - referencedDataDistance);
                if (pos < 0 || pos >= outputData.Count) break; // Prevent any overflowing
                outputData.Add(outputData[pos + i]);
            }
        }
    }
}
