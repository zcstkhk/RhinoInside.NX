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
using static RhinoInside.NX.Extensions.Globals;
using RhinoInside.NX.Extensions;

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

        public static Matrix4x4 AsTransform(Rhino.Geometry.Transform value)
        {
            Debug.Assert(value.IsAffine);

            var result = Matrix4x4Ex.Create(new NXOpen.Vector3d(value.M03, value.M13, value.M23));

            result.SetAxisX(new Vector4d(value.M00, value.M10, value.M20));
            result.SetAxisY(new Vector4d(value.M01, value.M11, value.M21));
            result.SetAxisZ(new Vector4d(value.M02, value.M12, value.M22));
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
