using NXOpen.UF;
using System;
using NXOpen;
using static RhinoInside.NX.Extensions.Globals;

namespace RhinoInside.NX.Extensions
{
    /// <summary>
    /// NXOpen.Vector3d 的扩展类
    /// </summary>
    public static partial class Vector3dEx
    {
        #region 创建 Vector3d
        /// <summary>
        /// 创建 Vector3d
        /// </summary>
        /// <param name="toPoint3d"></param>
        /// <param name="fromPoint3d"></param>
        /// <returns></returns>
        public static Vector3d Create(Point3d toPoint3d, Point3d fromPoint3d)
        {
            return new Vector3d(toPoint3d.X - fromPoint3d.X, toPoint3d.Y - fromPoint3d.Y, toPoint3d.Z - fromPoint3d.Z);
        }

        /// <summary>
        /// 创建 Vector3d
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3d Create(double[] vector)
        {
            return new Vector3d(vector[0], vector[1], vector[2]);
        }
        #endregion

        public static readonly Vector3d AxisX = new Vector3d(1.0, 0.0, 0.0);

        public static readonly Vector3d AxisY = new Vector3d(0.0, 1.0, 0.0);

        public static readonly Vector3d AxisZ = new Vector3d(0.0, 0.0, 1.0);

        public static readonly Vector3d Zero = new Vector3d(0.0, 0.0, 0.0);

        public static readonly Vector3d AxisNegativeX = new Vector3d(-1.0, 0.0, 0.0);

        public static readonly Vector3d AxisNegativeY = new Vector3d(0.0, -1.0, 0.0);

        public static readonly Vector3d AxisNegativeZ = new Vector3d(0.0, 0.0, -1.0);

        /// <summary>
        /// 计算矢量夹角，返回的角度为锐角，返回值为弧度
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static double AngleTo(this Vector3d vector1, Vector3d vector2)
        {
            double smallAngle;
            double largeAngle;
            TheUfSession.Modl.AskVectorAngle(vector1.ToArray(), vector2.ToArray(), out smallAngle, out largeAngle);

            if (smallAngle > Math.PI / 2)
                smallAngle = Math.PI - smallAngle;

            return smallAngle;
        }

        /// <summary>
        /// 求向量与向量的点积
        /// </summary>
        /// <param name="vec1">第一个向量</param>
        /// <param name="vec2">第二个向量</param>
        /// <returns></returns>
        public static double DotProduct(this Vector3d vec1, Vector3d vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z * vec2.Z;
        }

        /// <summary>
        /// 求两个矢量的和
        /// </summary>
        /// <param name="vec1">第一个矢量</param>
        /// <param name="vec2">第二个矢量</param>
        /// <returns></returns>
        public static Vector3d Add(this Vector3d vec1, Vector3d vec2)
        {
            return new Vector3d(vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z);
        }

        /// <summary>
        /// 求矢量的模
        /// </summary>
        /// <param name="vector">矢量</param>
        /// <returns></returns>
        public static double GetLength(this Vector3d vector)
        {
            return Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2) + Math.Pow(vector.Z, 2));
        }

        /// <summary>
        /// 将矢量转换为点
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Point3d ToPoint3d(this Vector3d vector)
        {
            return new Point3d(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// 将矢量与常数相乘
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Vector3d Multiply(this Vector3d vector, double scale)
        {
            return new Vector3d(vector.X * scale, vector.Y * scale, vector.Z * scale);
        }

        /// <summary>
        /// 判断两个向量是否平行
        /// </summary>
        /// <param name="vector">第一个向量</param>
        /// <param name="target">第二个向量</param>
        /// <returns></returns>
        public static bool IsParallel(this Vector3d vector, Vector3d target) => vector.CrossProduct(target).IsZero();

        /// <summary>
        /// 求两个基准平面的相交的方向矢量
        /// </summary>
        /// <param name="plane1Origin"></param>
        /// <param name="plane1Normal"></param>
        /// <param name="plane2Origin"></param>
        /// <param name="plane2Normal"></param>
        public static Vector3d Intersect(Point3d plane1Origin, Vector3d plane1Normal, Point3d plane2Origin, Vector3d plane2Normal)
        {
            double[] perpendicularVectorInDouble = new double[3];
            TheUfSession.Vec3.Cross(plane1Normal.ToArray(), plane2Normal.ToArray(), perpendicularVectorInDouble);

            var vectorStartPoint3d = plane1Origin.Project(plane2Origin, plane2Normal);

            var vectorSecondPoint3d = plane1Origin.Move(perpendicularVectorInDouble.ToVector3d(), 100).Project(plane2Origin, plane2Normal);

            var vector = vectorSecondPoint3d.Subtract(vectorStartPoint3d);

            double[] unit_vec = new double[3];
            TheUfSession.Vec3.Unitize(vector.ToArray(), 0.01, out _, unit_vec);

            return unit_vec.ToVector3d();
        }

        /// <summary>
        /// 两个向量的叉积
        /// </summary>
        /// <param name="vector1">第一个向量</param>
        /// <param name="vector2">第二个向量</param>
        /// <returns></returns>
        public static Vector3d CrossProduct(this Vector3d vector1, Vector3d vector2)
        {
            return new Vector3d(vector1.Y * vector2.Z - vector1.Z * vector2.Y, vector1.Z * vector2.X - vector1.X * vector2.Z, vector1.X * vector2.Y - vector1.Y * vector2.X);
        }

        /// <summary>
        /// 判断向量的三个分量值是否为零
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static bool IsZero(this Vector3d vector)
        {
            return Math.Abs(vector.X) < Globals.DistanceTolerance && Math.Abs(vector.Y) < Globals.DistanceTolerance && Math.Abs(vector.Z) < Globals.DistanceTolerance;
        }

        /// <summary>
        /// 将数组转换为 Vector3d
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3d ToVector3d(this double[] vector)
        {
            if (vector.Length != 3)
            {
                "矢量格式不正确，转换可能存在问题。".ListingWindowWrite();
                return new Vector3d();
            }
            else
                return new Vector3d(vector[0], vector[1], vector[2]);
        }

        /// <summary>
        /// 求两个矢量的差
        /// </summary>
        /// <param name="vec1">第一个矢量</param>
        /// <param name="vec2">第二个矢量</param>
        /// <returns></returns>
        public static Vector3d Subtract(this Vector3d vec1, Vector3d vec2)
        {
            return new Vector3d(vec1.X - vec2.X, vec1.Y - vec2.Y, vec1.Z - vec2.Z);
        }

        /// <summary>
        /// 向量单位化
        /// </summary>
        /// <param name="vector">
        /// 要计算的向量
        /// </param>
        /// <returns></returns>
        [Obsolete("2021-01-15，请使用 GetUnitVector")]
        public static Vector3d Unitize(this Vector3d vector)
        {
            double original_length = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);

            return new Vector3d(vector.X / original_length, vector.Y / original_length, vector.Z / original_length);
        }

        /// <summary>
        /// 向量单位化
        /// </summary>
        /// <param name="vector">
        /// 要计算的向量
        /// </param>
        /// <returns></returns>
        public static void Normalize(this ref Vector3d vector)
        {
            vector = vector.GetUnitVector();
        }

        /// <summary>
        /// 向量单位化
        /// </summary>
        /// <param name="vector">
        /// 要计算的向量
        /// </param>
        /// <returns></returns>
        public static Vector3d GetUnitVector(this Vector3d vector)
        {
            double original_length = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);

            return new Vector3d(vector.X / original_length, vector.Y / original_length, vector.Z / original_length);
        }

        /// <summary>
        /// 将矢量反向
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3d Reverse(this Vector3d vector)
        {
            return new Vector3d(-vector.X, -vector.Y, -vector.Z);
        }

        /// <summary>
        /// 将 Vector3d 转换为数组
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static double[] ToArray(this Vector3d vector) => new double[] { vector.X, vector.Y, vector.Z };

        public static Vector4d ToVector4d(this Vector3d vector) => new Vector4d(vector);
    }
}
