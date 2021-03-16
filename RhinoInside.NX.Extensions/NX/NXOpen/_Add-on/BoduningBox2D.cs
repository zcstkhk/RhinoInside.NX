using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Extensions
{
    public class BoundingBox2D
    {
        public UV Max;

        public UV Min;

        public BoundingBox2D(double uMin, double uMax, double vMin, double vMax)
        {
            Max = new UV(uMax, vMax);
            Min = new UV(uMin, vMin);
        }

        /// <summary>
        /// UV 中点
        /// </summary>
        public UV Middle => new UV((Max.U + Min.U) / 2, (Max.V + Min.V) / 2);
    }
}
