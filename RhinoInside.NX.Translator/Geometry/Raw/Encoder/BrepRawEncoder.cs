using Rhino.Geometry;
using Rhino.Geometry.Collections;
using RhinoInside.NX.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RhinoInside.NX.Extensions.Globals;

namespace RhinoInside.NX.Translator.Geometry.Raw
{
    internal static partial class RawEncoder
    {
        //public static IEnumerable<NXOpen.BRepBuilderEdgeGeometry> ToHost(BrepEdge edge)
        //{
        //    var edgeCurve = edge.EdgeCurve.Trim(edge.Domain);

        //    if (edge.ProxyCurveIsReversed)
        //        edgeCurve.Reverse();

        //    switch (edgeCurve)
        //    {
        //        case LineCurve line:
        //            yield return NXOpen.BRepBuilderEdgeGeometry.Create(ToHost(line));
        //            yield break;
        //        case ArcCurve arc:
        //            yield return NXOpen.BRepBuilderEdgeGeometry.Create(ToHost(arc));
        //            yield break;
        //        case NurbsCurve nurbs:
        //            yield return NXOpen.BRepBuilderEdgeGeometry.Create(ToHost(nurbs));
        //            yield break;
        //        default:
        //            Debug.Fail($"{edgeCurve} is not supported as a Solid Edge");
        //            yield break;
        //    }
        //}

        public static double[] ToHost(NurbsSurfaceKnotList list)
        {
            //list.CreateUniformKnots(1.0);

            var count = list.Count;
            var knots = new double[count + 2];

            var minValue = list.First();
            var maxValue = list.Last();

            var range = maxValue - minValue;

            int j = 0, k = 0;
            while (j < count)
                knots[++k] = (list[j++] - minValue) / range;

            knots[0] = knots[1];
            knots[count + 1] = knots[count];

            return knots;
        }

        public static NXOpen.Point3d[] ToHost(NurbsSurfacePointList list)
        {
            var count = list.CountU * list.CountV;
            var points = new NXOpen.Point3d[count];

            int p = 0;
            foreach (var point in list)
            {
                var location = point.Location;
                points[p++] = new NXOpen.Point3d(location.X, location.Y, location.Z);
            }

            return points;
        }

        /// <summary>
        /// 将面转换为未修剪的曲面
        /// </summary>
        /// <param name="nurbsSurface"></param>
        /// <returns></returns>
        public static unsafe NXOpen.Body ToHost(NurbsSurface nurbsSurface)
        {
            // TODO: Implement conversion from other Rhino surface types like PlaneSurface, RevSurface and SumSurface.
            try
            {
                NXOpen.Body bSurfaceBody = null;
                var domainU = nurbsSurface.Domain(0);
                var domainV = nurbsSurface.Domain(1);

                var knotsU = ToHost(nurbsSurface.KnotsU);
                var knotsV = ToHost(nurbsSurface.KnotsV);
                var controlPoints = ToHost(nurbsSurface.Points);
                var bboxUV = new BoundingBox2D(domainU.Min, domainV.Min, domainU.Max, domainV.Max);

#if DEBUG
                Console.WriteLine($"U 方向封闭性：{nurbsSurface.IsClosed(0)}");
                Console.WriteLine($"V 方向封闭性：{nurbsSurface.IsClosed(1)}");
                Console.WriteLine($"U 方向阶次：{nurbsSurface.Degree(0)}");
                Console.WriteLine($"V 方向阶次：{nurbsSurface.Degree(1)}");
                Console.WriteLine($"U 方向结点数：{knotsU.Length}");
                Console.WriteLine($"V 方向结点数：{knotsV.Length}");
                Console.WriteLine($"U 方向控制点个数：{nurbsSurface.Points.CountU}");
                Console.WriteLine($"V 方向控制点个数：{nurbsSurface.Points.CountV}");
#endif
                #region 使用修剪曲面方式创建，报错，无法创建曲面
                //double[] poles = new double[nurbsSurface.Points.CountU * nurbsSurface.Points.CountV * 4];

                //for (int i = 0; i < nurbsSurface.Points.CountV; i++)
                //{
                //    for (int j = 0; j < nurbsSurface.Points.CountU; j++)
                //    {
                //        int currentIndex = (nurbsSurface.Points.CountU * i + j);
                //        var currentControlPoint = nurbsSurface.Points.GetControlPoint(j, i);
                //        var pt = WorkPart.Points.CreatePoint(currentControlPoint.Location.ToXYZ());
                //        pt.SetVisibility(NXOpen.SmartObject.VisibilityOption.Visible);
                //        pt.SetUserAttribute("UV", -1, $"{i},{j}", NXOpen.Update.Option.Now);
                //        poles[currentIndex * 4] = currentControlPoint.X;
                //        poles[currentIndex * 4 + 1] = currentControlPoint.Y;
                //        poles[currentIndex * 4 + 2] = currentControlPoint.Z;
                //        poles[currentIndex * 4 + 3] = currentControlPoint.Weight;
                //    }
                //}

                //int[] edge_counts = new int[face.Loops.Count];
                //List<int> edge_senses = new List<int>();
                //List<Tag> edge_curves = new List<Tag>();
                //for (int i = 0; i < face.Loops.Count; i++)
                //{
                //    var trims = face.Loops[i].Trims;

                //    List<Tag> currentTrimEdgeCurves = new List<Tag>();
                //    List<int> currentTrimEdgeSenses = new List<int>();
                //    int ss = 0;
                //    for (int j = 0; j < trims.Count; j++)
                //    {
                //        if (trims[j].TrimType != BrepTrimType.Boundary && trims[j].TrimType != BrepTrimType.Mated)
                //            continue;

                //        BrepEdge edge = trims[j].Edge;

                //        ss += trims[j].ProxyCurveIsReversed ? 1 : 0;

                //        if (edge is null)
                //            continue;

                //        var currentEdgeCurve = edge.EdgeCurve.ToCurve();

                //        currentEdgeCurve.SetUserAttribute("Index", -1, j, Update.Option.Now);

                //        currentTrimEdgeCurves.Add(currentEdgeCurve.Tag);

                //        currentTrimEdgeSenses.Add(trims[j].IsReversed() ? -1 : 1);
                //    }

                //    edge_counts[i] = currentTrimEdgeCurves.Count;
                //    edge_senses.AddRange(currentTrimEdgeSenses);
                //    edge_curves.AddRange(currentTrimEdgeCurves);

                //}

                //face.Loops.Count.ToString().ConsoleWriteLine();

                //Tag bSurfaceTag = Tag.Null;
                //try
                //{
                //    theUfSession.Modl.CreTrimBsurf(
                //        nurbsSurface.Points.CountU,
                //        nurbsSurface.Points.CountV,
                //        nurbsSurface.OrderU,
                //        nurbsSurface.OrderV,
                //        knotsU,
                //        knotsV,
                //        poles,
                //        face.Loops.Count,
                //        edge_counts,
                //        edge_senses.ToArray(),
                //        edge_curves.ToArray(),
                //        0,
                //        DistanceTolerance,
                //        out bSurfaceTag,
                //        out int knotFixup,
                //        out int poleFixup
                //        );
                //}
                //catch (Exception ex)
                //{
                //    ex.ToString().ConsoleWriteLine();
                //}
                #endregion

                #region 使用拟合方式创建，误差较大
                //NXOpen.UF.UFModl.BsurfRowInfo[] bSurfRowInfo = new NXOpen.UF.UFModl.BsurfRowInfo[nurbsSurface.Points.CountV];
                //for (int i = 0; i < bSurfRowInfo.Length; i++)
                //{
                //    double[] currentRowPoint = new double[nurbsSurface.Points.CountU * 3];
                //    double[] weights = new double[nurbsSurface.Points.CountU];
                //    for (int j = 0; j < nurbsSurface.Points.CountU; j++)
                //    {
                //        var currentControlPoint = nurbsSurface.Points.GetControlPoint(j, i);

                //        var pt = WorkPart.Points.CreatePoint(currentControlPoint.Location.ToXYZ());
                //        pt.SetVisibility(NXOpen.SmartObject.VisibilityOption.Visible);
                //        pt.SetUserAttribute("UV", -1, $"{j},{i}", NXOpen.Update.Option.Now);

                //        currentRowPoint[j * 3] = currentControlPoint.Location.X;
                //        currentRowPoint[j * 3 + 1] = currentControlPoint.Location.Y;
                //        currentRowPoint[j * 3 + 2] = currentControlPoint.Location.Z;
                //        weights[j] = currentControlPoint.Weight;
                //    }

                //    bSurfRowInfo[i] = new NXOpen.UF.UFModl.BsurfRowInfo
                //    {
                //        num_points = nurbsSurface.Points.CountU,
                //        points = currentRowPoint,
                //        weight = weights,
                //    };
                //}

                //theUfSession.Modl.CreateBsurfThruPts(1,
                //    nurbsSurface.IsClosed(0) ? 1 : 0,
                //    nurbsSurface.IsClosed(1) ? 1 : 0,
                //    nurbsSurface.Degree(0),
                //    nurbsSurface.Degree(1),
                //    nurbsSurface.Points.CountV,
                // bSurfRowInfo,
                // out NXOpen.Tag bSurfaceTag);
                #endregion

                #region CreateBsurf
                double[] poles = new double[nurbsSurface.Points.CountU * nurbsSurface.Points.CountV * 4];

#if DEBUG
                List<NXOpen.Point> polePoints = new List<NXOpen.Point>();
#endif
                for (int i = 0; i < nurbsSurface.Points.CountV; i++)
                {
                    for (int j = 0; j < nurbsSurface.Points.CountU; j++)
                    {
                        int currentIndex = (nurbsSurface.Points.CountU * i + j);
                        var currentControlPoint = nurbsSurface.Points.GetControlPoint(j, i);

                        poles[currentIndex * 4] = currentControlPoint.X;
                        poles[currentIndex * 4 + 1] = currentControlPoint.Y;
                        poles[currentIndex * 4 + 2] = currentControlPoint.Z;
                        poles[currentIndex * 4 + 3] = currentControlPoint.Weight;
#if DEBUG
                        var pt = WorkPart.Points.CreatePoint(currentControlPoint.Location.ToXYZ());
                        pt.SetVisibility(NXOpen.SmartObject.VisibilityOption.Visible);
                        pt.SetUserAttribute("UV", -1, $"{j},{i}", NXOpen.Update.Option.Now);
                        polePoints.Add(pt);
#endif
                    }
                }

                TheUfSession.Modl.CreateBsurf(
                    nurbsSurface.Points.CountU,
                    nurbsSurface.Points.CountV,
                    nurbsSurface.OrderU,
                    nurbsSurface.OrderV,
                    knotsU,
                    knotsV,
                    poles,
                    out NXOpen.Tag bSurfaceTag,
                    out int knotFixup,
                    out int poleFixup);
                #endregion

                #region CreateBsurface 报错，指定结构必须能直接复制到本机结构中
                //double[,] vertices = new double[nurbsSurface.Points.CountU * nurbsSurface.Points.CountV, 4];

                //for (int i = 0; i < nurbsSurface.Points.CountV; i++)
                //{
                //    for (int j = 0; j < nurbsSurface.Points.CountU; j++)
                //    {
                //        var currentIndex = (nurbsSurface.Points.CountU * i + j);
                //        var currentControlPoint = nurbsSurface.Points.GetControlPoint(j, i);

                //        vertices[currentIndex, 0] = currentControlPoint.X;
                //        vertices[currentIndex, 1] = currentControlPoint.Y;
                //        vertices[currentIndex, 2] = currentControlPoint.Z;
                //        vertices[currentIndex, 3] = currentControlPoint.Weight;
                //    }
                //}

                //NXOpen.UF.UFModl.Bsurface surfaceInfo = new NXOpen.UF.UFModl.Bsurface()
                //{
                //    is_rational = nurbsSurface.IsRational ? 1 : 0,
                //    order_u = nurbsSurface.OrderU,
                //    order_v = nurbsSurface.OrderV,
                //    knots_u = knotsU,
                //    knots_v = knotsV,
                //    num_poles_u = nurbsSurface.Points.CountU,
                //    num_poles_v = nurbsSurface.Points.CountV,
                //    poles = vertices,
                //};

                //double[,] poles = new double[nurbsSurface.Points.Count(), 4];

                //for (int i = 0; i < nurbsSurface.Points.CountU; i++)
                //{
                //    for (int j = 0; j < nurbsSurface.Points.CountV; j++)
                //    {
                //        var currentPoint3d = nurbsSurface.Points.GetControlPoint(i, j);
                //        poles[nurbsSurface.Points.CountV * i + j, 0] = currentPoint3d.X;
                //        poles[nurbsSurface.Points.CountV * i + j, 1] = currentPoint3d.Y;
                //        poles[nurbsSurface.Points.CountV * i + j, 2] = currentPoint3d.Z;
                //        poles[nurbsSurface.Points.CountV * i + j, 3] = nurbsSurface.IsRational ? currentPoint3d.Weight : 1.0;
                //    }
                //}

                //theUfSession.Modl.CreateBsurface(
                //ref surfaceInfo,
                //out NXOpen.Tag bSurfaceTag,
                //out int numStates,
                //out NXOpen.UF.UFCurve.State[] states);
                #endregion

                #region Snap

                //                Snap.Position[,] poles = new Snap.Position[nurbsSurface.Points.CountU, nurbsSurface.Points.CountV];

                //#if DEBUG
                //                List<NXOpen.Point> polePoints = new List<NXOpen.Point>();
                //#endif
                //                for (int i = 0; i < nurbsSurface.Points.CountU; i++)
                //                {
                //                    for (int j = 0; j < nurbsSurface.Points.CountV; j++)
                //                    {
                //                        var currentControlPoint = nurbsSurface.Points.GetControlPoint(i, j);
                //#if DEBUG
                //                        var pt = WorkPart.Points.CreatePoint(currentControlPoint.Location.ToXYZ());
                //                        pt.SetVisibility(NXOpen.SmartObject.VisibilityOption.Visible);
                //                        pt.SetUserAttribute("UV", -1, $"{i},{j}", NXOpen.Update.Option.Now);
                //                        polePoints.Add(pt);
                //#endif
                //                        poles[i, j] = new Snap.Position(currentControlPoint.Location.ToXYZ());
                //                    }
                //                }

                //                var snapBSurface = Snap.Create.Bsurface(poles, knotsU, knotsV);

                //                bSurfaceBody = snapBSurface.NXOpenBody;
                #endregion

                bSurfaceBody = bSurfaceTag.GetTaggedObject() as NXOpen.Body;
#if DEBUG
                polePoints.ToArray().Delete();
#endif
                return bSurfaceBody;
            }
            catch (Exception ex)
            {
                ex.ToString().ConsoleWriteLine();
                return null;
            }
        }

        //public static NXOpen.Solid ToHost(Brep brep)
        //{
        //    var brepType = NXOpen.BRepType.OpenShell;
        //    switch (brep.SolidOrientation)
        //    {
        //        case BrepSolidOrientation.Inward: brepType = NXOpen.BRepType.Void; break;
        //        case BrepSolidOrientation.Outward: brepType = NXOpen.BRepType.Solid; break;
        //    }

        //    using (var builder = new NXOpen.BRepBuilder(brepType))
        //    {
        //        var brepEdges = new List<NXOpen.BRepBuilderGeometryId>[brep.Edges.Count];
        //        foreach (var face in brep.Faces)
        //        {
        //            var faceId = builder.AddFace(ToHost(face), face.OrientationIsReversed);
        //            builder.SetFaceMaterialId(faceId, GeometryEncoder.Context.Peek.MaterialId);

        //            foreach (var loop in face.Loops)
        //            {
        //                var loopId = builder.AddLoop(faceId);

        //                IEnumerable<BrepTrim> trims = loop.Trims;
        //                if (face.OrientationIsReversed)
        //                    trims = trims.Reverse();

        //                foreach (var trim in trims)
        //                {
        //                    if (trim.TrimType != BrepTrimType.Boundary && trim.TrimType != BrepTrimType.Mated)
        //                        continue;

        //                    var edge = trim.Edge;
        //                    if (edge is null)
        //                        continue;

        //                    var edgeIds = brepEdges[edge.EdgeIndex];
        //                    if (edgeIds is null)
        //                    {
        //                        edgeIds = brepEdges[edge.EdgeIndex] = new List<NXOpen.BRepBuilderGeometryId>();
        //                        edgeIds.AddRange(ToHost(edge).Select(e => builder.AddEdge(e)));
        //                    }

        //                    bool trimReversed = face.OrientationIsReversed ?
        //                                        !trim.IsReversed() :
        //                                         trim.IsReversed();

        //                    if (trimReversed)
        //                    {
        //                        for (int e = edgeIds.Count - 1; e >= 0; --e)
        //                            builder.AddCoEdge(loopId, edgeIds[e], true);
        //                    }
        //                    else
        //                    {
        //                        for (int e = 0; e < edgeIds.Count; ++e)
        //                            builder.AddCoEdge(loopId, edgeIds[e], false);
        //                    }
        //                }

        //                builder.FinishLoop(loopId);
        //            }

        //            builder.FinishFace(faceId);
        //        }

        //        var brepBuilderOutcome = builder.Finish();
        //        if (builder.IsResultAvailable())
        //            return builder.GetResult();
        //    }

        //    return null;
        //}
    }
}
