using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using NXOpen.Extensions;
using static NXOpen.Extensions.Globals;
using PK = PLMComponents.Parasolid.PK_.Unsafe;

namespace RhinoInside.NX.Translator.Geometry.Raw
{
    public static partial class RawDecoder
    {
        public struct BrepBoundary
        {
            public BrepLoopType type;
            public List<BrepEdge> edges;
            public PolyCurve trims;
            public List<NXOpen.Sense> orientation;
        }

        public static int AddSurface(Brep brep, NXOpen.Face face, out List<BrepBoundary>[] shells, Dictionary<NXOpen.Edge, BrepEdge> brepEdges = null)
        {
            // Extract base surface
            if (ToRhinoSurface(face, out bool parametricOrientation) is Surface surface)
            {
                int si = brep.AddSurface(surface);

                if (surface is PlaneSurface planar)
                {
                    var nurbs = planar.ToNurbsSurface();
                    nurbs.KnotsU.InsertKnot(surface.Domain(0).Mid);
                    nurbs.KnotsV.InsertKnot(surface.Domain(1).Mid);
                    surface = nurbs;
                }

                var brepFace = brep.Faces.Add(si);

                // 抽取并分类边的 Loop
                var edgeLoops = new List<BrepBoundary>(face.GetLoops().Length);
                foreach (var currentLoop in face.GetLoops())
                {
                    if (currentLoop.Edges.Length == 0)
                        continue;

                    var loopVertices = currentLoop.Vertices;

                    Dictionary<int, BrepVertex> loopVerticesDic = new Dictionary<int, BrepVertex>();

                    foreach (var vertex in loopVertices)
                    {
                        var brepVertex = brep.Vertices.Add(new Point3d(vertex.Position.X, vertex.Position.Y, vertex.Position.Z), DistanceTolerance);
                        loopVerticesDic.Add(vertex.ParasolidTag, brepVertex);
                    }

                    var brepLoop = brep.Loops.Add(currentLoop.Type == NXOpen.Extensions.Topology.LoopType.Inner ? BrepLoopType.Inner : BrepLoopType.Outer, brepFace);

                    foreach (var loopEdge in currentLoop.Edges)
                    {
                        var brepCurve3dIndex = brep.Curves3D.Add(loopEdge.ToCurve().ToRhino());

                        var brepEdge = brep.Edges.Add(loopVerticesDic[loopEdge.GetParasolidVertices()[0].ParasolidTag], loopVerticesDic[loopEdge.GetParasolidVertices()[1].ParasolidTag], brepCurve3dIndex, Globals.DistanceTolerance);

                        var curve2D = brep.Curves2D.Add(CreateTrimmingCurve());

                        // Create new trim topology that references edge, direction
                        // reletive to edge, loop and trim curve geometry
                        var trim = brep.Trims.Add(brepEdge, false, brepLoop, curve2D);
                        trim.IsoStatus = IsoStatus.None;

                        // This one Brep face, so all trims are boundary ones.
                        trim.TrimType = BrepTrimType.Boundary;

                        // This simple example is exact - for models with
                        // non-exact data, set tolerance as explained in
                        // definition of BrepTrim.
                        trim.SetTolerances(0.0, 0.0);
                    }

                    continue;













                    var edges = currentLoop.Edges;
                    if (face.GetSense() == NXOpen.Sense.Reverse)
                        edges = edges.Reverse().ToArray();

                    var loop = new BrepBoundary()
                    {
                        type = BrepLoopType.Unknown,
                        edges = new List<BrepEdge>(edges.Length),
                        trims = new PolyCurve(),
                        orientation = new List<NXOpen.Sense>(edges.Length),
                    };

                    foreach (var edge in edges)
                    {
                        var brepEdge = default(BrepEdge);
                        var curve = edge.ToCurve();
                        if (brepEdges?.TryGetValue(edge, out brepEdge) != true)
                        {
                            if (curve is null)
                                continue;

                            var curveIndex = brep.AddEdgeCurve(curve.ToRhino());

                            brepEdge = brep.Edges.Add(loopVerticesDic[edge.GetParasolidVertices()[0].ParasolidTag], loopVerticesDic[edge.GetParasolidVertices()[1].ParasolidTag], curveIndex, Globals.DistanceTolerance);
                            //brepEdge.SetStartPoint = loopVerticesDic[edge.Vertices[0]];
                            brepEdges?.Add(edge, brepEdge);
                        }

                        loop.edges.Add(brepEdge);

                        Rhino.Geometry.Curve segment = null;

                        var currentEdgeFin = currentLoop.Fins.FirstOrDefault(obj => obj.Edge.Tag == edge.Tag);
                        if (currentEdgeFin.Sense == NXOpen.Sense.Reverse)
                        {
                            unsafe
                            {
                                TheUfSession.Ps.AskPsTagOfObject(edge.Tag, out var edgePsTag);

                                PK.CURVE_t edgeCurve;
                                PK.EDGE.ask_curve(new PK.EDGE_t((int)edgePsTag), &edgeCurve);

                                PK.CURVE_t reversedPsCurve;
                                PK.CURVE.make_curve_reversed(edgeCurve, &reversedPsCurve);

                                //TheUfSession.Ps.AskObjectOfPsTag((NXOpen.Tag)reversedPsCurve.Value, out var reversedCurve);

                                //segment = (reversedCurve.GetTaggedObject() as NXOpen.Curve).ToRhino();
                            }
                        }

                        segment = curve.ToRhino();

                        loop.orientation.Add(currentEdgeFin.Sense);

                        var trim = surface.Pullback(segment, DistanceTolerance);
                        loop.trims.Append(trim);
                    }

                    loop.trims.MakeClosed(DistanceTolerance);

                    switch (loop.trims.ClosedCurveOrientation())
                    {
                        case CurveOrientation.Undefined:
                            loop.type = BrepLoopType.Unknown;
                            break;
                        case CurveOrientation.Clockwise:
                            loop.type = parametricOrientation ? BrepLoopType.Inner : BrepLoopType.Outer;
                            break;
                        case CurveOrientation.CounterClockwise:
                            loop.type = parametricOrientation ? BrepLoopType.Outer : BrepLoopType.Inner;
                            break;
                        default:
                            break;
                    }

                    edgeLoops.Add(loop);
                }

                var outerLoops = edgeLoops.Where(obj => obj.type == BrepLoopType.Outer);
                var innerLoops = edgeLoops.Where(obj => obj.type == BrepLoopType.Inner);

                // 将边的 Loops 归类到 shells 重，使外部loop为第一个
                shells = outerLoops.Select(obj => new List<BrepBoundary>() { obj }).ToArray();

                if (shells.Length == 1)
                {
                    shells[0].AddRange(innerLoops);
                }
                else
                {
                    foreach (var edgeLoop in innerLoops)
                    {
                        foreach (var shell in shells)
                        {
                            var containment = Curve.PlanarClosedCurveRelationship(edgeLoop.trims, shell[0].trims, Plane.WorldXY, DistanceTolerance);
                            if (containment == RegionContainment.AInsideB)
                            {
                                shell.Add(edgeLoop);
                                break;
                            }
                        }
                    }
                }

                return si;


                //// 1、 添加顶点  2、创建3D 曲线
                //var faceEdges = face.GetEdges();
                //for (int i = 0; i < faceEdges.Length; i++)
                //{
                //    var vertices = faceEdges[i].GetVertices();
                //    brep.Vertices.Add(vertices.startPoint3d.ToRhino(), DistanceTolerance);
                //    brep.Vertices.Add(vertices.endPoint3d.ToRhino(), DistanceTolerance);

                //    TheUfSession.Modl.CreateCurveFromEdge(faceEdges[i].Tag, out var edgeCurveTag);
                //    brep.Curves3D.Add((edgeCurveTag.GetTaggedObject() as NXOpen.Curve).ToRhino());
                //}

                //// 3、创建边
                //for (int i = 0; i < faceEdges.Length; i++)
                //{
                //    brep.Edges.Add(i * 2, i * 2 + 1, i, 0.0);
                //}

                //// 4、创建曲面
                //int si = brep.AddSurface(surface);

                //// 5、修剪
                //BrepFace brepFace = brep.Faces.Add(si);
                //Surface brepSurface = brep.Surfaces[brepFace.SurfaceIndex];

                //Snap.NX.Face snapFace = Snap.NX.Face.Wrap(face.Tag);
                //Snap.Topology.Loop[] snapFaceLoops = snapFace.Loops;

                //for (int i = 0; i < snapFaceLoops.Length; i++)
                //{
                //    var loop = brep.Loops.Add((BrepLoopType)snapFaceLoops[i].Type, brepFace);

                //    var finCount = snapFaceLoops[i].Fins.Length;
                //    for (int e = 0; e < finCount; ++e)
                //    {
                //        var currentFin = snapFaceLoops[i].Fins[e];

                //        var rhinoCurve = (snapFaceLoops[i].Fins[e].Edge.ToCurve().NXOpenTaggedObject as NXOpen.Curve).ToRhino();

                //        int ti = brep.AddTrimCurve(rhinoCurve);

                //        brep.Trims.Add(currentFin.Sense == Snap.Topology.Sense.Positive, loop, ti);
                //    }
                //}

                //shells = null;

                //return 0;

















            }

            shells = default;
            return -1;
        }

        private static Curve CreateTrimmingCurve()
        {
            // A trimming curve is a 2d curve whose image lies in the surface's domain.
            // The "active" portion of the surface is to the left of the trimming curve.
            // An outer trimming loop consists of a simple closed curve running 
            // counter-clockwise around the region it trims.
            //
            // An inner trimming loop consists of a simple closed curve running 
            // clockwise around the region the hole.

            var curve = new LineCurve(new Point2d(0.0, 0.0), new Point2d(1.0, 1.0)) { Domain = new Interval(0.0, 1.0) };

            return curve;
        }

        static void TrimSurface(Brep brep, int surface, bool orientationIsReversed, List<BrepBoundary>[] shells)
        {
            foreach (var shell in shells)
            {
                BrepFace brepFace = brep.Faces.Add(surface);
                brepFace.OrientationIsReversed = orientationIsReversed;

                foreach (var loop in shell)
                {
                    var brepLoop = brep.Loops.Add(loop.type, brepFace);

                    var edgeCount = loop.edges.Count;
                    for (int e = 0; e < edgeCount; ++e)
                    {
                        var brepEdge = loop.edges[e];

                        int orientation = (int)loop.orientation[e];
                        if (orientation == 0)
                            continue;

                        if (loop.trims.SegmentCurve(e) is Curve trim)
                        {
                            int ti = brep.AddTrimCurve(trim);
                            brep.Trims.Add(brepEdge, orientation < 0, brepLoop, ti);
                        }
                    }

                    brep.Trims.MatchEnds(brepLoop);
                }
            }
        }

        public static Brep ToRhinoBrep(NXOpen.Face face)
        {
            Brep brep;

            if (face is null)
                return null;

            brep = new Brep();

            // Set surface
            var si = AddSurface(brep, face, out List<BrepBoundary>[] shells);
            if (si < 0)
                return null;

            // Set edges & trims
            //Snap.NX.Face snapFace = Snap.NX.Face.Wrap(face.Tag);
            //TrimSurface(brep, si, snapFace.Sense == Snap.Topology.Sense.Negative, shells);

            // Set vertices
            //brep.SetVertices();

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
                log.ConsoleWriteLine();
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
    }
}
