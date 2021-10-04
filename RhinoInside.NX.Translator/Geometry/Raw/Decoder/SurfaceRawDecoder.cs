using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using NXOpen.Extensions;
using static NXOpen.Extensions.Globals;

namespace RhinoInside.NX.Translator.Geometry.Raw
{
    public static partial class RawDecoder
    {
        public static Surface ToRhinoSurface(NXOpen.Face face, out bool parametricOrientation, double relativeTolerance = 0.0)
        {
            var faceData = face.GetData();
            parametricOrientation = faceData.NormalReversed;

            switch (faceData.FaceType)
            {
                case NXOpen.Face.FaceType.Planar:
                    return ToRhinoPlaneSurface(face, relativeTolerance);
                case NXOpen.Face.FaceType.Conical:
                    return ToRhinoRevSurface(face, relativeTolerance);
                case NXOpen.Face.FaceType.Cylindrical:
                    return FromCylindricalSurface(face, relativeTolerance);
                case NXOpen.Face.FaceType.Spherical:
                    return FromSphereSurface(face, relativeTolerance);
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
            //throw new NotImplementedException("曲面转换尚未完成");

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
    }
}
