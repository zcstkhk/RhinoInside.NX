using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RhinoInside.NX.Extensions.Globals;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
    /// <summary>
    /// 几何计算的常用方法
    /// </summary>
    public static class Geometry
    {
        /// <summary>
        /// 求两个基准平面的相交方向矢量
        /// </summary>
        /// <param name="plane1Origin"></param>
        /// <param name="plane1Normal"></param>
        /// <param name="plane2Origin"></param>
        /// <param name="plane2Normal"></param>
        public static Vector3d Intersect(Point3d plane1Origin, Vector3d plane1Normal, Point3d plane2Origin, Vector3d plane2Normal)
        {
            double[] perpendicularVectorInDouble = new double[3];
            TheUfSession.Vec3.Cross(plane1Normal.ToArray(), plane2Normal.ToArray(), perpendicularVectorInDouble);

            var vectorStartPoint3d = plane1Origin.Project(plane2Origin, plane2Normal);

            var vectorSecondPoint3d = plane1Origin.Move(perpendicularVectorInDouble.ToVector3d(), 100).Project(plane2Origin, plane2Normal);

            var vector = vectorSecondPoint3d.Subtract(vectorStartPoint3d);

            double[] unit_vec = new double[3];
            TheUfSession.Vec3.Unitize(vector.ToArray(), 0.01, out _, unit_vec);

            return unit_vec.ToVector3d();
        }

        /// <summary>
		/// 求三个基准平面的交点
		/// </summary>
		/// <param name="plane1Origin"></param>
		/// <param name="plane1Normal"></param>
		/// <param name="plane2Origin"></param>
		/// <param name="plane2Normal"></param>
		/// <param name="plane3Origin"></param>
		/// <param name="plane3Normal"></param>
		public static Point3d Intersect(Point3d plane1Origin, Vector3d plane1Normal, Point3d plane2Origin, Vector3d plane2Normal, Point3d plane3Origin, Vector3d plane3Normal)
        {
            double x = -((-plane1Normal.X * plane3Normal.Y * plane2Normal.Z * plane1Origin.X + plane1Normal.X * plane2Normal.Y * plane3Normal.Z * plane1Origin.X + plane2Normal.X * plane3Normal.Y * plane1Normal.Z * plane2Origin.X - plane2Normal.X * plane1Normal.Y * plane3Normal.Z * plane2Origin.X - plane3Normal.X * plane2Normal.Y * plane1Normal.Z * plane3Origin.X + plane3Normal.X * plane1Normal.Y * plane2Normal.Z * plane3Origin.X - plane1Normal.Y * plane3Normal.Y * plane2Normal.Z * plane1Origin.Y + plane1Normal.Y * plane2Normal.Y * plane3Normal.Z * plane1Origin.Y + plane2Normal.Y * plane3Normal.Y * plane1Normal.Z * plane2Origin.Y - plane1Normal.Y * plane2Normal.Y * plane3Normal.Z * plane2Origin.Y - plane2Normal.Y * plane3Normal.Y * plane1Normal.Z * plane3Origin.Y + plane1Normal.Y * plane3Normal.Y * plane2Normal.Z * plane3Origin.Y - plane3Normal.Y * plane1Normal.Z * plane2Normal.Z * plane1Origin.Z + plane2Normal.Y * plane1Normal.Z * plane3Normal.Z * plane1Origin.Z + plane3Normal.Y * plane1Normal.Z * plane2Normal.Z * plane2Origin.Z - plane1Normal.Y * plane2Normal.Z * plane3Normal.Z * plane2Origin.Z - plane2Normal.Y * plane1Normal.Z * plane3Normal.Z * plane3Origin.Z + plane1Normal.Y * plane2Normal.Z * plane3Normal.Z * plane3Origin.Z) / (plane3Normal.X * plane2Normal.Y * plane1Normal.Z - plane2Normal.X * plane3Normal.Y * plane1Normal.Z - plane3Normal.X * plane1Normal.Y * plane2Normal.Z + plane1Normal.X * plane3Normal.Y * plane2Normal.Z + plane2Normal.X * plane1Normal.Y * plane3Normal.Z - plane1Normal.X * plane2Normal.Y * plane3Normal.Z));

            double y = -((plane1Normal.X * plane3Normal.X * plane2Normal.Z * plane1Origin.X - plane1Normal.X * plane2Normal.X * plane3Normal.Z * plane1Origin.X - plane2Normal.X * plane3Normal.X * plane1Normal.Z * plane2Origin.X + plane1Normal.X * plane2Normal.X * plane3Normal.Z * plane2Origin.X + plane2Normal.X * plane3Normal.X * plane1Normal.Z * plane3Origin.X - plane1Normal.X * plane3Normal.X * plane2Normal.Z * plane3Origin.X + plane3Normal.X * plane1Normal.Y * plane2Normal.Z * plane1Origin.Y - plane2Normal.X * plane1Normal.Y * plane3Normal.Z * plane1Origin.Y - plane3Normal.X * plane2Normal.Y * plane1Normal.Z * plane2Origin.Y + plane1Normal.X * plane2Normal.Y * plane3Normal.Z * plane2Origin.Y + plane2Normal.X * plane3Normal.Y * plane1Normal.Z * plane3Origin.Y - plane1Normal.X * plane3Normal.Y * plane2Normal.Z * plane3Origin.Y + plane3Normal.X * plane1Normal.Z * plane2Normal.Z * plane1Origin.Z - plane2Normal.X * plane1Normal.Z * plane3Normal.Z * plane1Origin.Z - plane3Normal.X * plane1Normal.Z * plane2Normal.Z * plane2Origin.Z + plane1Normal.X * plane2Normal.Z * plane3Normal.Z * plane2Origin.Z + plane2Normal.X * plane1Normal.Z * plane3Normal.Z * plane3Origin.Z - plane1Normal.X * plane2Normal.Z * plane3Normal.Z * plane3Origin.Z) / (plane3Normal.X * plane2Normal.Y * plane1Normal.Z - plane2Normal.X * plane3Normal.Y * plane1Normal.Z - plane3Normal.X * plane1Normal.Y * plane2Normal.Z + plane1Normal.X * plane3Normal.Y * plane2Normal.Z + plane2Normal.X * plane1Normal.Y * plane3Normal.Z - plane1Normal.X * plane2Normal.Y * plane3Normal.Z));

            double z = -((-plane1Normal.X * plane3Normal.X * plane2Normal.Y * plane1Origin.X + plane1Normal.X * plane2Normal.X * plane3Normal.Y * plane1Origin.X + plane2Normal.X * plane3Normal.X * plane1Normal.Y * plane2Origin.X - plane1Normal.X * plane2Normal.X * plane3Normal.Y * plane2Origin.X - plane2Normal.X * plane3Normal.X * plane1Normal.Y * plane3Origin.X + plane1Normal.X * plane3Normal.X * plane2Normal.Y * plane3Origin.X - plane3Normal.X * plane1Normal.Y * plane2Normal.Y * plane1Origin.Y + plane2Normal.X * plane1Normal.Y * plane3Normal.Y * plane1Origin.Y + plane3Normal.X * plane1Normal.Y * plane2Normal.Y * plane2Origin.Y - plane1Normal.X * plane2Normal.Y * plane3Normal.Y * plane2Origin.Y - plane2Normal.X * plane1Normal.Y * plane3Normal.Y * plane3Origin.Y + plane1Normal.X * plane2Normal.Y * plane3Normal.Y * plane3Origin.Y - plane3Normal.X * plane2Normal.Y * plane1Normal.Z * plane1Origin.Z + plane2Normal.X * plane3Normal.Y * plane1Normal.Z * plane1Origin.Z + plane3Normal.X * plane1Normal.Y * plane2Normal.Z * plane2Origin.Z - plane1Normal.X * plane3Normal.Y * plane2Normal.Z * plane2Origin.Z - plane2Normal.X * plane1Normal.Y * plane3Normal.Z * plane3Origin.Z + plane1Normal.X * plane2Normal.Y * plane3Normal.Z * plane3Origin.Z) / (plane3Normal.X * plane2Normal.Y * plane1Normal.Z - plane2Normal.X * plane3Normal.Y * plane1Normal.Z - plane3Normal.X * plane1Normal.Y * plane2Normal.Z + plane1Normal.X * plane3Normal.Y * plane2Normal.Z + plane2Normal.X * plane1Normal.Y * plane3Normal.Z - plane1Normal.X * plane2Normal.Y * plane3Normal.Z));

            Point3d intersectionPoint3d = new Point3d(x, y, z);

            return intersectionPoint3d;
        }
    }
}
