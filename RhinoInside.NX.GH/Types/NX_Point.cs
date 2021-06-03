using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.GH.Types
{
    class NX_Point : NX_SmartObject<NXOpen.Point>
    {
        public override string TypeName => "NX Point";

        public override string TypeDescription => Properties.Languages.GetString("NXPointTypeDesc");

        public NX_Point() : base()
        {

        }

        /// <summary>
        /// 存放于当前工作部件中的对象
        /// </summary>
        /// <param name="handle"></param>
        public NX_Point(NXOpen.Tag tag) : base(tag)
        {

        }

        public NX_Point(NXOpen.Tag tag, bool highlight) : base(tag, highlight)
        {

        }
    }
}
