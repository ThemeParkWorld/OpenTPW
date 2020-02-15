namespace OpenTPW.Files.FileFormats
{
    public static class ByteExtension
    {
        private static int _GetNibble(this byte byte_, bool high)
        {
            return (byte_ >> (high ? 0 : 4) & 0x0F);
        }

        public static int GetHighNibble(this byte byte_)
        {
            return byte_._GetNibble(true);
        }

        public static int GetLowNibble(this byte byte_)
        {
            return byte_._GetNibble(false);
        }
        public static int GetBitsAsInt(this byte byte_, int pos, int length)
        {
            return ((byte_ >> (pos)) & (length));
        }
        public static bool GetBit(this byte byte_, int index)
        {
            var value = ((byte_ >> (index)) & 1) != 0;
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
}
