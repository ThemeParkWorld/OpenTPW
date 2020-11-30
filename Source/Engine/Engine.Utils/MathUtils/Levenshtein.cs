using Engine.Utils.Attributes;
using System;

namespace Engine.Utils.MathUtils
{
    public static class Levenshtein
    {
        /// <summary>
        /// Compares two <see cref="string"/>s using the Levenshtein Distance algorithm in order to calculate their difference.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>The difference between the two <see cref="string"/>s</returns>
        /// 
        [ConsoleFunction("CalcLevenshteinDistance")]
        public static int CalcDistance(string a, string b)
        {
            int aLen = a.Length, bLen = b.Length;
            if (aLen == 0 || bLen == 0) return -1;
            int[,] matrix = new int[aLen + 1, bLen + 1];
            for (int i = 0; i <= aLen; ++i) matrix[i, 0] = i;
            for (int i = 0; i <= bLen; ++i) matrix[0, i] = i;
            for (int j = 1; j <= bLen; ++j)
            {
                for (int i = 1; i <= aLen; ++i)
                {
                    int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                    int val0 = matrix[i - 1, j] + 1;
                    int val1 = matrix[i, j - 1] + 1;
                    int val2 = matrix[i - 1, j - 1] + cost;
                    matrix[i, j] = Math.Min(val0, Math.Min(val1, val2));
                }
            }
            return matrix[aLen, bLen];
        }
    }
}
