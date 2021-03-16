using Grasshopper.Kernel.Types;
using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.GH.Types
{
    public class NX_Curve : NX_DisplayableObject<Curve>
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
    }
}
