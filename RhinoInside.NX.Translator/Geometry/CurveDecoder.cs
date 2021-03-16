using Rhino.Geometry;
using RhinoInside.NX.Translator.Geometry.Raw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Translator.Geometry
{
    /// <summary>
    /// 将 NX 对象转换为 Rhino 对象
    /// </summary>
    public static class CurveDecoder
    {
        public static Point ToRhino(this NXOpen.Point point)
        {
            var rhino = RawDecoder.ToRhino(point); 
            UnitConverter.Scale(rhino, UnitConverter.NXToRhinoUnitsRatio); 
            return rhino; 
        }

        public static Curve ToRhinoCurve(this NXOpen.Arc value)
        { 
            var rhino = RawDecoder.ToRhinoArcCurve(value);
            UnitConverter.Scale(rhino, UnitConverter.NXToRhinoUnitsRatio);
            return rhino;
        }

        public static Curve ToRhinoCurve(this NXOpen.Ellipse value)
        { 
            var rhino = RawDecoder.ToRhinoNurbsCurve(value);
            UnitConverter.Scale(rhino, UnitConverter.NXToRhinoUnitsRatio);
            return rhino;
        }

        public static Curve ToRhinoCurve(this NXOpen.Spline value)
        {
            var rhino = RawDecoder.ToRhinoNurbsCurve(value);
            UnitConverter.Scale(rhino, UnitConverter.NXToRhinoUnitsRatio); return rhino;
        }

        public static Curve ToRhino(this NXOpen.Curve value)
        {
            Curve rhino;

            switch (value)
            {
                case null: return null;
                case NXOpen.Line line:
                    rhino = RawDecoder.ToRhinoLineCurve(line);
                    break;
                case NXOpen.Arc arc:
                    rhino = RawDecoder.ToRhinoArcCurve(arc);
                    break;
                case NXOpen.Ellipse ellipse:
                    rhino = RawDecoder.ToRhinoNurbsCurve(ellipse);
                    break;
                case NXOpen.Spline nurb:
                    rhino = RawDecoder.ToRhinoNurbsCurve(nurb);
                    break;
                default: throw new NotImplementedException();
            }

            UnitConverter.Scale(rhino, UnitConverter.NXToRhinoUnitsRatio); return rhino;
        }

        public static PolylineCurve ToPolylineCurve(this NXOpen.Polyline value)
        {
            var rhino = RawDecoder.ToRhinoPolylineCurve(value); 
            UnitConverter.Scale(rhino, UnitConverter.NXToRhinoUnitsRatio); 
            return rhino; 
        }
    }
}
