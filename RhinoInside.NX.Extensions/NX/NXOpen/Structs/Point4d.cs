using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public struct Point4d
    {
        #region 字段
        public double X;

        public double Y;

        public double Z;

        public double W;
        #endregion

        #region 构造函数
        public Point4d(double x0, double y0, double z0, double w0)
        {
            X = x0;
            Y = y0;
            Z = z0;
            W = w0;
        }

        // Token: 0x0600025E RID: 606 RVA: 0x0000F743 File Offset: 0x0000D943
        public Point4d(double x0, double y0, double z0)
        {
            X = x0;
            Y = y0;
            Z = z0;
            W = 1.0;
        }

        // Token: 0x0600025F RID: 607 RVA: 0x0000F76C File Offset: 0x0000D96C
        public Point4d(double[] arr)
        {
            X = arr[0];
            Y = arr[1];
            Z = arr[2];
            if (arr.Length > 3)
                W = arr[3];
            else
                W = 1.0;
        }

        public Point4d(Point3d pt, double w0)
        {
            X = pt.X;
            Y = pt.Y;
            Z = pt.Z;
            W = w0;
        }

        public Point4d(Point3d pt)
        {
            X = pt.X;
            Y = pt.Y;
            Z = pt.Z;
            W = 1.0;
        }
        #endregion

        /// <summary>
        /// 转换为数组
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            return new double[]
            {
                X,
                Y,
                Z,
                W
            };
        }

        /// <summary>
        /// 转换为数组
        /// </summary>
        /// <returns></returns>
        public Point3d ToPoint3d()
        {
            return new Point3d(X, Y, Z);
        }

        public static Point4d operator *(Point4d point, Matrix4x4 matrix)
        {
            return new Point4d(MatrixMath.Multiply(point.ToArray(), matrix.To2DArray()));
        }
    }
}
