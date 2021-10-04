using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using PLMComponents.Parasolid.PK_.Unsafe;

namespace RhinoInside.NX.Extensions.Parasolid
{
    public static class BODY_tEx
    {
        public static Body GetBody(this BODY_t psBody)
        {
            Tag ps_tag = (Tag)psBody.Value;
            Globals.TheUfSession.Ps.AskObjectOfPsTag(ps_tag, out var bodyTag);
            return bodyTag.GetTaggedObject() as Body;
        }
    }
}
