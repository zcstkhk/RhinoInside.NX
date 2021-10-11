using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using NXOpen.Extensions;

namespace RhinoInside.NX.Translator.Geometry
{
    using Raw;

    /// <summary>
    /// Methods in this class do a full geometry conversion.
    /// <para>It converts geometry from Revit internal units to Active Rhino model units.</para>
    /// <para>For direct conversion methods see <see cref="Raw.RawDecoder"/> class.</para>
    /// </summary>
    public static class GeometryDecoder
    {
        #region Geometry values
        





        //public static BoundingBox ToBoundingBox(this DB.BoundingBoxXYZ value)
        //{ var rhino = RawDecoder.AsBoundingBox(value); UnitConverter.Scale(ref rhino, UnitConverter.ToRhinoUnits); return rhino; }

        //public static BoundingBox ToBoundingBox(this DB.BoundingBoxXYZ value, out Transform transform)
        //{
        //    var rhino = RawDecoder.AsBoundingBox(value, out transform);
        //    UnitConverter.Scale(ref rhino, UnitConverter.ToRhinoUnits);
        //    UnitConverter.Scale(ref transform, UnitConverter.ToRhinoUnits);
        //    return rhino;
        //}

        //public static BoundingBox ToBoundingBox(this DB.Outline value)
        //{
        //    return new BoundingBox(value.MinimumPoint.ToPoint3d(), value.MaximumPoint.ToPoint3d());
        //}

        //public static Box ToBox(this DB.BoundingBoxXYZ value)
        //{
        //    var rhino = RawDecoder.AsBoundingBox(value, out var transform);
        //    UnitConverter.Scale(ref rhino, UnitConverter.ToRhinoUnits);
        //    UnitConverter.Scale(ref transform, UnitConverter.ToRhinoUnits);

        //    return new Box
        //    (
        //      new Plane
        //      (
        //        origin: new Point3d(transform.M03, transform.M13, transform.M23),
        //        xDirection: new Vector3d(transform.M00, transform.M10, transform.M20),
        //        yDirection: new Vector3d(transform.M01, transform.M11, transform.M21)
        //      ),
        //      xSize: new Interval(rhino.Min.X, rhino.Max.X),
        //      ySize: new Interval(rhino.Min.Y, rhino.Max.Y),
        //      zSize: new Interval(rhino.Min.Z, rhino.Max.Z)
        //    );
        //}
        #endregion

        #region GeometryBase





        //public static Mesh ToMesh(this DB.Mesh value)
        //{ var rhino = MeshDecoder.ToRhino(value); UnitConverter.Scale(rhino, UnitConverter.ToRhinoUnits); return rhino; }
        #endregion


    }
}
