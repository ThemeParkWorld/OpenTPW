using System;

namespace Engine.Utils.MathUtils
{
    public static class Noise
    {
        private static Vector2f[,] gradientValues = new Vector2f[512, 512];

        private static float Lerp(float a, float b, float t) => (1.0f - t) * a + (t * b);

        private static float DotGridGradient(int ix, int iy, float x, float y)
        {
            var dx = x - ix;
            var dy = y - iy;

            return (dx * gradientValues[iy, ix].x) + (dy * gradientValues[iy, ix].y);
        }

        private static void CalculateGradientValues(int seed)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (gradientValues[0, 0].x != 0f && gradientValues[0, 0].y != 0f) return;
            // ReSharper restore CompareOfFloatsByEqualityOperator

            var random = new Random(seed);
            for (var x = 0; x < gradientValues.GetLength(0); x++)
            {
                for (var y = 0; y < gradientValues.GetLength(1); y++)
                {
                    gradientValues[x, y] = new Vector2f(
                        ((float)random.NextDouble() * 2) - 1,
                        ((float)random.NextDouble() * 2) - 1
                        );
                }
            }
        }

        public static float PerlinNoise(int seed, float x, float y)
        {
            CalculateGradientValues(seed);

            var x0 = (int)x;
            var x1 = x0 + 1;
            var y0 = (int)y;
            var y1 = y0 + 1;

            var sx = x - x0;
            var sy = y - y0;

            float n0, n1, ix0, ix1;
            n0 = DotGridGradient(x0, y0, x, y);
            n1 = DotGridGradient(x1, y0, x, y);
            ix0 = Lerp(n0, n1, sx);

            n0 = DotGridGradient(x0, y1, x, y);
            n1 = DotGridGradient(x1, y1, x, y);
            ix1 = Lerp(n0, n1, sx);

            //Logging.Log($"Perlin noise for pos {x}, {y}\n" +
            //          $"x0 = {x0}, x1 = {x1}, sx = {sx}\n" +
            //          $"y0 = {y0}, y1 = {y1}, sy = {sy}\n" +
            //          $"ix0 = {ix0}, ix1 = {ix1}, sy = {sy}\n" +
            //          $"final val = {Lerp(ix0, ix1, sy)}");

            return Lerp(ix0, ix1, sy);
        }

        public static byte PerlinNoiseAsByte(int seed, float x, float y)
        {
            return (byte)(
                Math.Min(Math.Max(0, PerlinNoise(seed, x, y)), 1f) * 255
            );
        }
    }
}
