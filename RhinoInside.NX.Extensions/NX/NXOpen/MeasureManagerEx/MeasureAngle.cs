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
    /// NXOpen.MeasureManager 的扩展类
    /// </summary>
    public static partial class MeasureManagerEx
    {
        /// <summary>
        /// 根据三点测量角度
        /// </summary>
        /// <param name="theMeasureManager"></param>
        /// <param name="basePoint3d">基点</param>
        /// <param name="baseLineEndPoint3d">基线的终点</param>
        /// <param name="protractorEndPoint3d">量角器的终点</param>
        /// <returns></returns>
        public static double MeasureAngle(this MeasureManager theMeasureManager, Point3d basePoint3d, Point3d baseLineEndPoint3d, Point3d protractorEndPoint3d)
        {
            Unit degreeUnit = WorkPart.UnitCollection.FindObject("Degrees");

            var basePoint = WorkPart.Points.CreatePoint(basePoint3d);

            var baseEnd = WorkPart.Points.CreatePoint(baseLineEndPoint3d);

            var protractorEnd = WorkPart.Points.CreatePoint(protractorEndPoint3d);


            var measurement = WorkPart.MeasureManager.NewAngle(degreeUnit, basePoint, baseEnd, protractorEnd, true);

            return measurement.Value;
        }

        /// <summary>
        /// 根据矢量测量角度
        /// </summary>
        /// <param name="theMeasureManager"></param>
        /// <param name="firstVector">测量出发矢量</param>
        /// <param name="secondVector">测量目标矢量</param>
        /// <param name="counterClockWiseOrientation">确定测量的正方向</param>
        /// <returns>角度值，若结果不是想要的那个角，尝试交换两个参数</returns>
        public static double MeasureAngle(this MeasureManager theMeasureManager, Vector3d firstVector, Vector3d secondVector, Vector3d counterClockWiseOrientation)
        {
            TheUfSession.Vec3.AngleBetween(firstVector.ToArray(), secondVector.ToArray(), counterClockWiseOrientation.ToArray(), out var angle);

            return angle * 180.0 / Math.PI;
        }
    }
}
