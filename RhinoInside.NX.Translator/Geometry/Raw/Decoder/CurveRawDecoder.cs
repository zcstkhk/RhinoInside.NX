using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using RhinoInside.NX.Extensions;
using static RhinoInside.NX.Extensions.Globals;

namespace RhinoInside.NX.Translator.Geometry.Raw
{
    internal static partial class RawDecoder
    {
        #region Curve
        public static LineCurve ToRhinoLineCurve(NXOpen.Line line)
        {
            return new LineCurve(new Line(ToRhino(line.StartPoint), ToRhino(line.EndPoint)));
        }

        public static ArcCurve ToRhinoArcCurve(NXOpen.Arc arc)
        {
            var circle = new Circle(new Plane(ToRhino(arc.CenterPoint), ToRhino(arc.Matrix.Element.GetAxisX()), ToRhino(arc.Matrix.Element.GetAxisY())), arc.Radius);

            if (Math.Abs((arc.EndAngle - arc.StartAngle) - 2 * Math.PI) < AngleTolerance)
            {
                return new ArcCurve(circle);
            }
            else
            {
                return new ArcCurve(new Arc(circle, new Interval(arc.StartAngle, arc.EndAngle)));
            }
        }

        public static NurbsCurve ToRhinoNurbsCurve(NXOpen.Ellipse ellipse)
        {
            var plane = new Plane(ToRhino(ellipse.CenterPoint), ToRhino(ellipse.Matrix.Element.GetAxisX()), ToRhino(ellipse.Matrix.Element.GetAxisY()));
            var e = new Ellipse(plane, ellipse.MajorRadius, ellipse.MinorRadius);
            var nurbsCurve = e.ToNurbsCurve();

            if (Math.Abs(Math.Abs(ellipse.EndAngle - ellipse.StartAngle) - Math.PI * 2) > DistanceTolerance)
            {
                nurbsCurve.ClosestPoint(ToRhino(ellipse.GetPoint(0.0)), out var param0);
                if (!nurbsCurve.ChangeClosedCurveSeam(param0))
                    nurbsCurve.Domain = new Interval(param0, param0 + nurbsCurve.Domain.Length);

                nurbsCurve.ClosestPoint(ToRhino(ellipse.GetPoint(1.0)), out var param1);
                nurbsCurve = nurbsCurve.Trim(param0, param1) as NurbsCurve;
                nurbsCurve.Domain = new Interval(ellipse.StartAngle / (Math.PI * 2.0), ellipse.EndAngle / (Math.PI * 2.0));
            }

            return nurbsCurve;
        }

        public static NurbsCurve ToRhinoNurbsCurve(NXOpen.Spline spline)
        {
            var controlPoints = spline.GetPoles();
            var n = new NurbsCurve(3, spline.Rational, spline.Order, controlPoints.Length);

            if (spline.Rational)
            {
                var weights = controlPoints.Select(obj => obj.W).ToArray();
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

            var Knots = spline.GetKnots();

            int knotIndex = 0;
            foreach (var w in Knots.Cast<double>().Skip(1).Take(n.Knots.Count))
                n.Knots[knotIndex++] = w;

            return n;
        }

        public static PolylineCurve ToRhinoPolylineCurve(NXOpen.Polyline polyline)
        {
            return new PolylineCurve(polyline.GetPoints().Select(x => ToRhino(x)));
        }

        public static Curve ToRhino(NXOpen.Curve curve)
        {
            switch (curve)
            {
                case null: return null;
                case NXOpen.Line line: return ToRhinoLineCurve(line);
                case NXOpen.Arc arc: return ToRhinoArcCurve(arc);
                case NXOpen.Ellipse ellipse: return ToRhinoNurbsCurve(ellipse);
                case NXOpen.Spline nurb: return ToRhinoNurbsCurve(nurb);
                default: throw new NotImplementedException();
            }
        }
        #endregion
    }
}
