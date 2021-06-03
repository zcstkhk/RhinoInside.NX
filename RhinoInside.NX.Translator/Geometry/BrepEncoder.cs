using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rhino.Geometry;
using BodyStyle = NXOpen.GeometricUtilities.FeatureOptions.BodyStyle;
using RhinoInside.NX.Extensions;
using System;
using static RhinoInside.NX.Extensions.Globals;
using RhinoInside.NX.Translator.Geometry.Raw;

namespace RhinoInside.NX.Translator
{
    /// <summary>
    /// Converts a "complex" <see cref="Brep"/> to be transfered to a <see cref="NXOpen.Solid"/>.
    /// </summary>
    public static class BrepEncoder
    {
        private static NXOpen.Session _theSession = NXOpen.Session.GetSession();

        private static NXOpen.UF.UFSession _theUfSession = NXOpen.UF.UFSession.GetUFSession();

        #region Encode
        //internal static Brep ToRawBrep(/*const*/ Brep brep, double scaleFactor)
        //{
        //    brep = brep.DuplicateShallow() as Brep;
        //    return EncodeRaw(ref brep, scaleFactor) ? brep : default;
        //}

        //internal static bool EncodeRaw(ref Brep brep, double scaleFactor)
        //{
        //    if (scaleFactor != 1.0 && !brep.Scale(scaleFactor))
        //        return default;

        //    var bbox = brep.GetBoundingBox(false);
        //    if (!bbox.IsValid || bbox.Diagonal.Length < Revit.ShortCurveTolerance)
        //        return default;

        //    return SplitFaces(ref brep);
        //}

        //static bool SplitFaces(ref Brep brep)
        //{
        //    Brep brepToSplit = null;
        //    while (!ReferenceEquals(brepToSplit, brep))
        //    {
        //        brepToSplit = brep;

        //        foreach (var face in brepToSplit.Faces)
        //        {
        //            var splitters = new List<Curve>();

        //            var trimsBBox = BoundingBox.Empty;
        //            foreach (var trim in face.OuterLoop.Trims)
        //                trimsBBox.Union(trim.GetBoundingBox(true));

        //            var domainUV = new Interval[]
        //            {
        //    new Interval(trimsBBox.Min.X, trimsBBox.Max.X),
        //    new Interval(trimsBBox.Min.Y, trimsBBox.Max.Y),
        //            };

        //            // Compute splitters
        //            var splittedUV = new bool[2] { false, false };
        //            for (int d = 0; d < 2; d++)
        //            {
        //                var domain = domainUV[d];
        //                var t = domain.Min;

        //                while (face.GetNextDiscontinuity(d, Continuity.Gsmooth_continuous, t, domain.Max, out t))
        //                {
        //                    splitters.AddRange(face.TrimAwareIsoCurve(1 - d, t));
        //                    splittedUV[d] = true;
        //                }
        //            }

        //            var closedUV = new bool[2] { face.IsClosed(0), face.IsClosed(1) };
        //            if (!splittedUV[0] && closedUV[0])
        //            {
        //                splitters.AddRange(face.TrimAwareIsoCurve(1, face.Domain(0).Mid));
        //                splittedUV[0] = true;
        //            }
        //            if (!splittedUV[1] && closedUV[1])
        //            {
        //                splitters.AddRange(face.TrimAwareIsoCurve(0, face.Domain(1).Mid));
        //                splittedUV[1] = true;
        //            }

        //            if (splitters.Count > 0)
        //            {
        //                var surfaceIndex = face.SurfaceIndex;
        //                var splitted = face.Split(splitters, Revit.ShortCurveTolerance);
        //                if (splitted is null)
        //                {
        //                    Debug.Fail("BrepFace.Split", "Failed to split a closed face.");
        //                    return false;
        //                }

        //                if (brepToSplit.Faces.Count == splitted.Faces.Count)
        //                {
        //                    // Split was ok but for tolerance reasons no new faces were created.
        //                    // Too near from the limits.
        //                }
        //                else
        //                {

        //                    foreach (var f in splitted.Faces)
        //                    {
        //                        if (f.SurfaceIndex != surfaceIndex)
        //                            continue;

        //                        if (splittedUV[0] && splittedUV[1])
        //                            f.ShrinkFace(BrepFace.ShrinkDisableSide.ShrinkAllSides);
        //                        else if (splittedUV[0])
        //                            f.ShrinkFace(BrepFace.ShrinkDisableSide.DoNotShrinkSouthSide | BrepFace.ShrinkDisableSide.DoNotShrinkNorthSide);
        //                        else if (splittedUV[1])
        //                            f.ShrinkFace(BrepFace.ShrinkDisableSide.DoNotShrinkEastSide | BrepFace.ShrinkDisableSide.DoNotShrinkWestSide);
        //                    }

        //                    // Start again until no face is splitted
        //                    brep = splitted;
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    return brep is object;
        //}
        #endregion

        #region Transfer
        /// <summary>
        /// Replaces <see cref="Raw.RawEncoder.ToHost(Brep)"/> to catch Revit Exceptions
        /// </summary>
        /// <param name="brep"></param>
        /// <returns></returns>
        public static NXOpen.Body ToSolid(/*const*/ Brep brep)
        {
            try
            {
                BodyStyle brepType;
                switch (brep.SolidOrientation)
                {
                    case BrepSolidOrientation.Inward: brepType = BodyStyle.Sheet; break;
                    case BrepSolidOrientation.Outward: brepType = BodyStyle.Solid; break;
                }

                List<NXOpen.Body> faceBodies = new List<NXOpen.Body>();
                foreach (var face in brep.Faces)
                {
                    using (var nurbSurface = face.ToNurbsSurface())
                    {
                        List<(List<NXOpen.Curve> edgeCurves, List<int> directions)> boundaryEdgeCurves = new List<(List<NXOpen.Curve>, List<int>)>();
                        for (int i = 0; i < face.Loops.Count; i++)
                        {
                            List<NXOpen.Curve> currentEdgeCurves = new List<NXOpen.Curve>();

                            List<int> currentDirections = new List<int>();

                            var trims = face.Loops[i].Trims;
                            for (int j = 0; j < trims.Count; j++)
                            {
                                if (trims[j].TrimType != BrepTrimType.Boundary && trims[j].TrimType != BrepTrimType.Mated)
                                    continue;

                                BrepEdge edge = trims[j].Edge;

                                if (edge is null)
                                    continue;

                                var currentEdgeCurve = edge.Trim(edge.Domain).ToCurve();

                                currentEdgeCurves.Add(currentEdgeCurve);
                                currentDirections.Add(edge.ProxyCurveIsReversed ? -1 : 1);
                            }

                            boundaryEdgeCurves.Add((currentEdgeCurves, currentDirections));
                        }

                        if (nurbSurface.IsPlanar())
                        {
                            // 使用有界平面创建
                            var boundaryCurves = boundaryEdgeCurves.SelectMany(obj => obj.edgeCurves).ToArray();
                            var boundedPlaneBody = _theSession.Parts.Work.Features.CreateBoundedPlane(boundaryCurves).GetBodies()[0];
                            _theSession.Parts.Work.Features.RemoveParameters(boundedPlaneBody);
                            faceBodies.Add(boundedPlaneBody);
                            //boundaryCurves.Delete();

                            //NXOpen.UF.StringList bplaneBoundaries = new NXOpen.UF.StringList()
                            //{
                            //    num = boundaryEdgeCurves.Count,
                            //    id = boundaryEdgeCurves.SelectMany(obj => obj.edgeCurveTags).ToArray(),
                            //    dir = boundaryEdgeCurves.SelectMany(obj => obj.directions).ToArray(),
                            //    _string = boundaryEdgeCurves.Select(obj => obj.edgeCurveTags.Count).ToArray()
                            //};

                            //double[] tolerrances = new double[bplaneBoundaries.dir.Length];
                            //for (int i = 0; i < tolerrances.Length; i++)
                            //    tolerrances[i] = DistanceTolerance;
                            //theUfSession.Modl.CreateBplane(ref bplaneBoundaries, tolerrances, out NXOpen.Tag body);

                            //bplaneBoundaries.id.Select(obj => obj.GetTaggedObject() as NXOpen.NXObject).ToArray().Delete();
                        }
                        else
                        {
                            var faceBody = RawEncoder.ToHost(nurbSurface);

                            #region 找到一个位于面上的点
                            Point3d point3dOnFace = new Point3d();
                            bool findInteriorPoint = false;
                            if (face.IsPointOnFace(face.Domain(0).Mid, face.Domain(1).Mid) == PointFaceRelation.Interior)
                            {
                                face.Evaluate(face.Domain(0).Mid, face.Domain(1).Mid, 0, out point3dOnFace, out _);
                                findInteriorPoint = true;
                            }
                            else
                            {
                                var firstEdge = face.Loops.ElementAt(0).Trims.ElementAt(0).Edge;

                                Point3d ptOnFirstEdge = firstEdge.PointAt(firstEdge.Domain.Mid);

                                face.ClosestPoint(ptOnFirstEdge, out double u, out double v);

                                double offsetValue = 0.02;

                                u += offsetValue;
                                v += offsetValue;
                                if (face.IsPointOnFace(u, v) == PointFaceRelation.Interior)
                                {
                                    face.Evaluate(u, v, 0, out point3dOnFace, out _);
                                    findInteriorPoint = true;
                                }
                                else
                                {
                                    u -= 2 * offsetValue;
                                    v -= 2 * offsetValue;
                                    if (face.IsPointOnFace(u, v) == PointFaceRelation.Interior)
                                    {
                                        face.Evaluate(u, v, 0, out point3dOnFace, out _);
                                        findInteriorPoint = true;
                                    }
                                }
                            }

                            if (!findInteriorPoint)
                            {
                                findInteriorPoint.ToString().ListingWindowWriteLine();
                                var copiedFaceBody = faceBody.CopyAndPaste()[0] as NXOpen.Body;
                                copiedFaceBody.SetColor(216);
                            }
                            #endregion

                            for (int i = 0; i < boundaryEdgeCurves.Count; i++)
                            {
                                NXOpen.UF.UFModl.TrimObject[] trimBoundaryObjects = new NXOpen.UF.UFModl.TrimObject[boundaryEdgeCurves[i].edgeCurves.Count];

                                for (int j = 0; j < trimBoundaryObjects.Length; j++)
                                {
                                    trimBoundaryObjects[j] = new NXOpen.UF.UFModl.TrimObject { object_tag = boundaryEdgeCurves[i].edgeCurves[j].Tag, curve_project_method = boundaryEdgeCurves[i].directions[j] };
                                }

                                _theUfSession.Modl.TrimSheet(faceBody.Tag, trimBoundaryObjects.Length, trimBoundaryObjects, null, 1, 1, point3dOnFace.ToXYZ().ToArray(), 0.01, out int numGapPoints, out double[] gapPoints);
                            }

                            faceBodies.Add(faceBody);
                        }
#if !DEBUG
                        boundaryEdgeCurves.SelectMany(obj => obj.edgeCurves).ToArray().Delete();
#endif
                    }
                }

                NXOpen.Body resultBody = null;
                if (faceBodies.Count > 1)
                {
                    var sewFeature = WorkPart.Features.CreateSew(faceBodies.ToArray());
                    resultBody = sewFeature.GetBodies()[0];
#if !DEBUG
                    resultBody.RemoveParameter();
#endif
                }
                else
                    resultBody = faceBodies[0];

                var brepArea = brep.GetArea();
                var bodyArea = resultBody.GetArea();
                resultBody.SetUserAttribute("Area", -1, bodyArea, NXOpen.Update.Option.Now);
                var error = Math.Abs(brepArea - bodyArea) / brepArea;
                resultBody.SetUserAttribute("Error", -1, error, NXOpen.Update.Option.Now);
                if (error > 0.001)
                    resultBody.SetColor(186);
            }
            catch (NXOpen.NXException e)
            {
                Logger.Error(e.ToString());
            }

            return null;
        }

        static NXOpen.Line ToEdgeCurve(LineCurve curve)
        {
            return curve.Line.ToLine();
        }

        static IEnumerable<NXOpen.Line> ToEdgeCurveMany(PolylineCurve curve)
        {
            NXOpen.Part workPart = _theSession.Parts.Work;

            int pointCount = curve.PointCount;
            if (pointCount > 1)
            {
                var point = curve.Point(0);
                NXOpen.Point3d end, start = new NXOpen.Point3d(point.X, point.Y, point.Z);
                for (int p = 1; p < pointCount; start = end, ++p)
                {
                    point = curve.Point(p);
                    end = new NXOpen.Point3d(point.X, point.Y, point.Z);
                    yield return workPart.Curves.CreateLine(start, end);
                }
            }
        }

        static IEnumerable<NXOpen.Arc> ToEdgeCurveMany(ArcCurve curve)
        {
            NXOpen.Point3d ToXYZ(Point3d p) => new NXOpen.Point3d(p.X, p.Y, p.Z);

            if (curve.IsClosed(DistanceTolerance * 1.01))
            {
                var interval = curve.Domain;
                double min = interval.Min, mid = interval.Mid, max = interval.Max;
                var points = new NXOpen.Point3d[]
                {
          ToXYZ(curve.PointAt(min)),
          ToXYZ(curve.PointAt(min + (mid - min) * 0.5)),
          ToXYZ(curve.PointAt(mid)),
          ToXYZ(curve.PointAt(mid + (max - mid) * 0.5)),
          ToXYZ(curve.PointAt(max)),
                };

                yield return WorkPart.Curves.CreateArc(points[0], points[2], points[1], false, out _);
                yield return WorkPart.Curves.CreateArc(points[2], points[4], points[3], false, out _);
            }
            else yield return WorkPart.Curves.CreateArc(ToXYZ(curve.Arc.StartPoint), ToXYZ(curve.Arc.EndPoint), ToXYZ(curve.Arc.MidPoint), false, out _);
        }

        static IEnumerable<NXOpen.Curve> ToEdgeCurveMany(PolyCurve curve)
        {
            int segmentCount = curve.SegmentCount;
            for (int s = 0; s < segmentCount; ++s)
            {
                foreach (var segment in ToEdgeCurveMany(curve.SegmentCurve(s)))
                    yield return segment;
            }
        }

        static IEnumerable<NXOpen.Curve> ToEdgeCurveMany(Curve curve)
        {
            switch (curve)
            {
                case LineCurve lineCurve:

                    yield return ToEdgeCurve(lineCurve);
                    yield break;

                case PolylineCurve polylineCurve:

                    foreach (var line in ToEdgeCurveMany(polylineCurve))
                        yield return line;
                    yield break;

                case ArcCurve arcCurve:

                    foreach (var arc in ToEdgeCurveMany(arcCurve))
                        yield return arc;
                    yield break;

                case PolyCurve polyCurve:

                    foreach (var segment in ToEdgeCurveMany(polyCurve))
                        yield return segment;
                    yield break;

                case NurbsCurve nurbsCurve:

                    foreach (var nurbs in nurbsCurve.ToCurveMany(UnitConverter.NoScale))
                        yield return nurbs;
                    yield break;

                default:
                    if (curve.HasNurbsForm() != 0)
                    {
                        var nurbsForm = curve.ToNurbsCurve();
                        foreach (var c in nurbsForm.ToCurveMany(UnitConverter.NoScale))
                            yield return c;
                    }
                    else throw new Exception($"Unable to convert {curve} to DB.Curve");
                    yield break;
            }
        }

        //static IEnumerable<NXOpen.Curve> ToBRepBuilderEdgeGeometry(BrepEdge edge)
        //{
        //    var edgeCurve = edge.EdgeCurve.Trim(edge.Domain) ?? edge.EdgeCurve;

        //    if (edgeCurve is null || edge.IsShort(DistanceTolerance, edge.Domain))
        //    {
        //        Debug.WriteLine($"Short edge skipped, Length = {edge.GetLength(edge.Domain)}");
        //        return Enumerable.Empty<NXOpen.Curve>();
        //    }

        //    if (edge.ProxyCurveIsReversed)
        //        edgeCurve.Reverse();

        //    if (edgeCurve.RemoveShortSegments(DistanceTolerance))
        //        Debug.WriteLine("Short segment removed");

        //    return ToEdgeCurveMany(edgeCurve).Select(x => NXOpen.BRepBuilderEdgeGeometry.Create(x));
        //}
        #endregion
    }
}
