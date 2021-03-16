using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
    public static class Matrix3x3Ex
    {
        #region 创建 Matrix3x3
        /// <summary>
        /// 使用二维数组来初始化 Matrix3x3
        /// </summary>
        /// <param name="orientaition"></param>
        public static Matrix3x3 Create(double[,] orientaition)
        {
            return Create(MatrixMath.GetRow(orientaition, 0), MatrixMath.GetRow(orientaition, 1), MatrixMath.GetRow(orientaition, 2));
        }

        /// <summary>
        /// 使用 X 和 Y 向量来初始化 Matrix3x3
        /// </summary>
        /// <param name="xVector"></param>
        /// <param name="yVector"></param>
        public static Matrix3x3 Create(Vector3d xVector, Vector3d yVector)
        {
            double[] mtx = new double[9];
            Globals.TheUfSession.Mtx3.Initialize(xVector.ToArray(), yVector.ToArray(), mtx);

            return new Matrix3x3()
            {
                Xx = mtx[0],
                Xy = mtx[1],
                Xz = mtx[2],

                Yx = mtx[3],
                Yy = mtx[4],
                Yz = mtx[5],

                Zx = mtx[6],
                Zy = mtx[7],
                Zz = mtx[8],
            };
        }

        public static Matrix3x3 Create(Vector3d zVector)
        {
            double[] mtx = new double[9];
            Globals.TheUfSession.Mtx3.InitializeZ(zVector.ToArray(), mtx);

            return new Matrix3x3()
            {
                Xx = mtx[0],
                Xy = mtx[1],
                Xz = mtx[2],

                Yx = mtx[3],
                Yy = mtx[4],
                Yz = mtx[5],

                Zx = mtx[6],
                Zy = mtx[7],
                Zz = mtx[8],
            };
        }

        /// <summary>
        /// 使用 X 和 Y 向量来初始化 Matrix3x3
        /// </summary>
        /// <param name="xVector"></param>
        /// <param name="yVector"></param>
        public static Matrix3x3 Create(double[] xVector, double[] yVector)
        {
            double[] mtx = new double[9];
            Globals.TheUfSession.Mtx3.Initialize(xVector, yVector, mtx);

            return new Matrix3x3()
            {
                Xx = mtx[0],
                Xy = mtx[1],
                Xz = mtx[2],

                Yx = mtx[3],
                Yy = mtx[4],
                Yz = mtx[5],

                Zx = mtx[6],
                Zy = mtx[7],
                Zz = mtx[8],
            };
        }

        /// <summary>
        /// 使用向量来初始化 Matrix3x3
        /// </summary>
        /// <param name="xVector"></param>
        /// <param name="yVector"></param>
        /// <param name="zVector"></param>
        public static Matrix3x3 Create(Vector3d xVector, Vector3d yVector, Vector3d zVector)
        {
            var unitizedX = xVector.GetUnitVector();
            var unitizedY = yVector.GetUnitVector();
            var unitizedZ = zVector.GetUnitVector();
            return new Matrix3x3()
            {
                Xx = unitizedX.X,
                Xy = unitizedX.Y,
                Xz = unitizedX.Z,

                Yx = unitizedY.X,
                Yy = unitizedY.Y,
                Yz = unitizedY.Z,

                Zx = unitizedZ.X,
                Zy = unitizedZ.Y,
                Zz = unitizedZ.Z,
            };
        }

        /// <summary>
        /// 使用向量来初始化 Matrix3x3
        /// </summary>
        /// <param name="xVector"></param>
        /// <param name="yVector"></param>
        /// <param name="zVector"></param>
        public static Matrix3x3 Create(double[] xVector, double[] yVector, double[] zVector)
        {
            return new Matrix3x3()
            {
                Xx = xVector[0],
                Xy = xVector[1],
                Xz = xVector[2],

                Yx = yVector[0],
                Yy = yVector[1],
                Yz = yVector[2],

                Zx = zVector[0],
                Zy = zVector[1],
                Zz = zVector[2],
            };
        }


        /// <summary>
        /// 创建 Matrix3x3
        /// </summary>
        /// <param name="mtx"></param>
        /// <returns></returns>
        public static Matrix3x3 Create(double[] mtx)
        {
            return new Matrix3x3()
            {
                Xx = mtx[0],
                Xy = mtx[1],
                Xz = mtx[2],
                Yx = mtx[3],
                Yy = mtx[4],
                Yz = mtx[5],
                Zx = mtx[6],
                Zy = mtx[7],
                Zz = mtx[8]
            };
        }
        #endregion

        /// <summary>
        /// 获取单位矩阵
        /// </summary>
        public static Matrix3x3 Identity
        {
            get => new Matrix3x3() { Xx = 1.0, Xy = 0, Xz = 0, Yx = 0.0, Yy = 1.0, Yz = 0.0, Zx = 0.0, Zy = 0.0, Zz = 1.0 };
        }

        /// <summary>
        /// 将矩阵转换为数组
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double[] Matrix3x3ToArray(Matrix3x3 matrix) => matrix.ToArray();

        /// <summary>
        /// 矩阵单位化
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix3x3 Matrix3x3Unitize(Matrix3x3 matrix) => matrix.Unitize();

        /// <summary>
        /// 判断两个矩阵是否相等
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        public static bool Matrix3x3IsSame(Matrix3x3 matrix1, Matrix3x3 matrix2) => matrix1.EqualsTo(matrix2);

        /// <summary>
        /// 求逆矩阵
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix3x3 Inverse(this Matrix3x3 matrix)
        {
            double num = 1.0 / matrix.GetDeterminant();
            Matrix3x3 result;
            result.Xx = num * (matrix.Yy * matrix.Zz - matrix.Yz * matrix.Zy);
            result.Yx = -num * (matrix.Yx * matrix.Zz - matrix.Yz * matrix.Zx);
            result.Zx = num * (matrix.Yx * matrix.Zy - matrix.Yy * matrix.Zx);
            result.Xy = -num * (matrix.Xy * matrix.Zz - matrix.Xz * matrix.Zy);
            result.Yy = num * (matrix.Xx * matrix.Zz - matrix.Xz * matrix.Zx);
            result.Zy = -num * (matrix.Xx * matrix.Zy - matrix.Xy * matrix.Zx);
            result.Xz = num * (matrix.Xy * matrix.Yz - matrix.Yy * matrix.Xz);
            result.Yz = -num * (matrix.Xx * matrix.Yz - matrix.Xz * matrix.Yx);
            result.Zz = num * (matrix.Xx * matrix.Yy - matrix.Xy * matrix.Yx);
            return result;
        }

        /// <summary>
        /// 矩阵行列式值
        /// </summary>
        public static double GetDeterminant(this Matrix3x3 matrix)
        {
            return matrix.Xx * (matrix.Yy * matrix.Zz - matrix.Yz * matrix.Zy) - matrix.Xy * (matrix.Yx * matrix.Zz - matrix.Yz * matrix.Zx) + matrix.Xz * (matrix.Yx * matrix.Zy - matrix.Yy * matrix.Zx);
        }

        /// <summary>Get the X-axis of the orientation (unit vector)</summary>
        public static Vector3d GetAxisX(this Matrix3x3 matrix)
        {
            return new Vector3d(matrix.Xx, matrix.Xy, matrix.Xz);
        }

        /// <summary>Set the X-axis of the orientation (unit vector)</summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        public static void SetAxisX(this Matrix3x3 matrix, Vector3d vector)
        {
            matrix.Xx = vector.X;
            matrix.Xy = vector.Y;
            matrix.Xz = vector.Z;
        }

        /// <summary>Get the Y-axis of the orientation (unit vector)</summary>  
        public static Vector3d GetAxisY(this Matrix3x3 matrix)
        {
            return new Vector3d(matrix.Yx, matrix.Yy, matrix.Yz);
        }

        /// <summary>Set the Y-axis of the orientation (unit vector)</summary>  
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        public static void SetAxisY(this Matrix3x3 matrix, Vector3d vector)
        {
            matrix.Yx = vector.X;
            matrix.Yy = vector.Y;
            matrix.Yz = vector.Z;
        }

        /// <summary>Get the Z-axis of the orientation (unit vector)</summary>
        public static Vector3d GetAxisZ(this Matrix3x3 matrix)
        {
            return new Vector3d(matrix.Zx, matrix.Zy, matrix.Zz);
        }

        /// <summary>Set the Z-axis of the orientation (unit vector)</summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        public static void SetAxisZ(this Matrix3x3 matrix, Vector3d vector)
        {
            matrix.Zx = vector.X;
            matrix.Zy = vector.Y;
            matrix.Zz = vector.Z;
        }

        /// <summary>
        /// 正则化
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix3x3 Unitize(this Matrix3x3 matrix)
        {
            double[] array = matrix.ToArray();
            Globals.TheUfSession.Mtx3.OrthoNormalize(array);
            return new Matrix3x3() { Xx = array[0], Xy = array[1], Xz = array[2], Yx = array[3], Yy = array[4], Yz = array[5], Zx = array[6], Zy = array[7], Zz = array[8] };
        }

        /// <summary>
        /// 与矢量相乘
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3d Multiply(this Matrix3x3 matrix, Vector3d vector)
        {
            Vector3d result;
            result.X = matrix.Xx * vector.X + matrix.Yx * vector.Y + matrix.Zx * vector.Z;
            result.Y = matrix.Xy * vector.X + matrix.Yy * vector.Y + matrix.Zy * vector.Z;
            result.Z = matrix.Xz * vector.X + matrix.Yz * vector.Y + matrix.Zz * vector.Z;
            return result;
        }

        /// <summary>
        /// 与点相乘
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Point3d Multiply(this Matrix3x3 matrix, Point3d point)
        {
            Point3d result;
            result.X = matrix.Xx * point.X + matrix.Yx * point.Y + matrix.Zx * point.Z;
            result.Y = matrix.Xy * point.X + matrix.Yy * point.Y + matrix.Zy * point.Z;
            result.Z = matrix.Xz * point.X + matrix.Yz * point.Y + matrix.Zz * point.Z;
            return result;
        }

        /// <summary>
        /// 判断两个矩阵是否相等
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        public static bool EqualsTo(this Matrix3x3 matrix1, Matrix3x3 matrix2)
        {
            if (Math.Abs(matrix1.Xx - matrix2.Xx) > 0.01)
                return false;

            if (Math.Abs(matrix1.Xy - matrix2.Xy) > 0.01)
                return false;

            if (Math.Abs(matrix1.Xz - matrix2.Xz) > 0.01)
                return false;

            if (Math.Abs(matrix1.Yx - matrix2.Yx) > 0.01)
                return false;

            if (Math.Abs(matrix1.Yy - matrix2.Yy) > 0.01)
                return false;

            if (Math.Abs(matrix1.Yz - matrix2.Yz) > 0.01)
                return false;

            if (Math.Abs(matrix1.Zx - matrix2.Zx) > 0.01)
                return false;

            if (Math.Abs(matrix1.Zy - matrix2.Zy) > 0.01)
                return false;

            if (Math.Abs(matrix1.Zz - matrix2.Zz) > 0.01)
                return false;

            return true;
        }

        /// <summary>
        /// 将矩阵转换为数组
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double[] ToArray(this Matrix3x3 matrix) => new double[] { matrix.Xx, matrix.Xy, matrix.Xz, matrix.Yx, matrix.Yy, matrix.Yz, matrix.Zx, matrix.Zy, matrix.Zz };

        /// <summary>
        /// 矩阵转置
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix3x3 Transpose(this Matrix3x3 matrix)
        {
            double tempNum = matrix.Xy;
            matrix.Xy = matrix.Yx;
            matrix.Yx = tempNum;
            tempNum = matrix.Xz;
            matrix.Xz = matrix.Zx;
            matrix.Zx = tempNum;
            tempNum = matrix.Yz;
            matrix.Yz = matrix.Zy;
            matrix.Zy = tempNum;

            return matrix;
        }
    }
}
