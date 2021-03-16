using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
    public class Rectangle
    {
        public Point3d V1;
        public Point3d V2;
        public Point3d V3;
        public Point3d V4;

        public SimpleLine L1 { get => new SimpleLine(V1, V2); }
        public SimpleLine L2 { get => new SimpleLine(V2, V3); }
        public SimpleLine L3 { get => new SimpleLine(V3, V4); }
        public SimpleLine L4 { get => new SimpleLine(V4, V1); }

        /// <summary>
        /// 使用四个点初始化矩形
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        public Rectangle(Point3d v1, Point3d v2, Point3d v3, Point3d v4)
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
            V4 = v4;
        }

        public SimpleLine[] GetEdges()
        {
            return new SimpleLine[] { L1, L2, L3, L4 };
        }

        public Line[] CreateEdges()
        {
            return new Line[] { L1.CreateLine(), L2.CreateLine(), L3.CreateLine(), L4.CreateLine() };
        }
    }
}
