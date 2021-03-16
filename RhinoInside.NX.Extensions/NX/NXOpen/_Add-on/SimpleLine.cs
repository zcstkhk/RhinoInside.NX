using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
    /// <summary>
    /// 用于表示虚拟直线，它具有起点和中点属性
    /// </summary>
    public struct SimpleLine
    {
        public Point3d StartPoint { get; set; }

        public Point3d EndPoint { get; set; }

        public SimpleLine(Point3d startPoint, Point3d endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public Line CreateLine()
        {
            return Globals.WorkPart.Curves.CreateLine(StartPoint, EndPoint);
        }
    }
}
