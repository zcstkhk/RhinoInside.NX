using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using RhinoInside.NX.Extensions;
using RhinoInside.NX.Translator.Geometry;
using GH_IO.Serialization;

namespace RhinoInside.NX.GH.Types
{
    public class NX_Edge : NX_DisplayableObject<NXOpen.Edge>
    {
        public NX_Edge() : base()
        {

        }

        /// <summary>
        /// 存放于当前工作部件中的对象
        /// </summary>
        /// <param name="handle"></param>
        public NX_Edge(NXOpen.Tag tag) : base(tag)
        {

        }

        public NX_Edge(NXOpen.Tag tag, bool highlight) : base(tag, highlight)
        {

        }

        public override string TypeName => "NX Edge";

        public override string TypeDescription => Properties.Languages.GetString("NXEdgeTypeDesc");

        public override bool CastTo<Q>(ref Q target)
        {
            if (typeof(Q) == typeof(GH_Curve))
            {
                Globals.TheUfSession.Modl.CreateCurveFromEdge(Tag, out var curveTag);

                target = (Q)(object)new GH_Curve((curveTag.GetTaggedObject() as NXOpen.Curve).ToRhino());

                Globals.TheUfSession.Obj.DeleteObject(curveTag);
                
                return true;
            }
            else
                return base.CastTo(ref target);
        }

        public override string ToString()
        {
            if (Value != null)
                return base.ToString() + $"Edge Type: {Value.SolidEdgeType}";

            return string.Empty;
        }
    }
}
