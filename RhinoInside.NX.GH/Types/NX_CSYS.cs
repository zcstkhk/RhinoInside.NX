using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.GH.Types
{
    class NX_CoordinateSystem : NX_SmartObject<NXOpen.CoordinateSystem>
    {
        public override string TypeName => "NX CSYS";

        public override string TypeDescription => Properties.Languages.GetString("NXCSYSTypeDesc");

        public NX_CoordinateSystem() : base()
        {

        }

        /// <summary>
        /// 存放于当前工作部件中的对象
        /// </summary>
        /// <param name="handle"></param>
        public NX_CoordinateSystem(NXOpen.Tag tag) : base(tag)
        {

        }

        public NX_CoordinateSystem(NXOpen.Tag tag, bool highlight) : base(tag, highlight)
        {

        }
    }
}
