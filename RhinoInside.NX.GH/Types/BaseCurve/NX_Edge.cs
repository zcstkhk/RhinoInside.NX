using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using NXOpen;
using RhinoInside.NX.Extensions;
using GH_IO.Serialization;

namespace RhinoInside.NX.GH.Types
{
    public class NX_Edge : NX_DisplayableObject<Edge>
    {
        public NX_Edge() : base()
        {

        }

        /// <summary>
        /// 存放于当前工作部件中的对象
        /// </summary>
        /// <param name="handle"></param>
        public NX_Edge(Tag tag) : base(tag)
        {

        }

        public NX_Edge(Tag tag, bool highlight) : base(tag, highlight)
        {

        }

        public override string TypeName => "NX Edge";

        public override string TypeDescription => Properties.Languages.GetString("NXEdgeTypeDesc");

        public override bool CastTo<Q>(ref Q target)
        {
            if (typeof(Q) == typeof(NX_Curve))
            {
                target = (Q)(object)new NX_Curve(Tag);
                return true;
            }
            else
                return false;
        }

        public override string ToString()
        {
            if (Value != null)
                return base.ToString() + $"Edge Type: {Value.SolidEdgeType}";

            return string.Empty;
        }
    }
}
