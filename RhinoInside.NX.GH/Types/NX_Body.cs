using Grasshopper.Kernel.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.GH.Types
{
    public class NX_Body : NX_DisplayableObject<NXOpen.Body>
    {
        public override string TypeName => "NX Body";

        public override string TypeDescription => Properties.Languages.GetString("NXBodyTypeDesc");

        public NX_Body() : base()
        {

        }

        /// <summary>
        /// 存放于当前工作部件中的对象
        /// </summary>
        /// <param name="handle"></param>
        public NX_Body(NXOpen.Tag tag) : base(tag)
        {

        }

        public NX_Body(NXOpen.Tag tag, bool highlight) : base(tag, highlight)
        {

        }

        public override string ToString()
        {
            if (Value != null)
                return base.ToString() + $"Solid Body: {Value.IsSolidBody}";

            return string.Empty;
        }
    }
}

namespace RhinoInside.NX.GH.Parameters.Hints
{
    public class NX_BodyHint : IGH_TypeHint
    {
        public string TypeName => "NXOpen.Body";

        public Guid HintID => new Guid("96FA89B5-2370-4A0E-AD5C-7145A37D8E9A");

        public bool Cast(object data, out object target)
        {
            target = data;

            return true;
        }
    }
}