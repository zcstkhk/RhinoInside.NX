using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Rhino.Geometry;
using RhinoInside.NX.Extensions;
using static RhinoInside.NX.Extensions.Globals;

namespace RhinoInside.NX.Translator.Geometry.Raw
{
    /// <summary>
    /// 此类中的方法是将 NX 几何体转换为 NX 单位的对象。
    /// <para>如需在 Rhino 中使用，需要转换到 Rhino 的单位。</para>
    /// </summary>
    static class RawDecoder
    {
        #region Values
        public static Point3d ToRhino(NXOpen.Point3d p)
        {
            return new Point3d(p.X, p.Y, p.Z);
        }

        public static Vector3d ToRhino(NXOpen.Vector3d p)
        {
            return new Vector3d(p.X, p.Y, p.Z);
        }

        public static Transform ToRhinoTransform(NXOpen.Matrix4x4 transform)
        {
            var value = new Transform
            {
                M00 = transform.Rxx,
                M10 = transform.Rxy,
                M20 = transform.Rxz,
                M30 = 0.0,

                M01 = transform.Ryx,
                M11 = transform.Ryy,
                M21 = transform.Ryz,
                M31 = 0.0,

                M02 = transform.Rzx,
                M12 = transform.Rzy,
                M22 = transform.Rzz,
                M32 = 0.0,

                M03 = transform.Sx,
                M13 = transform.Sy,
                M23 = transform.Sz,
                M33 = 1.0
            };

            return value;
        }

        public static Plane ToRhino(NXOpen.Plane plane)
        {
            return new Plane(ToRhino(plane.Origin), ToRhino(plane.Matrix.GetAxisX()), ToRhino(plane.Matrix.GetAxisY()));
        }
        #endregion

        #region Point
        public static Point ToRhino(NXOpen.Point point)
        {
            return new Point(ToRhino(point.Coordinates));
        }
        #endregion

        #region Curve
        public static LineCurve ToRhinoLineCurve(NXOpen.Line line)
        {
            return new LineCurve(new Line(ToRhino(line.StartPoint), ToRhino(line.EndPoint)));
        }

        public static ArcCurve ToRhinoArcCurve(NXOpen.Arc arc)
        {
            return new ArcCurve(new Circle(new Plane(ToRhino(arc.CenterPoint), ToRhino(arc.Matrix.Element.GetAxisX()), ToRhino(arc.Matrix.Element.GetAxisY())), arc.Radius), arc.StartAngle, arc.EndAngle);
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
        #endregion

        #region Surfaces

        public static Surface ToRhinoSurface(NXOpen.Face face, out bool parametricOrientation, double relativeTolerance = 0.0)
        {
            var faceData = face.GetData();
            parametricOrientation = faceData.NormalReversed;

            switch (faceData.FaceType)
            {
                case NXOpen.Face.FaceType.Planar: return ToRhinoPlaneSurface(face, relativeTolerance);
                case NXOpen.Face.FaceType.Conical: return ToRhinoRevSurface(face, relativeTolerance);
                case NXOpen.Face.FaceType.Cylindrical: return FromCylindricalSurface(face, relativeTolerance);
                case NXOpen.Face.FaceType.Spherical: return FromSphereSurface(face, relativeTolerance);
                default: return FromBsurf(face);
            }
        }


        static PlaneSurface ToRhinoPlaneSurface(NXOpen.Face face, double relativeTolerance)
        {
            double[] facePt = new double[3];
            double[] direction = new double[3];
            double[] box = new double[6];
            TheUfSession.Modl.AskFaceData(face.Tag, out int type, facePt, direction, box, out double radius, out double radData, out int normDir);

            var bboxUV = face.GetUVBoundingBox();

            var ctol = relativeTolerance * DistanceTolerance;
            var uu = new Interval(bboxUV.Min.U - ctol, bboxUV.Max.U + ctol);
            var vv = new Interval(bboxUV.Min.V - ctol, bboxUV.Max.V + ctol);

            var plane = new Plane(new Point3d(facePt[0], facePt[1], facePt[2]), new Vector3d(direction[0], direction[1], direction[2]));

            return new PlaneSurface(plane, uu, vv);
        }

        static RevSurface ToRhinoRevSurface(NXOpen.Face face, double relativeTolerance)
        {
            var bboxUV = face.GetUVBoundingBox();
            var faceData = face.GetData();

            var atol = relativeTolerance * AngleTolerance * 10.0;
            var ctol = relativeTolerance * DistanceTolerance;
            var uu = new Interval(bboxUV.Min.U - atol, bboxUV.Max.U + atol);
            var vv = new Interval(bboxUV.Min.V - ctol, bboxUV.Max.V + ctol);

            var plane = new Plane(ToRhino(faceData.Point), ToRhino(faceData.Direction));

            var dir = ToRhino(faceData.Direction) + Math.Tan(faceData.RadiusData) * plane.XAxis;
            dir.Unitize();

            var curve = new LineCurve
            (
              new Line
              (
                plane.Origin + (vv.Min * dir),
                plane.Origin + (vv.Max * dir)
              ),
              vv.Min,
              vv.Max
            );

            var axis = new Line(plane.Origin, plane.Normal);
            return RevSurface.Create(curve, axis, uu.Min, uu.Max);
        }

        static RevSurface FromSphereSurface(NXOpen.Face face, double relativeTolerance)
        {
            var faceData = face.GetData();

            return RevSurface.CreateFromSphere(new Sphere(ToRhino(faceData.Point), faceData.Radius));
        }

        static RevSurface FromCylindricalSurface(NXOpen.Face face, double relativeTolerance)
        {
            var bboxUV = face.GetUVBoundingBox();
            var faceData = face.GetData();

            var atol = relativeTolerance * AngleTolerance;
            var ctol = relativeTolerance * DistanceTolerance;
            var uu = new Interval(bboxUV.Min.U - atol, bboxUV.Max.U + atol);
            var vv = new Interval(bboxUV.Min.V - ctol, bboxUV.Max.V + ctol);

            var plane = new Plane(ToRhino(faceData.Point), ToRhino(faceData.Direction));

            var curve = new LineCurve
            (
              new Line
              (
                plane.Origin + (faceData.Radius * plane.XAxis) + (vv.Min * ToRhino(faceData.Direction)),
                plane.Origin + (faceData.Radius * plane.XAxis) + (vv.Max * ToRhino(faceData.Direction))
              ),
              vv.Min,
              vv.Max
            );

            var axis = new Line(plane.Origin, plane.Normal);
            return RevSurface.Create(curve, axis, uu.Min, uu.Max);
        }







        //        public static NurbsSurface ToRhinoSurface(DB.HermiteFace face, double relativeTolerance)
        //        {
        //            NurbsSurface nurbsSurface = default;
        //            try
        //            {
        //#if REVIT_2021
        //        using (var surface = DB.ExportUtils.GetNurbsSurfaceDataForSurface(face.GetSurface()))
        //          nurbsSurface = ToRhino(surface, face.GetBoundingBox());
        //#else
        //                using (var surface = DB.ExportUtils.GetNurbsSurfaceDataForFace(face))
        //                    nurbsSurface = ToRhino(surface, face.GetBoundingBox());
        //#endif
        //            }
        //            catch (Autodesk.Revit.Exceptions.ApplicationException) { }

        //            if (nurbsSurface is null)
        //            {
        //                nurbsSurface = FromHermiteSurface
        //                (
        //                  face.Points,
        //                  face.MixedDerivs,
        //                  face.get_Params(0).Cast<double>().ToArray(),
        //                  face.get_Params(1).Cast<double>().ToArray(),
        //                  face.get_Tangents(0),
        //                  face.get_Tangents(1)
        //                );
        //            }

        //            if (nurbsSurface is object)
        //            {
        //                double ctol = relativeTolerance * Revit.ShortCurveTolerance * 5.0;
        //                if (ctol != 0.0)
        //                {
        //                    // Extend using smooth way avoids creating C2 discontinuities
        //                    nurbsSurface = nurbsSurface.Extend(IsoStatus.West, ctol, true) as NurbsSurface ?? nurbsSurface;
        //                    nurbsSurface = nurbsSurface.Extend(IsoStatus.East, ctol, true) as NurbsSurface ?? nurbsSurface;
        //                    nurbsSurface = nurbsSurface.Extend(IsoStatus.South, ctol, true) as NurbsSurface ?? nurbsSurface;
        //                    nurbsSurface = nurbsSurface.Extend(IsoStatus.North, ctol, true) as NurbsSurface ?? nurbsSurface;
        //                }
        //            }

        //            return nurbsSurface;
        //        }

        public static NurbsSurface FromBsurf(NXOpen.Face face)
        {
            throw new NotImplementedException("曲面转换尚未完成");

            TheUfSession.Modl.AskBsurf(face.Tag, out var surface);

            var degreeU = surface.order_u - 1;
            var degreeV = surface.order_v - 1;

            var knotsU = surface.knots_u;
            var knotsV = surface.knots_v;

            int controlPointCountU = knotsU.Length - degreeU - 1;
            int controlPointCountV = knotsV.Length - degreeV - 1;

            var nurbsSurface = NurbsSurface.Create(3, surface.is_rational == 1, degreeU + 1, degreeV + 1, controlPointCountU, controlPointCountV);

            //var controlPoints = surface.poles;
            //var weights = surface.GetWeights();

            //var points = nurbsSurface.Points;
            //for (int u = 0; u < controlPointCountU; u++)
            //{
            //    int u_offset = u * controlPointCountV;
            //    for (int v = 0; v < controlPointCountV; v++)
            //    {
            //        var pt = controlPoints[u_offset + v];
            //        if (surface.is_rational == 1)
            //        {
            //            double w = weights[u_offset + v];
            //            points.SetPoint(u, v, pt.X * w, pt.Y * w, pt.Z * w, w);
            //        }
            //        else
            //        {
            //            points.SetPoint(u, v, pt.X, pt.Y, pt.Z);
            //        }
            //    }
            //}

            //{
            //    var knots = nurbsSurface.KnotsU;
            //    int index = 0;
            //    foreach (var w in knotsU.Skip(1).Take(knots.Count))
            //        knots[index++] = w;
            //}

            //{
            //    var knots = nurbsSurface.KnotsV;
            //    int index = 0;
            //    foreach (var w in knotsV.Skip(1).Take(knots.Count))
            //        knots[index++] = w;
            //}

            return nurbsSurface;
        }


        #endregion

        #region Brep
        struct BrepBoundary
        {
            public BrepLoopType type;
            public List<BrepEdge> edges;
            public PolyCurve trims;
            public List<int> orientation;
        }

        static int AddSurface(Brep brep, NXOpen.Face face, out List<BrepBoundary>[] shells, Dictionary<NXOpen.Edge, BrepEdge> brepEdges = null)
        {
            throw new NotImplementedException("尚未完成此功能");

            // Extract base surface
            //if (ToRhinoSurface(face, out var parametricOrientation) is Surface surface)
            //{
            //    int si = brep.AddSurface(surface);

            //    if (surface is PlaneSurface planar)
            //    {
            //        var nurbs = planar.ToNurbsSurface();
            //        nurbs.KnotsU.InsertKnot(surface.Domain(0).Mid);
            //        nurbs.KnotsV.InsertKnot(surface.Domain(1).Mid);
            //        surface = nurbs;
            //    }

            //    // Extract and classify Edge Loops
            //    var edgeLoops = new List<BrepBoundary>(face.EdgeLoops.Size);
            //    foreach (var edgeLoop in face.GetEdges())
            //    {
            //        if (edgeLoop.IsEmpty)
            //            continue;

            //        var edges = edgeLoop.Cast<NXOpen.Edge>();

            //        var loop = new BrepBoundary()
            //        {
            //            type = BrepLoopType.Unknown,
            //            edges = new List<BrepEdge>(edgeLoop.Size),
            //            trims = new PolyCurve(),
            //            orientation = new List<int>(edgeLoop.Size)
            //        };

            //        foreach (var edge in edges)
            //        {
            //            var brepEdge = default(BrepEdge);
            //            if (brepEdges?.TryGetValue(edge, out brepEdge) != true)
            //            {
            //                var curve = edge.AsCurve();
            //                if (curve is null)
            //                    continue;

            //                brepEdge = brep.Edges.Add(brep.AddEdgeCurve(ToRhino(curve)));
            //                brepEdges?.Add(edge, brepEdge);
            //            }

            //            loop.edges.Add(brepEdge);
            //            var segment = ToRhino(edge.AsCurveFollowingFace(face));

            //            //if (!face.MatchesSurfaceOrientation())
            //            //    segment.Reverse();

            //            loop.orientation.Add(segment.TangentAt(segment.Domain.Mid).IsParallelTo(brepEdge.TangentAt(brepEdge.Domain.Mid)));

            //            var trim = surface.Pullback(segment, DistanceTolerance);
            //            loop.trims.Append(trim);
            //        }

            //        loop.trims.MakeClosed(DistanceTolerance);

            //        switch (loop.trims.ClosedCurveOrientation())
            //        {
            //            case CurveOrientation.Undefined: loop.type = BrepLoopType.Unknown; break;
            //            case CurveOrientation.CounterClockwise: loop.type = BrepLoopType.Outer; break;
            //            case CurveOrientation.Clockwise: loop.type = BrepLoopType.Inner; break;
            //        }

            //        edgeLoops.Add(loop);
            //    }

            //    var outerLoops = edgeLoops.Where(x => x.type == BrepLoopType.Outer);
            //    var innerLoops = edgeLoops.Where(x => x.type == BrepLoopType.Inner);

            //    // Group Edge loops in shells with the outer loop as the first one
            //    shells = outerLoops.
            //             Select(x => new List<BrepBoundary>() { x }).
            //             ToArray();

            //    if (shells.Length == 1)
            //    {
            //        shells[0].AddRange(innerLoops);
            //    }
            //    else
            //    {
            //        foreach (var edgeLoop in innerLoops)
            //        {
            //            foreach (var shell in shells)
            //            {
            //                var containment = Curve.PlanarClosedCurveRelationship(edgeLoop.trims, shell[0].trims, Plane.WorldXY, DistanceTolerance);
            //                if (containment == RegionContainment.AInsideB)
            //                {
            //                    shell.Add(edgeLoop);
            //                    break;
            //                }
            //            }
            //        }
            //    }

            //    return si;
            //}

            shells = default;
            return -1;
        }

        static void TrimSurface(Brep brep, int surface, List<BrepBoundary>[] shells)
        {
            foreach (var shell in shells)
            {
                var brepFace = brep.Faces.Add(surface);

                foreach (var loop in shell)
                {
                    var brepLoop = brep.Loops.Add(loop.type, brepFace);

                    var edgeCount = loop.edges.Count;
                    for (int e = 0; e < edgeCount; ++e)
                    {
                        var brepEdge = loop.edges[e];

                        int orientation = loop.orientation[e];
                        if (orientation == 0)
                            continue;

                        if (loop.trims.SegmentCurve(e) is Curve trim)
                        {
                            var ti = brep.AddTrimCurve(trim);
                            brep.Trims.Add(brepEdge, orientation < 0, brepLoop, ti);
                        }
                    }

                    brep.Trims.MatchEnds(brepLoop);
                }
            }
        }

        public static Brep ToRhino(NXOpen.Face face)
        {
            if (face is null)
                return null;

            var brep = new Brep();

            // Set surface
            var si = AddSurface(brep, face, out var shells);
            if (si < 0)
                return null;

            // Set edges & trims
            TrimSurface(brep, si, shells);

            // Set vertices
            brep.SetVertices();

            // Set flags
            brep.SetTolerancesBoxesAndFlags
            (
              true,
              true,
              true,
              true,
              true,
              true,
              true,
              true
            );

            if (!brep.IsValid)
            {
#if DEBUG
                brep.IsValidWithLog(out var log);
#endif
                brep.Repair(DistanceTolerance);
            }

            return brep;
        }

        //        public static Brep ToRhino(DB.Solid solid)
        //        {
        //            if (solid is null)
        //                return null;

        //            var brep = new Brep();

        //            if (!solid.Faces.IsEmpty)
        //            {
        //                var brepEdges = new Dictionary<DB.Edge, BrepEdge>();

        //                foreach (var face in solid.Faces.Cast<DB.Face>())
        //                {
        //                    // Set surface
        //                    var si = AddSurface(brep, face, out var shells, brepEdges);
        //                    if (si < 0)
        //                        continue;

        //                    // Set edges & trims
        //                    TrimSurface(brep, si, !face.MatchesSurfaceOrientation(), shells);
        //                }

        //                // Set vertices
        //                brep.SetVertices();

        //                // Set flags
        //                brep.SetTolerancesBoxesAndFlags
        //                (
        //                  true,
        //                  true,
        //                  true,
        //                  true,
        //                  true,
        //                  true,
        //                  true,
        //                  true
        //                );

        //                if (!brep.IsValid)
        //                {
        //#if DEBUG
        //                    brep.IsValidWithLog(out var log);
        //                    Debug.WriteLine($"{MethodInfo.GetCurrentMethod().DeclaringType.FullName}.{MethodInfo.GetCurrentMethod().Name}()\n{log}");
        //#endif
        //                    brep.Repair(Revit.VertexTolerance);
        //                }
        //            }

        //            return brep;
        //        }
        #endregion
    };
}
