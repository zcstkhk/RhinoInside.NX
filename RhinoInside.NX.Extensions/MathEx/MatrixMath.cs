using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Extensions
{
    public static class MatrixMath
    {
        /// <summary>
        /// 计算矩阵的行数
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static int RowCount(double[,] a)
        {
            return a.GetLength(0);
        }

        // Token: 0x060002FE RID: 766 RVA: 0x000313DA File Offset: 0x0002F5DA
        public static int RowCount(Vector3d[,] vectors)
        {
            return vectors.GetLength(0);
        }

        // Token: 0x060002FF RID: 767 RVA: 0x000313E3 File Offset: 0x0002F5E3
        public static int RowCount(Point3d[,] a)
        {
            return a.GetLength(0);
        }

        // Token: 0x06000300 RID: 768 RVA: 0x000313EC File Offset: 0x0002F5EC
        public static int ColumnCount(double[,] a)
        {
            return a.GetLength(1);
        }

        // Token: 0x06000301 RID: 769 RVA: 0x000313F5 File Offset: 0x0002F5F5
        public static int ColumnCount(Vector3d[,] a)
        {
            return a.GetLength(1);
        }

        // Token: 0x06000302 RID: 770 RVA: 0x000313FE File Offset: 0x0002F5FE
        public static int ColumnCount(Point3d[,] a)
        {
            return a.GetLength(1);
        }

        public static Vector3d[] GetRow(Vector3d[,] a, int i)
        {
            int num = ColumnCount(a);
            Vector3d[] array = new Vector3d[num];
            for (int j = 0; j < num; j++)
            {
                array[j] = a[i, j];
            }
            return array;
        }

        public static Point3d[] GetRow(Point3d[,] a, int i)
        {
            int num = ColumnCount(a);
            Point3d[] array = new Point3d[num];
            for (int j = 0; j < num; j++)
            {
                array[j] = a[i, j];
            }
            return array;
        }

        // Token: 0x06000307 RID: 775 RVA: 0x000314F8 File Offset: 0x0002F6F8
        public static Vector3d[] GetColumn(Vector3d[,] a, int j)
        {
            int num = RowCount(a);
            Vector3d[] array = new Vector3d[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = a[i, j];
            }
            return array;
        }

        // Token: 0x06000308 RID: 776 RVA: 0x0003153C File Offset: 0x0002F73C
        public static Point3d[] GetColumn(Point3d[,] a, int j)
        {
            int num = RowCount(a);
            Point3d[] array = new Point3d[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = a[i, j];
            }
            return array;
        }

        /// <summary>Copies an array of doubles</summary>
        /// <param name="original">Original array</param>
        /// <returns>New array with elements equal to input</returns>
        public static double[] Copy(double[] original)
        {
            int num = original.Length;
            double[] array = new double[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = original[i];
            }
            return array;
        }

        // Token: 0x0600030A RID: 778 RVA: 0x000315AC File Offset: 0x0002F7AC
        public static double[,] Copy(double[,] original)
        {
            int num = RowCount(original);
            int num2 = ColumnCount(original);
            double[,] array = new double[num, num2];
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num2; j++)
                {
                    array[i, j] = original[i, j];
                }
            }
            return array;
        }


        /// <summary>
        /// 两个二维矩阵相加
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double[,] Addition(double[,] a, double[,] b)
        {
            int num = RowCount(a);
            int num2 = ColumnCount(a);
            double[,] array = new double[num, num2];
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num2; j++)
                {
                    array[i, j] = a[i, j] + b[i, j];
                }
            }
            return array;
        }

        /// <summary>Returns a square zero matrix of size n</summary>
        /// <param name="n">Required size, n</param>
        /// <returns>Zero matrix of size n × n</returns>         
        // Token: 0x06000314 RID: 788 RVA: 0x000318AC File Offset: 0x0002FAAC
        public static double[,] ZeroMatrix(int n)
        {
            double[,] array = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    array[i, j] = 0.0;
                }
            }
            return array;
        }

        /// <summary>Returns an identity matrix of size n</summary>
        /// <param name="n">Required size, n</param>
        /// <returns>Identity matrix of size n × n</returns>         
        // Token: 0x06000315 RID: 789 RVA: 0x000318EC File Offset: 0x0002FAEC
        public static double[,] IdentityMatrix(int n)
        {
            double[,] array = ZeroMatrix(n);
            for (int i = 0; i < n; i++)
            {
                array[i, i] = 1.0;
            }
            return array;
        }

        // Token: 0x06000316 RID: 790 RVA: 0x00031920 File Offset: 0x0002FB20
        public static double[,] Transpose(double[,] a)
        {
            int num = RowCount(a);
            int num2 = ColumnCount(a);
            double[,] array = new double[num2, num];
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num2; j++)
                {
                    array[j, i] = a[i, j];
                }
            }
            return array;
        }

        // Token: 0x06000317 RID: 791 RVA: 0x00031974 File Offset: 0x0002FB74
        public static Point3d[,] Transpose(Point3d[,] a)
        {
            int num = RowCount(a);
            int num2 = ColumnCount(a);
            Point3d[,] array = new Point3d[num2, num];
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num2; j++)
                {
                    array[j, i] = a[i, j];
                }
            }
            return array;
        }

        /// <summary>Rearranges an Integer vector (1-D array) into a matrix (2-D array)</summary>
        /// <param name="vector">The vector</param>
        /// <param name="nrows">Number of rows in output matrix</param>
        /// <param name="ncols">Number of columns in output matrix</param>
        /// <returns>Matrix</returns>         
        // Token: 0x06000318 RID: 792 RVA: 0x000319D0 File Offset: 0x0002FBD0
        public static int[,] VectorToMatrix(int[] vector, int nrows, int ncols)
        {
            int[,] array = new int[nrows, ncols];
            for (int i = 0; i < nrows; i++)
            {
                for (int j = 0; j < ncols; j++)
                {
                    array[i, j] = vector[i * ncols + j];
                }
            }
            return array;
        }

        /// <summary>Rearranges a Double vector (1-D array) into a matrix (2-D array)</summary>
        /// <param name="vector">The vector</param>
        /// <param name="nrows">Number of rows in output matrix</param>
        /// <param name="ncols">Number of columns in output matrix</param>
        /// <returns>Matrix</returns>         
        // Token: 0x06000319 RID: 793 RVA: 0x00031A10 File Offset: 0x0002FC10
        public static double[,] VectorToMatrix(double[] vector, int nrows, int ncols)
        {
            double[,] array = new double[nrows, ncols];
            for (int i = 0; i < nrows; i++)
            {
                for (int j = 0; j < ncols; j++)
                {
                    array[i, j] = vector[i * ncols + j];
                }
            }
            return array;
        }

        // Token: 0x0600031A RID: 794 RVA: 0x00031A50 File Offset: 0x0002FC50
        public static int[] MatrixToVector(int[,] matrix)
        {
            int length = matrix.GetLength(0);
            int length2 = matrix.GetLength(1);
            int[] array = new int[length * length2];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length2; j++)
                {
                    array[i * length2 + j] = matrix[i, j];
                }
            }
            return array;
        }

        // Token: 0x0600031B RID: 795 RVA: 0x00031AA4 File Offset: 0x0002FCA4
        public static double[] MatrixToVector(double[,] matrix)
        {
            int length = matrix.GetLength(0);
            int length2 = matrix.GetLength(1);
            double[] array = new double[length * length2];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length2; j++)
                {
                    array[i * length2 + j] = matrix[i, j];
                }
            }
            return array;
        }

        #region Multiply
        /// <summary>
        /// 二维矩阵与一维向量相乘，结果为一维向量
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double[] Multiply(double[,] a, double[] b)
        {
            int num = RowCount(a);
            ColumnCount(a);
            double[] array = new double[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = Multiply(GetRow(a, i), b);
            }
            return array;
        }

        /// <summary>
        /// 二维矩阵与一维向量相乘，结果为一维向量
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double[] Multiply(double[] a, double[,] b)
        {
            int num = RowCount(b);
            double[] array = new double[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = Multiply(a, GetColumn(b, i));
            }
            return array;
        }

        /// <summary>计算两个向量的点积</summary>
        /// <param name="firstVector">First array (of length n)</param>
        /// <param name="secondVector">Second array (of same length, n)</param>
        /// <returns>Dot product</returns>
        /// <remarks>
        /// 不会检查输入向量的长度是否相同
        /// </remarks>
        public static double Multiply(double[] firstVector, double[] secondVector)
        {
            int vectorLength = firstVector.Length;
            double result = 0.0;
            for (int i = 0; i < vectorLength; i++)
            {
                result += firstVector[i] * secondVector[i];
            }
            return result;
        }

        /// <summary>Calculates the dot product of array of doubles and array of Vectors</summary>
        /// <param name="u">Array of doubles (of length n)</param>
        /// <param name="v">Array of Vectors (of same length, n)</param>
        /// <returns>Dot product</returns>
        /// <remarks>
        /// Does not check that the arrays have the same length
        /// </remarks>
        public static Vector3d Multiply(double[] u, Vector3d[] v)
        {
            int num = u.Length;
            Vector3d vector = default(Vector3d);
            for (int i = 0; i < num; i++)
            {
                vector = vector.Add(v[i].Multiply(u[i]));
            }
            return vector;
        }

        /// <summary>Calculates the dot product of array of doubles and array of Vectors</summary>
        /// <param name="u">Array of Vectors (of length n)</param>
        /// <param name="v">Array of doubles (of same length, n)</param>
        /// <returns>Dot product</returns>
        /// <remarks>
        /// Does not check that the arrays have the same length
        /// </remarks>
        public static Vector3d Multiply(Vector3d[] u, double[] v)
        {
            return Multiply(v, u);
        }

        // Token: 0x06000311 RID: 785 RVA: 0x00031780 File Offset: 0x0002F980
        public static Vector3d[,] Multiply(double[,] a, Vector3d[,] b)
        {
            int num = RowCount(a);
            ColumnCount(a);
            int num2 = ColumnCount(b);
            Vector3d[,] array = new Vector3d[num, num2];
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num2; j++)
                {
                    array[i, j] = Multiply(GetRow(a, i), GetColumn(b, j));
                }
            }
            return array;
        }

        // Token: 0x06000312 RID: 786 RVA: 0x000317EC File Offset: 0x0002F9EC
        public static Vector3d[,] Multiply(Vector3d[,] a, double[,] b)
        {
            int num = RowCount(a);
            ColumnCount(a);
            int num2 = ColumnCount(b);
            Vector3d[,] array = new Vector3d[num, num2];
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num2; j++)
                {
                    array[i, j] = Multiply(GetRow(a, i), GetColumn(b, j));
                }
            }
            return array;
        }

        public static double[,] Multiply(double[,] a, double s)
        {
            int num = RowCount(a);
            int num2 = ColumnCount(a);
            double[,] array = new double[num, num2];
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num2; j++)
                {
                    array[i, j] = s * a[i, j];
                }
            }
            return array;
        }

        /// <summary>
        /// 二维矩阵与二维矩阵相乘
        /// </summary>
        /// <param name="firstMatrix"></param>
        /// <param name="secondMatrix"></param>
        /// <returns></returns>
        public static double[,] Multiply(double[,] firstMatrix, double[,] secondMatrix)
        {
            int matrix1RowCount = RowCount(firstMatrix);
            int secondMatrixColumnCount = ColumnCount(secondMatrix);
            double[,] array = new double[matrix1RowCount, secondMatrixColumnCount];
            for (int i = 0; i < matrix1RowCount; i++)
            {
                for (int j = 0; j < secondMatrixColumnCount; j++)
                {
                    array[i, j] = Multiply(GetRow(firstMatrix, i), GetColumn(secondMatrix, j));
                }
            }
            return array;
        }

        public static double[] GetRow(this double[,] a, int i)
        {
            int num = ColumnCount(a);
            double[] array = new double[num];
            for (int j = 0; j < num; j++)
            {
                array[j] = a[i, j];
            }
            return array;
        }

        public static double[] GetColumn(this double[,] a, int j)
        {
            int num = RowCount(a);
            double[] array = new double[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = a[i, j];
            }
            return array;
        }

        #endregion
    }
}
