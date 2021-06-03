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
    internal static partial class RawDecoder
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
    };
}
