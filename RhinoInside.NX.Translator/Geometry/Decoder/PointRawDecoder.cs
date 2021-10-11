using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Translator.Geometry
{
    public static partial class RawDecoder
    {
        public static Vector3d ToRhino(NXOpen.Vector3d value)
        {
            return new Vector3d(value.X, value.Y, value.Z);
        }

        public static Point3d ToRhino(NXOpen.Point3d value)
        {
            return new Point3d(value.X, value.Y, value.Z);
        }

        public static Point3d ToRhinoPoint3d(double[] value) => new Point3d(value[0], value[1], value[2]);

        public static Vector3d ToRhinoVector3d(double[] value) => new Vector3d(value[0], value[1], value[2]);

        public static Rhino.Geometry.Transform ToRhinoTransform(NXOpen.Matrix4x4 transform)
        {
            var value = new Rhino.Geometry.Transform
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

        public static Point ToRhino(NXOpen.Point point)
        {
            return new Point(point.Coordinates.ToRhino());
        }
    }
}
