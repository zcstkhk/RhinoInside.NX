using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Extensions
{
    /// <summary>
    /// 线性代数常用方法
    /// </summary>
    public static class LinearAlgebra
    {
        // Token: 0x060002E7 RID: 743 RVA: 0x0003022C File Offset: 0x0002E42C
        public static EigenSystemResult[] EigenSystem(double[,] A)
        {
            EigenSolver3 eigenSolver = new EigenSolver3();
            eigenSolver.ComputeEigenStuff(A);
            EigenSystemResult[] array = new EigenSystemResult[3];
            for (int i = 0; i < 3; i++)
            {
                array[2 - i] = new EigenSystemResult(eigenSolver.mEigenvalue[i], eigenSolver.mEigenvector[i].ToArray());
            }
            return array;
        }

        public static void LUDecomposition(double[,] a, int[] index, double d)
        {
            int num = 0;
            int length = a.GetLength(0);
            double[] array = new double[length];
            d = 1.0;
            for (int i = 0; i < length; i++)
            {
                double num2 = 0.0;
                for (int j = 0; j < length; j++)
                {
                    double num3 = System.Math.Abs(a[i, j]);
                    if (num3 > num2)
                    {
                        num2 = num3;
                    }
                }
                if (num2 == 0.0)
                {
                    throw new ArgumentException("The input matrix is singular");
                }
                array[i] = 1.0 / num2;
            }
            for (int j = 0; j < length; j++)
            {
                for (int i = 0; i < j; i++)
                {
                    double num4 = a[i, j];
                    for (int k = 0; k < i; k++)
                    {
                        num4 -= a[i, k] * a[k, j];
                    }
                    a[i, j] = num4;
                }
                double num2 = 0.0;
                for (int i = j; i < length; i++)
                {
                    double num4 = a[i, j];
                    for (int k = 0; k < j; k++)
                    {
                        num4 -= a[i, k] * a[k, j];
                    }
                    a[i, j] = num4;
                    double num5 = array[i] * System.Math.Abs(num4);
                    if (num5 >= num2)
                    {
                        num2 = num5;
                        num = i;
                    }
                }
                if (j != num)
                {
                    for (int k = 0; k < length; k++)
                    {
                        double num5 = a[num, k];
                        a[num, k] = a[j, k];
                        a[j, k] = num5;
                    }
                    d = -d;
                    array[num] = array[j];
                }
                index[j] = num;
                if (a[j, j] == 0.0)
                {
                    a[j, j] = 1E-20;
                }
                if (j != length - 1)
                {
                    double num5 = 1.0 / a[j, j];
                    for (int i = j + 1; i < length; i++)
                    {
                        a[i, j] *= num5;
                    }
                }
            }
        }

        // Token: 0x060002E9 RID: 745 RVA: 0x00030478 File Offset: 0x0002E678
        public static void BackSubstitution(double[,] a, int[] index, double[] b)
        {
            int num = 0;
            int length = a.GetLength(0);
            for (int i = 0; i < length; i++)
            {
                int num2 = index[i];
                double num3 = b[num2];
                b[num2] = b[i];
                if (num != 0)
                {
                    for (int j = num - 1; j < i; j++)
                    {
                        num3 -= a[i, j] * b[j];
                    }
                }
                else if (num3 != 0.0)
                {
                    num = i + 1;
                }
                b[i] = num3;
            }
            for (int i = length - 1; i >= 0; i--)
            {
                double num3 = b[i];
                for (int j = i + 1; j < length; j++)
                {
                    num3 -= a[i, j] * b[j];
                }
                b[i] = num3 / a[i, i];
            }
        }

        // Token: 0x060002EA RID: 746 RVA: 0x00030528 File Offset: 0x0002E728
        public static double[] BackSolve(double[,] a, int[] index, double[] b)
        {
            double[] array = MatrixMath.Copy(b);
            BackSubstitution(a, index, array);
            return array;
        }

        // Token: 0x060002EB RID: 747 RVA: 0x00030548 File Offset: 0x0002E748
        public static double[] LinearSystemSolve(double[,] a, double[] b)
        {
            int length = a.GetLength(0);
            int[] index = new int[length];
            double[,] a2 = MatrixMath.Copy(a);
            double d = 1.0;
            LUDecomposition(a2, index, d);
            return BackSolve(a2, index, b);
        }

        /// <summary>Solves a system of two linear equations</summary>
        /// <param name="a">Coefficient of x in the first equation</param>
        /// <param name="b">Coefficient of y in the first equation</param>
        /// <param name="c">Coefficient of x in the second equation</param>
        /// <param name="d">Coefficient of y in the second equation</param>
        /// <param name="h">Constant term (on right-hand side) in first equation</param>
        /// <param name="k">Constant term (on right-hand side) in second equation</param>
        /// <returns>Solutions for x and y</returns>
        /// <remarks>
        /// The system solved is:
        /// <para>
        /// <c>      a*x + b*y = h</c><br />
        /// <c>      c*x + d*y = k</c>
        /// </para>
        /// <para>
        /// If <c>a*d - b*c = 0</c>, no exception will be raised, but
        /// each of the solutions returned will be either 
        /// <see cref="F:System.Double.PositiveInfinity">Double.PositiveInfinity</see> or
        /// <see cref="F:System.Double.NegativeInfinity">Double.NegativeInfinity</see>.
        /// </para>
        /// </remarks>
        public static double[] LinearSystemSolve(double a, double b, double c, double d, double h, double k)
        {
            double num = a * d - b * c;
            double num2 = (d * h - b * k) / num;
            double num3 = (a * k - c * h) / num;
            return new double[]
            {
                    num2,
                    num3
            };
        }

        // Token: 0x060002EE RID: 750 RVA: 0x00030660 File Offset: 0x0002E860
        public static double[,] Inverse(double[,] a)
        {
            int length = a.GetLength(0);
            double[,] a2 = MatrixMath.Copy(a);
            double[,] array = new double[length, length];
            double[] array2 = new double[length];
            int[] index = new int[length];
            double d = 1.0;
            LUDecomposition(a2, index, d);
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    array2[j] = 0.0;
                }
                array2[i] = 1.0;
                BackSubstitution(a2, index, array2);
                for (int k = 0; k < length; k++)
                {
                    array[k, i] = array2[k];
                }
            }
            return array;
        }

        // Token: 0x060002EF RID: 751 RVA: 0x0003070C File Offset: 0x0002E90C
        public static double Determinant3(double[,] u)
        {
            double num = u[0, 0] * (u[1, 1] * u[2, 2] - u[1, 2] * u[2, 1]);
            double num2 = u[0, 1] * (u[1, 0] * u[2, 2] - u[1, 2] * u[2, 0]);
            double num3 = u[0, 2] * (u[1, 0] * u[2, 1] - u[1, 1] * u[2, 0]);
            return num - num2 + num3;
        }

        // Token: 0x060002F0 RID: 752 RVA: 0x000307A8 File Offset: 0x0002E9A8
        public static double[,] Adjugate3(double[,] a)
        {
            double[,] array = new double[3, 3];
            array[0, 0] = -a[1, 2] * a[2, 1] + a[1, 1] * a[2, 2];
            array[0, 1] = a[0, 2] * a[2, 1] - a[0, 1] * a[2, 2];
            array[0, 2] = -a[0, 2] * a[1, 1] + a[0, 1] * a[1, 2];
            array[1, 0] = a[1, 2] * a[2, 0] - a[1, 0] * a[2, 2];
            array[1, 1] = -a[0, 2] * a[2, 0] + a[0, 0] * a[2, 2];
            array[1, 2] = a[0, 2] * a[1, 0] - a[0, 0] * a[1, 2];
            array[2, 0] = -a[1, 1] * a[2, 0] + a[1, 0] * a[2, 1];
            array[2, 1] = a[0, 1] * a[2, 0] - a[0, 0] * a[2, 1];
            array[2, 2] = -a[0, 1] * a[1, 0] + a[0, 0] * a[1, 1];
            return array;
        }

        // Token: 0x060002F1 RID: 753 RVA: 0x00030948 File Offset: 0x0002EB48
        public static double[,] Inverse3(double[,] a)
        {
            double num = Determinant3(a);
            double[,] a2 = Adjugate3(a);
            return MatrixMath.Multiply(a2, 1.0 / num);
        }

        /// <summary>Represents the results of an eigenvalue/eigenvector calculation</summary>
        // Token: 0x0200003C RID: 60
        public class EigenSystemResult
        {
            // Token: 0x060002F2 RID: 754 RVA: 0x00030976 File Offset: 0x0002EB76
            internal EigenSystemResult(double eigenvalue, double[] eigenvector)
            {
                Eigenvalue = eigenvalue;
                Eigenvector = eigenvector;
            }

            /// <summary>An eigenvalue</summary>
            // Token: 0x1700008C RID: 140
            // (get) Token: 0x060002F3 RID: 755 RVA: 0x0003098C File Offset: 0x0002EB8C
            // (set) Token: 0x060002F4 RID: 756 RVA: 0x00030994 File Offset: 0x0002EB94
            public double Eigenvalue { get; internal set; }

            /// <summary>The corresponding eigenvector</summary>
            // Token: 0x1700008D RID: 141
            // (get) Token: 0x060002F5 RID: 757 RVA: 0x0003099D File Offset: 0x0002EB9D
            // (set) Token: 0x060002F6 RID: 758 RVA: 0x000309A5 File Offset: 0x0002EBA5
            public double[] Eigenvector { get; internal set; }
        }

        internal class EigenSolver3
        {
            internal void ComputeEigenStuff(double[,] A)
            {
                double num = Math.Abs(A[0, 0]);
                double num2 = Math.Abs(A[0, 1]);
                double num3 = Math.Abs(A[0, 2]);
                double num4 = Math.Abs(A[1, 1]);
                double num5 = Math.Abs(A[1, 2]);
                double num6 = Math.Abs(A[2, 2]);
                double num7 = MathEx.Max(new double[]
                {
                        num,
                        num2,
                        num3,
                        num4,
                        num5,
                        num6
                });
                double[,] array = A;
                if (num7 > 1.0)
                {
                    double s = 1.0 / num7;
                    array = MatrixMath.Multiply(A, s);
                }
                double[] array2 = new double[3];
                ComputeRoots(array, array2);
                mEigenvalue[0] = array2[0];
                mEigenvalue[1] = array2[1];
                mEigenvalue[2] = array2[2];
                double[] array3 = new double[3];
                Vector3d[] array4 = new Vector3d[3];
                for (int i = 0; i < 3; i++)
                {
                    double[,] array5 = (double[,])array.Clone();
                    array5[0, 0] -= mEigenvalue[i];
                    array5[1, 1] -= mEigenvalue[i];
                    array5[2, 2] -= mEigenvalue[i];
                    if (!IsPositiveRank(array5, ref array3[i], ref array4[i]))
                    {
                        if (num7 > 1.0)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                mEigenvalue[j] *= num7;
                            }
                        }
                        mEigenvector[0] = Vector3dEx.AxisX;
                        mEigenvector[1] = Vector3dEx.AxisY;
                        mEigenvector[2] = Vector3dEx.AxisZ;
                        return;
                    }
                }
                double num8 = array3[0];
                int num9 = 0;
                if (array3[1] > num8)
                {
                    num8 = array3[1];
                    num9 = 1;
                }
                if (array3[2] > num8)
                {
                    num9 = 2;
                }
                if (num9 == 0)
                {
                    array4[0].Normalize();
                    ComputeVectors(array, array4[0], 1, 2, 0);
                }
                else if (num9 == 1)
                {
                    array4[1].Normalize();
                    ComputeVectors(array, array4[1], 2, 0, 1);
                }
                else
                {
                    array4[2].Normalize();
                    ComputeVectors(array, array4[2], 0, 1, 2);
                }
                if (num7 > 1.0)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        mEigenvalue[k] *= num7;
                    }
                }
            }

            // Token: 0x060002F8 RID: 760 RVA: 0x00030CA4 File Offset: 0x0002EEA4
            private void ComputeRoots(double[,] A, double[] root)
            {
                double num = A[0, 0];
                double num2 = A[0, 1];
                double num3 = A[0, 2];
                double num4 = A[1, 1];
                double num5 = A[1, 2];
                double num6 = A[2, 2];
                double num7 = num * num4 * num6 + 2.0 * num2 * num3 * num5 - num * num5 * num5 - num4 * num3 * num3 - num6 * num2 * num2;
                double num8 = num * num4 - num2 * num2 + num * num6 - num3 * num3 + num4 * num6 - num5 * num5;
                double num9 = num + num4 + num6;
                double num10 = 0.33333333333333331 * num9;
                double num11 = 0.33333333333333331 * (num8 - num9 * num10);
                if (num11 > 0.0)
                {
                    num11 = 0.0;
                }
                double num12 = 0.5 * (num7 + num10 * (2.0 * num10 * num10 - num8));
                double num13 = num12 * num12 + num11 * num11 * num11;
                if (num13 > 0.0)
                {
                    num13 = 0.0;
                }
                double num14 = Math.Sqrt(-num11);
                double num15 = 0.33333333333333331 * Math.Atan2(Math.Sqrt(-num13), num12);
                double num16 = Math.Cos(num15);
                double num17 = Math.Sin(num15);
                double num18 = num10 + 2.0 * num14 * num16;
                double num19 = Math.Sqrt(3.0);
                double num20 = num10 - num14 * (num16 + num19 * num17);
                double num21 = num10 - num14 * (num16 - num19 * num17);
                if (num20 >= num18)
                {
                    root[0] = num18;
                    root[1] = num20;
                }
                else
                {
                    root[0] = num20;
                    root[1] = num18;
                }
                if (num21 >= root[1])
                {
                    root[2] = num21;
                    return;
                }
                root[2] = root[1];
                if (num21 >= root[0])
                {
                    root[1] = num21;
                    return;
                }
                root[1] = root[0];
                root[0] = num21;
            }

            // Token: 0x060002F9 RID: 761 RVA: 0x00030E84 File Offset: 0x0002F084
            private bool IsPositiveRank(double[,] M, ref double maxEntry, ref Vector3d maxRow)
            {
                maxEntry = -1.0;
                int i = -1;
                for (int j = 0; j < 3; j++)
                {
                    for (int k = j; k < 3; k++)
                    {
                        double num = Math.Abs(M[j, k]);
                        if (num > maxEntry)
                        {
                            maxEntry = num;
                            i = j;
                        }
                    }
                }
                maxRow = MatrixMath.GetRow(M, i).ToVector3d();
                return maxEntry >= 1E-08;
            }

            // Token: 0x060002FA RID: 762 RVA: 0x00030EF0 File Offset: 0x0002F0F0
            private void ComputeVectors(double[,] A, Vector3d U2, int i0, int i1, int i2)
            {
                Vector3d vector;
                Vector3d vector2;
                GenerateComplementBasis(out vector, out vector2, U2);
                Vector3d v = MatrixMath.Multiply(A, vector.ToArray()).ToVector3d();
                double num = mEigenvalue[i2] - vector.DotProduct(v);
                double num2 = vector2.DotProduct(v);
                Vector3d v2 = MatrixMath.Multiply(A, vector2.ToArray()).ToVector3d();
                double num3 = mEigenvalue[i2] - vector2.DotProduct(v2);
                double num4 = Math.Abs(num);
                int num5 = 0;
                double num6 = Math.Abs(num2);
                if (num6 > num4)
                {
                    num4 = num6;
                }
                num6 = Math.Abs(num3);
                if (num6 > num4)
                {
                    num4 = num6;
                    num5 = 1;
                }
                if (num4 >= 1E-08)
                {
                    if (num5 == 0)
                    {
                        double num7 = 1.0 / Math.Sqrt(num * num + num2 * num2);
                        num *= num7;
                        num2 *= num7;
                        mEigenvector[i2] = vector.Multiply(num2).Add(vector2.Multiply(num));
                    }
                    else
                    {
                        double num7 = 1.0 / Math.Sqrt(num3 * num3 + num2 * num2);
                        num3 *= num7;
                        num2 *= num7;
                        mEigenvector[i2] = vector.Multiply(num3).Add(vector2.Multiply(num2));
                    }
                }
                else if (num5 == 0)
                {
                    mEigenvector[i2] = vector2;
                }
                else
                {
                    mEigenvector[i2] = vector;
                }
                Vector3d vector3 = U2.CrossProduct(mEigenvector[i2]);
                v = MatrixMath.Multiply(A, U2.ToArray()).ToVector3d();
                num = mEigenvalue[i0] - U2.DotProduct(v);
                num2 = vector3.DotProduct(v);
                Vector3d v3 = MatrixMath.Multiply(A, vector3.ToArray()).ToVector3d();
                num3 = mEigenvalue[i0] - vector3.DotProduct(v3);
                num4 = Math.Abs(num);
                num5 = 0;
                num6 = Math.Abs(num2);
                if (num6 > num4)
                {
                    num4 = num6;
                }
                num6 = Math.Abs(num3);
                if (num6 > num4)
                {
                    num4 = num6;
                    num5 = 1;
                }
                if (num4 >= 1E-08)
                {
                    if (num5 == 0)
                    {
                        double num7 = 1.0 / Math.Sqrt(num * num + num2 * num2);
                        num *= num7;
                        num2 *= num7;
                        mEigenvector[i0] = U2.Multiply(num2).Add(vector3.Multiply(num));
                    }
                    else
                    {
                        double num7 = 1.0 / Math.Sqrt(num3 * num3 + num2 * num2);
                        num3 *= num7;
                        num2 *= num7;
                        mEigenvector[i0] = U2.Multiply(num3).Add(vector3.Multiply(num2));
                    }
                }
                else if (num5 == 0)
                {
                    mEigenvector[i0] = vector3;
                }
                else
                {
                    mEigenvector[i0] = U2;
                }
                mEigenvector[i1] = mEigenvector[i2].CrossProduct(mEigenvector[i0]);
            }

            // Token: 0x060002FB RID: 763 RVA: 0x00031234 File Offset: 0x0002F434
            private void GenerateComplementBasis(out Vector3d vec0, out Vector3d vec1, Vector3d vec2)
            {
                double num;
                if (Math.Abs(vec2.X) >= Math.Abs(vec2.Y))
                {
                    num = 1.0 / Math.Sqrt(vec2.X * vec2.X + vec2.Z * vec2.Z);
                    vec0.X = -vec2.Z * num;
                    vec0.Y = 0.0;
                    vec0.Z = vec2.X * num;
                    vec1.X = vec2.Y * vec0.Z;
                    vec1.Y = vec2.Z * vec0.X - vec2.X * vec0.Z;
                    vec1.Z = -vec2.Y * vec0.X;
                    return;
                }
                num = 1.0 / Math.Sqrt(vec2.Y * vec2.Y + vec2.Z * vec2.Z);
                vec0.X = 0.0;
                vec0.Y = vec2.Z * num;
                vec0.Z = -vec2.Y * num;
                vec1.X = vec2.Y * vec0.Z - vec2.Z * vec0.Y;
                vec1.Y = -vec2.X * vec0.Z;
                vec1.Z = vec2.X * vec0.Y;
            }

            // Token: 0x060002FC RID: 764 RVA: 0x000313B1 File Offset: 0x0002F5B1
            public EigenSolver3()
            {
            }

            // Token: 0x04000126 RID: 294
            private const double oneThird = 0.33333333333333331;

            // Token: 0x04000127 RID: 295
            private const double ZERO_TOLERANCE = 1E-08;

            // Token: 0x04000128 RID: 296
            internal double[] mEigenvalue = new double[3];

            // Token: 0x04000129 RID: 297
            internal Vector3d[] mEigenvector = new Vector3d[3];
        }
    }
}
