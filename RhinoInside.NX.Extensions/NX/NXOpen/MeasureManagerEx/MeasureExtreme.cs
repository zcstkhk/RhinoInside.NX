using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using static RhinoInside.NX.Extensions.Globals;

namespace RhinoInside.NX.Extensions
{
    public static partial class MeasureManagerEx
    {
        /// <summary>
        /// 获取极限点
        /// </summary>
        /// <returns></returns>
        public static Point3d MeasureExtreme(Body body, Vector3d vector) => Globals.WorkPart.MeasureManager.MeasureExtreme(body, vector);

        /// <summary>
        /// 获取极限点
        /// </summary>
        /// <param name="body"></param>
        /// <param name="measureDirection"></param>
        /// <param name="secondDirection"></param>
        /// <param name="thirdDirection"></param>
        /// <returns></returns>
        public static Point3d MeasureExtreme(Body body, Vector3d measureDirection, Vector3d secondDirection, Vector3d thirdDirection) => Globals.WorkPart.MeasureManager.MeasureExtreme(body, measureDirection, secondDirection, thirdDirection);

        /// <summary>
        /// 获取极限点
        /// </summary>
        /// <param name="bodies"></param>
        /// <param name="measureDirection"></param>
        /// <param name="secondDirection"></param>
        /// <param name="thirdDirection"></param>
        /// <returns></returns>
        public static Point3d MeasureExtreme(Body[] bodies, Vector3d measureDirection, Vector3d secondDirection, Vector3d thirdDirection) => Globals.WorkPart.MeasureManager.MeasureExtreme(bodies, measureDirection, secondDirection, thirdDirection);

        /// <summary>
        /// 获取极限点
        /// </summary>
        /// <param name="face">要进行计算的面</param>
        /// <param name="measureDirection">第一个方向</param>
        /// <param name="secondDirection">第二个方向</param>
        /// <param name="thirdDirection">第三个方向</param>
        /// <returns>结果点</returns>
        public static Point3d MeasureExtreme(Face face, Vector3d measureDirection, Vector3d secondDirection, Vector3d thirdDirection) => Globals.WorkPart.MeasureManager.MeasureExtreme(face, measureDirection, secondDirection, thirdDirection);

        /// <summary>
        /// 获取极限点
        /// </summary>
        /// <param name="curve">要计算的曲线</param>
        /// <param name="measureDirection">测量方向</param>
        /// <param name="secondDirection">第二方向，以防在第一个方向上出现多个极值点</param>
        /// <param name="thirdDirection">第三方向</param>
        /// <returns>极限点</returns>
        public static Point3d MeasureExtreme(IBaseCurve curve, Vector3d measureDirection, Vector3d secondDirection, Vector3d thirdDirection) => Globals.WorkPart.MeasureManager.MeasureExtreme(curve, measureDirection, secondDirection, thirdDirection);

        /// <summary>
        /// 获取极限点
        /// </summary>
        /// <param name="curves">要计算的曲线</param>
        /// <param name="measureDirection">测量方向</param>
        /// <param name="secondDirection">第二方向，以防在第一个方向上出现多个极值点</param>
        /// <param name="thirdDirection">第三方向</param>
        /// <returns>极限点</returns>
        public static Point3d MeasureExtreme(IBaseCurve[] curves, Vector3d measureDirection, Vector3d secondDirection, Vector3d thirdDirection) => Globals.WorkPart.MeasureManager.MeasureExtreme(curves, measureDirection, secondDirection, thirdDirection);
    }

    public static partial class Globals
    {
        /// <summary>
        /// 获取极限点，先创建一个较远的平面，然后测量与这个平面的距离
        /// </summary>
        /// <returns></returns>
        public static Point3d MeasureExtreme(this MeasureManager measureManager, Body body, Vector3d vector)
        {
            Plane normal_plane = WorkPart.Planes.CreatePlane(new Point3d(vector.X * 100.0, vector.Y * 100.0, vector.Z * 100.0), vector, SmartObject.UpdateOption.WithinModeling);
            normal_plane.SetVisibility(SmartObject.VisibilityOption.Visible);

            double min_dist;
            double[] pt_on_obj1 = new double[3];
            double[] pt_on_obj2 = new double[3];
            double accuracy;
            TheUfSession.Modl.AskMinimumDist3(2, body.Tag, normal_plane.Tag, 0, null, 0, null, out min_dist, pt_on_obj1, pt_on_obj2, out accuracy);

            normal_plane.Delete();

            return pt_on_obj1.ToPoint3d();
        }

        /// <summary>
        /// 获取极限点
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="body"></param>
        /// <param name="measureDirection"></param>
        /// <param name="secondDirection"></param>
        /// <param name="thirdDirection"></param>
        /// <returns></returns>
        public static Point3d MeasureExtreme(this MeasureManager measureManager, Body body, Vector3d measureDirection, Vector3d secondDirection, Vector3d thirdDirection)
        {
            NXOpen.Unit lengthUnit = WorkPart.UnitCollection.FindObject("MilliMeter");
#if NX12
            BodyDumbRule bodyDumbRule = (WorkPart as BasePart).ScRuleFactory.CreateRuleBodyDumb(new Body[] { body });
#else
            BodyDumbRule bodyDumbRule = WorkPart.ScRuleFactory.CreateRuleBodyDumb(new Body[] { body });
#endif
            ScCollector scCollector = WorkPart.ScCollectors.CreateCollector();
            scCollector.ReplaceRules(new SelectionIntentRule[] { bodyDumbRule }, false);
            Direction direction1 = WorkPart.Directions.CreateDirection(new Point3d(), measureDirection, NXOpen.SmartObject.UpdateOption.WithinModeling);
            Direction direction2 = WorkPart.Directions.CreateDirection(new Point3d(), secondDirection, NXOpen.SmartObject.UpdateOption.WithinModeling);
            Direction direction3 = WorkPart.Directions.CreateDirection(new Point3d(), thirdDirection, NXOpen.SmartObject.UpdateOption.WithinModeling);
            var bodiesExtreme = measureManager.NewRectangularExtreme(lengthUnit, direction1, direction2, direction3, scCollector, false);
            return bodiesExtreme.Point;
        }

        /// <summary>
        /// 获取极限点
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="bodies"></param>
        /// <param name="measureDirection"></param>
        /// <param name="secondDirection"></param>
        /// <param name="thirdDirection"></param>
        /// <returns></returns>
        public static Point3d MeasureExtreme(this MeasureManager measureManager, Body[] bodies, Vector3d measureDirection, Vector3d secondDirection, Vector3d thirdDirection)
        {
            NXOpen.Unit lengthUnit = WorkPart.UnitCollection.FindObject("MilliMeter");

#if NX12
            BodyDumbRule bodyDumbRule = (WorkPart as BasePart).ScRuleFactory.CreateRuleBodyDumb(bodies);
#else
            BodyDumbRule bodyDumbRule = WorkPart.ScRuleFactory.CreateRuleBodyDumb(bodies);
#endif

            ScCollector scCollector = WorkPart.ScCollectors.CreateCollector();
            scCollector.ReplaceRules(new SelectionIntentRule[] { bodyDumbRule }, false);
            Direction direction1 = WorkPart.Directions.CreateDirection(new Point3d(), measureDirection, NXOpen.SmartObject.UpdateOption.WithinModeling);
            Direction direction2 = WorkPart.Directions.CreateDirection(new Point3d(), secondDirection, NXOpen.SmartObject.UpdateOption.WithinModeling);
            Direction direction3 = WorkPart.Directions.CreateDirection(new Point3d(), thirdDirection, NXOpen.SmartObject.UpdateOption.WithinModeling);
            var bodiesExtreme = measureManager.NewRectangularExtreme(lengthUnit, direction1, direction2, direction3, scCollector, false);
            return bodiesExtreme.Point;
        }

        /// <summary>
        /// 获取极限点
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="face">要进行计算的面</param>
        /// <param name="measureDirection">第一个方向</param>
        /// <param name="secondDirection">第二个方向</param>
        /// <param name="thirdDirection">第三个方向</param>
        /// <returns>结果点</returns>
        public static Point3d MeasureExtreme(this MeasureManager measureManager, Face face, Vector3d measureDirection, Vector3d secondDirection, Vector3d thirdDirection) => MeasureExtreme(measureManager, new Face[] { face }, measureDirection, secondDirection, thirdDirection);

        /// <summary>
        /// 获取极限点
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="faces">要进行计算的面</param>
        /// <param name="measureDirection">第一个方向</param>
        /// <param name="secondDirection">第二个方向</param>
        /// <param name="thirdDirection">第三个方向</param>
        /// <returns>结果点</returns>
        public static Point3d MeasureExtreme(this MeasureManager measureManager, Face[] faces, Vector3d measureDirection, Vector3d secondDirection, Vector3d thirdDirection)
        {
            NXOpen.Unit lengthUnit = WorkPart.UnitCollection.FindObject("MilliMeter");
#if NX12
            FaceDumbRule faceDumbRule = (WorkPart as BasePart).ScRuleFactory.CreateRuleFaceDumb(faces);
#else
            FaceDumbRule faceDumbRule = WorkPart.ScRuleFactory.CreateRuleFaceDumb(faces);
#endif
            ScCollector scCollector = WorkPart.ScCollectors.CreateCollector();
            scCollector.ReplaceRules(new SelectionIntentRule[] { faceDumbRule }, false);
            Direction direction1 = WorkPart.Directions.CreateDirection(new Point3d(), measureDirection, NXOpen.SmartObject.UpdateOption.WithinModeling);
            Direction direction2 = WorkPart.Directions.CreateDirection(new Point3d(), secondDirection, NXOpen.SmartObject.UpdateOption.WithinModeling);
            Direction direction3 = WorkPart.Directions.CreateDirection(new Point3d(), thirdDirection, NXOpen.SmartObject.UpdateOption.WithinModeling);
            var facesExtreme = measureManager.NewRectangularExtreme(lengthUnit, direction1, direction2, direction3, scCollector, false);
            return facesExtreme.Point;
        }

        /// <summary>
        /// 获取极限点
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="curve"></param>
        /// <param name="measureDirection">测量方向</param>
        /// <param name="secondDirection">第二方向，以防在第一个方向上出现多个极值点</param>
        /// <param name="thirdDirection">第三方向</param>
        /// <returns>极限点</returns>
        public static Point3d MeasureExtreme(this MeasureManager measureManager, IBaseCurve curve, Vector3d measureDirection, Vector3d secondDirection, Vector3d thirdDirection) => MeasureExtreme(measureManager, new IBaseCurve[] { curve }, measureDirection, secondDirection, thirdDirection);

        /// <summary>
        /// 获取极限点
        /// </summary>
        /// <param name="measureManager"></param>
        /// <param name="curves">要计算的曲线</param>
        /// <param name="measureDirection">测量方向</param>
        /// <param name="secondDirection">第二方向，以防在第一个方向上出现多个极值点</param>
        /// <param name="thirdDirection">第三方向</param>
        /// <returns>极限点</returns>
        public static Point3d MeasureExtreme(this MeasureManager measureManager, IBaseCurve[] curves, Vector3d measureDirection, Vector3d secondDirection, Vector3d thirdDirection)
        {
            NXOpen.Unit unit1 = WorkPart.UnitCollection.FindObject("MilliMeter");
            Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
            Direction direction1 = WorkPart.Directions.CreateDirection(origin1, measureDirection, NXOpen.SmartObject.UpdateOption.WithinModeling);
            Direction direction2 = WorkPart.Directions.CreateDirection(origin1, secondDirection, NXOpen.SmartObject.UpdateOption.WithinModeling);
            Direction direction3 = WorkPart.Directions.CreateDirection(origin1, thirdDirection, NXOpen.SmartObject.UpdateOption.WithinModeling);
            ScCollector scCollector1;
            scCollector1 = WorkPart.ScCollectors.CreateCollector();

            CurveDumbRule curveDumbRule1 = WorkPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves);

            SelectionIntentRule[] rules1 = new SelectionIntentRule[1] { curveDumbRule1 };
            scCollector1.ReplaceRules(rules1, false);
            MeasureRectangularExtreme extreme = WorkPart.MeasureManager.NewRectangularExtreme(unit1, direction1, direction2, direction3, scCollector1, false);

            Point3d extremePoint = extreme.Point;

            int nErrs1 = TheSession.UpdateManager.AddToDeleteList(direction1);

            scCollector1.Destroy();

            return extremePoint;
        }
        

    }
}
