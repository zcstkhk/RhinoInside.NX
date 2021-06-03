using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.GH.Types
{
    public abstract class NX_SmartObject<T> : NX_DisplayableObject<T> where T : SmartObject
    {
        public NX_SmartObject()
        {

        }

        public NX_SmartObject(Tag tag) : base(tag)
        {

        }

        public NX_SmartObject(Tag tag, bool highlight) : base(tag, highlight)
        {

        }

        public override bool Visible
        {
            get => _visible;
            set
            {
                if (value)
                    Value.SetVisibility(SmartObject.VisibilityOption.Visible);
                else
                    Value.SetVisibility(SmartObject.VisibilityOption.Invisible);

                _visible = value;
            }
        }
    }
}
