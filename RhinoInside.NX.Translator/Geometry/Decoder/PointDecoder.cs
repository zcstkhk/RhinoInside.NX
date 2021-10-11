using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Translator.Geometry
{
    public static partial class Decoder
    {
        public static Vector3d ToRhinoVector3d(this double[] value)
        {
            var rhino = RawDecoder.ToRhinoVector3d(value);
            UnitConverter.Scale(ref rhino, UnitConverter.NXToRhinoUnitsRatio);
            return rhino;
        }

        public static Point3d ToRhinoPoint3d(this double[] value)
        {
            var rhino = RawDecoder.ToRhinoPoint3d(value);
            UnitConverter.Scale(ref rhino, UnitConverter.NXToRhinoUnitsRatio);
            return rhino;
        }

        public static Point3d ToRhino(this NXOpen.Point3d value)
        {
            Point3d rhino = RawDecoder.ToRhino(value);
            UnitConverter.Scale(ref rhino, UnitConverter.NXToRhinoUnitsRatio);
            return rhino;
        }

        public static Vector3d ToRhino(this NXOpen.Vector3d value)
        {
            Vector3d rhino = RawDecoder.ToRhino(value);
            UnitConverter.Scale(ref rhino, UnitConverter.NXToRhinoUnitsRatio);
            return rhino;
        }

        public static Rhino.Geometry.Transform ToRhino(this NXOpen.Matrix4x4 value)
        {
            var rhino = RawDecoder.ToRhinoTransform(value);
            UnitConverter.Scale(ref rhino, UnitConverter.NXToRhinoUnitsRatio);
            return rhino;
        }
    }
}
