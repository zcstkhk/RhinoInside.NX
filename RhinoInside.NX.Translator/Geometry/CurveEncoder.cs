using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using static RhinoInside.NX.Extensions.Globals;
using RhinoInside.NX.Extensions;
using System.Diagnostics;
using Rhino.Geometry.Collections;

namespace RhinoInside.NX.Translator
{
    public static class CurveEncoder
    {
        #region Curve
        public static NXOpen.Curve ToRhino(this Curve value, double factor)
        {
            switch (value)
            {
                case LineCurve line:
                    return line.Line.ToLine(factor);

                case ArcCurve arc:
                    return arc.Arc.ToArc(factor);

                case PolylineCurve polyline:
                    value = polyline.Simplify
                    (
                      CurveSimplifyOptions.RebuildLines |
                      CurveSimplifyOptions.Merge,
                      DistanceTolerance * factor,
                      AngleTolerance
                    )
                    ?? value;

                    if (value is PolylineCurve)
                        return value.ToNurbsCurve().ToCurve(factor);
                    else
                        return value.ToRhino(factor);

                case PolyCurve polyCurve:
                    value = polyCurve.Simplify
                    (
                      CurveSimplifyOptions.AdjustG1 |
                      CurveSimplifyOptions.Merge,
                      DistanceTolerance * factor,
                      AngleTolerance
                    )
                    ?? value;

                    if (value is PolyCurve)
                        return value.ToNurbsCurve().ToCurve(factor);
                    else
                        return value.ToRhino(factor);

                case NurbsCurve nurbsCurve:
                    return nurbsCurve.ToCurve(factor);

                default:
                    return value.ToNurbsCurve().ToCurve(factor);
            }
        }
        public static NXOpen.Curve ToCurve(this Curve value) => value.ToRhino(UnitConverter.RhinoToNXUnitsRatio);

        public static IEnumerable<NXOpen.Curve> ToCurveMany(this Curve curve, double factor)
        {
            switch (curve)
            {
                case LineCurve lineCurve:
                    yield return lineCurve.Line.ToLine(factor);
                    yield break;

                case PolylineCurve polylineCurve:
                    foreach (var line in polylineCurve.ToCurveMany(factor))
                        yield return line;
                    yield break;

                case ArcCurve arcCurve:
                    yield return arcCurve.Arc.ToArc(factor);
                    yield break;

                case PolyCurve poly:
                    foreach (var segment in poly.ToCurveMany(factor))
                        yield return segment;
                    yield break;

                case NurbsCurve nurbs:
                    foreach (var segment in nurbs.ToCurveMany(factor))
                        yield return segment;
                    yield break;

                default:
                    if (curve.HasNurbsForm() != 0)
                    {
                        var nurbsForm = curve.ToNurbsCurve();
                        foreach (var c in nurbsForm.ToCurveMany(factor))
                            yield return c;
                    }
                    else throw new Exception($"Failed to convert {curve} to DB.Curve");

                    yield break;
            }
        }
        public static IEnumerable<NXOpen.Curve> ToCurveMany(this Curve value) => value.ToCurveMany(UnitConverter.RhinoToNXUnitsRatio);

        public static IEnumerable<NXOpen.Curve> ToCurveMany(this PolyCurve value) => value.ToCurveMany(UnitConverter.RhinoToNXUnitsRatio);
        public static IEnumerable<NXOpen.Curve> ToCurveMany(this PolyCurve value, double factor)
        {
            int segmentCount = value.SegmentCount;
            for (int s = 0; s < segmentCount; ++s)
            {
                foreach (var segment in value.SegmentCurve(s).ToCurveMany(factor))
                    yield return segment;
            }
        }
        #endregion

        #region ArcCurve
        public static NXOpen.Curve ToCurve(this ArcCurve value) => value.Arc.ToArc(UnitConverter.RhinoToNXUnitsRatio);
        public static NXOpen.Curve ToCurve(this ArcCurve value, double factor) => value.Arc.ToArc(factor);
        #endregion

        #region Arc Structure
        public static NXOpen.Arc ToArc(this Arc value) => value.ToArc(UnitConverter.RhinoToNXUnitsRatio);
        public static NXOpen.Arc ToArc(this Arc value, double factor)
        {
            return WorkPart.Curves.CreateArc(value.Center.ToXYZ(factor), value.Plane.XAxis.ToXYZ(factor), value.Plane.YAxis.ToXYZ(factor), value.Radius * factor, value.StartAngle, value.EndAngle);
        }
        #endregion

        #region Circle Structure
        public static NXOpen.Arc ToArc(this Circle value) => value.ToArc(UnitConverter.RhinoToNXUnitsRatio);
        public static NXOpen.Arc ToArc(this Circle value, double factor)
        {
            return WorkPart.Curves.CreateArc(value.Center.ToXYZ(factor), value.Plane.XAxis.ToXYZ(factor), value.Plane.YAxis.ToXYZ(factor), value.Radius * factor, 0.0, 2 * Math.PI);
        }
        #endregion

        #region Ellipse Structure
        public static NXOpen.Curve ToCurve(this Ellipse value) => value.ToCurve(new Interval(0.0, 2.0 * Math.PI), UnitConverter.RhinoToNXUnitsRatio);
        public static NXOpen.Curve ToCurve(this Ellipse value, double factor) => value.ToCurve(new Interval(0.0, 2.0 * Math.PI), factor);
        public static NXOpen.Curve ToCurve(this Ellipse value, Interval interval) => value.ToCurve(interval, UnitConverter.RhinoToNXUnitsRatio);
        public static NXOpen.Curve ToCurve(this Ellipse value, Interval interval, double factor)
        {
            return WorkPart.Curves.CreateEllipse(value.Plane.Origin.ToXYZ(factor), value.Plane.XAxis.ToXYZ(), value.Plane.YAxis.ToXYZ(), value.Radius1, value.Radius2, interval.Min, interval.Max);
        }
        #endregion

        #region Line Structure
        public static NXOpen.Line ToLine(this Line value) => value.ToLine(UnitConverter.RhinoToNXUnitsRatio);
        public static NXOpen.Line ToLine(this Line value, double factor) => WorkPart.Curves.CreateLine(value.From.ToXYZ(factor), value.To.ToXYZ(factor));
        #endregion

        #region LineCurve
        public static NXOpen.Curve ToCurve(this LineCurve value) => value.Line.ToLine(UnitConverter.RhinoToNXUnitsRatio);
        public static NXOpen.Curve ToCurve(this LineCurve value, double factor) => value.Line.ToLine(factor);
        #endregion

        #region NurbsCurve
        public static NXOpen.Curve ToCurve(this NurbsCurve value) => value.ToCurve(UnitConverter.RhinoToNXUnitsRatio);
        public static NXOpen.Curve ToCurve(this NurbsCurve value, double factor)
        {
            if (value.TryGetEllipse(out Ellipse ellipse, out var interval, DistanceTolerance * factor))
                return ellipse.ToCurve(interval, factor);

            var gap = DistanceTolerance * 0.5;
            if (value.IsClosed(gap * factor))
            {
                var length = value.GetLength();
                if (length > gap &&
                  value.LengthParameter((gap / 2.0), out var t0) &&
                  value.LengthParameter(length - (gap / 2.0), out var t1)
                )
                {
                    var segments = value.Split(new double[] { t0, t1 });
                    value = segments[0] as NurbsCurve ?? value;
                }
                else throw new Exception($"Failed to Split closed NurbsCurve, Length = {length}");
            }

            if (value.Degree < 3 && value.SpanCount > 1)
            {
                value = value.DuplicateCurve() as NurbsCurve;
                value.IncreaseDegree(3);
            }

            return ToNurbsSpline(value, factor);
        }

        static bool KnotAlmostEqualTo(double max, double min) =>
          KnotAlmostEqualTo(max, min, 1.0e-09);

        static bool KnotAlmostEqualTo(double max, double min, double tol)
        {
            var length = max - min;
            if (length <= tol)
                return true;

            return length <= max * tol;
        }

        static double KnotPrevNotEqual(double max) =>
          KnotPrevNotEqual(max, 1.0000000E-9 * 1000.0);

        static double KnotPrevNotEqual(double max, double tol)
        {
            const double delta2 = 2.0 * 1E-16;
            var value = max - tol - delta2;

            if (!KnotAlmostEqualTo(max, value, tol))
                return value;

            return max - (max * (tol + delta2));
        }

        static double[] ToDoubleArray(NurbsCurveKnotList list, int degree)
        {
            var count = list.Count;
            var knots = new double[count + 2];

            var min = list[0];
            var max = list[count - 1];
            var factor = 1.0 / (max - min); // normalized

            // End knot
            knots[count + 1] = (list[count - 1] - min) * factor;
            for (int k = count - 1; k >= count - degree; --k)
                knots[k + 1] = (list[k] - min) * factor;

            // Interior knots (in reverse order)
            int multiplicity = degree + 1;
            for (int k = count - degree - 1; k >= degree; --k)
            {
                double current = (list[k] - min) * factor;
                double next = knots[k + 2];
                if (KnotAlmostEqualTo(next, current))
                {
                    multiplicity++;
                    if (multiplicity > degree - 2)
                        current = KnotPrevNotEqual(next);
                    else
                        current = next;
                }
                else multiplicity = 1;

                knots[k + 1] = current;
            }

            // Start knot
            for (int k = degree - 1; k >= 0; --k)
                knots[k + 1] = (list[k] - min) * factor;
            knots[0] = (list[0] - min) * factor;

            return knots;
        }

        static global::NXOpen.Point3d[] ToXYZArray(NurbsCurvePointList list, double factor)
        {
            var count = list.Count;
            var points = new global::NXOpen.Point3d[count];

            int p = 0;
            if (factor == 1.0)
            {
                while (p < count)
                {
                    var location = list[p].Location;
                    points[p++] = new NXOpen.Point3d(location.X, location.Y, location.Z);
                }
            }
            else
            {
                while (p < count)
                {
                    var location = list[p].Location;
                    points[p++] = new NXOpen.Point3d(location.X * factor, location.Y * factor, location.Z * factor);
                }
            }

            return points;
        }

        public static NXOpen.Curve ToNurbsSpline(this NurbsCurve value, double factor)
        {
            var degree = value.Degree;
            var knots = ToDoubleArray(value.Knots, degree);

            Debug.Assert(degree > 2 || value.SpanCount == 1);
            Debug.Assert(degree >= 1);
            Debug.Assert(value.Points.Count > degree);
            Debug.Assert(knots.Length == degree + value.Points.Count + 1);

            var weights = value.Points.Select(p => p.Weight).ToArray();
            double[,] controlPts = new double[value.Points.Count, 4];
            for (int i = 0; i < value.Points.Count; i++)
            {
                var currentPoint3d = value.Points.ElementAt(i);
                controlPts[i, 0] = currentPoint3d.X * factor;
                controlPts[i, 1] = currentPoint3d.Y * factor;
                controlPts[i, 2] = currentPoint3d.Z * factor;
                controlPts[i, 3] = value.IsRational ? currentPoint3d.Weight : 1.0;
            }

            NXOpen.UF.UFCurve.Spline splineData = new NXOpen.UF.UFCurve.Spline()
            {
                is_rational = value.IsRational ? 1 : 0,
                knots = knots.ToArray(),
                order = value.Order,
                num_poles = value.Points.Count,
                start_param = 0.0,
                end_param = 1.0,
                poles = controlPts,
            };

            TheUfSession.Curve.CreateSpline(ref splineData, out NXOpen.Tag splineTag, out int numStates, out NXOpen.UF.UFCurve.State[] states);

            Globals.TheUfSession.Modl.Update();

            //splineTag.GetTaggedObject().GetType().ToString().ListingWindowWriteLine();

            return splineTag.GetTaggedObject() as NXOpen.Curve;

        }

        public static IEnumerable<NXOpen.Curve> ToCurveMany(this NurbsCurve value) => value.ToCurveMany(UnitConverter.RhinoToNXUnitsRatio);
        public static IEnumerable<NXOpen.Curve> ToCurveMany(this NurbsCurve value, double factor)
        {
            if (value.Degree == 1)
            {
                var curvePoints = value.Points;
                int pointCount = curvePoints.Count;
                if (pointCount > 1)
                {
                    NXOpen.Point3d end, start = curvePoints[0].Location.ToXYZ(factor);
                    for (int p = 1; p < pointCount; start = end, ++p)
                    {
                        end = curvePoints[p].Location.ToXYZ(factor);
                        yield return WorkPart.Curves.CreateLine(start, end);
                    }
                }
            }
            else if (value.Degree == 2)
            {
                for (int s = 0; s < value.SpanCount; ++s)
                {
                    var segment = value.Trim(value.SpanDomain(s)) as NurbsCurve;
                    yield return CurveEncoder.ToNurbsSpline(segment, factor);
                }
            }
            else if (value.IsClosed(DistanceTolerance * 1.01))
            {
                var segments = value.DuplicateSegments();
                if (segments.Length == 1)
                {
                    if
                    (
                      value.NormalizedLengthParameter(0.5, out var mid) &&
                      value.Split(mid) is Curve[] half
                    )
                    {
                        yield return (half[0] as NurbsCurve).ToNurbsSpline(factor);
                        yield return (half[1] as NurbsCurve).ToNurbsSpline(factor);
                    }
                    else throw new Exception("Failed to Split closed Edge");
                }
                else
                {
                    foreach (var segment in segments)
                        yield return (segment as NurbsCurve).ToNurbsSpline(factor);
                }
            }
            else if (value.GetNextDiscontinuity(Continuity.C1_continuous, value.Domain.Min, value.Domain.Max, out var t))
            {
                var splitters = new List<double>() { t };
                while (value.GetNextDiscontinuity(Continuity.C1_continuous, t, value.Domain.Max, out t))
                    splitters.Add(t);

                var segments = value.Split(splitters);
                foreach (var segment in segments.Cast<NurbsCurve>())
                    yield return CurveEncoder.ToNurbsSpline(segment, factor);
            }
            else
            {
                yield return CurveEncoder.ToNurbsSpline(value, factor);
            }
        }
        #endregion

        #region Polyline
        public static NXOpen.Line[] ToLines(this Polyline value) => value.ToLines(UnitConverter.RhinoToNXUnitsRatio);
        public static NXOpen.Line[] ToLines(this Polyline value, double factor)
        {
            value.ReduceSegments(DistanceTolerance);

            int count = value.Count;
            var list = new NXOpen.Line[Math.Max(0, count - 1)];
            if (count > 1)
            {
                var point = value[0];
                NXOpen.Point3d end, start = new NXOpen.Point3d(point.X * factor, point.Y * factor, point.Z * factor);
                for (int p = 1; p < count; start = end, ++p)
                {
                    point = value[p];
                    end = new NXOpen.Point3d(point.X * factor, point.Y * factor, point.Z * factor);
                    list[p - 1] = WorkPart.Curves.CreateLine(start, end);
                }
            }

            return list;
        }

        public static NXOpen.Line[] ToPolyLine(this Polyline value) => value.ToPolyLine(UnitConverter.RhinoToNXUnitsRatio);
        public static NXOpen.Line[] ToPolyLine(this Polyline value, double factor)
        {
            int count = value.Count;
            NXOpen.Line[] lines = new NXOpen.Line[value.SegmentCount];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = value.SegmentAt(i).ToLine();
            }

            return lines;
        }
        #endregion

        #region PolylineCurve
        public static IEnumerable<NXOpen.Curve> ToCurveMany(this PolylineCurve value) => value.ToCurveMany(UnitConverter.RhinoToNXUnitsRatio);
        public static IEnumerable<NXOpen.Curve> ToCurveMany(this PolylineCurve value, double factor)
        {
            int pointCount = value.PointCount;
            if (pointCount > 1)
            {
                NXOpen.Point3d end, start = value.Point(0).ToXYZ(factor);
                for (int p = 1; p < pointCount; start = end, ++p)
                {
                    end = value.Point(p).ToXYZ(factor);
                    yield return WorkPart.Curves.CreateLine(start, end);
                }
            }
        }
        #endregion
    }
}
