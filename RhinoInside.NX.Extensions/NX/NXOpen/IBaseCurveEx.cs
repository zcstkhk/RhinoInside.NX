using NXOpen.Features;
using NXOpen;
using static RhinoInside.NX.Extensions.Globals;
using PK = PLMComponents.Parasolid.PK_.Unsafe;
using NXOpen.Extensions.Parasolid;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace RhinoInside.NX.Extensions
{
    /// <summary>
    /// NXOpen.IbaseCurve 接口的扩展类
    /// </summary>
    public static partial class IBaseCurveEx
    {
        static IBaseCurveEx() => AppDomain.CurrentDomain.AssemblyResolve += ManagedLibraryResolver;

        /// <summary>
        /// 沿面的法向投影曲线
        /// </summary>
        /// <param name="curves"></param>
        /// <param name="faces_to_project_to"></param>
        /// <returns></returns>
        public static ProjectCurve Project(this IBaseCurve[] curves, Face[] faces_to_project_to) => WorkPart.Features.CreateProjectCurve(curves, faces_to_project_to);

        /// <summary>
        /// 投影曲线
        /// </summary>
        /// <param name="curves">要投影的曲线</param>
        /// <param name="originPoint">投影平面原点</param>
        /// <param name="normal">投影方向</param>
        /// <returns></returns>
        public static ProjectCurve Project(this IBaseCurve[] curves, Point3d originPoint, Vector3d normal) => WorkPart.Features.CreateProjectCurve(curves, originPoint, normal);

        /// <summary>
        /// 通过参数访问曲线点 (UFUN)
        /// </summary>
        /// <param name="parm">曲线参数, 值为0到1之间</param>
        /// <param name="curve">曲线</param>
        /// <returns></returns>
        public static Point3d GetPoint(this IBaseCurve curve, double parm)
        {
            double[] point = new double[3];

            double torsion;
            double rad_of_cur;
            TheUfSession.Modl.AskCurveProps((curve as TaggedObject).Tag, parm, point, new double[3], new double[3], new double[3], out torsion, out rad_of_cur);
            return point.ToPoint3d();
        }

        /// <summary>
        /// 获取曲线的端点 (UFUN)
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static (Point3d startPoint3d, Point3d endPoint3d) GetVertices(this IBaseCurve curve)
        {
            Point3d startPoint3d = curve.GetPoint(0.0);
            Point3d endPoint3d = curve.GetPoint(1.0);

            return (startPoint3d, endPoint3d);
        }

        /// <summary>
        /// 转换为数组
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static Point3d[] ToArray(this (Point3d startPoint3d, Point3d endPoint3d) vertices)
        {
            return new Point3d[] { vertices.startPoint3d, vertices.endPoint3d };
        }

        /// <summary>
        /// 计算一组曲线的顶点，最少为 0，若为开放轮廓，则为多个
        /// </summary>
        /// <param name="curves"></param>
        public static List<Point3d> GetVertices(this IEnumerable<IBaseCurve> curves)
        {
            List<Point3d> verticeList = new List<Point3d>();
            for (int i = 0; i < curves.Count(); i++)
            {
                var curveVertices = curves.ElementAt(i).GetVertices();

                double minimum_dist = double.MaxValue;
                Point3d? cloestPoint3d = null;
                for (int j = 0; j < verticeList.Count; j++)
                {
                    double dist = verticeList[j].DistanceTo(curveVertices.startPoint3d);
                    if (dist < minimum_dist)
                    {
                        minimum_dist = dist;
                        cloestPoint3d = verticeList[j];
                    }
                }

                if (minimum_dist > 0.01)        // 添加起点
                    verticeList.Add(curveVertices.startPoint3d);
                else
                    verticeList.Remove(cloestPoint3d.Value);

                minimum_dist = double.MaxValue;
                for (int j = 0; j < verticeList.Count; j++)
                {
                    double dist = verticeList[j].DistanceTo(curveVertices.endPoint3d);
                    if (dist < minimum_dist)
                    {
                        minimum_dist = dist;
                        cloestPoint3d = verticeList[j];
                    }
                }

                if (minimum_dist > 0.01)
                    verticeList.Add(curveVertices.endPoint3d);
                else
                    verticeList.Remove(cloestPoint3d.Value);
            }

            return verticeList;
        }

        /// <summary>
        /// 获取曲线在某一参数处的属性
        /// </summary>
        /// <param name="curve">曲线</param>
        /// <param name="parm">参数，从 0 到 1</param>
        /// <returns></returns>
        public static (Point3d Point, Vector3d Tangent, Vector3d PrincipalNormal, Vector3d Binormal, double Torsion, double Radius) GetProperties(this Curve curve, double parm)
        {
            double[] point = new double[3];
            double[] tangent = new double[3];
            double[] p_norm = new double[3];
            double[] b_norm = new double[3];

            double tor;
            double rad_of_cur;
            TheUfSession.Modl.AskCurveProps(curve.Tag, parm, point, tangent, p_norm, b_norm, out tor, out rad_of_cur);
            return (point.ToPoint3d(), tangent.ToVector3d().GetUnitVector(), p_norm.ToVector3d().GetUnitVector(), b_norm.ToVector3d().GetUnitVector(), tor, rad_of_cur);
        }

        /// <summary>
        /// 获取曲线在某一参数处的点和切线方向
        /// </summary>
        /// <param name="curve">曲线或边</param>
        /// <param name="parm">参数，从 0 到 1</param>
        /// <returns></returns>
        public static (Point3d Point, Vector3d Tangent, Vector3d Normal, Vector3d Binormal) Evaluate(this IBaseCurve curve, double parm)
        {
            TheUfSession.Eval.Initialize2((curve as TaggedObject).Tag, out var evaluator);

            double[] curvePt = new double[3];
            double[] tangent = new double[3];
            double[] normal = new double[3];
            double[] biNormal = new double[3];
            TheUfSession.Eval.EvaluateUnitVectors(evaluator, parm, curvePt, tangent, normal, biNormal);

            TheUfSession.Eval.Free(evaluator);

            return (curvePt.ToPoint3d(), tangent.ToVector3d().GetUnitVector(), normal.ToVector3d().GetUnitVector(), biNormal.ToVector3d().GetUnitVector());
        }

        /// <summary>
        /// 获取曲线中点
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static Point3d GetMidPoint(this IBaseCurve curve)
        {
            TheUfSession.Eval.Initialize2(((TaggedObject)curve).Tag, out IntPtr intPtr);
            double[] limits = new double[2];
            TheUfSession.Eval.AskLimits(intPtr, limits);
            double midParm = (limits[0] + limits[1]) * 0.5;
            double[] point = new double[3];
            double[] derivatives = new double[10];
            TheUfSession.Eval.Evaluate(intPtr, 1, midParm, point, derivatives);
            return point.ToPoint3d();
        }

        /// <summary>
        /// 获取曲线或边的最近点
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="knownPoint3d"></param>
        /// <returns></returns>
        public static (Point3d CurvePoint, double CurveParm) GetCloestPoint(this IBaseCurve curve, Point3d knownPoint3d)
        {
            if (curve is Curve)
            {
                double[] curvePt = new double[3];
                TheUfSession.Modl.AskCurveParm((curve as TaggedObject).Tag, knownPoint3d.ToArray(), out double parm, curvePt);

                return (curvePt.ToPoint3d(), parm);
            }
            else
            {
                TheUfSession.Eval.Initialize2((curve as TaggedObject).Tag, out var evaluator);
                double[] curvePt = new double[3];
                TheUfSession.Eval.EvaluateClosestPoint(evaluator, knownPoint3d.ToArray(), out double parm, curvePt);

                TheUfSession.Eval.Free(evaluator);

                return (curvePt.ToPoint3d(), parm);
            }
        }

        public unsafe static (bool HasMinimumRadius, double MinimumRadius, Point3d MinimumPosition, double Param) GetMinRadius(this IBaseCurve baseCurve)
        {
            TheUfSession.Ps.CreatePartition(out _);

            TaggedObject curve = baseCurve as TaggedObject;

            TheUfSession.Ps.CreatePsTrimmedCurve(curve.Tag, out Tag curvePsTag);

            PK.INTERVAL_t curveInterval;
            PK.CURVE.ask_interval(new PK.CURVE_t((int)curvePsTag), &curveInterval);

            int n_radii;
            double radius;
            PK.VECTOR_t vECTOR_T;
            double param;
            PK.CURVE.find_min_radius(new PK.CURVE_t((int)curvePsTag), curveInterval, &n_radii, &radius, &vECTOR_T, &param);

            if (n_radii != 0)
                return (true, radius * 1000.0, vECTOR_T.ToPoint3d(), param);
            else
                return (false, 0, new Point3d(), 0);
        }
    }
}
