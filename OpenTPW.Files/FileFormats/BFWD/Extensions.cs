using System;

namespace OpenTPW.Files.BFWD
{
    public static class ByteExtension
    {
        public static bool GetBit(this byte byte_, int index)
        {
            var value = ((byte_ >> (7 - index)) & 1) != 0;
            return value;
        }
        public static bool[] GetBits(this byte byte_, params int[] indices)
        {
            bool[] bits = new bool[indices.Length];
            for (int i = 0; i < indices.Length; ++i)
            {
                bits[i] = byte_.GetBit(indices[i]);
            }
            return bits;
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
}
