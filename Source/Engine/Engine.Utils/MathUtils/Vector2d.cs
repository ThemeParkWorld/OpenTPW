namespace Engine.Utils.MathUtils
{
    public struct Vector2d
    {
        /// <summary>
        /// The point at which the vector resides on the X axis
        /// </summary>
        public double x;

        /// <summary>
        /// The point at which the vector resides on the Y axis
        /// </summary>
        public double y;

        /// <summary>
        /// Construct a <see cref="Vector2f"/> with three initial values.
        /// </summary>
        /// <param name="x">The initial x coordinate</param>
        /// <param name="y">The initial y coordinate</param>
        public Vector2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2d Multiply(Vector2d a, Vector2d b) => new Vector2d(a.x * b.x,
                                                                            a.y * b.y);
        public static Vector2d Subtract(Vector2d a, Vector2d b) => new Vector2d(a.x - b.x,
                                                                            a.y - b.y);
        public static Vector2d Divide(Vector2d a, Vector2d b) => new Vector2d(a.x / b.x,
                                                                            a.y / b.y);
        public static Vector2d Add(Vector2d a, Vector2d b) => new Vector2d(a.x + b.x,
                                                                            a.y + b.y);

        public static Vector2d operator *(Vector2d a, Vector2d b) => Multiply(a, b);
        public static Vector2d operator -(Vector2d a, Vector2d b) => Subtract(a, b);
        public static Vector2d operator /(Vector2d a, Vector2d b) => Divide(a, b);
        public static Vector2d operator +(Vector2d a, Vector2d b) => Add(a, b);

        public static Vector2d operator *(Vector2d a, double b) => new Vector2d(a.x * b,
                                                                            a.y * b);
        public static Vector2d operator -(Vector2d a, double b) => new Vector2d(a.x - b,
                                                                            a.y - b);
        public static Vector2d operator /(Vector2d a, double b) => new Vector2d(a.x / b,
                                                                            a.y / b);
        public static Vector2d operator +(Vector2d a, double b) => new Vector2d(a.x + b,
                                                                            a.y + b);
        /// <summary>
        /// Get all values within the <see cref="Vector2d"/> as a string.
        /// </summary>
        /// <returns>Both coordinates (<see cref="x"/> and <see cref="y"/>) concatenated as a string.</returns>
        public override string ToString()
        {
            return $"{x}, {y}";
        }
    }
}
