using Grasshopper.Kernel.Types;
using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RhinoInside.NX.Translator.Geometry;
using RhinoInside.NX.Translator;

namespace RhinoInside.NX.GH.Types
{
    public class NX_Curve : NX_SmartObject<Curve>
    {
        public NX_Curve()
        {

        }

        public NX_Curve(Tag tag) : base(tag)
        {

        }

        public NX_Curve(Tag tag, bool highlight) : base(tag, highlight)
        {

        }

        public override string TypeName => "NX Curve";

        public override string TypeDescription => Properties.Languages.GetString("NXCurveTypeDesc");

        public override string ToString()
        {
            if (Value != null)
                return base.ToString();
            
            return string.Empty;
        }

        public override bool CastTo<Q>(ref Q target)
        {
            if (typeof(Q) == typeof(GH_Curve))
            {
                target = (Q)(object)new GH_Curve(Value.ToRhino());

                return true;
            }

            return base.CastTo(ref target);
        }

        public override bool CastFrom(object source)
        {
            if (source is GH_Curve gh_curve)
            {
                Value = gh_curve.Value.ToCurve();

                return true;
            }
            else
            {
                Console.WriteLine($"无法将 {source.GetType()} 转换为 NXOpen.Curve");
                return base.CastFrom(source);
            }
        }
    }
}
