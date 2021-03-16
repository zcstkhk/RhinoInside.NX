using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PK = PLMComponents.Parasolid.PK_.Unsafe;

namespace NXOpen.Extensions.Parasolid
{
    public static class VECTOR_TEx
    {
        public unsafe static Point3d ToPoint3d(this PK.VECTOR_t vec, double factor = 1000.0) => new Point3d(vec.coord[0] * factor, vec.coord[1] * factor, vec.coord[2] * factor);

        public unsafe static Point3d ToPoint3d(this PK.VECTOR1_t vec, double factor = 1000.0) => new Point3d(vec.coord[0] * factor, vec.coord[1] * factor, vec.coord[2] * factor);
    }
}
