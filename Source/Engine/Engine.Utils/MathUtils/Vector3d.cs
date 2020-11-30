using System;

namespace Engine.Utils.MathUtils
{
    public struct Vector3d
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
        /// The point at which the vector resides on the Z axis
        /// </summary>
        public double z;

        public double Magnitude => Math.Sqrt(SqrMagnitude);
        public double SqrMagnitude => x * x + y * y + z * z;

        public Vector3d Normalized
        {
            get
            {
                if (Math.Abs(Magnitude) < 0.0001f)
                    return new Vector3d(0, 0, 0);
                return this / Magnitude;
            }
        }

        public void Normalize() => this = Normalized;

        /// <summary>
        /// Construct a <see cref="Vector3d"/> with three initial values.
        /// </summary>
        /// <param name="x">The initial x coordinate</param>
        /// <param name="y">The initial y coordinate</param>
        /// <param name="z">The initial z coordinate</param>
        public Vector3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3d operator *(Vector3d a, Vector3d b) => new Vector3d(a.x * b.x,
                                                                                a.y * b.y,
                                                                                a.z * b.z);
        public static Vector3d operator -(Vector3d a, Vector3d b) => new Vector3d(a.x - b.x,
                                                                                a.y - b.y,
                                                                                a.z - b.z);
        public static Vector3d operator /(Vector3d a, Vector3d b) => new Vector3d(a.x / b.x,
                                                                                a.y / b.y,
                                                                                a.z / b.z);
        public static Vector3d operator +(Vector3d a, Vector3d b) => new Vector3d(a.x + b.x,
                                                                                a.y + b.y,
                                                                                a.z + b.z);

        public static Vector3d operator *(Vector3d a, double b) => new Vector3d(a.x * b,
                                                                            a.y * b,
                                                                            a.z * b);
        public static Vector3d operator -(Vector3d a, double b) => new Vector3d(a.x - b,
                                                                            a.y - b,
                                                                            a.z - b);
        public static Vector3d operator /(Vector3d a, double b) => new Vector3d(a.x / b,
                                                                            a.y / b,
                                                                            a.z / b);

        public static Vector3d operator +(Vector3d a, double b) => new Vector3d(a.x + b,
                                                                            a.y + b,
                                                                            a.z + b);

        public static Vector3d operator %(Vector3d a, double b) => new Vector3d(a.x % b,
                                                                            a.y % b,
                                                                            a.z % b);

        public static Vector3d up = new Vector3d(0, 1, 0);
        public static Vector3d right = new Vector3d(1, 0, 0);
        public static Vector3d forward = new Vector3d(0, 0, 1);
        public static Vector3d one = new Vector3d(1, 1, 1);

        /// <summary>
        /// Get all values within the <see cref="Vector3d"/> as a string.
        /// </summary>
        /// <returns>All coordinates (<see cref="x"/>, <see cref="y"/> and <see cref="z"/>) concatenated as a string.</returns>
        public override string ToString()
        {
            return $"{x}, {y}, {z}";
        }

        public static Vector3d ConvertFromNumerics(System.Numerics.Vector3 numericsVector3d)
        {
            return new Vector3d(numericsVector3d.X, numericsVector3d.Y, numericsVector3d.Z);
        }

        public System.Numerics.Vector3 ConvertToNumerics()
        {
            return new System.Numerics.Vector3((float)x, (float)y, (float)z);
        }

        public Vector3f ToVector3f()
        {
            return new Vector3f((float)x, (float)y, (float)z);
        }
    }
}
