using Rhino.Geometry;
using RhinoInside.NX.Translator.Geometry.Raw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NXOpen.Extensions.Globals;
using NXOpen.Extensions;

namespace RhinoInside.NX.Translator.Geometry
{
    using Raw;

    /// <summary>
    /// 将 NX 对象转换为 Rhino 对象
    /// </summary>
    public static partial class Decoder
    {
        public static Point ToRhino(this NXOpen.Point point)
        {
            var rhino = RawDecoder.ToRhino(point);
            UnitConverter.Scale(rhino, UnitConverter.NXToRhinoUnitsRatio);
            return rhino;
        }

        public static Curve ToRhinoCurve(this NXOpen.Arc value)
        {
            var rhino = RawDecoder.ToRhinoCurve(value);
            UnitConverter.Scale(rhino, UnitConverter.NXToRhinoUnitsRatio);
            return rhino;
        }

        public static Curve ToRhinoCurve(this NXOpen.Ellipse value)
        {
            var rhino = RawDecoder.ToRhinoCurve(value);
            UnitConverter.Scale(rhino, UnitConverter.NXToRhinoUnitsRatio);
            return rhino;
        }

        public static Curve ToRhinoCurve(this NXOpen.Spline value)
        {
            var rhino = RawDecoder.ToRhinoCurve(value);
            UnitConverter.Scale(rhino, UnitConverter.NXToRhinoUnitsRatio); return rhino;
        }

        public static Curve ToRhinoCurve(this NXOpen.IBaseCurve value)
        {
            Curve rhino = null;
            switch (value)
            {
                case NXOpen.Edge edge:
                    rhino = RawDecoder.ToRhinoCurve(edge);
                    break;
                case NXOpen.Curve c:
                    rhino = RawDecoder.ToRhinoCurve(c);
                    break;
                default:
                    throw new NotImplementedException("未实现的曲线转换类型：" + value.GetType().FullName);
            }

            UnitConverter.Scale(rhino, UnitConverter.NXToRhinoUnitsRatio);

            return rhino;
        }

        public static Curve ToRhino(this NXOpen.Curve value)
        {
            Curve rhino = RawDecoder.ToRhinoCurve(value);

            UnitConverter.Scale(rhino, UnitConverter.NXToRhinoUnitsRatio);

            return rhino;
        }

        public static PolylineCurve ToPolylineCurve(this NXOpen.Polyline value)
        {
            var rhino = RawDecoder.ToRhinoCurve(value);
            UnitConverter.Scale(rhino, UnitConverter.NXToRhinoUnitsRatio);
            return rhino;
        }
    }
}
