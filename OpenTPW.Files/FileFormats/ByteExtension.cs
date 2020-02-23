namespace OpenTPW.Files.FileFormats
{
    public static class ByteExtension
    {
        private static int GetNibble(this byte @byte, bool high)
        {
            return (@byte >> (high ? 0 : 4) & 0x0F);
        }

        public static int GetHighNibble(this byte @byte)
        {
            return @byte.GetNibble(true);
        }

        public static int GetLowNibble(this byte @byte)
        {
            return @byte.GetNibble(false);
        }
        public static int GetBitsAsInt(this byte @byte, int pos, int length)
        {
            return ((@byte >> (pos)) & (length));
        }
        public static bool GetBit(this byte @byte, int index)
        {
            var value = ((@byte >> (index)) & 1) != 0;
            return value;
        }
        public static bool[] GetBits(this byte @byte, params int[] indices)
        {
            bool[] bits = new bool[indices.Length];
            for (int i = 0; i < indices.Length; ++i)
            {
                bits[i] = @byte.GetBit(indices[i]);
            }
            return bits;
        }
    }
}
