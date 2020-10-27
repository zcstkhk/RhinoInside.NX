using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Extensions.NX
{
    public static class Vector3dEx
    {
        /// <summary>
        /// Arbitrary Axis Algorithm
        /// <para>Given a vector to be used as the Z axis of a coordinate system, this algorithm generates a corresponding X axis for the coordinate system.</para>
        /// <para>The Y axis follows by application of the right-hand rule.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tolerance"></param>
        /// <returns>X axis of the corresponding coordinate system</returns>
        public static Vector3d PerpVector(this Vector3d value, double tolerance = 1e-9)
        {
            var length = value.GetLength();
            if (length < tolerance)
                return Zero;

            var normal = new Vector3d(value.X / length, value.Y / length, value.Z / length);

            if (Zero.IsAlmostEqualTo(new Vector3d(normal.X, normal.Y, 0.0), tolerance))
                return new Vector3d(value.Z, 0.0, -value.X);
            else
                return new Vector3d(-value.Y, value.X, 0.0);
        }

        /// <summary>
        /// Checks if the the given two vectors are perpendicular
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="tolerance"></param>
        /// <returns>true if <paramref name="a"/> and <paramref name="b"/> are perpendicular</returns>
        public static bool IsPerpendicularTo(this Vector3d a, Vector3d b, double tolerance = 1e-9)
        {
            var A = a.Normalize();
            var B = b.Normalize();

            return A.DotProduct(B) < tolerance;
        }

        /// <summary>
        /// Checks if the the given two vectors are codirectional
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="tolerance"></param>
        /// <returns>true if <paramref name="a"/> and <paramref name="b"/> are codirectional</returns>
        public static bool IsCodirectionalTo(this Vector3d a, Vector3d b, double tolerance = 1e-9)
        {
            var A = a.Normalize();
            var B = b.Normalize();

            return A.IsAlmostEqualTo(B, tolerance);
        }

        /// <summary>
        /// Checks if the the given two vectors are parallel
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="tolerance"></param>
        /// <returns>true if <paramref name="a"/> and <paramref name="b"/> are parallel</returns>
        public static bool IsParallelTo(this Vector3d a, Vector3d b, double tolerance = 1e-9)
        {
            var A = a.Normalize();
            var B = b.Normalize();

            return A.IsAlmostEqualTo(A.DotProduct(B) < 0.0 ? B.Negate() : B, tolerance);
        }

        /// <summary>Returns a new Vector3d whose coordinates are the normalized values from this vector.</summary>
        /// <remarks> 
        /// Normalized indicates that the length of this vector equals one (a unit vector).
        /// </remarks>
        /// <param name="vector"></param>
        /// <returns>The normalized Vector3d or zero if the vector is almost Zero.</returns>
        public static Vector3d Normalize(this Vector3d vector)
        {
            if (vector.DotProduct(vector) < 1E-18)
            {
                return new Vector3d(0.0, 0.0, 0.0);
            }
            return vector.Divide(vector.GetLength());
        }

        public static double DotProduct(this Vector3d vector1, Vector3d vector2) => vector1.Y * vector2.Y + vector1.X * vector2.X + vector1.Z * vector2.Z;

        public static double GetLength(this Vector3d vector) => Math.Sqrt(vector.Y * vector.Y + vector.X * vector.X + vector.Z * vector.Z);

        /// <summary>
        /// Divides this vector by the specified value and returns the result.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3d Divide(this Vector3d vector, double value) => new Vector3d(vector.X / value, vector.Y / value, vector.Z / value);

        /// <summary>
        /// Determines whether this vector and the specified vector are the same within the tolerance (1.0e-09).
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsAlmostEqualTo(this Vector3d vector1, Vector3d vector2, double tolerance = 1E-9)
        {
            double num = tolerance * tolerance;
            double num2 = vector2.DotProduct(vector2) + vector1.DotProduct(vector1);
            if (num2 < num)
                return true;

            Vector3d xyzproxy = vector1.Subtract(vector2);
            return ((xyzproxy.DotProduct(xyzproxy) >= num2 * num) ? 0 : 1) != 0;
        }

        /// <summary>Subtracts the two specified vectors and returns the result.</summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The vector equal to the difference between the two source vectors.</returns>
        /// <remarks>
        /// The subtracted vector is obtained by subtracting each coordinate of 
        /// the right vector from the corresponding coordinate of the left vector.
        /// </remarks>
        public static Vector3d Subtract(this Vector3d vector1, Vector3d vector2) => new Vector3d(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z);

        /// <summary>Negates this vector.</summary>
        /// <returns>The vector opposite to this vector.</returns>
        /// <remarks>
        /// The negated vector is obtained by changing the sign of each coordinate 
        /// of this vector.
        /// </remarks>
        public static Vector3d Negate(this Vector3d vector) => new Vector3d(-vector.X, -vector.Y, -vector.Z);

        public static readonly Vector3d AxisX = new Vector3d(1.0, 0.0, 0.0);

        public static readonly Vector3d AxisY = new Vector3d(0.0, 1.0, 0.0);

        public static readonly Vector3d AxisZ = new Vector3d(0.0, 0.0, 1.0);

        public static readonly Vector3d Zero = new Vector3d(0.0, 0.0, 0.0);

        public static readonly Vector3d AxisNegativeX = new Vector3d(-1.0, 0.0, 0.0);

        public static readonly Vector3d AxisNegativeY = new Vector3d(0.0, -1.0, 0.0);

        public static readonly Vector3d AxisNegativeZ = new Vector3d(0.0, 0.0, -1.0);

    }
}
