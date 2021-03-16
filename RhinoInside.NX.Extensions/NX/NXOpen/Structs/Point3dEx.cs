using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using static RhinoInside.NX.Extensions.Globals;

namespace RhinoInside.NX.Extensions
{
    /// <summary>
    /// 存放 Point3d 的扩展方法
    /// </summary>
    public static partial class Point3dEx
    {
        #region 创建 Point3d
        public static Point3d Create(double[] point)=> point.ToPoint3d();
        #endregion

        #region point3d 静态方法
        /// <summary>
        /// 转换为 double 数组
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double[] ToArray(this Point3d point) => new double[] { point.X, point.Y, point.Z };

        /// <summary>
        /// 绕旋转轴旋转点
        /// </summary>
        /// <param name="orginalPoint3d"></param>
        /// <param name="rotateVector"></param>
        /// <param name="rotateOrigin"></param>
        /// <param name="rotateAngle">旋转角度值（非弧度）</param>
        /// <returns></returns>
        public static Point3d Rotate(this Point3d orginalPoint3d, Point3d rotateOrigin, Vector3d rotateVector, double rotateAngle)
        {
            rotateVector = rotateVector.GetUnitVector();

            double u = rotateVector.X, v = rotateVector.Y, w = rotateVector.Z;
            double a = rotateOrigin.X, b = rotateOrigin.Y, c = rotateOrigin.Z;
            double angle = rotateAngle * Math.PI / 180;

            double[] rotateMtx = new double[16];

            rotateMtx[0] = u * u + (v * v + w * w) * Math.Cos(angle);

            rotateMtx[1] = u * v * (1 - Math.Cos(angle)) - w * Math.Sin(angle);

            rotateMtx[2] = u * w * (1 - Math.Cos(angle)) + v * Math.Sin(angle);

            rotateMtx[3] = (a * (v * v + w * w) - u * (b * v + c * w)) * (1 - Math.Cos(angle)) + (b * w - c * v) * Math.Sin(angle);

            rotateMtx[4] = u * v * (1 - Math.Cos(angle)) + w * Math.Sin(angle);

            rotateMtx[5] = v * v + (u * u + w * w) * Math.Cos(angle);

            rotateMtx[6] = v * w * (1 - Math.Cos(angle)) - u * Math.Sin(angle);

            rotateMtx[7] = (b * (u * u + w * w) - v * (a * u + c * w)) * (1 - Math.Cos(angle)) + (c * u - a * w) * Math.Sin(angle);

            rotateMtx[8] = u * w * (1 - Math.Cos(angle)) - v * Math.Sin(angle);

            rotateMtx[9] = v * w * (1 - Math.Cos(angle)) + u * Math.Sin(angle);

            rotateMtx[10] = w * w + (u * u + v * v) * Math.Cos(angle);

            rotateMtx[11] = (c * (u * u + v * v) - w * (a * u + b * v)) * (1 - Math.Cos(angle)) + (a * v - b * u) * Math.Sin(angle);

            rotateMtx[12] = 0;

            rotateMtx[13] = 0;

            rotateMtx[14] = 0;

            rotateMtx[15] = 1;

            double[] temVec4 = new double[4];
            temVec4[0] = rotateMtx[0] * orginalPoint3d.X + rotateMtx[1] * orginalPoint3d.Y + rotateMtx[2] * orginalPoint3d.Z + rotateMtx[3];
            temVec4[1] = rotateMtx[4] * orginalPoint3d.X + rotateMtx[5] * orginalPoint3d.Y + rotateMtx[6] * orginalPoint3d.Z + rotateMtx[7];
            temVec4[2] = rotateMtx[8] * orginalPoint3d.X + rotateMtx[9] * orginalPoint3d.Y + rotateMtx[10] * orginalPoint3d.Z + rotateMtx[11];
            temVec4[3] = rotateMtx[12] * orginalPoint3d.X + rotateMtx[13] * orginalPoint3d.Y + rotateMtx[14] * orginalPoint3d.Z + rotateMtx[15];

            double[] temRotatePt = { 0, 0, 0 };
            if (temVec4[3] == 0)
            {
                temRotatePt[0] = temVec4[0];
                temRotatePt[1] = temVec4[1];
                temRotatePt[2] = temVec4[2];
            }
            else
            {
                temRotatePt[0] = temVec4[0] / temVec4[3];
                temRotatePt[1] = temVec4[1] / temVec4[3];
                temRotatePt[2] = temVec4[2] / temVec4[3];
            }

            return temRotatePt.ToPoint3d();
        }

        public static Point3d Scale(this Point3d point, double scale)
        {
            return new Point3d(point.X * scale, point.Y * scale, point.Z * scale);
        }

        /// <summary>
        /// 投影点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="planeOrigin"></param>
        /// <param name="planeNormal"></param>
        /// <returns></returns>
        public static Point3d Project(this Point3d point, Point3d planeOrigin, Vector3d planeNormal) => WorkPart.Features.CreateProjectCurve(point, planeOrigin, planeNormal);

        /// <summary>
        /// 比较两个点的坐标是否相同，使用计算两点距离的方式，小于0.001认为是相同
        /// </summary>
        /// <param name="currentPoint3d"></param>
        /// <param name="point3d2"></param>
        /// <returns></returns>
        public static bool IsSame(this Point3d currentPoint3d, Point3d point3d2)
        {
            if (currentPoint3d.DistanceTo(point3d2) < 0.001)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 坐标转换
        /// </summary>
        /// <param name="originalOrientationMatrix">初始方位矩阵</param>
        /// <param name="targetOrientationMatrix">目标方位矩阵</param>
        /// <param name="originalOrigin3d">初始原点</param>
        /// <param name="targetOrigin3d">目标原点</param>
        /// <param name="pointToMap">要映射的点</param>
        /// <returns></returns>
        public static Point3d Map(this Point3d pointToMap, Matrix3x3 originalOrientationMatrix, Matrix3x3 targetOrientationMatrix, Point3d originalOrigin3d, Point3d targetOrigin3d)
        {
            double[] transformMtx = new double[16];

            TheUfSession.Mtx4.CsysToCsys(originalOrigin3d.ToArray(), new double[] { originalOrientationMatrix.Xx, originalOrientationMatrix.Xy, originalOrientationMatrix.Xz }, new double[] { originalOrientationMatrix.Yx, originalOrientationMatrix.Yy, originalOrientationMatrix.Yz }, targetOrigin3d.ToArray(), new double[] { targetOrientationMatrix.Xx, targetOrientationMatrix.Xy, targetOrientationMatrix.Xz }, new double[] { targetOrientationMatrix.Yx, targetOrientationMatrix.Yy, targetOrientationMatrix.Yz }, transformMtx);

            double[] targetPoint = new double[3];
            TheUfSession.Mtx4.Vec3Multiply(pointToMap.ToArray(), transformMtx, targetPoint);

            return targetPoint.ToPoint3d();
        }

        /// <summary>
        /// 将一个点偏置指定距离
        /// </summary>
        /// <param name="originalPoint3d"></param>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        /// <param name="deltaZ"></param>
        /// <returns></returns>
        public static Point3d Move(this Point3d originalPoint3d, double deltaX = 0, double deltaY = 0, double deltaZ = 0) => new Point3d(originalPoint3d.X + deltaX, originalPoint3d.Y + deltaY, originalPoint3d.Z + deltaZ);

        /// <summary>
        /// 将一个点沿某矢量偏置指定距离
        /// </summary>
        /// <param name="originalPoint3d"></param>
        /// <param name="vector"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Point3d Move(this Point3d originalPoint3d, Vector3d vector, double distance)
        {
            double vectorLength = vector.GetLength();

            if (vectorLength == 0)
                return originalPoint3d;

            Vector3d unitVector3d = new Vector3d(vector.X / vectorLength, vector.Y / vectorLength, vector.Z / vectorLength);

            Point3d newPoint3d = new Point3d(originalPoint3d.X + unitVector3d.X * distance, originalPoint3d.Y + unitVector3d.Y * distance, originalPoint3d.Z + unitVector3d.Z * distance);

            return newPoint3d;
        }

        /// <summary>
        /// 将一个点沿某矢量偏置，距离为矢量长度
        /// </summary>
        /// <param name="originalPoint3d"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Point3d Move(this Point3d originalPoint3d, Vector3d vector) => new Point3d(originalPoint3d.X + vector.X, originalPoint3d.Y + vector.Y, originalPoint3d.Z + vector.Z);

        /// <summary>
        /// 点的坐标转换
        /// </summary>
        /// <param name="originalPoint3d"></param>
        /// <param name="transformationMatrix">变换矩阵</param>
        /// <returns></returns>
        public static Point3d Multiply(this Point3d originalPoint3d, Matrix4x4 transformationMatrix)
        {
            Point3d result;
            result.X = transformationMatrix.Rxx * originalPoint3d.X + transformationMatrix.Ryx * originalPoint3d.Y + transformationMatrix.Rzx * originalPoint3d.Z + transformationMatrix.Xt;
            result.Y = transformationMatrix.Rxy * originalPoint3d.X + transformationMatrix.Ryy * originalPoint3d.Y + transformationMatrix.Rzy * originalPoint3d.Z + transformationMatrix.Yt;
            result.Z = transformationMatrix.Rxz * originalPoint3d.X + transformationMatrix.Ryx * originalPoint3d.Y + transformationMatrix.Rzz * originalPoint3d.Z + transformationMatrix.Zt;
            return result;
        }

        /// <summary>
        /// 计算两个点的中点
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static Point3d MidPoint3d(this Point3d point1, Point3d point2)
        {
            return new Point3d((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, (point1.Z + point2.Z) / 2);
        }

        /// <summary>
        /// 计算多个点的平均中点
        /// </summary>
        /// <param name="points">点集合</param>
        /// <returns></returns>
        public static Point3d MidPoint3d(this Point3d[] points)
        {
            Point3d p = new Point3d();
            for (int i = 0; i < points.Length; i++)
            {
                p.X += points[i].X;
                p.Y += points[i].Y;
                p.Z += points[i].Z;
            }

            return new Point3d(p.X / points.Length, p.Y / points.Length, p.Z / points.Length);
        }

        /// <summary>
        /// 计算两个 Point3d 对象之间的数学距离
        /// </summary>
        /// <param name="point1">第一个点坐标</param>
        /// <param name="point2">第二个点坐标</param>
        /// <returns></returns>
        public static double DistanceTo(this Point3d point1, Point3d point2) => WorkPart.MeasureManager.MeasureDistance(point1, point2);

        /// <summary>
        /// 测量点到平面的距离
        /// </summary>
        /// <param name="point">要测量的点</param>
        /// <param name="pointOnPlane">平面上的点</param>
        /// <param name="planeNormal">平面法向</param>
        /// <returns>距离值，若点位于平面法向一侧，则值为正，反之为负</returns>
        public static double DistanceTo(this Point3d point, Point3d pointOnPlane, Vector3d planeNormal) => Globals.WorkPart.MeasureManager.MeasureDistance(point, pointOnPlane, planeNormal);

        /// <summary>
        /// 测量点到一组对象之间的最短距离，只能是当前工作部件中的对象，可用类型为 Point, Curve, Edge, Face, Body, Datum Plane. 
        /// </summary>
        /// <param name="point">出发点</param>
        /// <param name="objects"></param>
        /// <returns>距离值</returns>
        public static (bool Success, double Distance, Point3d PointOnObjects) DistanceTo(this Point3d point, NXObject[] objects) => WorkPart.MeasureManager.MeasureDistance(point, objects);

        /// <summary>
        /// 测量点到对象之间的投影距离
        /// </summary>
        /// <param name="point"></param>
        /// <param name="object">对象2</param>
        /// <param name="projectVector">投影矢量</param>
        /// <param name="type">测量类型</param>
        /// <returns>投影距离值</returns>
        public static double DistanceTo(this Point3d point, DisplayableObject @object, Vector3d projectVector, MeasureManager.ProjectionType type) => WorkPart.MeasureManager.MeasureProjectDistance(point, @object, projectVector, type);

        /// <summary>
        /// 使用 UFUN 测量点到给定对象间的最小距离
        /// </summary>
        /// <param name="point1">第一个点坐标</param>
        /// <param name="objectTag">目标对象</param>
        /// <returns></returns>
        public static (double Distance, Point3d PointOn2ndObject) DistanceTo(this Point3d point1, Tag objectTag) => WorkPart.MeasureManager.MeasureDistance(point1, objectTag);

        public unsafe static (bool Success, double Distance, Point3d PointOnObjects) DistanceTo(this Point3d targetPoint, Body[] bodies) => bodies.DistanceTo(targetPoint);

        /// <summary>
        /// 求点的反向点（三个坐标值反向）
        /// </summary>
        /// <param name="point">要求的点</param>
        /// <returns></returns>
        public static Point3d Reverse(this Point3d point) => new Point3d(-point.X, -point.Y, -point.Z);

        /// <summary>
        /// 数组转换为 Point3d
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Point3d ToPoint3d(this double[] point)
        {
            if (point.Length != 3)
            {
                "点坐标格式不正确，转换可能存在问题。".ListingWindowWrite();
                return new Point3d();
            }
            else
                return new Point3d(point[0], point[1], point[2]);
        }

        /// <summary>
        /// 将当前显示部件坐标系中的点映射到目标坐标系中 (UFUN)
        /// </summary>
        /// <param name="currentMatrix"></param>
        /// <param name="currentOrigin"></param>
        /// <param name="pointToMap"></param>
        /// <returns></returns>
        public static Point3d Map(this Point3d pointToMap, Matrix3x3 currentMatrix, Point3d currentOrigin)
        {
            double[] transformMtx = new double[16];
            TheUfSession.Mtx4.CsysToCsys(new double[] { 0, 0, 0 }, new double[] { 1, 0, 0 }, new double[] { 0, 1, 0 }, currentOrigin.ToArray(), new double[] { currentMatrix.Xx, currentMatrix.Xy, currentMatrix.Xz }, new double[] { currentMatrix.Yx, currentMatrix.Yy, currentMatrix.Yz }, transformMtx);

            double[] targetPoint = new double[3];
            TheUfSession.Mtx4.Vec3Multiply(pointToMap.ToArray(), transformMtx, targetPoint);

            return targetPoint.ToPoint3d();
        }

        /// <summary>
        /// 求两个点之间的向量
        /// </summary>
        /// <param name="destPoint3d">向量终点</param>
        /// <param name="originPoint3d">向量起点</param>
        /// <returns></returns>
        public static Vector3d Subtract(this Point3d destPoint3d, Point3d originPoint3d)
        {
            return new Vector3d(destPoint3d.X - originPoint3d.X, destPoint3d.Y - originPoint3d.Y, destPoint3d.Z - originPoint3d.Z);
        }

        /// <summary>
        /// 求点沿给定向量移动向量长度后的点
        /// </summary>
        /// <param name="p1">要移动的点</param>
        /// <param name="vector">向量</param>
        /// <returns></returns>
        public static Point3d Add(this Point3d p1, Vector3d vector) => Move(p1, vector);

        /// <summary>
        /// 求原点到此点的向量
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3d ToVector3d(this Point3d point) => new Vector3d(point.X, point.Y, point.Z);

        /// <summary>
        /// 求三个基准平面的交点
        /// </summary>
        /// <param name="plane1Origin"></param>
        /// <param name="plane1Normal"></param>
        /// <param name="plane2Origin"></param>
        /// <param name="plane2Normal"></param>
        /// <param name="plane3Origin"></param>
        /// <param name="plane3Normal"></param>
        public static Point3d Intersect(Point3d plane1Origin, Vector3d plane1Normal, Point3d plane2Origin, Vector3d plane2Normal, Point3d plane3Origin, Vector3d plane3Normal)
        {
            double x = -((-plane1Normal.X * plane3Normal.Y * plane2Normal.Z * plane1Origin.X + plane1Normal.X * plane2Normal.Y * plane3Normal.Z * plane1Origin.X + plane2Normal.X * plane3Normal.Y * plane1Normal.Z * plane2Origin.X - plane2Normal.X * plane1Normal.Y * plane3Normal.Z * plane2Origin.X - plane3Normal.X * plane2Normal.Y * plane1Normal.Z * plane3Origin.X + plane3Normal.X * plane1Normal.Y * plane2Normal.Z * plane3Origin.X - plane1Normal.Y * plane3Normal.Y * plane2Normal.Z * plane1Origin.Y + plane1Normal.Y * plane2Normal.Y * plane3Normal.Z * plane1Origin.Y + plane2Normal.Y * plane3Normal.Y * plane1Normal.Z * plane2Origin.Y - plane1Normal.Y * plane2Normal.Y * plane3Normal.Z * plane2Origin.Y - plane2Normal.Y * plane3Normal.Y * plane1Normal.Z * plane3Origin.Y + plane1Normal.Y * plane3Normal.Y * plane2Normal.Z * plane3Origin.Y - plane3Normal.Y * plane1Normal.Z * plane2Normal.Z * plane1Origin.Z + plane2Normal.Y * plane1Normal.Z * plane3Normal.Z * plane1Origin.Z + plane3Normal.Y * plane1Normal.Z * plane2Normal.Z * plane2Origin.Z - plane1Normal.Y * plane2Normal.Z * plane3Normal.Z * plane2Origin.Z - plane2Normal.Y * plane1Normal.Z * plane3Normal.Z * plane3Origin.Z + plane1Normal.Y * plane2Normal.Z * plane3Normal.Z * plane3Origin.Z) / (plane3Normal.X * plane2Normal.Y * plane1Normal.Z - plane2Normal.X * plane3Normal.Y * plane1Normal.Z - plane3Normal.X * plane1Normal.Y * plane2Normal.Z + plane1Normal.X * plane3Normal.Y * plane2Normal.Z + plane2Normal.X * plane1Normal.Y * plane3Normal.Z - plane1Normal.X * plane2Normal.Y * plane3Normal.Z));

            double y = -((plane1Normal.X * plane3Normal.X * plane2Normal.Z * plane1Origin.X - plane1Normal.X * plane2Normal.X * plane3Normal.Z * plane1Origin.X - plane2Normal.X * plane3Normal.X * plane1Normal.Z * plane2Origin.X + plane1Normal.X * plane2Normal.X * plane3Normal.Z * plane2Origin.X + plane2Normal.X * plane3Normal.X * plane1Normal.Z * plane3Origin.X - plane1Normal.X * plane3Normal.X * plane2Normal.Z * plane3Origin.X + plane3Normal.X * plane1Normal.Y * plane2Normal.Z * plane1Origin.Y - plane2Normal.X * plane1Normal.Y * plane3Normal.Z * plane1Origin.Y - plane3Normal.X * plane2Normal.Y * plane1Normal.Z * plane2Origin.Y + plane1Normal.X * plane2Normal.Y * plane3Normal.Z * plane2Origin.Y + plane2Normal.X * plane3Normal.Y * plane1Normal.Z * plane3Origin.Y - plane1Normal.X * plane3Normal.Y * plane2Normal.Z * plane3Origin.Y + plane3Normal.X * plane1Normal.Z * plane2Normal.Z * plane1Origin.Z - plane2Normal.X * plane1Normal.Z * plane3Normal.Z * plane1Origin.Z - plane3Normal.X * plane1Normal.Z * plane2Normal.Z * plane2Origin.Z + plane1Normal.X * plane2Normal.Z * plane3Normal.Z * plane2Origin.Z + plane2Normal.X * plane1Normal.Z * plane3Normal.Z * plane3Origin.Z - plane1Normal.X * plane2Normal.Z * plane3Normal.Z * plane3Origin.Z) / (plane3Normal.X * plane2Normal.Y * plane1Normal.Z - plane2Normal.X * plane3Normal.Y * plane1Normal.Z - plane3Normal.X * plane1Normal.Y * plane2Normal.Z + plane1Normal.X * plane3Normal.Y * plane2Normal.Z + plane2Normal.X * plane1Normal.Y * plane3Normal.Z - plane1Normal.X * plane2Normal.Y * plane3Normal.Z));

            double z = -((-plane1Normal.X * plane3Normal.X * plane2Normal.Y * plane1Origin.X + plane1Normal.X * plane2Normal.X * plane3Normal.Y * plane1Origin.X + plane2Normal.X * plane3Normal.X * plane1Normal.Y * plane2Origin.X - plane1Normal.X * plane2Normal.X * plane3Normal.Y * plane2Origin.X - plane2Normal.X * plane3Normal.X * plane1Normal.Y * plane3Origin.X + plane1Normal.X * plane3Normal.X * plane2Normal.Y * plane3Origin.X - plane3Normal.X * plane1Normal.Y * plane2Normal.Y * plane1Origin.Y + plane2Normal.X * plane1Normal.Y * plane3Normal.Y * plane1Origin.Y + plane3Normal.X * plane1Normal.Y * plane2Normal.Y * plane2Origin.Y - plane1Normal.X * plane2Normal.Y * plane3Normal.Y * plane2Origin.Y - plane2Normal.X * plane1Normal.Y * plane3Normal.Y * plane3Origin.Y + plane1Normal.X * plane2Normal.Y * plane3Normal.Y * plane3Origin.Y - plane3Normal.X * plane2Normal.Y * plane1Normal.Z * plane1Origin.Z + plane2Normal.X * plane3Normal.Y * plane1Normal.Z * plane1Origin.Z + plane3Normal.X * plane1Normal.Y * plane2Normal.Z * plane2Origin.Z - plane1Normal.X * plane3Normal.Y * plane2Normal.Z * plane2Origin.Z - plane2Normal.X * plane1Normal.Y * plane3Normal.Z * plane3Origin.Z + plane1Normal.X * plane2Normal.Y * plane3Normal.Z * plane3Origin.Z) / (plane3Normal.X * plane2Normal.Y * plane1Normal.Z - plane2Normal.X * plane3Normal.Y * plane1Normal.Z - plane3Normal.X * plane1Normal.Y * plane2Normal.Z + plane1Normal.X * plane3Normal.Y * plane2Normal.Z + plane2Normal.X * plane1Normal.Y * plane3Normal.Z - plane1Normal.X * plane2Normal.Y * plane3Normal.Z));

            Point3d intersectionPoint3d = new Point3d(x, y, z);

            return intersectionPoint3d;
        }

        /// <summary>
        /// 转换为 Point4d
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static Point4d ToPoint4d(this Point3d pt) => new Point4d(pt);
        #endregion
    }
}