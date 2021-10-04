using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using PLMComponents.Parasolid.PK_.Unsafe;

namespace RhinoInside.NX.Extensions.Parasolid
{
   public static class EDGE_tEx
    {
        public static Edge GetEdge(this EDGE_t psEdge)
        {
            Tag ps_tag = (Tag)psEdge.Value;
            Globals.TheUfSession.Ps.AskObjectOfPsTag(ps_tag, out var edgeTag);
            return edgeTag.GetTaggedObject() as Edge;
        }
    }
}
