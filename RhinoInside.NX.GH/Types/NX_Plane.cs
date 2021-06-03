using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.GH.Types
{
    class NX_Plane : NX_SmartObject<NXOpen.Plane>
    {
        public override string TypeName => "NX Plane";

        public override string TypeDescription => Properties.Languages.GetString("NXPlaneTypeDesc");

        public NX_Plane() : base()
        {

        }

        /// <summary>
        /// 存放于当前工作部件中的对象
        /// </summary>
        /// <param name="handle"></param>
        public NX_Plane(NXOpen.Tag tag) : base(tag)
        {

        }

        public NX_Plane(NXOpen.Tag tag, bool highlight) : base(tag, highlight)
        {

        }
    }
}
