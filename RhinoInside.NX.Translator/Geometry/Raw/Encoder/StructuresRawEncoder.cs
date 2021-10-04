using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NXOpen;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using Plane = Rhino.Geometry.Plane;
using Point = Rhino.Geometry.Point;
using Point2d = Rhino.Geometry.Point2d;
using Point3d = Rhino.Geometry.Point3d;
using Polyline = Rhino.Geometry.Polyline;
using Vector3d = Rhino.Geometry.Vector3d;
using PK = PLMComponents.Parasolid.PK_.Unsafe;
using static NXOpen.Extensions.Globals;
using NXOpen.Extensions;

namespace RhinoInside.NX.Translator.Geometry.Raw
{
    /// <summary>
    /// 此类中的方法是将 RAW 形式的对象转换为 NX 几何体。
    /// <para>The input geometry is granted not to be modified on any way, no copies are necessary before calling this methods.</para>
    /// <para>Raw form is Rhino geometry in NX internal units</para>
    /// </summary>
    static partial class RawEncoder
    {
        #region Values
        public static UV AsUV(Point2f value)
        {
            return new UV(value.X, value.Y);
        }

        public static UV AsUV(Point2d value)
        {
            return new UV(value.X, value.Y);
        }

        public static UV AsUV(Vector2d value)
        {
            return new UV(value.X, value.Y);
        }
        public static UV AsUV(Vector2f value)
        {
            return new UV(value.X, value.Y);
        }

        public static NXOpen.Point3d AsXYZ(Point3f value)
        {
            return new NXOpen.Point3d(value.X, value.Y, value.Z);
        }

        public static NXOpen.Point3d AsXYZ(Point3d value)
        {
            return new NXOpen.Point3d(value.X, value.Y, value.Z);
        }

        public static NXOpen.Point3d AsXYZ(Vector3d value)
        {
            return new NXOpen.Point3d(value.X, value.Y, value.Z);
        }

        public static NXOpen.Point3d AsXYZ(Vector3f value)
        {
            return new NXOpen.Point3d(value.X, value.Y, value.Z);
        }

        public static NXOpen.Matrix4x4 ToMatrix4x4(this Rhino.Geometry.Transform value) => ToMatrix4x4(value, UnitConverter.RhinoToNXUnitsRatio);

        public static NXOpen.Matrix4x4 ToMatrix4x4(this Rhino.Geometry.Transform value, double factor)
        {
            Debug.Assert(value.IsAffine);

            NXOpen.Matrix4x4 result = default;
            result.Ss = factor;
            result.Sx = 0.0;
            result.Sy = 0.0;
            result.Sz = 0.0;

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

        public static BoundingBox3D AsBoundingBoxXYZ(Rhino.Geometry.BoundingBox value)
        {
            return new BoundingBox3D(AsXYZ(value.Min), AsXYZ(value.Max));
        }

        //public static NXOpen.Extensions.BoundingBox AsBoundingBoxXYZ(Rhino.Geometry.Box value)
        //{
        //    return new NXOpen.Extensions.BoundingBox(AsXYZ(value.BoundingBox.Min), AsXYZ(value.BoundingBox.Max), AsTransform(Transform.PlaneToPlane(Plane.WorldXY, value.Plane)));
        //}

        public static NXOpen.Plane AsPlane(Plane value)
        {
            return WorkPart.Planes.CreatePlane(value.Origin.ToXYZ(), value.Normal.ToXYZ(), SmartObject.UpdateOption.WithinModeling);
        }

        public static NXOpen.Point3d[] AsPolyLine(Polyline value)
        {

            int count = value.Count;
            var points = new NXOpen.Point3d[count];

            for (int p = 0; p < count; ++p)
                points[p] = AsXYZ(value[p]);

            return points;
        }
        #endregion

        #region Point
        public static NXOpen.Point ToHost(Point value)
        {
            return value.ToPoint();
        }
        #endregion
    }
}
