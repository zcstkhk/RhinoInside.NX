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
        /// 求两个点集合之间的距离，使用遍历的方式
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="points1"></param>
        /// <param name="points2"></param>
        /// <returns></returns>
        public static (double Distance, Point3d PointOn1stObject, Point3d PointOn2ndObject) MeasureDistance(this MeasureManager measureManager, List<Point3d> points1, List<Point3d> points2)
        {
            Point3d p1 = default(Point3d);
            Point3d p2 = default(Point3d);
            double distance = double.MaxValue;
            for (int i = 0; i < points1.Count; i++)
            {
                for (int j = 0; j < points2.Count; j++)
                {
                    double tempDist = points1[i].DistanceTo(points2[j]);

                    if (tempDist < distance)
                    {
                        distance = tempDist;
                        p1 = points1[i];
                        p2 = points2[j];
                    }
                }
            }
            return (distance, p1, p2);
        }

        /// <summary>
        /// 计算两个 Point3d 对象之间的数学距离
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="point1">第一个点坐标</param>
        /// <param name="point2">第二个点坐标</param>
        /// <returns></returns>
        public static double MeasureDistance(this MeasureManager measureManager, Point3d point1, Point3d point2)
        {
            double[] pnt1 = new double[] { point1.X, point1.Y, point1.Z };
            double[] pnt2 = new double[] { point2.X, point2.Y, point2.Z };
            double distance;

            TheUfSession.Vec3.Distance(pnt1, pnt2, out distance);
            return distance;
        }

        /// <summary>
        /// 使用 UFUN 测量两个对象间的最小距离
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="object1">对象1</param>
        /// <param name="object2">对象2</param>
        /// <returns></returns>
        public static (double Distance, Point3d PointOn1stObject, Point3d Pointon2ndObject) MeasureDistance(this MeasureManager measureManager, Tag object1, Tag object2)
        {
            double[] pt_on_obj1 = new double[3];
            double[] pt_on_obj2 = new double[3];
            TheUfSession.Modl.AskMinimumDist3(3, object1, object2, 0, new double[3], 0, new double[3], out double distance, pt_on_obj1, pt_on_obj2, out double accuracy);

            return (distance, pt_on_obj1.ToPoint3d(), pt_on_obj2.ToPoint3d());
        }

        /// <summary>
        /// 使用 UFUN 测量点到给定对象间的最小距离
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="point">要测量的点</param>
        /// <param name="objectToMeasure">对象2</param>
        /// <returns></returns>
        public static (double Distance, Point3d PointOn2ndObject) MeasureDistance(this MeasureManager measureManager, Point3d point, Tag objectToMeasure)
        {
            double[] pt_on_obj1 = new double[3];
            double[] pt_on_obj2 = new double[3];
            TheUfSession.Modl.AskMinimumDist3(3, Tag.Null, objectToMeasure, 1, point.ToArray(), 0, new double[3], out double minimumDistance, pt_on_obj1, pt_on_obj2, out double accuracy);

            return (minimumDistance, pt_on_obj2.ToPoint3d());
        }

        /// <summary>
        /// 测量点到一组对象之间的最短距离，只能是当前工作部件中的对象，可用类型为 Point, Curve, Edge, Face, Body, Datum Plane. 
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="point">出发点</param>
        /// <param name="objects"></param>
        /// <returns>距离值</returns>
        public static (bool Success, double Distance, Point3d PointOnObjects) MeasureDistance(this MeasureManager measureManager, Point3d point, NXObject[] objects)
        {
            try
            {
#if NX12
                NXOpen.CurveDumbRule curveDumbRule1 = (WorkPart as BasePart).ScRuleFactory.CreateRuleCurveDumbFromPoints(new Point[] { WorkPart.Points.CreatePoint(point) });
#else
                NXOpen.CurveDumbRule curveDumbRule1 = WorkPart.ScRuleFactory.CreateRuleCurveDumbFromPoints(new Point[] { WorkPart.Points.CreatePoint(point) });
#endif
                ScCollector scCollector1 = WorkPart.ScCollectors.CreateCollector();

                scCollector1.ReplaceRules(new SelectionIntentRule[] { curveDumbRule1 }, false);

                ScCollector scCollector2 = CreateCollector(objects);

                NXOpen.Unit lengthUnit = WorkPart.UnitCollection.FindObject("MilliMeter");

                var minimumMeasure = measureManager.NewScDistance(lengthUnit, MeasureManager.MeasureType.Minimum, true, scCollector1, scCollector2);

                var measureFeature = minimumMeasure.CreateFeature();

                var measureLine = measureFeature.GetEntities()[0] as Line;

                var minimumDistance = measureLine.GetLength();

                Point3d pointOnObjects = measureLine.EndPoint;

                measureFeature.Delete();

                return (true, minimumDistance, pointOnObjects);
            }
            catch (Exception)
            {
                return (false, 0, new Point3d());
            }

        }

        /// <summary>
        /// 测量两个对象之间的投影距离
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="object1">对象1</param>
        /// <param name="object2">对象2</param>
        /// <param name="projectVector">投影矢量</param>
        /// <param name="type">测量类型</param>
        /// <returns>投影距离值</returns>
        [Obsolete("2020-09-28,请使用 MeasureProjectDistance")]
        public static double MeasureDistance(this MeasureManager measureManager, DisplayableObject object1, DisplayableObject object2, Vector3d projectVector, MeasureManager.ProjectionType type) => measureManager.MeasureProjectDistance(object1, object2, projectVector, type);

        /// <summary>
        /// 测量点到对象之间的投影距离
        /// </summary>
        /// <param name="theMeasureManager"></param>
        /// <param name="point"></param>
        /// <param name="object">对象2</param>
        /// <param name="projectVector">投影矢量</param>
        /// <param name="type">测量类型</param>
        /// <returns>投影距离值</returns>
        [Obsolete("2020-09-28，请使用MeasureProjectDistance")]
        public static double MeasureDistance(this MeasureManager theMeasureManager, Point3d point, DisplayableObject @object, Vector3d projectVector, MeasureManager.ProjectionType type) => theMeasureManager.MeasureProjectDistance(point, @object, projectVector, type);

        /// <summary>
        /// 测量两个对象集之间的最短距离，只能是当前工作部件中的对象，可用类型为 Point, Curve, Edge, Face, Body, Datum Plane. 
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="objects1"></param>
        /// <param name="objects2"></param>
        /// <returns>投影距离值</returns>
        public static (bool Success, double Distance, Point3d PointOn1stObjects, Point3d PointOn2ndObjects) MeasureDistance(this MeasureManager measureManager, NXObject[] objects1, NXObject[] objects2)
        {
            try
            {
                NXOpen.Unit lengthUnit = WorkPart.UnitCollection.FindObject("MilliMeter");

                var minimumMeasure = measureManager.NewScDistance(lengthUnit, MeasureManager.MeasureType.Minimum, true, CreateCollector(objects1), CreateCollector(objects2));

                var measureFeature = minimumMeasure.CreateFeature();

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
        /// 测量两个对象集之间的投影距离，只能是当前工作部件中的对象，可用类型为 Point, Curve, Edge, Face, Body, Datum Plane. 
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="objects1"></param>
        /// <param name="objects2"></param>
        /// <param name="projectVector">投影矢量</param>
        /// <param name="type">测量类型</param>
        /// <returns>投影距离值</returns>
        [Obsolete("2020-09-28，请使用MeasureProjectDistance")]
        public static (bool Success, double Distance, Point3d PointOn1stObjects, Point3d PointOn2ndObjects) MeasureDistance(this MeasureManager measureManager, NXObject[] objects1, NXObject[] objects2, Vector3d projectVector, MeasureManager.ProjectionType type) => measureManager.MeasureProjectDistance(objects1, objects2, projectVector, type);

        /// <summary>
        /// 测量两个组件集之间的距离
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="components1">组件集1</param>
        /// <param name="components2">组件集2</param>
        /// <returns>距离值</returns>
        public static double MeasureDistance(this MeasureManager measureManager, Component[] components1, Component[] components2)
        {
            NXOpen.Unit lengthUnit = WorkPart.UnitCollection.FindObject("MilliMeter");

            ComponentGroup group1 = WorkPart.ComponentGroups.CreateComponentGroup(System.DateTime.Now.ToLongTimeString() + "_1");
            for (int i = 0; i < components1.Length; i++)
            {
                group1.AddComponent(components1[i], false);
            }

            ComponentGroup group2 = WorkPart.ComponentGroups.CreateComponentGroup(System.DateTime.Now.ToLongTimeString() + "_2");
            for (int i = 0; i < components2.Length; i++)
            {
                group2.AddComponent(components2[i], false);
            }

            double distanceValue = measureManager.NewDistance(lengthUnit, group1, group2).Value;

            group1.Delete();
            group2.Delete();

            return distanceValue;
        }

        /// <summary>
        /// 测量两个组件之间的距离
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="component1">组件1</param>
        /// <param name="component2">组件2</param>
        /// <returns>距离值</returns>
        public static double MeasureDistance(this MeasureManager measureManager, Component component1, Component component2)
        {
            NXOpen.Unit lengthUnit = WorkPart.UnitCollection.FindObject("MilliMeter");

            double distanceValue = measureManager.NewDistance(lengthUnit, component1, component2).Value;

            return distanceValue;
        }

        /// <summary>
        /// 使用 NXOpen 测量两个对象之间的距离
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="object1">对象1</param>
        /// <param name="object2">对象2</param>
        /// <returns>距离值</returns>
        public static (bool Success, double Distance, Point3d PointOn1stObject, Point3d PointOn2ndObject) MeasureDistance(this MeasureManager measureManager, DisplayableObject object1, DisplayableObject object2)
        {
            try
            {
                NXOpen.Unit lengthUnit = WorkPart.UnitCollection.FindObject("MilliMeter");

                MeasureDistance distance = measureManager.NewDistance(lengthUnit, MeasureManager.MeasureType.Minimum, true, object1, object2);

                var measureFeature = distance.CreateFeature();

                var measureLine = measureFeature.GetEntities()[0] as Line;

                var minimumDistance = measureLine.GetLength();

                Point3d pointOn1stObject = measureLine.StartPoint;

                Point3d pointOn2ndObject = measureLine.EndPoint;

                measureFeature.Delete();

                return (true, minimumDistance, pointOn1stObject, pointOn2ndObject);
            }
            catch (Exception)
            {
                return (false, 0, new Point3d(), new Point3d());
            }
        }

        private static ScCollector CreateCollector(NXObject[] objects)
        {
            List<SelectionIntentRule> selectionIntentRules = new List<SelectionIntentRule>();

            var baseCurvesInCurrenObjects = objects.Where(obj => obj is IBaseCurve).Select(obj => obj as IBaseCurve).ToArray();
            if (baseCurvesInCurrenObjects.Length != 0)
            {
#if NX12
                NXOpen.CurveDumbRule curveDumbRule2 = (WorkPart as BasePart).ScRuleFactory.CreateRuleBaseCurveDumb(baseCurvesInCurrenObjects);
                selectionIntentRules.Add(curveDumbRule2);
#else
                NXOpen.CurveDumbRule curveDumbRule2 = WorkPart.ScRuleFactory.CreateRuleBaseCurveDumb(baseCurvesInCurrenObjects);
                selectionIntentRules.Add(curveDumbRule2);
#endif
            }

            var bodiesInCurrenObjects = objects.Where(obj => obj is Body).Select(obj => obj as Body).ToArray();
            if (bodiesInCurrenObjects.Length != 0)
            {
#if NX12
                var bodyDumbRule = (WorkPart as BasePart).ScRuleFactory.CreateRuleBodyDumb(bodiesInCurrenObjects);
                selectionIntentRules.Add(bodyDumbRule);
#else
                var bodyDumbRule = WorkPart.ScRuleFactory.CreateRuleBodyDumb(bodiesInCurrenObjects);
                selectionIntentRules.Add(bodyDumbRule);
#endif
            }

            var facesInCurrenObjects = objects.Where(obj => obj is Face).Select(obj => obj as Face).ToArray();
            if (facesInCurrenObjects.Length != 0)
            {
#if NX12
                var faceDumbRule = (WorkPart as BasePart).ScRuleFactory.CreateRuleFaceDumb(facesInCurrenObjects);
                selectionIntentRules.Add(faceDumbRule);
#else
                var faceDumbRule = WorkPart.ScRuleFactory.CreateRuleFaceDumb(facesInCurrenObjects);
                selectionIntentRules.Add(faceDumbRule);
#endif
            }

            var edgesInCurrenObjects = objects.Where(obj => obj is Edge).Select(obj => obj as Edge).ToArray();
            if (edgesInCurrenObjects.Length != 0)
            {
#if NX12
                var edgeDumbRule = (WorkPart as BasePart).ScRuleFactory.CreateRuleEdgeDumb(edgesInCurrenObjects);
                selectionIntentRules.Add(edgeDumbRule);
#else
                var edgeDumbRule = WorkPart.ScRuleFactory.CreateRuleEdgeDumb(edgesInCurrenObjects);
                selectionIntentRules.Add(edgeDumbRule);
#endif
            }

            var pointsInCurrenObjects = objects.Where(obj => obj is Point).Select(obj => obj as Point).ToArray();
            if (pointsInCurrenObjects.Length != 0)
            {
#if NX12
                var pointDumbRule = (WorkPart as BasePart).ScRuleFactory.CreateRuleCurveDumbFromPoints(pointsInCurrenObjects);
#else
                var pointDumbRule = WorkPart.ScRuleFactory.CreateRuleCurveDumbFromPoints(pointsInCurrenObjects);
#endif
                selectionIntentRules.Add(pointDumbRule);
            }

            var datumplanesInCurrenObjects = objects.Where(obj => obj is DatumPlane).Select(obj => obj as DatumPlane).ToArray();
            if (datumplanesInCurrenObjects.Length != 0)
            {
#if NX12
                var datumDumbRule = (WorkPart as BasePart).ScRuleFactory.CreateRuleFaceDatum(datumplanesInCurrenObjects);
                selectionIntentRules.Add(datumDumbRule);
#else
                var datumDumbRule = WorkPart.ScRuleFactory.CreateRuleFaceDatum(datumplanesInCurrenObjects);
                selectionIntentRules.Add(datumDumbRule);
#endif

            }

            var collector = WorkPart.ScCollectors.CreateCollector();

            collector.ReplaceRules(selectionIntentRules.ToArray(), false);

            return collector;
        }

        /// <summary>
        /// 测量点到平面的距离
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="point">要测量的点</param>
        /// <param name="pointOnPlane">平面上的点</param>
        /// <param name="planeNormal">平面法向</param>
        /// <returns>距离值，若点位于平面法向一侧，则值为正，反之为负</returns>
        public static double MeasureDistance(this MeasureManager measureManager, Point3d point, Point3d pointOnPlane, Vector3d planeNormal)
        {
            TheUfSession.Vec3.DistanceToPlane(point.ToArray(), pointOnPlane.ToArray(), planeNormal.ToArray(), 0.01, out double distance);

            return distance;
        }

        public struct MeasureResult
        {
            public bool Success;
            public double Distance;
            public Point3d PointOn1stObject;
            public Point3d PointOn2ndObject;

            public MeasureResult(bool success, double distance, Point3d firstPoint, Point3d secondPoint)
            {
                Success = success;
                Distance = distance;
                PointOn1stObject = firstPoint;
                PointOn2ndObject = secondPoint;
            }

            public MeasureResult(double distance, Point3d firstPoint, Point3d secondPoint)
            {
                Success = true;
                Distance = distance;
                PointOn1stObject = firstPoint;
                PointOn2ndObject = secondPoint;
            }
        }
    }
}
