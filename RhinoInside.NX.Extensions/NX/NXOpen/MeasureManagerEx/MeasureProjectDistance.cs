using NXOpen.Assemblies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen.UF;
using NXOpen;
using static RhinoInside.NX.Extensions.Globals;

namespace RhinoInside.NX.Extensions
{
    /// <summary>
    /// NXOpen.MeasureManager 的扩展类
    /// </summary>
    public static partial class MeasureManagerEx
    {
        /// <summary>
        /// 测量两个对象之间的投影距离
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="object1">对象1</param>
        /// <param name="object2">对象2</param>
        /// <param name="projectVector">投影矢量</param>
        /// <param name="type">测量类型</param>
        /// <returns>投影距离值</returns>
        public static double MeasureProjectDistance(this MeasureManager measureManager, DisplayableObject object1, DisplayableObject object2, Vector3d projectVector, MeasureManager.ProjectionType type)
        {
            NXOpen.Unit lengthUnit = WorkPart.UnitCollection.FindObject("MilliMeter");

            Direction direction1 = WorkPart.Directions.CreateDirection(new Point3d(), projectVector, NXOpen.SmartObject.UpdateOption.WithinModeling);

            var projectDistance = measureManager.NewDistance(lengthUnit, object1, object2, direction1, type);

            return projectDistance.Value;
        }

        /// <summary>
        /// 测量两个对象之间的投影距离，返回距离以及点信息，只能是当前工作部件中的对象，可用类型为 Point, Curve, Edge, Face, Body, Datum Plane. 
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="object1">对象1</param>
        /// <param name="object2">对象2</param>
        /// <param name="projectVector">投影矢量</param>
        /// <param name="type">测量类型</param>
        /// <returns>投影距离值</returns>
        public static MeasureResult MeasureProjectDistanceWithDetail(this MeasureManager measureManager, DisplayableObject object1, DisplayableObject object2, Vector3d projectVector, MeasureManager.ProjectionType type)
        {
            var measure = MeasureProjectDistance(measureManager, new NXObject[] { object1 }, new NXObject[] { object2 }, projectVector, type);

            return new MeasureResult(measure.Success, measure.Distance, measure.PointOn1stObjects, measure.PointOn2ndObjects);
        }

        /// <summary>
        /// 测量两个对象之间的投影距离，返回距离以及点信息，只能是当前工作部件中的对象，可用类型为 Point, Curve, Edge, Face, Body, Datum Plane. 
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="objects1"></param>
        /// <param name="objects2"></param>
        /// <param name="projectVector">投影矢量</param>
        /// <param name="type">测量类型</param>
        /// <returns>投影距离值</returns>
        public static MeasureResult MeasureProjectDistanceWithDetail(this MeasureManager measureManager, DisplayableObject[] objects1, DisplayableObject[] objects2, Vector3d projectVector, MeasureManager.ProjectionType type)
        {
            var measure = MeasureProjectDistance(measureManager, objects1, objects2, projectVector, type);

            return new MeasureResult(measure.Success, measure.Distance, measure.PointOn1stObjects, measure.PointOn2ndObjects);
        }

        /// <summary>
        /// 测量点到对象之间的投影距离
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="point"></param>
        /// <param name="object">对象2</param>
        /// <param name="projectVector">投影矢量</param>
        /// <param name="type">测量类型</param>
        /// <returns>投影距离值</returns>
        public static double MeasureProjectDistance(this MeasureManager measureManager, Point3d point, DisplayableObject @object, Vector3d projectVector, MeasureManager.ProjectionType type)
        {
            var tempPt = WorkPart.Points.CreatePoint(point);

            double distance = measureManager.MeasureProjectDistance(tempPt, @object, projectVector, type);

            tempPt.Delete();

            return distance;
        }

        /// <summary>
        /// 测量两个对象集之间的投影距离，只能是当前工作部件中的对象，可用类型为 Point, Curve, Edge, Face, Body, Datum Plane. 
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="objects1"></param>
        /// <param name="objects2"></param>
        /// <param name="projectVector">投影矢量</param>
        /// <param name="type">测量类型</param>
        /// <returns>投影距离值</returns>
        public static (bool Success, double Distance, Point3d PointOn1stObjects, Point3d PointOn2ndObjects) MeasureProjectDistance(this MeasureManager measureManager, NXObject[] objects1, NXObject[] objects2, Vector3d projectVector, MeasureManager.ProjectionType type)
        {
            try
            {
                NXOpen.Unit lengthUnit = WorkPart.UnitCollection.FindObject("MilliMeter");

                Direction direction = WorkPart.Directions.CreateDirection(new Point3d(), projectVector, NXOpen.SmartObject.UpdateOption.WithinModeling);

                MeasureDistance minimumMeasure = measureManager.NewScDistance(lengthUnit, CreateCollector(objects1), CreateCollector(objects2), direction, type, true);

                Measure measureFeature = minimumMeasure.CreateFeature();

                var measureLine = measureFeature.GetEntities()[0] as Line;

                var minimumDistance = measureLine.GetLength();

                Point3d pointOn1stObjects = measureLine.StartPoint;

                Point3d pointOn2ndObjects = measureLine.EndPoint;

                measureFeature.Delete();

                return (true, minimumDistance, pointOn1stObjects, pointOn2ndObjects);
            }
            catch (Exception)
            {
                return (false, 0, new Point3d(), new Point3d());
            }
        }

        /// <summary>
        /// 测量点到平面的距离
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="point">要测量的点</param>
        /// <param name="pointOnPlane">平面上的点</param>
        /// <param name="projectVector">平面法向</param>
        /// <returns>距离值，若点位于投影方向一侧，则值为正，反之为负</returns>
        public static double MeasureProjectDistance(this MeasureManager measureManager, Point3d point, Point3d pointOnPlane, Vector3d projectVector)
        {
            TheUfSession.Vec3.DistanceToPlane(point.ToArray(), pointOnPlane.ToArray(), projectVector.ToArray(), 0.01, out double distance);

            return distance;
        }
    }
}
