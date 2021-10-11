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
        public static Brep ToBrep(this NXOpen.Body value)
        {
            var rhino = RawDecoder.ToRhinoBrep(value);
            UnitConverter.Scale(rhino, UnitConverter.NXToRhinoUnitsRatio);
            return rhino;
        }

        public static Brep ToBrep(this NXOpen.Face value)
        {
            var rhino = RawDecoder.ToRhinoBrep(value);
            UnitConverter.Scale(rhino, UnitConverter.NXToRhinoUnitsRatio);
            return rhino;
        }

        public static Surface ToRhinoSurface(this NXOpen.Face face)
        {
            var rhino = RawDecoder.ToRhinoSurface(face, out _);
            UnitConverter.Scale(rhino, UnitConverter.NXToRhinoUnitsRatio);
            return rhino;
        }

        public static Plane ToRhino(this NXOpen.Plane value)
        {
            var rhino = RawDecoder.ToRhino(value);
            UnitConverter.Scale(ref rhino, UnitConverter.NXToRhinoUnitsRatio);
            return rhino;
        }
    }
}
