using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Extensions
{
    public static class SplineMath
    {
        /// <summary>Calculates nodes (parameter values for interpolation) based on chordal distances</summary>
        /// <param name="intPoints">Data points to be interpolated</param>
        /// <returns>Chordal distances, normalized to the range [0,1]</returns>
        // Token: 0x06000322 RID: 802 RVA: 0x00031E84 File Offset: 0x00030084
        public static double[] ChordalNodes(Point3d[] intPoints)
        {
            int num = intPoints.Length;
            double[] array = new double[num];
            array[0] = 0.0;
            for (int i = 0; i < num - 1; i++)
            {
                array[i + 1] = array[i] + (intPoints[i + 1].Subtract(intPoints[i])).GetLength();
            }
            double[] array2 = new double[num];
            for (int j = 0; j < num; j++)
            {
                array2[j] = array[j] / array[num - 1];
            }
            return array2;
        }

        /// <summary>Calculates nodes (parameter values for interpolation) based on the centripedal idea</summary>
        /// <param name="intPoints">Data points to be interpolated</param>
        /// <returns>Centripedal node values, normalized to the range [0,1]</returns>
        /// <remarks>
        /// Using centripedal nodes, rather than chordal ones, sometimes gives curves with a "nicer" shape.
        /// </remarks>
        // Token: 0x06000323 RID: 803 RVA: 0x00031F0C File Offset: 0x0003010C
        public static double[] CentripedalNodes(Point3d[] intPoints)
        {
            int num = intPoints.Length;
            double[] array = new double[num];
            array[0] = 0.0;
            for (int i = 0; i < num - 1; i++)
            {
                array[i + 1] = array[i] + Math.Sqrt((intPoints[i + 1].Subtract(intPoints[i])).GetLength());
            }
            double[] array2 = new double[num];
            for (int j = 0; j < num; j++)
            {
                array2[j] = array[j] / array[num - 1];
            }
            return array2;
        }

        // Token: 0x06000324 RID: 804 RVA: 0x00031F98 File Offset: 0x00030198
        private static double[] ChordalNodesU(Point3d[,] intPoints)
        {
            int length = intPoints.GetLength(0);
            int length2 = intPoints.GetLength(1);
            Point3d[] intPoints2 = new Point3d[length2];
            double[,] array = new double[length, length2];
            for (int i = 0; i < length; i++)
            {
                intPoints2 = MatrixMath.GetRow(intPoints, i);
                double[] array2 = SplineMath.ChordalNodes(intPoints2);
                for (int j = 0; j < length2; j++)
                {
                    array[i, j] = array2[j];
                }
            }
            double[] array3 = new double[length2];
            for (int k = 0; k < length2; k++)
            {
                array3[k] = MathEx.Mean(MatrixMath.GetColumn(array, k));
            }
            return array3;
        }

        // Token: 0x06000325 RID: 805 RVA: 0x00032030 File Offset: 0x00030230
        public static double[][] ChordalNodes(Point3d[,] intPoints)
        {
            double[] array = SplineMath.ChordalNodesU(intPoints);
            double[] array2 = SplineMath.ChordalNodesU(MatrixMath.Transpose(intPoints));
            return new double[][]
            {
                    array2,
                    array
            };
        }

        /// <summary>Calculates Greville knots based on given node values</summary>
        /// <param name="nodes">Node values</param>
        /// <param name="m">Degree of curve</param>
        /// <returns>Knot values</returns>
        /// <remarks>
        /// Greville knots give good numerical stability, and also guarantee that the
        /// Schoenberg-Whitney condition is satisfied, so interpolation will work.
        /// </remarks>
        // Token: 0x06000326 RID: 806 RVA: 0x00032064 File Offset: 0x00030264
        public static double[] GrevilleKnots(double[] nodes, int m)
        {
            int num = m + 1;
            int num2 = nodes.Length;
            double[] array = new double[num2 + num];
            for (int i = 0; i < num; i++)
            {
                array[i] = 0.0;
            }
            for (int j = num; j < num2; j++)
            {
                array[j] = SplineMath.Sum(nodes, j - m, j - 1) / (double)m;
            }
            for (int k = num2; k < num2 + num; k++)
            {
                array[k] = 1.0;
            }
            return array;
        }

        /// <summary>Builds Bezier knot sequence for degree m (m+1 zeros and m+1 1's)</summary>
        /// <param name="m">Degree</param>
        /// <returns>Array of knot values</returns>
        // Token: 0x06000327 RID: 807 RVA: 0x000320E0 File Offset: 0x000302E0
        public static double[] BezierKnots(int m)
        {
            double[] array = new double[2 * (m + 1)];
            for (int i = 0; i <= m; i++)
            {
                array[i] = 0.0;
            }
            for (int i = 0; i <= m; i++)
            {
                array[2 * m + 1 - i] = 1.0;
            }
            return array;
        }

        // Token: 0x06000328 RID: 808 RVA: 0x00032130 File Offset: 0x00030330
        private static double Sum(double[] tau, int i, int j)
        {
            double num = 0.0;
            for (int k = i; k <= j; k++)
            {
                num += tau[k];
            }
            return num;
        }

        /// <summary>B-spline interpolation (1D -- real-valued)</summary>
        /// <param name="intValues">The n values to interpolate, q[0],...,q[n-1]</param>
        /// <param name="nodes">The n parameter values at which to interpolate, tau[0],...,tau[n-1]</param>
        /// <param name="knots">Knot sequence : n-k values t[0], ... , t[n+k-1]</param>
        /// <returns>The n "poles" (ordinate values) p[0],...,p[n-1] of the interpolating b-spline</returns>
        /// <remarks>
        ///  This function does 1-D (i.e. real-valued) interpolation. 
        ///  To do 3D interpolation, you would have to call it three times -- for x, y, and z.
        ///  However, it is much more efficient to call 
        /// <see cref="M:Snap.Math.SplineMath.BsplineInterpolation(Snap.Position[],System.Double[],System.Double[])">the other BsplineInterpolation overload</see>, 
        ///  which receives positions as input.
        /// </remarks>
        // Token: 0x06000329 RID: 809 RVA: 0x0003215C File Offset: 0x0003035C
        public static double[] BsplineInterpolation(double[] intValues, double[] nodes, double[] knots)
        {
            int num = intValues.Length;
            int num2 = knots.Length;
            int k = num2 - num;
            double[,] a = SplineMath.BasisMatrix(knots, k, nodes);
            int[] index = new int[num];
            double d = 1.0;
            LinearAlgebra.LUDecomposition(a, index, d);
            double[] array = new double[num];
            intValues.CopyTo(array, 0);
            LinearAlgebra.BackSubstitution(a, index, array);
            return array;
        }

        /// <summary>Evaluates a matrix of basis function values</summary>
        /// <param name="knots">Knot sequence : n+k values t[0], ... , t[n+k-1]</param>
        /// <param name="k">Order of the b-spline</param>
        /// <param name="nodes">Parameter (n) values at which to evaluate, tau[0],...,tau[n-1]</param>
        /// <returns>Matrix (n x n) of basis function values, B(i,k)(nodes[j])</returns>
        /// <remarks>
        /// There are n basis functions, numbered <c>B(0,k), B(1,k), ... , B(n-1,k)</c>.
        /// <para>The i-th basis function B(i,k) is non-zero only for t[i] &lt; t &lt; t[i+k]</para>
        /// <para>In the matrix A : <c>A[j][i] = B(i,k)(tau[j])</c>  (i = 0,1,...,n-1 ; j = 0,1,...,n-1)</para>
        /// </remarks>
        // Token: 0x0600032C RID: 812 RVA: 0x00032398 File Offset: 0x00030598
        public static double[,] BasisMatrix(double[] knots, int k, double[] nodes)
        {
            int num = knots.Length;
            int num2 = num - k;
            double[,] array = new double[num2, num2];
            for (int i = 0; i < num2; i++)
            {
                for (int j = 0; j < num2; j++)
                {
                    array[j, i] = SplineMath.EvaluateBasisFunction(knots, i, k, nodes[j]);
                }
            }
            return array;
        }

        /// <summary>Evaluates value of a b-spline basis function</summary>
        /// <param name="knots">Knot sequence : n+k values t[0], ... , t[n+k-1]</param>
        /// <param name="i">Index of basis function (see below)</param>
        /// <param name="k">Order of the b-spline</param>
        /// <param name="t">Parameter value at which to evaluate</param>
        /// <returns>Value of i-th basis function B(i,k)(t)</returns>
        /// <remarks>
        /// There are n basis functions, numbered <c>B(0,k), B(1,k), ... , B(n-1,k)</c>.
        /// <para>The i-th basis function B(i,k) is non-zero only for t[i] &lt; t &lt; t[i+k]</para>
        /// </remarks>
        // Token: 0x0600032D RID: 813 RVA: 0x000323E8 File Offset: 0x000305E8
        public static double EvaluateBasisFunction(double[] knots, int i, int k, double t)
        {
            double[] array = SplineMath.EvaluateBasisFunction(knots, i, k, t, 0);
            return array[0];
        }

        /// <summary>
        /// Evaluates a b-spline basis function and/or optional derivatives
        /// </summary>
        /// <param name="knots">The knot sequence : n+k values t[0], ... , t[n+k-1]</param>
        /// <param name="i">Index of basis function (see below)</param>
        /// <param name="k">Order of the b-spline</param>
        /// <param name="tau">Parameter value at which to evaluate</param>        
        /// <param name="derivs">Number of derivatives requested (0 for position alone)</param>
        /// <returns>Position and derivatives</returns>
        /// <remarks>
        /// <para>Copied from h0916, which in turn came from deBoor's book.</para>
        /// <para>There are n basis functions, numbered <c>B(0,k), B(1,k), ... , B(n-1,k)</c>.</para>
        /// <para>The i-th basis function B(i,k) is non-zero only for t[i] &lt; tau &lt; t[i+k]</para>
        /// </remarks>
        // Token: 0x0600032E RID: 814 RVA: 0x00032404 File Offset: 0x00030604
        private static double[] EvaluateBasisFunction(double[] knots, int i, int k, double tau, int derivs)
        {
            int num = knots.Length;
            double[] array = new double[derivs + 1];
            int num2 = 25;
            double num3 = 1E-07;
            double num4 = 1E-11;
            double[,] array2 = new double[num2, num2];
            if (derivs >= k)
            {
                for (int j = k; j <= derivs; j++)
                {
                    array[k] = 0.0;
                }
                derivs = k - 1;
            }
            double num5;
            if (Math.Abs(tau - knots[num - 1]) > num4)
            {
                num5 = tau;
            }
            else
            {
                num5 = knots[num - 1] - num4;
            }
            for (int l = i; l < i + k; l++)
            {
                array2[0, l - i] = ((knots[l] <= num5 && knots[l + 1] > num5) ? 1.0 : 0.0);
            }
            for (int j = 1; j <= derivs; j++)
            {
                for (int l = 0; l < k; l++)
                {
                    array2[j, l] = array2[0, l];
                }
            }
            for (int l = 2; l <= k; l++)
            {
                for (int j = 0; j <= derivs; j++)
                {
                    double num6 = knots[i + l - 1] - knots[i];
                    double num7;
                    if (Math.Abs(num6) <= num3)
                    {
                        num7 = 0.0;
                    }
                    else
                    {
                        num7 = array2[j, 0] / num6;
                    }
                    for (int m = 0; m <= k - l; m++)
                    {
                        num6 = knots[i + m + l] - knots[i + m + 1];
                        double num8;
                        if (Math.Abs(num6) <= num3)
                        {
                            num8 = 0.0;
                        }
                        else
                        {
                            num8 = array2[j, m + 1] / num6;
                        }
                        if (l < k - j + 1)
                        {
                            array2[j, m] = num7 * (tau - knots[i + m]) + num8 * (knots[i + m + l] - tau);
                        }
                        else
                        {
                            array2[j, m] = (double)(2 * k - l - j) * (num7 - num8);
                        }
                        num7 = num8;
                    }
                }
            }
            for (int j = 0; j <= derivs; j++)
            {
                array[j] = array2[j, 0];
            }
            return array;
        }

        // Token: 0x0600032F RID: 815 RVA: 0x00032628 File Offset: 0x00030828
        private static double EvaluateBasisFunction2(double[] knots, int i, int k, double t)
        {
            int num = knots.Length;
            int num2 = 25;
            double num3 = 1E-07;
            double num4 = 1E-11;
            double[] array = new double[num2];
            double num5;
            if (Math.Abs(t - knots[num - 1]) > num4)
            {
                num5 = t;
            }
            else
            {
                num5 = knots[num - 1] - num4;
            }
            for (int j = i; j < i + k; j++)
            {
                array[j - i] = ((knots[j] <= num5 && knots[j + 1] > num5) ? 1.0 : 0.0);
            }
            for (int j = 2; j <= k; j++)
            {
                double num6 = knots[i + j - 1] - knots[i];
                double num7;
                if (Math.Abs(num6) <= num3)
                {
                    num7 = 0.0;
                }
                else
                {
                    num7 = array[0] / num6;
                }
                for (int l = 0; l <= k - j; l++)
                {
                    num6 = knots[i + l + j] - knots[i + l + 1];
                    double num8;
                    if (Math.Abs(num6) <= num3)
                    {
                        num8 = 0.0;
                    }
                    else
                    {
                        num8 = array[l + 1] / num6;
                    }
                    if (j < k + 1)
                    {
                        array[l] = num7 * (t - knots[i + l]) + num8 * (knots[i + l + j] - t);
                    }
                    else
                    {
                        array[l] = (double)(2 * k - j) * (num7 - num8);
                    }
                    num7 = num8;
                }
            }
            return array[0];
        }

        /// <summary>
        /// Finds the knot vector span containing a given value
        /// </summary>
        /// <param name="knots">The knot sequence : n+k+1 values t[0], ... , t[n+k]</param>
        /// <param name="k">Order of the basis functions (k = m+1 = degree+1)</param>
        /// <param name="tau">Parameter value whose span we want to find</param>
        /// <returns>The span index; a number r such that t[r] ≤ tau &lt; t[r+1]</returns>
        /// <remarks>
        /// The span index r returned by this function is always in the range m ≤ r ≤ n.
        /// <para>
        /// If tau &lt; t[m], we return m, so subsequent evaluations of a basis function at tau
        /// will actually be using the polynomial that agrees with this basis function 
        /// on the first span t[m] ≤ t &lt; t[m+1].
        /// </para>
        /// <para>
        /// If tau &gt; t[n], we return n. So, subsequent evaluations of a basis function at tau
        /// will actually be using the polynomial that agrees with this basis function 
        /// on the last span t[n-1] ≤ t &lt; t[n].
        /// </para>
        /// </remarks>
        // Token: 0x06000330 RID: 816 RVA: 0x00032788 File Offset: 0x00030988
        internal static int FindSpan(double[] knots, int k, double tau)
        {
            int num = knots.Length;
            int num2 = k - 1;
            int num3 = num - k;
            int num4 = num3 - 1;
            if (tau >= knots[num4 + 1])
            {
                return num4;
            }
            if (tau <= knots[num2])
            {
                return num2;
            }
            int num5 = num2;
            int num6 = num4 + 1;
            int num7 = (num5 + num6) / 2;
            while (tau < knots[num7] || tau >= knots[num7 + 1])
            {
                if (tau < knots[num7])
                {
                    num6 = num7;
                }
                else
                {
                    num5 = num7;
                }
                num7 = (num5 + num6) / 2;
            }
            return num7;
        }

        /// <summary>
        /// Evaluates the k b-spline basis functions that are non-zero on a given span
        /// </summary>
        /// <param name="knots">The knot sequence : n+k+1 values t[0], ... , t[n+k]</param>
        /// <param name="k">Order of the basis functions (k = m+1 = degree+1)</param>
        /// <param name="tau">Parameter value at which to evaluate</param>        
        /// <param name="r">The span number: t[r] ≤ tau &lt; t[r+1]</param>
        /// <returns>Values at tau of the k basis functions that are non-zero on the span t[r] ≤ tau &lt; t[r+1]</returns>
        /// <remarks>
        /// This is based on the function "BasisFuncs" in Tiller's book. Algorithm 2.2, page 70.
        /// <para>
        /// There are k basis functions that are non-zero on the span where t[r] ≤ tau &lt; t[r+1]. 
        /// In fact these basis functions are <c>B(r-m,k), B(r-m+1,k), ... , B(r,k)</c>,
        /// where <c>B(i,k)</c> is the basis function of order k that is non-zero on the interval
        /// t[i] ≤ tau &lt; t[i+k]
        /// </para>
        /// <para>
        /// The span number r must lie in the range m ≤ r ≤ n.
        /// </para>
        /// </remarks>
        // Token: 0x06000331 RID: 817 RVA: 0x000327F8 File Offset: 0x000309F8
        internal static double[] EvaluateBasisFunctions(double[] knots, int k, double tau, int r)
        {
            double[] array = new double[k];
            array[0] = 1.0;
            for (int i = 1; i <= k - 1; i++)
            {
                double num = 0.0;
                for (int j = 0; j < i; j++)
                {
                    int num2 = r + j + 1;
                    int num3 = num2 - i;
                    double num4 = knots[num2];
                    double num5 = knots[num3];
                    double num6 = array[j] / (num4 - num5);
                    array[j] = num + (num4 - tau) * num6;
                    num = (tau - num5) * num6;
                }
                array[i] = num;
            }
            return array;
        }

        /// <summary>
        /// Calculates the value and derivatives of the k b-spline basis functions that are non-zero on a given span
        /// </summary>
        /// <param name="knots">The knot sequence : n+k+1 values t[0], ... , t[n+k]</param>
        /// <param name="order">Order of the basis functions (k = m+1 = degree+1)</param>
        /// <param name="tau">Parameter value at which to evaluate</param>  
        /// <param name="span">The span number r such that t[r] ≤ tau &lt; t[r+1]</param>
        /// <param name="numDerivs">How many derivatives to compute. Set numDerivs = 0 for function values alone</param>
        /// <returns>Values at tau of the k basis functions that are non-zero there.</returns>
        /// <remarks>
        /// This is based on the function "xxx" in Tiller's book. Algorithm 2.2, page 70.
        /// <para>
        /// There are k basis functions that are non-zero on the span where t[r] ≤ tau &lt; t[r+1]. 
        /// In fact these basis functions are <c>B(r-m,k), B(r-m+1,k), ... , B(r,k)</c>,
        /// where <c>B(i,k)</c> is the basis function of order k that is non-zero on the interval
        /// t[i] ≤ tau &lt; t[i+k]
        /// </para>
        /// <para>
        /// The span number r must lie in the range m ≤ r ≤ n.
        /// </para>
        /// <para>
        /// derivs[i,j] is the i-th derivative of the j-th basis function
        /// </para>
        /// <para>
        /// If you don't know the span corresponding to the given tau value, you can get
        /// it by calling FindSpan.
        /// </para>
        /// </remarks>
        // Token: 0x06000332 RID: 818 RVA: 0x00032898 File Offset: 0x00030A98
        internal static double[,] EvaluateBasisFunctions(double[] knots, int order, double tau, int span, int numDerivs)
        {
            double[,] array = new double[order, order];
            int num = order - 1;
            double[,] array2 = new double[numDerivs + 1, order];
            array[0, 0] = 1.0;
            for (int i = 1; i <= num; i++)
            {
                double num2 = 0.0;
                for (int j = 0; j < i; j++)
                {
                    int num3 = span + j + 1;
                    int num4 = num3 - i;
                    double num5 = knots[num3];
                    double num6 = knots[num4];
                    array[i, j] = num5 - num6;
                    double num7 = array[j, i - 1] / array[i, j];
                    array[j, i] = num2 + (num5 - tau) * num7;
                    num2 = (tau - num6) * num7;
                }
                array[i, i] = num2;
            }
            for (int i = 0; i <= num; i++)
            {
                array2[0, i] = array[i, num];
            }
            double[,] array3 = new double[2, num + 1];
            int k;
            for (k = 0; k <= num; k++)
            {
                int num8 = 0;
                int num9 = 1;
                array3[0, 0] = 1.0;
                for (int l = 1; l <= numDerivs; l++)
                {
                    double num10 = 0.0;
                    int num11 = k - l;
                    int num12 = num - l;
                    if (k >= l)
                    {
                        array3[num9, 0] = array3[num8, 0] / array[num12 + 1, num11];
                        num10 = array3[num9, 0] * array[num11, num12];
                    }
                    int num13;
                    if (num11 >= -1)
                    {
                        num13 = 1;
                    }
                    else
                    {
                        num13 = -num11;
                    }
                    int num14;
                    if (k - 1 <= num12)
                    {
                        num14 = l - 1;
                    }
                    else
                    {
                        num14 = num - k;
                    }
                    int i;
                    for (i = num13; i <= num14; i++)
                    {
                        array3[num9, i] = (array3[num8, i] - array3[num8, i - 1]) / array[num12 + 1, num11 + i];
                        num10 += array3[num9, i] * array[num11 + i, num12];
                    }
                    if (k <= num12)
                    {
                        array3[num9, l] = -array3[num8, l - 1] / array[num12 + 1, k];
                        num10 += array3[num9, l] * array[k, num12];
                    }
                    array2[l, k] = num10;
                    i = num8;
                    num8 = num9;
                    num9 = i;
                }
            }
            k = num;
            for (int m = 1; m <= numDerivs; m++)
            {
                for (int i = 0; i <= num; i++)
                {
                    array2[m, i] *= (double)k;
                }
                k *= num - m;
            }
            return array2;
        }
    }
}
