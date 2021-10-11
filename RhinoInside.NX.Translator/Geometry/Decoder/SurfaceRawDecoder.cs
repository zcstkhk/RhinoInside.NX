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
        public static Brep ToRhinoBrep(NXOpen.Body body)
        {
            if (false)
            {
                Brep brep = new Brep();

                for (int i = 0; i < body.GetFaces().Length; i++)
                {
                    AddNXFace(body.GetFaces()[i], brep);
                }

                return brep;
            }
            else
                return JoinAndMerge(body.GetFaces().Select(x => ToRhinoBrep(x)).ToArray(), Globals.DistanceTolerance);
        }

        static Brep JoinAndMerge(ICollection<Brep> brepFaces, double tolerance)
        {
            if (brepFaces.Count == 0)
                return null;

            if (brepFaces.Count == 1)
                return brepFaces.First();

            var joinedBreps = Brep.JoinBreps(brepFaces.OfType<Brep>(), tolerance) ?? brepFaces;
            if (joinedBreps.Count == 1)
                return joinedBreps.First();

            var merged = Brep.MergeBreps(joinedBreps, Globals.DistanceTolerance);
            if (merged?.IsValid == false)
                merged.Repair(tolerance);

            return merged;
        }

        public static Brep ToRhinoBrep(NXOpen.Face face)
        {
            //using RhinoInside.NX.Translator.Geometry;
            //using NXOpen.Extensions;
            //using System.Linq;

            Brep brep = new Brep();

            AddNXFace(face, brep);

            return brep;
#if 添加顶点
                        Point3d startVertetexpnt = curve3d.PointAtStart;

                        BrepVertex startVertex = brep.Vertices.Add(startVertetexpnt, Globals.DistanceTolerance);

                        Point3d endVertetexpnt = curve3d.PointAtEnd;

                        BrepVertex endVertex = brep.Vertices.Add(endVertetexpnt, Globals.DistanceTolerance);

                        // Curve curve2d = surface.Pullback(curve3d, Globals.DistanceTolerance, new Interval(0, 1.0));

                        int startVertexIndex = startVertex.VertexIndex;

                        int endVertexIndex = endVertex.VertexIndex;

                        int curve3dIndex = brep.AddEdgeCurve(curve3d);

                        BrepEdge edge = brep.Edges.Add(startVertexIndex, endVertexIndex, curve3dIndex, Globals.DistanceTolerance);

                        int curve2dIndex = brep.AddTrimCurve(curve2d);

                        BrepTrim trim = brepLoop.Trims.Add(edge, currentFin.Sense == NXOpen.Sense.Forward, brepLoop, curve2dIndex);

                        trim.IsoStatus = Rhino.Geometry.IsoStatus.None;

                        trim.TrimType = BrepTrimType.Boundary;

                        trim.SetTolerances(Globals.DistanceTolerance, Globals.DistanceTolerance);
#endif            
        }

        private static void AddNXFace(NXOpen.Face face, Brep brep)
        {
            Surface surface = face.ToRhinoSurface();

            var surfIndex = brep.AddSurface(surface);

            var brepFace = brep.Faces.Add(surfIndex);

            // Console.WriteLine("***************************************************");

            try
            {
                var loopList = face.GetLoops().ToList();

                if (loopList.Count == 0)
                    return;

                if (loopList[0].Type != NXOpen.Extensions.Topology.LoopType.Outer && loopList[0].Type != NXOpen.Extensions.Topology.LoopType.LikelyOuter)
                {
                    var outerLoop = loopList.FirstOrDefault(obj => obj.Type == NXOpen.Extensions.Topology.LoopType.Outer || obj.Type == NXOpen.Extensions.Topology.LoopType.LikelyOuter);

                    if (outerLoop != null)
                    {
                        loopList.Remove(outerLoop);

                        loopList.Insert(0, outerLoop);
                    }
                }

                var faceSense = face.GetSense();

                // Console.WriteLine("Face Sense:" + faceSense);

                var outerLoopSense = loopList[0].FirstFin.Sense;

                // Console.WriteLine("Outer loop:" + outerLoopSense);

                for (int i = 0; i < loopList.Count; i++)
                {
                    // Console.WriteLine("********Loop*******");

                    var currentNxLoop = loopList[i];

                    if (currentNxLoop.Fins.Length == 0)
                        continue;

                    BrepLoopType brepLoopType;
                    switch (currentNxLoop.Type)
                    {
                        case NXOpen.Extensions.Topology.LoopType.LikelyOuter:
                        case NXOpen.Extensions.Topology.LoopType.Outer:
                            brepLoopType = BrepLoopType.Outer;
                            break;
                        case NXOpen.Extensions.Topology.LoopType.LikelyInner:
                        case NXOpen.Extensions.Topology.LoopType.Inner:
                        case NXOpen.Extensions.Topology.LoopType.InnerSingular:
                            brepLoopType = BrepLoopType.Inner;
                            break;
                        case NXOpen.Extensions.Topology.LoopType.Winding:
                            if (i == 0)
                                brepLoopType = BrepLoopType.Outer;
                            else
                                brepLoopType = BrepLoopType.Inner;
                            break;
                        case NXOpen.Extensions.Topology.LoopType.Unknown:
                        default:
                            brepLoopType = BrepLoopType.Unknown;
                            break;
                    }

                    var brepLoop = brep.Loops.Add(brepLoopType, brepFace);

                    var currentFin = currentNxLoop.FirstFin;

                    var trims = new PolyCurve();

                    Dictionary<BrepEdge, int> brepEdges = new Dictionary<BrepEdge, int>();

                    int finCount = 0;

                    do
                    {
                        finCount++;

                        var curve3d = currentFin.Edge.ToRhinoCurve();

                        if (currentFin.StartVertex != null && curve3d.PointAtStart.DistanceTo(currentFin.StartVertex.Position.ToRhino()) > Globals.DistanceTolerance)
                        {
                            curve3d.Reverse();
                        }

                        var brepEdgeCurve = brep.AddEdgeCurve(curve3d);

                        var brepEdge = brep.Edges.Add(brepEdgeCurve);

                        //Console.WriteLine("Start of Fin" + currentFin.StartVertex.Position);
                        //Console.WriteLine("Start of Edge" + curve3d.PointAtStart);
                        //Console.WriteLine("End of Fin" + currentFin.EndVertex.Position);
                        //Console.WriteLine("End of Edge" + curve3d.PointAtEnd);

                        brepEdges.Add(brepEdge, curve3d.TangentAt(curve3d.Domain.Mid).IsParallelTo(brepEdge.TangentAt(brepEdge.Domain.Mid)));

                        var curve2d = surface.Pullback(curve3d, Globals.DistanceTolerance);

                        trims.Append(curve2d);

                        currentFin = currentFin.Next;

                    } while (currentFin != currentNxLoop.FirstFin);

                    trims.MakeClosed(Globals.DistanceTolerance);

                    for (int j = 0; j < brepEdges.Count; j++)
                    {
                        if (trims.SegmentCurve(j) is Curve)
                        {
                            var curve = trims.SegmentCurve(j) as Curve;
                            var ti = brep.AddTrimCurve(curve);

                            if (brepEdges.ElementAt(j).Value == 0)
                                continue;

                            var brepTrim = brep.Trims.Add(brepEdges.ElementAt(j).Key, brepEdges.ElementAt(j).Value < 0, brepLoop, ti);

                            brepTrim.TrimType = BrepTrimType.Boundary;
                        }
                    }

                    // brepLoop.Trims.MatchEnds();

                    // brep.Trims.MatchEnds(brepLoop.Trims[0], brepLoop.Trims.Last());

                    // brep.Repair(Globals.DistanceTolerance);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("无法添加 NX 面：" + ex);
            }

            brep.Trims.MatchEnds();

            brep.Repair(Globals.DistanceTolerance);
        }

        public static Plane ToRhino(NXOpen.Plane plane)
        {
            return new Plane(plane.Origin.ToRhino(),
                ToRhino(plane.Matrix.GetAxisX()),
                ToRhino(plane.Matrix.GetAxisY()));
        }

        public static Surface ToRhinoSurface(NXOpen.Face face, out bool parametricOrientation, double relativeTolerance = 0.0)
        {
            FaceEx.FaceData faceData = face.GetData();
            parametricOrientation = faceData.NormalReversed;

            switch (faceData.FaceType)
            {
                case NXOpen.Face.FaceType.Planar:
                    return ToRhinoPlaneSurface(face, relativeTolerance);
                case NXOpen.Face.FaceType.Conical:
                    return ToRhinoConicalSurface(face, faceData, relativeTolerance);
                case NXOpen.Face.FaceType.Cylindrical:
                    return ToRhinoCylindricalSurface(face, faceData);
                case NXOpen.Face.FaceType.SurfaceOfRevolution:
                    return ToRhinoRevSurface(face, faceData, relativeTolerance);
                case NXOpen.Face.FaceType.Spherical:
                    return ToRhinoSphereSurface(face);
                default: return FromBsurf(face);
            }
        }

        /// <summary>
        /// 创建未修剪的 Rhino PlaneSurface
        /// </summary>
        /// <param name="face"></param>
        /// <param name="relativeTolerance"></param>
        /// <returns></returns>
        static PlaneSurface ToRhinoPlaneSurface(NXOpen.Face face, double relativeTolerance)
        {
            double[] facePt = new double[3];
            double[] direction = new double[3];
            double[] box = new double[6];
            TheUfSession.Modl.AskFaceData(face.Tag, out int type, facePt, direction, box, out double radius, out double radData, out int normDir);

            Plane plane = new Plane(new Point3d(facePt[0], facePt[1], facePt[2]), new Vector3d(direction[0], direction[1], direction[2]));

            return PlaneSurface.CreateThroughBox(plane, new BoundingBox(new Point3d(box[0], box[1], box[2]), new Point3d(box[3], box[4], box[5])));
        }

        static RevSurface ToRhinoConicalSurface(NXOpen.Face face, FaceEx.FaceData faceData, double relativeTolerance)
        {
            double totalHeight;           // 圆锥总高度
            NXOpen.Point3d top;           // 圆锥顶点

            double radius;                // 锥底半径
            var faceBoundingBox = face.GetAlignedBoundingBox(faceData.Direction);

            if (face.GetEdges().Length == 1 && face.GetEdges()[0].SolidEdgeType == NXOpen.Edge.EdgeType.Circular)
            {
                // 无修剪的锥形
                TheUfSession.Eval.Initialize2(face.GetEdges()[0].Tag, out var edgeEvaluator);
                TheUfSession.Eval.AskArc(edgeEvaluator, out var arc);
                TheUfSession.Eval.Free(edgeEvaluator);

                NXOpen.Point3d bottom = arc.center.ToPoint3d();        // 锥底中心点

                totalHeight = faceBoundingBox.Height;

                top = bottom.Move(faceBoundingBox.HeightDirection, totalHeight);

                radius = arc.radius;
            }
            else
            {
                var tangentOfHalfAngle = Math.Tan(faceData.RadiusData);

                top = faceData.Point.Move(faceBoundingBox.HeightDirection, faceData.Radius / tangentOfHalfAngle);

                totalHeight = faceBoundingBox.Height + faceData.Radius / Math.Tan(faceData.RadiusData);

                radius = faceData.Radius + faceBoundingBox.Height * tangentOfHalfAngle;
            }

            var cone = new Cone(new Plane(top.ToRhino(), faceBoundingBox.HeightDirection.Reverse().ToRhino()), totalHeight, radius);

            return cone.ToRevSurface();

            // var curve = new LineCurve(outerPointOnCircle.ToRhino(), top.ToRhino());
            // return RevSurface.Create(curve, new Line(bottom.ToRhino(), top.ToRhino()), 0.0, Math.PI * 2.0);
        }

        static RevSurface ToRhinoRevSurface(NXOpen.Face face, FaceEx.FaceData faceData, double relativeTolerance)
        {
            var bodyFeatures = face.GetBody().GetFeatures();

            var revolveFeature = bodyFeatures.FirstOrDefault(obj => obj is NXOpen.Features.Revolve);

            Curve faceSectionCurve = default;
            double startRadian = 0.0;
            double endRadian = Math.PI * 2;
            if (revolveFeature != null)
            {
                NXOpen.Features.RevolveBuilder revolveBuilder = WorkPart.Features.CreateRevolveBuilder(revolveFeature);
                revolveBuilder.Section.GetOutputCurves(out var sectionCurves);

                startRadian = revolveBuilder.Limits.StartExtend.Value.Value * Math.PI / 180.0;

                endRadian = revolveBuilder.Limits.EndExtend.Value.Value * Math.PI / 180.0;

                revolveBuilder.Destroy();

                for (int i = 0; i < sectionCurves.Length; i++)
                {
                    var baseCurve = sectionCurves[i] as NXOpen.IBaseCurve;

                    var curveMidPt = baseCurve.GetPoint(0.5);
                    if (curveMidPt.DistanceTo(face.Tag).Distance < DistanceTolerance)
                    {
                        faceSectionCurve = baseCurve.ToRhinoCurve();
                        break;
                    }
                }
            }
            else
            {
                var faceBoundingBox = face.GetAlignedBoundingBox(faceData.Direction);

                var point1 = faceData.Point.Move(faceData.Direction, faceBoundingBox.Height * 1.5);

                var point2 = point1.Move(faceBoundingBox.LengthDirection, faceBoundingBox.Length);

                var point3 = point2.Move(faceData.Direction.Reverse(), faceBoundingBox.Height * 3);

                var point4 = point3.Move(faceBoundingBox.LengthDirection.Reverse(), faceBoundingBox.Length);

                var fourPointSurface = WorkPart.Features.CreateFourPointSurface(point1, point2, point3, point4);

                var intersectionCurveFeature = WorkPart.Features.CreateIntersectionCurve(fourPointSurface.GetFaces(), face);

                faceSectionCurve = (intersectionCurveFeature.GetEntities()[0] as NXOpen.Curve).ToRhino();

                intersectionCurveFeature.Delete();
                fourPointSurface.Delete();
            }

            return RevSurface.Create(faceSectionCurve, new Line(faceData.Point.ToRhino(), faceData.Point.Move(faceData.Direction, 10.0).ToRhino()), startRadian, endRadian);
        }

        static RevSurface ToRhinoSphereSurface(NXOpen.Face face)
        {
            var faceData = face.GetData();

            return RevSurface.CreateFromSphere(new Sphere(faceData.Point.ToRhino(), faceData.Radius));
        }

        static NurbsSurface ToRhinoCylindricalSurface(NXOpen.Face face, FaceEx.FaceData faceData)
        {
            NXOpen.Point3d top;           // 圆柱顶点

            double radius;                // 圆柱半径

            NXOpen.Point3d bottom;          // 圆柱底面中心点

            var faceBoundingBox = face.GetAlignedBoundingBox(faceData.Direction);

            double totalHeight = faceBoundingBox.Height;          // 圆柱总高度 

            if (face.GetEdges().Length == 2 && face.GetEdges().All(obj => obj.SolidEdgeType == NXOpen.Edge.EdgeType.Circular))
            {
                // 未修剪的圆柱
                TheUfSession.Eval.Initialize2(face.GetEdges()[0].Tag, out var edgeEvaluator);
                TheUfSession.Eval.AskArc(edgeEvaluator, out var arc1);
                TheUfSession.Eval.Free(edgeEvaluator);

                TheUfSession.Eval.Initialize2(face.GetEdges()[1].Tag, out edgeEvaluator);
                TheUfSession.Eval.AskArc(edgeEvaluator, out var arc2);
                TheUfSession.Eval.Free(edgeEvaluator);

                if (arc2.center.ToPoint3d().Subtract(arc1.center.ToPoint3d()).GetUnitVector().IsEqual(faceData.Direction.GetUnitVector()))
                {
                    bottom = arc1.center.ToPoint3d();
                }
                else
                {
                    bottom = arc2.center.ToPoint3d();
                }

                top = bottom.Move(faceBoundingBox.HeightDirection, totalHeight);

                radius = arc1.radius;
            }
            else
            {
                var topExtremePt = WorkPart.MeasureManager.MeasureRectangularExtreme(face, faceBoundingBox.HeightDirection, faceBoundingBox.LengthDirection, faceBoundingBox.WidthDirection);

                var distToFacePoint = topExtremePt.DistanceTo(faceData.Point, faceData.Direction);

                top = faceData.Point.Move(faceBoundingBox.HeightDirection, distToFacePoint);

                bottom = top.Move(faceBoundingBox.HeightDirection.Reverse(), totalHeight);

                radius = faceData.Radius;
            }

            Circle baseCircle = new Circle(new Plane(bottom.ToRhino(), faceBoundingBox.HeightDirection.ToRhino()), radius);

            var cyl = new Rhino.Geometry.Cylinder(baseCircle, totalHeight);

            return cyl.ToNurbsSurface();
        }

        public static NurbsSurface FromBsurf(NXOpen.Face face)
        {
            TheUfSession.Modl.AskBsurf(face.Tag, out NXOpen.UF.UFModl.Bsurface surfaceData);

            var degreeU = surfaceData.order_u - 1;
            var degreeV = surfaceData.order_v - 1;

            var knotsU = surfaceData.knots_u;
            var knotsV = surfaceData.knots_v;

            //int controlPointCountU = knotsU.Length - degreeU - 1;
            //int controlPointCountV = knotsV.Length - degreeV - 1;

            var nurbsSurface = NurbsSurface.Create(3, surfaceData.is_rational == 1, degreeU + 1, degreeV + 1, surfaceData.num_poles_u, surfaceData.num_poles_v);

            Point3d[,] controlPoints = new Point3d[surfaceData.num_poles_u, surfaceData.num_poles_v];
            double[,] weights = new double[surfaceData.num_poles_u, surfaceData.num_poles_v];
            for (int i = 0; i < surfaceData.num_poles_v; i++)
            {
                for (int j = 0; j < surfaceData.num_poles_u; j++)
                {
                    int offset = j + i * surfaceData.num_poles_u;

                    controlPoints[j, i] = new Point3d(surfaceData.poles[offset, 0], surfaceData.poles[offset, 1], surfaceData.poles[offset, 2]);

                    weights[j, i] = surfaceData.poles[offset, 3];
                }
            }

            var points = nurbsSurface.Points;
            for (int v = 0; v < surfaceData.num_poles_v; v++)
            {
                for (int u = 0; u < surfaceData.num_poles_u; u++)
                {
                    var pt = controlPoints[u, v];
                    if (surfaceData.is_rational == 1)
                    {
                        double w = weights[u, v];
                        points.SetPoint(u, v, pt.X * w, pt.Y * w, pt.Z * w, w);
                    }
                    else
                    {
                        points.SetPoint(u, v, pt.X, pt.Y, pt.Z);
                    }
                }
            }

            {
                var knots = nurbsSurface.KnotsU;
                int index = 0;
                foreach (var w in knotsU.Skip(1).Take(knots.Count))
                    knots[index++] = w;
            }

            {
                var knots = nurbsSurface.KnotsV;
                int index = 0;
                foreach (var w in knotsV.Skip(1).Take(knots.Count))
                    knots[index++] = w;
            }

            return nurbsSurface;
        }
    }
}
