using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using NXOpen.Extensions;
using static NXOpen.Extensions.Globals;

namespace RhinoInside.NX.Translator.Geometry
{
    public static partial class RawDecoder
    {
        public static LineCurve ToRhinoCurve(NXOpen.Line line)
        {
            return new LineCurve(new Line(line.StartPoint.ToRhino(), line.EndPoint.ToRhino()));
        }

        public static ArcCurve ToRhinoCurve(NXOpen.Arc arc)
        {
            return CreateArc(arc.CenterPoint.ToRhino(), arc.Matrix.Element.GetAxisX().ToRhino(), arc.Matrix.Element.GetAxisY().ToRhino(), arc.StartAngle, arc.EndAngle, arc.Radius);
        }

        internal static ArcCurve CreateArc(Point3d center, Vector3d axisX, Vector3d axisY, double startAngle, double endAngle, double radius)
        {
            var circle = new Circle(new Plane(center, axisX, axisY), radius);

            if (Math.Abs(Math.Abs(endAngle - startAngle) - 2 * Math.PI) < AngleTolerance)
                return new ArcCurve(circle);
            else
                return new ArcCurve(new Arc(circle, new Interval(startAngle, endAngle)));
        }

        public static NurbsCurve ToRhinoCurve(NXOpen.Ellipse ellipse)
        {
            return CreateNurbsCurve(ellipse.CenterPoint.ToRhino(), ellipse.Matrix.Element.GetAxisX().ToRhino(), ellipse.Matrix.Element.GetAxisY().ToRhino(), ellipse.StartAngle, ellipse.EndAngle, ellipse.MajorRadius, ellipse.MinorRadius, ellipse.GetPoint(0.0).ToRhino(), ellipse.GetPoint(1.0).ToRhino());
        }

        /// <summary>
        /// 创建椭圆形 NurbsCurve
        /// </summary>
        /// <param name="center"></param>
        /// <param name="axisX"></param>
        /// <param name="axisY"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="majorRadius"></param>
        /// <param name="minorRadius"></param>
        /// <param name="startPoint">如果椭圆不是完整椭圆，需要提供</param>
        /// <param name="endPoint">如果椭圆不是完整椭圆，需要提供</param>
        /// <returns></returns>
        internal static NurbsCurve CreateNurbsCurve(Point3d center, Vector3d axisX, Vector3d axisY, double startAngle, double endAngle, double majorRadius, double minorRadius, Point3d startPoint = default, Point3d endPoint = default)
        {
            var plane = new Plane(center, axisX, axisY);
            var e = new Ellipse(plane, majorRadius, minorRadius);
            var nurbsCurve = e.ToNurbsCurve();

            if (Math.Abs(Math.Abs(endAngle - startAngle) - Math.PI * 2) > DistanceTolerance)
            {
                nurbsCurve.ClosestPoint(startPoint, out var startParam);
                if (!nurbsCurve.ChangeClosedCurveSeam(startParam))
                    nurbsCurve.Domain = new Interval(startParam, startParam + nurbsCurve.Domain.Length);

                nurbsCurve.ClosestPoint(endPoint, out var endParam);
                nurbsCurve = nurbsCurve.Trim(startParam, endParam) as NurbsCurve;
                nurbsCurve.Domain = new Interval(startAngle / (Math.PI * 2.0), endAngle / (Math.PI * 2.0));
            }

            return nurbsCurve;
        }

        public static NurbsCurve ToRhinoCurve(NXOpen.Spline spline)
        {
            return CreateNurbsCurve(spline.GetPoles(), spline.GetKnots(), spline.Rational, spline.Order);
        }

        /// <summary>
        /// 根据极点和节点创建 NurbsCurve
        /// </summary>
        /// <param name="poles">包含控制点坐标以及权重信息的 Point4d 数组</param>
        /// <param name="knots">节点</param>
        /// <param name="isRational"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        internal static NurbsCurve CreateNurbsCurve(NXOpen.Point4d[] poles, double[] knots, bool isRational, int order)
        {
            var n = new NurbsCurve(3, isRational, order, poles.Length);

            Point3d[] controlPoints = new Point3d[poles.Length];
            double[] weights = new double[poles.Length];
            for (int i = 0; i < poles.Length; i++)
            {
                controlPoints[i] = new Point3d(poles[i].X, poles[i].Y, poles[i].Z);
                weights[i] = poles[i].W;
            }

            if (isRational)
            {
                int index = 0;
                foreach (var pt in controlPoints)
                {
                    var w = weights[index];
                    n.Points.SetPoint(index++, pt.X * w, pt.Y * w, pt.Z * w, w);
                }
            }
            else
            {
                int index = 0;
                foreach (var pt in controlPoints)
                    n.Points.SetPoint(index++, pt.X, pt.Y, pt.Z);
            }

            int knotIndex = 0;
            foreach (var w in knots.Cast<double>().Skip(1).Take(n.Knots.Count))
                n.Knots[knotIndex++] = w;

            return n;
        }

        public static PolylineCurve ToRhinoCurve(NXOpen.Polyline polyline)
        {
            return new PolylineCurve(polyline.GetPoints().Select(x => x.ToRhino()));
        }

        public static Curve ToRhinoCurve(NXOpen.Curve curve)
        {
            switch (curve)
            {
                case null: return null;
                case NXOpen.Line line: return ToRhinoCurve(line);
                case NXOpen.Arc arc: return ToRhinoCurve(arc);
                case NXOpen.Ellipse ellipse: return ToRhinoCurve(ellipse);
                case NXOpen.Spline nurb: return ToRhinoCurve(nurb);
                default: throw new NotImplementedException();
            }
        }

        public static Curve ToRhinoCurve(NXOpen.Edge edge)
        {
            var edgeType = edge.SolidEdgeType;

            switch (edgeType)
            {
                case NXOpen.Edge.EdgeType.Rubber:
                    break;
                case NXOpen.Edge.EdgeType.Linear:
                    edge.GetVertices(out var sp, out var ep);
                    return new LineCurve(new Line(sp.ToRhino(), ep.ToRhino()));
                case NXOpen.Edge.EdgeType.Circular:
                    TheUfSession.Eval.Initialize2(edge.Tag, out var edgeEvaluator);
                    TheUfSession.Eval.AskArc(edgeEvaluator, out var arc);
                    TheUfSession.Eval.Free(edgeEvaluator);

                    return RawDecoder.CreateArc(arc.center.ToRhinoPoint3d(), arc.x_axis.ToRhinoVector3d(), arc.y_axis.ToRhinoVector3d(), arc.limits[0], arc.limits[1], arc.radius);
                case NXOpen.Edge.EdgeType.Elliptical:
                    TheUfSession.Eval.Initialize2(edge.Tag, out var evaluator);
                    TheUfSession.Eval.AskEllipse(evaluator, out var ellipse);
                    TheUfSession.Eval.Free(evaluator);

                    NurbsCurve nurbsCurve = RawDecoder.CreateNurbsCurve(ellipse.center.ToRhinoPoint3d(), ellipse.x_axis.ToRhinoVector3d(), ellipse.y_axis.ToRhinoVector3d(), ellipse.limits[0], ellipse.limits[1], ellipse.major, ellipse.minor, edge.GetPoint(0.0).ToRhino(), edge.GetPoint(1.0).ToRhino());

                    return nurbsCurve;
                case NXOpen.Edge.EdgeType.Intersection:
                    break;
                case NXOpen.Edge.EdgeType.Spline:
                    TheUfSession.Eval.Initialize2(edge.Tag, out var splineEvaluator);
                    TheUfSession.Eval.AskSplineControlPts(splineEvaluator, out var controlPointCount, out var controlPoints);
                    TheUfSession.Eval.AskSplineKnots(splineEvaluator, out var knotsCount, out var knots);
                    TheUfSession.Eval.AskSpline(splineEvaluator, out var splineData);
                    TheUfSession.Eval.Free(splineEvaluator);

                    NXOpen.Point4d[] controlPoint4ds = new NXOpen.Point4d[controlPointCount];
                    for (int i = 0; i < controlPointCount; i++)
                    {
                        controlPoint4ds[i] = new NXOpen.Point4d
                        {
                            X = controlPoints[i * 4],
                            Y = controlPoints[i * 4 + 1],
                            Z = controlPoints[i * 4 + 2],
                            W = controlPoints[i * 4 + 3],
                        };
                    }

                    return RawDecoder.CreateNurbsCurve(controlPoint4ds, knots, splineData.is_rational, splineData.order);
                case NXOpen.Edge.EdgeType.SpCurve:
                    break;
                case NXOpen.Edge.EdgeType.Foreign:
                    break;
                case NXOpen.Edge.EdgeType.ConstantParameter:
                    break;
                case NXOpen.Edge.EdgeType.TrimmedCurve:
                    break;
                case NXOpen.Edge.EdgeType.Convergent:
                    break;
                case NXOpen.Edge.EdgeType.Undefined:
                    break;
                default:
                    break;
            }

            var nxCurve = edge.CreateCurveFromEdge();

            var rhinoCurve = nxCurve.ToRhino();

            nxCurve.Delete();

            return rhinoCurve;
        }
    }
}
