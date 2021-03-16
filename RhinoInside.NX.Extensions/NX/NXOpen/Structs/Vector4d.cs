using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
    public struct Vector4d
    {
        #region 字段
        public double X;

        public double Y;

        public double Z;

        public double W;
        #endregion

        #region 属性
        /// <summary>
        /// 判断向量是否包含非数字项
        /// </summary>
        public bool IsNaN
        {
            get
            {
                return double.IsNaN(X) || double.IsNaN(Y) || double.IsNaN(Z) || double.IsNaN(W);
            }
        }
        #endregion

        #region 构造函数
        public Vector4d(double x0, double y0, double z0, double w0)
        {
            X = x0;
            Y = y0;
            Z = z0;
            W = w0;
        }

        // Token: 0x0600025E RID: 606 RVA: 0x0000F743 File Offset: 0x0000D943
        public Vector4d(double x0, double y0, double z0)
        {
            X = x0;
            Y = y0;
            Z = z0;
            W = 0.0;
        }

        // Token: 0x0600025F RID: 607 RVA: 0x0000F76C File Offset: 0x0000D96C
        public Vector4d(double[] arr)
        {
            X = arr[0];
            Y = arr[1];
            Z = arr[2];
            if (arr.Length > 3)
                W = arr[3];
            else
                W = 1.0;
        }

        public Vector4d(Vector3d vec, double w0)
        {
            X = vec.X;
            Y = vec.Y;
            Z = vec.Z;
            W = w0;
        }

        public Vector4d(Vector3d vec)
        {
            X = vec.X;
            Y = vec.Y;
            Z = vec.Z;
            W = 0.0;
        }
        #endregion

        #region 重载函数
        public static Vector4d operator *(double lhs, Vector4d rhs)
        {
            return new Vector4d(rhs.X * lhs, rhs.Y * lhs, rhs.Z * lhs, rhs.W * lhs);
        }

        // Token: 0x0600026E RID: 622 RVA: 0x0000FAEC File Offset: 0x0000DCEC
        public static Vector4d operator /(Vector4d lhs, double rhs)
        {
            double num = 1.0 / rhs;
            return new Vector4d(lhs.X * num, lhs.Y * num, lhs.Z * num, lhs.W * num);
        }

        // Token: 0x0600026F RID: 623 RVA: 0x0000FB30 File Offset: 0x0000DD30
        public Vector4d Divide(double rhs)
        {
            double num = 1.0 / rhs;
            return new Vector4d(X * num, Y * num, Z * num, W * num);
        }

        // Token: 0x06000270 RID: 624 RVA: 0x0000FB74 File Offset: 0x0000DD74
        public double Dot(Vector4d vec)
        {
            return X * vec.X + Y * vec.Y + Z * vec.Z + W * vec.W;
        }
        public double this[int index]
        {
            get
            {
                double result;
                if (index >= 2)
                {
                    if (index != 2)
                        result = W;
                    else
                        result = Z;
                }
                else
                {
                    if (index != 0)
                        result = Y;
                    else
                        result = X;
                }
                return result;
            }
            set
            {
                if (index < 2)
                {
                    if (index == 0)
                        X = value;
                    else
                        Y = value;
                }
                else
                {
                    if (index == 2)
                        Z = value;
                    else
                        W = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstVector"></param>
        /// <param name="secondVector"></param>
        /// <returns></returns>
        public static bool operator ==(Vector4d firstVector, Vector4d secondVector)
        {
            return firstVector.EqualsTo(secondVector);
        }

        // Token: 0x06000262 RID: 610 RVA: 0x0000F838 File Offset: 0x0000DA38
        public static bool operator !=(Vector4d lhs, Vector4d rhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator Vector3d(Vector4d vector)
        {
            return new Vector3d(vector.X, vector.Y, vector.Z);
        }

        // Token: 0x06000263 RID: 611 RVA: 0x0000F854 File Offset: 0x0000DA54
        public override bool Equals(object obj)
        {
            return obj is Vector4d && this == (Vector4d)obj;
        }

        // Token: 0x06000264 RID: 612 RVA: 0x0000F884 File Offset: 0x0000DA84
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }


        public static explicit operator Vector4d(double[] arg)
        {
            return new Vector4d(arg);
        }


        public static Vector4d operator +(Vector4d lhs, Vector4d rhs)
        {
            return new Vector4d(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z, lhs.W + rhs.W);
        }

        public override string ToString()
        {
            return $"[{X},{Y},{Z},{W}]";
        }
        #endregion

        #region 普通方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector4d operator -(Vector4d lhs, Vector4d rhs)
        {
            return new Vector4d(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z, lhs.W - rhs.W);
        }

        /// <summary>
        /// 向量相减
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static Vector4d operator -(Vector4d arg)
        {
            return new Vector4d(-arg.X, -arg.Y, -arg.Z, -arg.W);
        }

        /// <summary>
        /// 数字与向量相乘
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Vector4d operator *(Vector4d vector, double scale)
        {
            return new Vector4d(vector.X * scale, vector.Y * scale, vector.Z * scale, vector.W * scale);
        }

        /// <summary>
        /// 向量长度
        /// </summary>
        public double Length
        {
            get => Math.Sqrt(X * X + Y * Y + Z * Z + W * W);

        }

        /// <summary>
        /// 向量正则化
        /// </summary>
        public void Normalize()
        {
            double num = 1.0 / Length;
            X *= num;
            Y *= num;
            Z *= num;
            W *= num;
        }

        /// <summary>
        /// 判断两个向量的差值是否大致相同
        /// </summary>
        /// <param name="vec">要判断的向量</param>
        /// <returns></returns>
        public bool EqualsTo(Vector4d vec)
        {
            return EqualsTo(vec, Globals.DistanceTolerance);
        }

        /// <summary>
        /// 判断两个向量的差值是否在给定的公差范围内
        /// </summary>
        /// <param name="vec">要判断的向量</param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool EqualsTo(Vector4d vec, double tolerance)
        {
            return Math.Abs(X - vec.X) < tolerance && Math.Abs(Y - vec.Y) < tolerance && Math.Abs(Z - vec.Z) < tolerance && Math.Abs(W - vec.W) < tolerance;
        }

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
        /// 转换为 3D 向量
        /// </summary>
        /// <returns></returns>
        public Vector3d ToVector3d()
        {
            return new Vector3d(X, Y, Z);
        }

        public static Vector4d operator *(Vector4d point, Matrix4x4 matrix)
        {
            return new Vector4d(MatrixMath.Multiply(point.ToArray(), matrix.To2DArray()));
        }
        #endregion
    }
}
