using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RhinoInside.NX.Extensions.Globals;
using NXOpen;

namespace RhinoInside.NX.Extensions.Parasolid
{
    public struct PKTag
    {
        public PKTag(TaggedObject obj)
        {
            NXTag = obj.Tag;
            TheUfSession.Ps.AskPsTagOfObject(obj.Tag, out Tag psvalue);
            PKValue = (int)psvalue;
        }

        public PKTag(Tag obj)
        {
            NXTag = obj;
            TheUfSession.Ps.AskPsTagOfObject(obj, out Tag psvalue);
            PKValue = (int)psvalue;
        }

        public PKTag(int psValue)
        {
            PKValue = psValue;
            Tag tag;
            TheUfSession.Ps.AskObjectOfPsTag((Tag)psValue, out tag);
            if (tag == Tag.Null)
            {
                TheUfSession.Ps.CreatePartition(out Tag tag2);
                TheUfSession.Ps.CreateObjFromPsTag((Tag)psValue, out tag);
            }
            NXTag = tag;
        }

        public int PKValue;

        public Tag NXTag;
    }
}
