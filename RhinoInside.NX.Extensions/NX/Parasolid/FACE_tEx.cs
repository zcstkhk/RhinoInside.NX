using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using PLMComponents.Parasolid.PK_.Unsafe;

namespace RhinoInside.NX.Extensions.Parasolid
{
    public static class FACE_tEx
    {
        public static Face GetFace(this FACE_t psFace)
        {
            Tag ps_tag = (Tag)psFace.Value;
            Globals.TheUfSession.Ps.AskObjectOfPsTag(ps_tag, out var faceTag);
            return faceTag.GetTaggedObject() as Face;
        }
    }
}
