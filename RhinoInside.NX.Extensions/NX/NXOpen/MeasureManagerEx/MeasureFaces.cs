using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RhinoInside.NX.Extensions.Globals;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
    public static partial class MeasureManagerEx
    {
        /// <summary>
        /// 测量面的属性
        /// </summary>
        /// <param name="theMeasureManaer"></param>
        /// <param name="face"></param>
        /// <param name="tolerance">测量公差，越接近1.0，结果越精确，但同时可能会花费较长时间</param>
        /// <returns></returns>
        public static MeasureFaces MeasureFace(this MeasureManager theMeasureManaer, Face face, double tolerance = 0.999) => theMeasureManaer.MeasureFace(new Face[] { face }, tolerance);

        /// <summary>
        /// 测量面的属性
        /// </summary>
        /// <param name="theMeasureManaer"></param>
        /// <param name="faces"></param>
        /// <param name="tolerance">测量公差，越接近1.0，结果越精确，但同时可能会花费较长时间</param>
        /// <returns></returns>
        public static MeasureFaces MeasureFace(this MeasureManager theMeasureManaer, Face[] faces, double tolerance = 0.999)
        {
            NXOpen.Unit areaUnit = WorkPart.UnitCollection.FindObject("SquareMilliMeter");

            NXOpen.Unit lengthUnit = WorkPart.UnitCollection.FindObject("MilliMeter");

            MeasureFaces measureFace = WorkPart.MeasureManager.NewFaceProperties(areaUnit, lengthUnit, tolerance, faces);

            return measureFace;
        }
    }
}
