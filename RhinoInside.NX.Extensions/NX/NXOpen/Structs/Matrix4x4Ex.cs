using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Globalization;
using NXOpen;
using PLMComponents.Parasolid.PK_.Unsafe;

namespace RhinoInside.NX.Extensions
{
    /// <summary>
    /// 用于表示对象的变换，形式为 Matrix4x4
    /// </summary>
    public static class Matrix4x4Ex
    {
        static Matrix4x4Ex() => AppDomain.CurrentDomain.AssemblyResolve += Globals.ManagedLibraryResolver;

        /// <summary>
        /// 单位矩阵
        /// </summary>
        public static Matrix4x4 Identity
        {
            get => new Matrix4x4
            {
                Rxx = 1.0,
                Rxy = 0.0,
                Rxz = 0.0,
                Ryx = 0.0,
                Ryy = 1.0,
                Ryz = 0.0,
                Rzx = 0.0,
                Rzy = 0.0,
                Rzz = 0.0,
                Ss = 1.0,
                Sx = 0.0,
                Sy = 0.0,
                Sz = 0.0,
                Xt = 0.0,
                Yt = 0.0,
                Zt = 0.0
            };
        }

        public static Vector4d GetAxisX(this Matrix4x4 matrix)
        {
            return new Vector4d(matrix.Rxx, matrix.Rxy, matrix.Rxz, 0.0);
        }

        public static void SetAxisX(this Matrix4x4 matrix, Vector4d vector)
        {
            matrix.Rxx = vector.X;
            matrix.Rxy = vector.Y;
            matrix.Rxz = vector.Z;
            matrix.Sx = vector.W;
        }

        public static Vector4d GetAxisY(this Matrix4x4 matrix)
        {
            return new Vector4d(matrix.Ryx, matrix.Ryy, matrix.Ryz, 0.0);
        }

        public static void SetAxisY(this Matrix4x4 matrix, Vector4d vector)
        {
            matrix.Ryx = vector.X;
            matrix.Ryy = vector.Y;
            matrix.Ryz = vector.Z;
            matrix.Sy = vector.W;
        }

        public static Vector4d GetAxisZ(this Matrix4x4 matrix)
        {
            return new Vector4d(matrix.Rzx, matrix.Rzy, matrix.Rzz, 0.0);
        }

        public static void SetAxisZ(this Matrix4x4 matrix, Vector4d vector)
        {
            matrix.Rzx = vector.X;
            matrix.Rzy = vector.Y;
            matrix.Rzz = vector.Z;
            matrix.Sz = vector.W;
        }

        public static Vector4d GetAxisT(this Matrix4x4 matrix)
        {
            return new Vector4d(matrix.Xt, matrix.Yt, matrix.Zt, matrix.Ss);
        }

        /// <summary>
        /// 矩阵行列式值
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double GetDeterminant(this Matrix4x4 matrix)
        {
            return matrix.Xt * matrix.Rzy * matrix.Ryz * matrix.Sx - matrix.Rzx * matrix.Yt * matrix.Ryz * matrix.Sx - matrix.Xt * matrix.Ryy * matrix.Rzz * matrix.Sx + matrix.Ryx * matrix.Yt * matrix.Rzz * matrix.Sx + matrix.Rzx * matrix.Ryy * matrix.Zt * matrix.Sx - matrix.Ryx * matrix.Rzy * matrix.Zt * matrix.Sx - matrix.Xt * matrix.Rzy * matrix.Rxz * matrix.Sy + matrix.Rzx * matrix.Yt * matrix.Rxz * matrix.Sy + matrix.Xt * matrix.Rxy * matrix.Rzz * matrix.Sy - matrix.Rxx * matrix.Yt * matrix.Rzz * matrix.Sy - matrix.Rzx * matrix.Rxy * matrix.Zt * matrix.Sy + matrix.Rxx * matrix.Rzy * matrix.Zt * matrix.Sy + matrix.Xt * matrix.Ryy * matrix.Rxz * matrix.Sz - matrix.Ryx * matrix.Yt * matrix.Rxz * matrix.Sz - matrix.Xt * matrix.Rxy * matrix.Ryz * matrix.Sz + matrix.Rxx * matrix.Yt * matrix.Ryz * matrix.Sz + matrix.Ryx * matrix.Rxy * matrix.Zt * matrix.Sz - matrix.Rxx * matrix.Ryy * matrix.Zt * matrix.Sz - matrix.Rzx * matrix.Ryy * matrix.Rxz * matrix.Ss + matrix.Ryx * matrix.Rzy * matrix.Rxz * matrix.Ss + matrix.Rzx * matrix.Rxy * matrix.Ryz * matrix.Ss - matrix.Rxx * matrix.Rzy * matrix.Ryz * matrix.Ss - matrix.Ryx * matrix.Rxy * matrix.Rzz * matrix.Ss + matrix.Rxx * matrix.Ryy * matrix.Rzz * matrix.Ss;
        }

        /// <summary>
        /// 获取逆矩阵
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix4x4 GetInvert(this Matrix4x4 matrix)
        {
            double num = 1.0 / matrix.GetDeterminant();
            Matrix4x4 result = Identity;
            result.Rxx = num * (matrix.Rzy * matrix.Zt * matrix.Sy - matrix.Yt * matrix.Rzz * matrix.Sy + matrix.Yt * matrix.Ryz * matrix.Sz - matrix.Ryy * matrix.Zt * matrix.Sz - matrix.Rzy * matrix.Ryz * matrix.Ss + matrix.Ryy * matrix.Rzz * matrix.Ss);
            result.Ryx = num * (matrix.Xt * matrix.Rzz * matrix.Sy - matrix.Rzx * matrix.Zt * matrix.Sy - matrix.Xt * matrix.Ryz * matrix.Sz + matrix.Ryx * matrix.Zt * matrix.Sz + matrix.Rzx * matrix.Ryz * matrix.Ss - matrix.Ryx * matrix.Rzz * matrix.Ss);
            result.Rzx = num * (matrix.Rzx * matrix.Yt * matrix.Sy - matrix.Xt * matrix.Rzy * matrix.Sy + matrix.Xt * matrix.Ryy * matrix.Sz - matrix.Ryx * matrix.Yt * matrix.Sz - matrix.Rzx * matrix.Ryy * matrix.Ss + matrix.Ryx * matrix.Rzy * matrix.Ss);
            result.Xt = num * (matrix.Xt * matrix.Rzy * matrix.Ryz - matrix.Rzx * matrix.Yt * matrix.Ryz - matrix.Xt * matrix.Ryy * matrix.Rzz + matrix.Ryx * matrix.Yt * matrix.Rzz + matrix.Rzx * matrix.Ryy * matrix.Zt - matrix.Ryx * matrix.Rzy * matrix.Zt);
            result.Rxy = num * (matrix.Yt * matrix.Rzz * matrix.Sx - matrix.Rzy * matrix.Zt * matrix.Sx - matrix.Yt * matrix.Rxz * matrix.Sz + matrix.Rxy * matrix.Zt * matrix.Sz + matrix.Rzy * matrix.Rxz * matrix.Ss - matrix.Rxy * matrix.Rzz * matrix.Ss);
            result.Ryy = num * (matrix.Rzx * matrix.Zt * matrix.Sx - matrix.Xt * matrix.Rzz * matrix.Sx + matrix.Xt * matrix.Rxz * matrix.Sz - matrix.Rxx * matrix.Zt * matrix.Sz - matrix.Rzx * matrix.Rxz * matrix.Ss + matrix.Rxx * matrix.Rzz * matrix.Ss);
            result.Rzy = num * (matrix.Xt * matrix.Rzy * matrix.Sx - matrix.Rzx * matrix.Yt * matrix.Sx - matrix.Xt * matrix.Rxy * matrix.Sz + matrix.Rxx * matrix.Yt * matrix.Sz + matrix.Rzx * matrix.Rxy * matrix.Ss - matrix.Rxx * matrix.Rzy * matrix.Ss);
            result.Yt = num * (matrix.Rzx * matrix.Yt * matrix.Rxz - matrix.Xt * matrix.Rzy * matrix.Rxz + matrix.Xt * matrix.Rxy * matrix.Rzz - matrix.Rxx * matrix.Yt * matrix.Rzz - matrix.Rzx * matrix.Rxy * matrix.Zt + matrix.Rxx * matrix.Rzy * matrix.Zt);
            result.Rxz = num * (matrix.Ryy * matrix.Zt * matrix.Sx - matrix.Yt * matrix.Ryz * matrix.Sx + matrix.Yt * matrix.Rxz * matrix.Sy - matrix.Rxy * matrix.Zt * matrix.Sy - matrix.Ryy * matrix.Rxz * matrix.Ss + matrix.Rxy * matrix.Ryz * matrix.Ss);
            result.Ryz = num * (matrix.Xt * matrix.Ryz * matrix.Sx - matrix.Ryx * matrix.Zt * matrix.Sx - matrix.Xt * matrix.Rxz * matrix.Sy + matrix.Rxx * matrix.Zt * matrix.Sy + matrix.Ryx * matrix.Rxz * matrix.Ss - matrix.Rxx * matrix.Ryz * matrix.Ss);
            result.Rzz = num * (matrix.Ryx * matrix.Yt * matrix.Sx - matrix.Xt * matrix.Ryy * matrix.Sx + matrix.Xt * matrix.Rxy * matrix.Sy - matrix.Rxx * matrix.Yt * matrix.Sy - matrix.Ryx * matrix.Rxy * matrix.Ss + matrix.Rxx * matrix.Ryy * matrix.Ss);
            result.Zt = num * (matrix.Xt * matrix.Ryy * matrix.Rxz - matrix.Ryx * matrix.Yt * matrix.Rxz - matrix.Xt * matrix.Rxy * matrix.Ryz + matrix.Rxx * matrix.Yt * matrix.Ryz + matrix.Ryx * matrix.Rxy * matrix.Zt - matrix.Rxx * matrix.Ryy * matrix.Zt);
            result.Sx = num * (matrix.Rzy * matrix.Ryz * matrix.Sx - matrix.Ryy * matrix.Rzz * matrix.Sx - matrix.Rzy * matrix.Rxz * matrix.Sy + matrix.Rxy * matrix.Rzz * matrix.Sy + matrix.Ryy * matrix.Rxz * matrix.Sz - matrix.Rxy * matrix.Ryz * matrix.Sz);
            result.Sy = num * (matrix.Ryx * matrix.Rzz * matrix.Sx - matrix.Rzx * matrix.Ryz * matrix.Sx + matrix.Rzx * matrix.Rxz * matrix.Sy - matrix.Rxx * matrix.Rzz * matrix.Sy - matrix.Ryx * matrix.Rxz * matrix.Sz + matrix.Rxx * matrix.Ryz * matrix.Sz);
            result.Sz = num * (matrix.Rzx * matrix.Ryy * matrix.Sx - matrix.Ryx * matrix.Rzy * matrix.Sx - matrix.Rzx * matrix.Rxy * matrix.Sy + matrix.Rxx * matrix.Rzy * matrix.Sy + matrix.Ryx * matrix.Rxy * matrix.Sz - matrix.Rxx * matrix.Ryy * matrix.Sz);
            result.Ss = num * (matrix.Ryx * matrix.Rzy * matrix.Rxz - matrix.Rzx * matrix.Ryy * matrix.Rxz + matrix.Rzx * matrix.Rxy * matrix.Ryz - matrix.Rxx * matrix.Rzy * matrix.Ryz - matrix.Ryx * matrix.Rxy * matrix.Rzz + matrix.Rxx * matrix.Ryy * matrix.Rzz);
            return result;
        }

        /// <summary>
        /// 用于旋转的部分
        /// </summary>
        public static Matrix3x3 GetRotation(this Matrix4x4 matrix)
        {
            return new Matrix3x3() { Xx = matrix.Rxx, Xy = matrix.Rxy, Xz = matrix.Rxz, Yx = matrix.Ryx, Yy = matrix.Ryy, Yz = matrix.Ryz, Zx = matrix.Rzx, Zy = matrix.Rzy, Zz = matrix.Rzz };
        }

        public static Vector3d GetTranslation(this Matrix4x4 matrix)
        {
            return matrix.GetAxisT();
        }

        /// <summary>
        /// 转换为二维数组
        /// </summary>
        /// <returns></returns>
        public static double[,] To2DArray(this Matrix4x4 matrix)
        {
            double[,] result = new double[4, 4];

            result[0, 0] = matrix.Rxx;
            result[0, 1] = matrix.Rxy;
            result[0, 2] = matrix.Rxz;
            result[0, 3] = matrix.Sx;

            result[1, 0] = matrix.Ryx;
            result[1, 1] = matrix.Ryy;
            result[1, 2] = matrix.Ryz;
            result[1, 3] = matrix.Sy;

            result[2, 0] = matrix.Rzx;
            result[2, 1] = matrix.Rzy;
            result[2, 2] = matrix.Rzz;
            result[2, 3] = matrix.Sz;

            result[3, 0] = matrix.Xt;
            result[3, 1] = matrix.Yt;
            result[3, 2] = matrix.Zt;
            result[3, 3] = matrix.Ss;

            return result;
        }

        /// <summary>
        /// 创建平移矩阵
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Matrix4x4 Create(Vector3d vector)
        {
            var mtx = Identity;
            mtx.Xt = vector.X;
            mtx.Yt = vector.Y;
            mtx.Zt = vector.Z;

            return mtx;
        }

        public static Matrix4x4 Create(Point3d point, Matrix3x3 matrix)
        {
            return new Matrix4x4
            {
             Rxx = matrix.Xx,
             Rxy = matrix.Xy,
             Rxz = matrix.Xz,
             Sx = 0.0,
             Ryx = matrix.Yx,
             Ryy = matrix.Yy,
             Ryz = matrix.Yz,
             Sy = 0.0,
             Rzx = matrix.Zx,
             Rzy = matrix.Zy,
             Rzz = matrix.Zz,
             Sz = 0.0,
             Xt = point.X,
             Yt = point.Y,
             Zt = point.Z,
             Ss = 1.0,
            };
        }

        /// <summary>
        /// 创建坐标系
        /// </summary>
        /// <param name="isVisible"></param>
        /// <returns></returns>
        public static Tag CreateCsys(this Matrix4x4 matrix, bool isVisible)
        {
            double[] translationArray = matrix.GetTranslation().ToArray();
            double[] rotationArray = matrix.GetRotation().ToArray();
            Globals.TheUfSession.Mtx3.OrthoNormalize(rotationArray);

            Globals.TheUfSession.Csys.CreateMatrix(rotationArray, out Tag mtxTag);
            Tag result;
            if (isVisible)
            {
                Globals.TheUfSession.Csys.CreateCsys(translationArray, mtxTag, out result);
            }
            else
            {
                Globals.TheUfSession.Csys.CreateTempCsys(translationArray, mtxTag, out result);
            }
            return result;
        }

        /// <summary>
        /// 点的坐标转换
        /// </summary>
        /// <param name="originalPoint3d"></param>
        /// <param name="transformationMatrix">变换矩阵</param>
        /// <returns></returns>
        public static Point3d Multiply(this Matrix4x4 matrix, Point3d originalPoint3d)
        {
            Point3d result;
            result.X = matrix.Rxx * originalPoint3d.X + matrix.Ryx * originalPoint3d.Y + matrix.Rzx * originalPoint3d.Z + matrix.Xt;
            result.Y = matrix.Rxy * originalPoint3d.X + matrix.Ryy * originalPoint3d.Y + matrix.Rzy * originalPoint3d.Z + matrix.Yt;
            result.Z = matrix.Rxz * originalPoint3d.X + matrix.Ryz * originalPoint3d.Y + matrix.Rzz * originalPoint3d.Z + matrix.Zt;
            return result;
        }
    }
}
