using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

namespace RhinoInside.NX.Translator.Geometry.Raw
{
    static partial class RawEncoder
    {
        public static NXOpen.Line ToHost(LineCurve value)
        {
            var line = value.Line;
            return line.ToNXLine();
        }

        public static NXOpen.Arc ToHost(ArcCurve value)
        {
            var arc = value.Arc;
            return arc.ToNXArc();
        }

        public static double[] ToHost(NurbsCurveKnotList list)
        {
            var count = list.Count;
            var knots = new double[count + 2];

            int j = 0, k = 0;
            while (j < count)
                knots[++k] = list[j++];

            knots[0] = knots[1];
            knots[count + 1] = knots[count];

            return knots;
        }

        public static NXOpen.Point3d[] ToHost(NurbsCurvePointList list)
        {
            var count = list.Count;
            var points = new NXOpen.Point3d[count];

            for (int p = 0; p < count; ++p)
            {
                var location = list[p].Location;
                points[p] = new NXOpen.Point3d(location.X, location.Y, location.Z);
            }

            return points;
        }

        public static NXOpen.Curve ToHost(NurbsCurve value)
        {
            return value.ToNXCurve();
        }
    }
}
