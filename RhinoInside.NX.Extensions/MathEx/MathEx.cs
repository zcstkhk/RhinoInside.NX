using System;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
    /// <summary>Mostly trigonometric functions that handle angles in degrees, rather than radians</summary>
    public static class MathEx
    {
        /// <summary>Converts degrees to radians</summary>
        /// <param name="angle">An angle measured in degrees</param>
        /// <returns>The same angle measured in radians</returns>
        // Token: 0x060002D8 RID: 728 RVA: 0x00030091 File Offset: 0x0002E291
        public static double DegreesToRadians(double angle)
        {
            return angle * 3.1415926535897931 / 180.0;
        }

        /// <summary>Converts radians to degrees</summary>
        /// <param name="angle">An angle measured in radians</param>
        /// <returns>The same angle measured in degrees</returns>
        // Token: 0x060002D9 RID: 729 RVA: 0x000300A8 File Offset: 0x0002E2A8
        public static double RadiansToDegrees(double angle)
        {
            return angle * 180.0 / 3.1415926535897931;
        }

        /// <summary>
        /// Calculates the sine of an angle given in degrees
        /// </summary>
        /// <param name="angle">The angle, in degrees</param>
        /// <returns>The sine of the given angle</returns>
        /// <remarks>
        /// To calculate the sine of an angle given in radians, use <see cref="M:System.Math.Sin(System.Double)">System.Math.Sin</see>
        /// </remarks>      
        // Token: 0x060002DA RID: 730 RVA: 0x000300BF File Offset: 0x0002E2BF
        public static double SinD(double angle)
        {
            return System.Math.Sin(angle * 3.1415926535897931 / 180.0);
        }

        /// <summary>
        /// Calculates the cosine of an angle given in degrees
        /// </summary>
        /// <param name="angle">The angle, in degrees</param>
        /// <returns>The cosine of the given angle</returns>
        /// <remarks>
        /// To calculate the cosine of an angle given in radians, use <see cref="M:System.Math.Cos(System.Double)">System.Math.Cos</see>
        /// </remarks>      
        // Token: 0x060002DB RID: 731 RVA: 0x000300DB File Offset: 0x0002E2DB
        public static double CosD(double angle)
        {
            return System.Math.Cos(angle * 3.1415926535897931 / 180.0);
        }

        /// <summary>
        /// Calculates the tangent of an angle given in degrees
        /// </summary>
        /// <param name="angle">The angle, in degrees</param>
        /// <returns>The tangent of the given angle</returns>
        /// <remarks>
        /// To calculate the tangent of an angle given in radians, use <see cref="M:System.Math.Tan(System.Double)">System.Math.Tan</see>
        /// </remarks>      
        // Token: 0x060002DC RID: 732 RVA: 0x000300F7 File Offset: 0x0002E2F7
        public static double TanD(double angle)
        {
            return System.Math.Tan(angle * 3.1415926535897931 / 180.0);
        }

        /// <summary>
        /// Calculates the arcsine (in degrees) of a given number
        /// </summary>
        /// <param name="x">The given number, which must be in the range -1 ≤ x ≤ 1</param>
        /// <returns>An angle, theta, such that sin(theta) = x, and 0 ≤ theta ≤ 180</returns>
        /// <remarks>
        /// To calculate the arcsine in radians of a given number, use <see cref="M:System.Math.Asin(System.Double)">System.Math.Asin</see>
        /// <para>
        /// If x &lt; -1 or x &gt; 1, then the returned value is NaN (Not A Number)</para>
        /// </remarks>
        public static double AsinD(double x)
        {
            return System.Math.Asin(x) * 180.0 / 3.1415926535897931;
        }

        /// <summary>
        /// Calculates the arccosine (in degrees) of a given number
        /// </summary>
        /// <param name="x">The given number, which must be in the range -1 ≤ x ≤ 1</param>
        /// <returns>An angle, theta, such that cos(theta) = x, and 0 ≤ theta ≤ 180</returns>
        /// <remarks>
        /// To calculate the arccosine in radians of a given number, use <see cref="M:System.Math.Acos(System.Double)">System.Math.Acos</see>
        /// <para>
        /// If x &lt; -1 or x &gt; 1, then the returned value is NaN (Not A Number)</para>
        /// </remarks>      
        // Token: 0x060002DE RID: 734 RVA: 0x0003012F File Offset: 0x0002E32F
        public static double AcosD(double x)
        {
            return System.Math.Acos(x) * 180.0 / 3.1415926535897931;
        }

        /// <summary>
        /// Calculates the arctangent (in degrees) of a given number
        /// </summary>
        /// <param name="x">The given number, which must be in the range -1 ≤ x ≤ 1</param>
        /// <returns>An angle, theta, such that tan(theta) = x, and 0 ≤ theta ≤ 180</returns>
        /// <remarks>
        /// To calculate the arctangent in radians of a given number, use <see cref="M:System.Math.Atan(System.Double)">System.Math.Atan</see>
        /// </remarks>      
        // Token: 0x060002DF RID: 735 RVA: 0x0003014B File Offset: 0x0002E34B
        public static double AtanD(double x)
        {
            return System.Math.Atan(x) * 180.0 / 3.1415926535897931;
        }

        /// <summary>
        /// Calculates the arctangent (in degrees) of a ratio of two given numbers
        /// </summary>
        /// <param name="y">The first given number (which can be regarded as a y-coordinate in the plane)</param>
        /// <param name="x">The second given number (which can be regarded as an x-coordinate in the plane)</param>
        /// <returns>An angle, theta, such that tan(theta) = y/x, sign(theta) = sign(y), and -180 ≤ theta ≤ 180</returns>
        /// <remarks>
        /// To obtain the same result in radians, use <see cref="M:System.Math.Atan2(System.Double,System.Double)">System.Math.Atan2</see>.
        /// <para>
        /// For limiting cases where either x or y is zero, please see the standard documentation for <see cref="M:System.Math.Atan2(System.Double,System.Double)">System.Math.Atan2</see>.
        /// </para>
        /// </remarks>      
        // Token: 0x060002E0 RID: 736 RVA: 0x00030167 File Offset: 0x0002E367
        public static double Atan2D(double y, double x)
        {
            return System.Math.Atan2(y, x) * 180.0 / 3.1415926535897931;
        }

        /// <summary>Find the index of the maximum element in an array</summary>
        /// <param name="values">The array of values</param>
        /// <returns>The index of the largest value in the array</returns> 
        /// <remarks>
        /// Note that "largest" means closest to plus-infinity, not furthest from zero.
        /// <para>
        /// So, MaxIndex(-5, 1, 3) is 2, not 0.
        /// </para>
        /// </remarks>
        // Token: 0x060002E1 RID: 737 RVA: 0x00030184 File Offset: 0x0002E384
        public static int MaxIndex(params double[] values)
        {
            int num = 0;
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] > values[num])
                {
                    num = i;
                }
            }
            return num;
        }

        /// <summary>Find the maximum element in an array</summary>
        /// <param name="values">The array of values</param>
        /// <returns>The largest value in the array</returns>      
        /// <remarks>
        /// Note that "largest" means closest to plus-infinity, not furthest from zero.
        /// <para>
        /// So, Max(-5, 1, 3) is 3, not 5.
        /// </para>
        /// </remarks>
        // Token: 0x060002E2 RID: 738 RVA: 0x000301AC File Offset: 0x0002E3AC
        public static double Max(params double[] values)
        {
            return values[MathEx.MaxIndex(values)];
        }

        /// <summary>Find the index of the minimum element in an array</summary>
        /// <param name="values">The array of values</param>
        /// <returns>The index of the smallest value in the array</returns>      
        /// <remarks>
        /// Note that "smallest" means closest to minus-infinity, not closest to zero.
        /// <para>
        /// So, MinIndex(-5, 1, 3) is 0, not 1.
        /// </para>
        /// </remarks>
        // Token: 0x060002E3 RID: 739 RVA: 0x000301B8 File Offset: 0x0002E3B8
        public static int MinIndex(params double[] values)
        {
            int num = 0;
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] < values[num])
                {
                    num = i;
                }
            }
            return num;
        }

        /// <summary>Find the minimum element in an array</summary>
        /// <param name="values">The array of values</param>
        /// <returns>The smallest value in the array</returns>
        /// <remarks>
        /// Note that "smallest" means closest to minus-infinity, not closest to zero.
        /// <para>
        /// So, Min(-5, 1, 3) is -5, not 1.
        /// </para>
        /// </remarks>
        public static double Min(params double[] values)
        {
            return values[MathEx.MinIndex(values)];
        }

        /// <summary>Find the mean (average) of an array of values</summary>
        /// <param name="values">The array of values</param>
        /// <returns>The mean of the values in the array</returns>      
        // Token: 0x060002E5 RID: 741 RVA: 0x000301EA File Offset: 0x0002E3EA
        public static double Mean(params double[] values)
        {
            return MathEx.Sum(values) / (double)values.Length;
        }

        /// <summary>Find the sum of an array of values</summary>
        /// <param name="values">The array of values</param>
        /// <returns>The sum of the values in the array</returns>          
        // Token: 0x060002E6 RID: 742 RVA: 0x000301F8 File Offset: 0x0002E3F8
        public static double Sum(params double[] values)
        {
            double num = 0.0;
            foreach (double num2 in values)
            {
                num += num2;
            }
            return num;
        }

        /// <summary>Provides simple manipulations of general matrices and vectors</summary>

        // Token: 0x0200003F RID: 63
        // (Invoke) Token: 0x0600031D RID: 797
        public delegate double DoubleFunction(object data, double x);


    }
}
