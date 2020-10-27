using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rhino.Geometry;
using RhinoInside.NX.Extensions.NX;
using static NXOpen.Extensions.Globals;
using DB = RhinoInside.NX.Extensions;
using RhinoInside.NX.Extensions;
using RhinoInside.NX.Extensions.RhinoEx;
using NXOpen.Extensions;

namespace RhinoInside.NX.Convert
{
    /// <summary>
    /// Methods in this class do a full geometry conversion.
    /// <para>It converts geometry from Active Rhino model units to Revit internal units.</para>
    /// <para>For direct conversion methods see <see cref="Raw.RawEncoder"/> class.</para>
    /// </summary>
    public static class PointEncoder
    {
        #region Context
        public sealed class Context : State<Context>
        {
            public NXOpen.Tag MaterialId = NXOpen.Tag.Null;
            public NXOpen.Tag GraphicsStyleId = NXOpen.Tag.Null;
        }
        #endregion

        #region Geometry values
        public static UV ToUV(this Point2f value)
        {
            double factor = UnitConverter.RhinoToNXUnitsRatio;
            return new UV(value.X * factor, value.Y * factor);
        }
        public static UV ToUV(this Point2f value, double factor)
        {
            return factor == 1.0 ?
              new UV(value.X, value.Y) :
              new UV(value.X * factor, value.Y * factor);
        }

        public static UV ToUV(this Point2d value)
        {
            double factor = UnitConverter.RhinoToNXUnitsRatio;
            return new UV(value.X * factor, value.Y * factor);
        }
        public static UV ToUV(this Point2d value, double factor)
        {
            return factor == 1.0 ?
              new UV(value.X, value.Y) :
              new UV(value.X * factor, value.Y * factor);
        }

        public static UV ToUV(this Vector2f value)
        {
            return new UV(value.X, value.Y);
        }
        public static UV ToUV(this Vector2f value, double factor)
        {
            return factor == 1.0 ?
              new UV(value.X, value.Y) :
              new UV(value.X * factor, value.Y * factor);
        }

        public static UV ToUV(this Vector2d value)
        {
            return new UV(value.X, value.Y);
        }
        public static UV ToUV(this Vector2d value, double factor)
        {
            return factor == 1.0 ?
              new UV(value.X, value.Y) :
              new UV(value.X * factor, value.Y * factor);
        }

        public static NXOpen.Point3d ToXYZ(this Point3f value)
        {
            double factor = UnitConverter.RhinoToNXUnitsRatio;
            return new NXOpen.Point3d(value.X * factor, value.Y * factor, value.Z * factor);
        }
        public static NXOpen.Point3d ToXYZ(this Point3f value, double factor)
        {
            return factor == 1.0 ?
              new NXOpen.Point3d(value.X, value.Y, value.Z) :
              new NXOpen.Point3d(value.X * factor, value.Y * factor, value.Z * factor);
        }

        public static NXOpen.Point3d ToXYZ(this Point3d value)
        {
            double factor = UnitConverter.RhinoToNXUnitsRatio;
            return new NXOpen.Point3d(value.X * factor, value.Y * factor, value.Z * factor);
        }
        public static NXOpen.Point3d ToXYZ(this Point3d value, double factor)
        {
            return factor == 1.0 ?
              new NXOpen.Point3d(value.X, value.Y, value.Z) :
              new NXOpen.Point3d(value.X * factor, value.Y * factor, value.Z * factor);
        }

        public static NXOpen.Vector3d ToXYZ(this Vector3f value)
        {
            return new NXOpen.Vector3d(value.X, value.Y, value.Z);
        }
        public static NXOpen.Vector3d ToXYZ(this Vector3f value, double factor)
        {
            return factor == 1.0 ?
              new NXOpen.Vector3d(value.X, value.Y, value.Z) :
              new NXOpen.Vector3d(value.X * factor, value.Y * factor, value.Z * factor);
        }

        public static NXOpen.Vector3d ToXYZ(this Vector3d value)
        {
            return new NXOpen.Vector3d(value.X, value.Y, value.Z);
        }
        public static NXOpen.Vector3d ToXYZ(this Vector3d value, double factor)
        {
            return factor == 1.0 ?
              new NXOpen.Vector3d(value.X, value.Y, value.Z) :
              new NXOpen.Vector3d(value.X * factor, value.Y * factor, value.Z * factor);
        }

        public static NXOpen.Plane ToPlane(this Plane value) => ToPlane(value, UnitConverter.RhinoToNXUnitsRatio);
        public static NXOpen.Plane ToPlane(this Plane value, double factor)
        {
            return WorkPart.Planes.CreatePlane(value.Origin.ToXYZ(factor), value.ZAxis.ToXYZ(), NXOpen.SmartObject.UpdateOption.WithinModeling);
        }

        public static NXOpen.Matrix4x4 ToTransform(this Rhino.Geometry.Transform value) => ToTransform(value, UnitConverter.RhinoToNXUnitsRatio);
        public static NXOpen.Matrix4x4 ToTransform(this Rhino.Geometry.Transform value, double factor)
        {
            Debug.Assert(value.IsAffine);

            var result = factor == 1.0 ?
              Matrix4x4Ex.CreateTranslation(new NXOpen.Vector3d(value.M03, value.M13, value.M23)) :
              Matrix4x4Ex.CreateTranslation(new NXOpen.Vector3d(value.M03 * factor, value.M13 * factor, value.M23 * factor));

            result.Rxx = value.M00;
            result.Rxy = value.M10;
            result.Rxz = value.M20;

            result.Ryx = value.M01;
            result.Ryy = value.M11;
            result.Ryz = value.M21;

            result.Rzx = value.M02;
            result.Rzy = value.M12;
            result.Rzz = value.M22;

            return result;
        }

        //public static BoundingBox3D ToBoundingBoxXYZ(this BoundingBox value) => ToBoundingBoxXYZ(value, UnitConverter.ToHostUnits());
        //public static BoundingBox3D ToBoundingBoxXYZ(this BoundingBox value, double factor)
        //{
        //    return new BoundingBox3D
        //    {
        //        Min = value.Min.ToXYZ(factor),
        //        Max = value.Min.ToXYZ(factor),
        //        Enabled = value.IsValid
        //    };
        //}

        //public static BoundingBox3D ToBoundingBoxXYZ(this Box value) => ToBoundingBoxXYZ(value, UnitConverter.ToHostUnits());
        //public static BoundingBox3D ToBoundingBoxXYZ(this Box value, double factor)
        //{
        //    return new BoundingBox3D
        //    {
        //        Transform = Transform.PlaneToPlane(Plane.WorldXY, value.Plane).ToTransform(factor),
        //        Min = new NXOpen.Point3d(value.X.Min * factor, value.Y.Min * factor, value.Z.Min * factor),
        //        Max = new NXOpen.Point3d(value.X.Max * factor, value.Y.Max * factor, value.Z.Max * factor),
        //        Enabled = value.IsValid
        //    };
        //}

        public static Outline ToOutline(this BoundingBox value) => ToOutline(value, UnitConverter.RhinoToNXUnitsRatio);
        public static Outline ToOutline(this BoundingBox value, double factor)
        {
            return new Outline(value.Min.ToXYZ(), value.Max.ToXYZ());
        }
        #endregion

        #region GeometryBase
        public static NXOpen.Point ToPoint(this Point value) => value.ToPoint(UnitConverter.RhinoToNXUnitsRatio);
        public static NXOpen.Point ToPoint(this Point value, double factor) => WorkPart.Points.CreatePoint(value.Location.ToXYZ(factor));

        public static NXOpen.Point[] ToPoints(this PointCloud value) => value.ToPoints(UnitConverter.RhinoToNXUnitsRatio);
        public static NXOpen.Point[] ToPoints(this PointCloud value, double factor)
        {
            var array = new NXOpen.Point[value.Count];
            int index = 0;
            if (factor == 1.0)
            {
                foreach (var point in value)
                {
                    var location = point.Location;
                    array[index++] = WorkPart.Points.CreatePoint(new NXOpen.Point3d(location.X, location.Y, location.Z));
                }
            }
            else
            {
                foreach (var point in value)
                {
                    var location = point.Location;
                    array[index++] = WorkPart.Points.CreatePoint(new NXOpen.Point3d(location.X * factor, location.Y * factor, location.Z * factor));
                }
            }

            return array;
        }

        //public static DB.CurveLoop ToCurveLoop(this Curve value)
        //{
        //    value = value.InOtherUnits(UnitConverter.ToHostUnits());
        //    value.RemoveShortSegments(Revit.ShortCurveTolerance);

        //    return DB.CurveLoop.Create(value.ToCurveMany(UnitConverter.NoScale).SelectMany(x => x.ToBoundedCurves()).ToList());
        //}

        //public static DB.CurveArray ToCurveArray(this Curve value)
        //{
        //    value = value.InOtherUnits(UnitConverter.ToHostUnits());
        //    value.RemoveShortSegments(Revit.ShortCurveTolerance);

        //    return value.ToCurveMany(UnitConverter.NoScale).SelectMany(x => x.ToBoundedCurves()).ToCurveArray();
        //}

        //public static DB.CurveArrArray ToCurveArrayArray(this IList<Curve> value)
        //{
        //    var curveArrayArray = new DB.CurveArrArray();
        //    foreach (var curve in value)
        //        curveArrayArray.Append(curve.ToCurveArray());

        //    return curveArrayArray;
        //}

        //public static NXOpen.Body ToSolid(this Brep value) => BrepEncoder.ToSolid(BrepEncoder.ToRawBrep(value, UnitConverter.RhinoToNXUnitsRatio));
        //public static NXOpen.Body ToSolid(this Brep value, double factor) => BrepEncoder.ToSolid(BrepEncoder.ToRawBrep(value, factor));

        //public static DB.Solid ToSolid(this Mesh value) => Raw.RawEncoder.ToHost(MeshEncoder.ToRawBrep(value, UnitConverter.ToHostUnits()));
        //public static DB.Solid ToSolid(this Mesh value, double factor) => BrepEncoder.ToSolid(MeshEncoder.ToRawBrep(value, factor));

        //public static DB.Mesh ToMesh(this Mesh value) => MeshEncoder.ToMesh(MeshEncoder.ToRawMesh(value, UnitConverter.ToHostUnits()));
        //public static DB.Mesh ToMesh(this Mesh value, double factor) => MeshEncoder.ToMesh(MeshEncoder.ToRawMesh(value, factor));

        //public static DB.GeometryObject ToGeometryObject(this GeometryBase geometry) => ToGeometryObject(geometry, UnitConverter.ToHostUnits());
        //public static DB.GeometryObject ToGeometryObject(this GeometryBase geometry, double scaleFactor)
        //{
        //    switch (geometry)
        //    {
        //        case Point point: return point.ToPoint(scaleFactor);
        //        case Curve curve: return curve.ToCurve(scaleFactor);
        //        case Brep brep: return brep.ToSolid(scaleFactor);
        //        case Mesh mesh: return mesh.ToMesh(scaleFactor);

        //        case Extrusion extrusion:
        //            {
        //                var brep = extrusion.ToBrep();
        //                if (BrepEncoder.EncodeRaw(ref brep, scaleFactor))
        //                    return BrepEncoder.ToSolid(brep);
        //            }
        //            break;

        //        case SubD subD:
        //            {
        //                var brep = subD.ToBrep();
        //                if (BrepEncoder.EncodeRaw(ref brep, scaleFactor))
        //                    return BrepEncoder.ToSolid(brep);
        //            }
        //            break;

        //        default:
        //            if (geometry.HasBrepForm)
        //            {
        //                var brepForm = Brep.TryConvertBrep(geometry);
        //                if (BrepEncoder.EncodeRaw(ref brepForm, scaleFactor))
        //                    return BrepEncoder.ToSolid(brepForm);
        //            }
        //            break;
        //    }

        //    throw new ConversionException($"Unable to convert {geometry} to Autodesk.Revit.DB.GeometryObject");
        //}
        #endregion

        public static IEnumerable<NXOpen.Point> ToPointMany(this PointCloud value) => value.ToPointMany(UnitConverter.RhinoToNXUnitsRatio);
        public static IEnumerable<NXOpen.Point> ToPointMany(this PointCloud value, double factor)
        {
            if (factor == 1.0)
            {
                foreach (var point in value)
                {
                    var location = point.Location;
                    yield return WorkPart.Points.CreatePoint(new NXOpen.Point3d(location.X, location.Y, location.Z));
                }
            }
            else
            {
                foreach (var point in value)
                {
                    var location = point.Location;
                    yield return WorkPart.Points.CreatePoint(new NXOpen.Point3d(location.X * factor, location.Y * factor, location.Z * factor));
                }
            }
        }








    }
}
