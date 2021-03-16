using NXOpen.Features;
using NXOpen.UF;
using System;
using NXOpen;
using static RhinoInside.NX.Extensions.Globals;

namespace RhinoInside.NX.Extensions
{
	public static partial class FeatureCollectionEx
	{
        /// <summary>
        /// 沿面的法向投影曲线
        /// </summary>
        /// <param name="features"></param>
        /// <param name="curves"></param>
        /// <param name="faces_to_project_to"></param>
        /// <returns></returns>
        public static ProjectCurve CreateProjectCurve(this FeatureCollection features, IBaseCurve[] curves, Face[] faces_to_project_to)
		{
			NXOpen.Features.ProjectCurveBuilder projectCurveBuilder = features.CreateProjectCurveBuilder(null);

			projectCurveBuilder.CurveFitData.Tolerance = 0.01;

			projectCurveBuilder.CurveFitData.AngleTolerance = 0.5;

			projectCurveBuilder.ProjectionDirectionMethod = NXOpen.Features.ProjectCurveBuilder.DirectionType.AlongFaceNormal;

			projectCurveBuilder.SectionToProject.DistanceTolerance = 0.01;

			projectCurveBuilder.SectionToProject.ChainingTolerance = 0.0094999999999999998;

			projectCurveBuilder.SectionToProject.AngleTolerance = 0.5;

			projectCurveBuilder.SectionToProject.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.CurvesAndPoints);

#if NX12
			NXOpen.CurveDumbRule curveDumbRule = (WorkPart as BasePart).ScRuleFactory.CreateRuleBaseCurveDumb(curves);
#else
			NXOpen.CurveDumbRule curveDumbRule = WorkPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves);
#endif
			projectCurveBuilder.SectionToProject.AllowSelfIntersection(true);

			NXOpen.SelectionIntentRule[] rules1 = new NXOpen.SelectionIntentRule[1] { curveDumbRule };
			projectCurveBuilder.SectionToProject.AddToSection(rules1, null, null, null, new Point3d(), NXOpen.Section.Mode.Create, false);

			NXOpen.ScCollector scCollector1 = WorkPart.ScCollectors.CreateCollector();
#if NX12
			NXOpen.FaceDumbRule faceDumbRule = (WorkPart as BasePart).ScRuleFactory.CreateRuleFaceDumb(faces_to_project_to);
#else
			NXOpen.FaceDumbRule faceDumbRule = WorkPart.ScRuleFactory.CreateRuleFaceDumb(faces_to_project_to);
#endif
			NXOpen.SelectionIntentRule[] rules2 = new NXOpen.SelectionIntentRule[1] { faceDumbRule };
			scCollector1.ReplaceRules(rules2, false);

			bool added1 = projectCurveBuilder.FaceToProjectTo.Add(scCollector1);

			ProjectCurve nXObject1 = (ProjectCurve)projectCurveBuilder.Commit();

			projectCurveBuilder.SectionToProject.CleanMappingData();

			projectCurveBuilder.Destroy();

			return nXObject1;
		}

        /// <summary>
        /// 投影曲线
        /// </summary>
        /// <param name="features"></param>
        /// <param name="curves">要投影的曲线</param>
        /// <param name="originPoint">投影平面原点</param>
        /// <param name="normal">投影方向</param>
        /// <returns></returns>
        public static ProjectCurve CreateProjectCurve(this FeatureCollection features, IBaseCurve[] curves, Point3d originPoint, Vector3d normal)
		{
			NXOpen.Features.ProjectCurveBuilder projectCurveBuilder = features.CreateProjectCurveBuilder(null);
			Plane plane1 = WorkPart.Planes.CreatePlane(originPoint, normal, NXOpen.SmartObject.UpdateOption.WithinModeling);
			plane1.SetMethod(NXOpen.PlaneTypes.MethodType.Distance);
			plane1.SetFlip(false);
			plane1.SetReverseSide(false);
			plane1.SetAlternate(NXOpen.PlaneTypes.AlternateType.One);
			plane1.Evaluate();
			projectCurveBuilder.PlaneToProjectTo = plane1;
			projectCurveBuilder.CurveFitData.Tolerance = 0.025;
			projectCurveBuilder.CurveFitData.AngleTolerance = 0.5;
			projectCurveBuilder.ProjectionOption = NXOpen.Features.ProjectCurveBuilder.ProjectionOptionType.ProjectBothSides;
			projectCurveBuilder.AngleToProjectionVector.RightHandSide = "0";
			projectCurveBuilder.SectionToProject.DistanceTolerance = 0.025;
			projectCurveBuilder.SectionToProject.ChainingTolerance = 0.02375;
			projectCurveBuilder.SectionToProject.AngleTolerance = 0.5;
			projectCurveBuilder.SectionToProject.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.CurvesAndPoints);
#if NX12
			CurveDumbRule curveFeatureRule1 = (WorkPart as BasePart).ScRuleFactory.CreateRuleBaseCurveDumb(curves);
#else
			CurveDumbRule curveFeatureRule1 = WorkPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves);
#endif
			projectCurveBuilder.SectionToProject.AllowSelfIntersection(true);
			SelectionIntentRule[] rules1 = new SelectionIntentRule[1] { curveFeatureRule1 };
			Point3d helpPoint1 = new Point3d(1614.13079321387, -713.841520793069, 769.877304064383);
			projectCurveBuilder.SectionToProject.AddToSection(rules1, (NXObject)curves[0], null, null, helpPoint1, NXOpen.Section.Mode.Create, false);
			ProjectCurve projectCurve = (ProjectCurve)projectCurveBuilder.CommitFeature();
			projectCurveBuilder.SectionToProject.CleanMappingData();
			projectCurveBuilder.Destroy();
			return projectCurve;
		}

        /// <summary>
        /// 投影点
        /// </summary>
        /// <param name="features"></param>
        /// <param name="point_to_project">要投影的点</param>
        /// <param name="plane_to_project_to">投影到的平面</param>
        public static ProjectCurve CreateProjectCurve(this FeatureCollection features, Point point_to_project, Plane plane_to_project_to)
		{
			NXOpen.Features.ProjectCurveBuilder projectCurveBuilder = features.CreateProjectCurveBuilder(null);

			projectCurveBuilder.CurveFitData.Tolerance = 0.025399999999999999;

			projectCurveBuilder.CurveFitData.AngleTolerance = 0.5;

			projectCurveBuilder.ProjectionDirectionMethod = NXOpen.Features.ProjectCurveBuilder.DirectionType.AlongFaceNormal;

			projectCurveBuilder.AngleToProjectionVector.RightHandSide = "0";

			NXOpen.Direction direction1 = WorkPart.Directions.CreateDirection(plane_to_project_to.Origin, plane_to_project_to.Normal, NXOpen.SmartObject.UpdateOption.WithinModeling);

			projectCurveBuilder.ProjectionVector = direction1;

			projectCurveBuilder.SectionToProject.DistanceTolerance = 0.025399999999999999;

			projectCurveBuilder.SectionToProject.ChainingTolerance = 0.024129999999999999;

			projectCurveBuilder.SectionToProject.AngleTolerance = 0.5;

			projectCurveBuilder.PlaneToProjectTo = plane_to_project_to;

			projectCurveBuilder.SectionToProject.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.CurvesAndPoints);

#if NX12
			NXOpen.CurveDumbRule curveDumbRule1 = (WorkPart as BasePart).ScRuleFactory.CreateRuleCurveDumbFromPoints(new Point[] { point_to_project });
#else
			NXOpen.CurveDumbRule curveDumbRule1 = WorkPart.ScRuleFactory.CreateRuleCurveDumbFromPoints(new Point[] { point_to_project });
#endif
			projectCurveBuilder.SectionToProject.AllowSelfIntersection(true);

			NXOpen.SelectionIntentRule[] rules1 = new NXOpen.SelectionIntentRule[1] { curveDumbRule1 };
			projectCurveBuilder.SectionToProject.AddToSection(rules1, null, null, null, new Point3d(), NXOpen.Section.Mode.Create, false);

			ProjectCurve projectCurve = (ProjectCurve)projectCurveBuilder.Commit();

			projectCurveBuilder.SectionToProject.CleanMappingData();

			projectCurveBuilder.Destroy();

			return projectCurve;
		}

        /// <summary>
        /// 投影点
        /// </summary>
        /// <param name="features"></param>
        /// <param name="point_to_project">要投影的点</param>
        /// <param name="originPoint">投影平面原点</param>
        /// <param name="normal">投影平面方向</param>
        public static ProjectCurve CreateProjectCurve(this FeatureCollection features, Point point_to_project, Point3d originPoint, Vector3d normal)
		{
			NXOpen.Features.ProjectCurveBuilder projectCurveBuilder = features.CreateProjectCurveBuilder(null);

			projectCurveBuilder.CurveFitData.Tolerance = 0.025399999999999999;

			projectCurveBuilder.CurveFitData.AngleTolerance = 0.5;

			projectCurveBuilder.ProjectionDirectionMethod = NXOpen.Features.ProjectCurveBuilder.DirectionType.AlongFaceNormal;

			projectCurveBuilder.AngleToProjectionVector.RightHandSide = "0";

			Plane plane_to_project_to = WorkPart.Planes.CreatePlane(originPoint, normal, NXOpen.SmartObject.UpdateOption.WithinModeling);
			plane_to_project_to.SetMethod(NXOpen.PlaneTypes.MethodType.Distance);
			plane_to_project_to.SetFlip(false);
			plane_to_project_to.SetReverseSide(false);
			plane_to_project_to.SetAlternate(NXOpen.PlaneTypes.AlternateType.One);
			plane_to_project_to.Evaluate();

			projectCurveBuilder.SectionToProject.DistanceTolerance = 0.025399999999999999;

			projectCurveBuilder.SectionToProject.ChainingTolerance = 0.024129999999999999;

			projectCurveBuilder.SectionToProject.AngleTolerance = 0.5;

			projectCurveBuilder.PlaneToProjectTo = plane_to_project_to;

			projectCurveBuilder.SectionToProject.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.CurvesAndPoints);

#if NX12
			NXOpen.CurveDumbRule curveDumbRule = (WorkPart as BasePart).ScRuleFactory.CreateRuleCurveDumbFromPoints(new Point[] { point_to_project });
#else
			NXOpen.CurveDumbRule curveDumbRule = WorkPart.ScRuleFactory.CreateRuleCurveDumbFromPoints(new Point[] { point_to_project });
#endif
			projectCurveBuilder.SectionToProject.AllowSelfIntersection(true);

			NXOpen.SelectionIntentRule[] rules1 = new NXOpen.SelectionIntentRule[1] { curveDumbRule };
			projectCurveBuilder.SectionToProject.AddToSection(rules1, null, null, null, new Point3d(), NXOpen.Section.Mode.Create, false);

			ProjectCurve projectCurve = (ProjectCurve)projectCurveBuilder.Commit();

			projectCurveBuilder.SectionToProject.CleanMappingData();

			projectCurveBuilder.Destroy();

			return projectCurve;
		}

        /// <summary>
        /// 投影点
        /// </summary>
        /// <param name="features"></param>
        /// <param name="point_to_project">要投影的点</param>
        /// <param name="faces_to_project_to">投影到的面</param>
        /// <param name="project_direction3d">投影方向，正反向均可</param>
        /// <returns></returns>
        public static ProjectCurve CreateProjectCurve(this FeatureCollection features, Point point_to_project, Face[] faces_to_project_to, Vector3d project_direction3d)
		{
			NXOpen.Features.ProjectCurveBuilder projectCurveBuilder = features.CreateProjectCurveBuilder(null);

			projectCurveBuilder.CurveFitData.Tolerance = 0.01;

			projectCurveBuilder.CurveFitData.AngleTolerance = 0.5;

			projectCurveBuilder.ProjectionDirectionMethod = NXOpen.Features.ProjectCurveBuilder.DirectionType.AlongVector;

			projectCurveBuilder.AngleToProjectionVector.RightHandSide = "0";

			projectCurveBuilder.SectionToProject.DistanceTolerance = 0.01;

			projectCurveBuilder.SectionToProject.ChainingTolerance = 0.0095;

			projectCurveBuilder.SectionToProject.AngleTolerance = 0.5;

			projectCurveBuilder.SectionToProject.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.CurvesAndPoints);

#if NX12
			NXOpen.CurveDumbRule curveDumbRule1 = (WorkPart as BasePart).ScRuleFactory.CreateRuleCurveDumbFromPoints(new Point[] { point_to_project });
#else
			NXOpen.CurveDumbRule curveDumbRule1 = WorkPart.ScRuleFactory.CreateRuleCurveDumbFromPoints(new Point[] { point_to_project });
#endif

			projectCurveBuilder.SectionToProject.AllowSelfIntersection(true);

			NXOpen.SelectionIntentRule[] rules1 = new NXOpen.SelectionIntentRule[1] { curveDumbRule1 };
			projectCurveBuilder.SectionToProject.AddToSection(rules1, null, null, null, new Point3d(), NXOpen.Section.Mode.Create, false);

			NXOpen.ScCollector scCollector = WorkPart.ScCollectors.CreateCollector();

#if NX12
			NXOpen.FaceDumbRule faceDumbRule =( WorkPart as BasePart).ScRuleFactory.CreateRuleFaceDumb(faces_to_project_to);
#else
			NXOpen.FaceDumbRule faceDumbRule =WorkPart.ScRuleFactory.CreateRuleFaceDumb(faces_to_project_to);
#endif

			NXOpen.SelectionIntentRule[] rules2 = new NXOpen.SelectionIntentRule[1] { faceDumbRule };
			scCollector.ReplaceRules(rules2, false);

			bool added1 = projectCurveBuilder.FaceToProjectTo.Add(scCollector);

			NXOpen.Direction direction = WorkPart.Directions.CreateDirection(new Point3d(), project_direction3d, NXOpen.SmartObject.UpdateOption.WithinModeling);

			projectCurveBuilder.ProjectionVector = direction;

			projectCurveBuilder.ProjectionOption = ProjectCurveBuilder.ProjectionOptionType.ProjectBothSides;

			ProjectCurve nXObject = (ProjectCurve)projectCurveBuilder.Commit();

			projectCurveBuilder.SectionToProject.CleanMappingData();

			projectCurveBuilder.Destroy();

			return nXObject;
		}

        /// <summary>
        /// 计算点投影到平面上的坐标
        /// </summary>
        /// <param name="features"></param>
        /// <param name="pointToProject">要投影的点</param>
        /// <param name="planeOrigin">要投影到的平面上的点</param>
        /// <param name="planeNormal">平面法向</param>
        public static Point3d CreateProjectCurve(this FeatureCollection features, Point3d pointToProject, Point3d planeOrigin, Vector3d planeNormal)
		{
			double resultX = pointToProject.X - (planeNormal.X * (planeNormal.X * pointToProject.X - planeNormal.X * planeOrigin.X + planeNormal.Y * pointToProject.Y - planeNormal.Y * planeOrigin.Y + planeNormal.Z * pointToProject.Z - planeNormal.Z * planeOrigin.Z)) / (Math.Pow(planeNormal.X, 2) + Math.Pow(planeNormal.Y, 2) + Math.Pow(planeNormal.Z, 2));

			double resultY = pointToProject.Y - (planeNormal.Y * (planeNormal.X * pointToProject.X - planeNormal.X * planeOrigin.X + planeNormal.Y * pointToProject.Y - planeNormal.Y * planeOrigin.Y + planeNormal.Z * pointToProject.Z - planeNormal.Z * planeOrigin.Z)) / (Math.Pow(planeNormal.X, 2) + Math.Pow(planeNormal.Y, 2) + Math.Pow(planeNormal.Z, 2));

			double resultZ = pointToProject.Z - (planeNormal.Z * (planeNormal.X * pointToProject.X - planeNormal.X * planeOrigin.X + planeNormal.Y * pointToProject.Y - planeNormal.Y * planeOrigin.Y + planeNormal.Z * pointToProject.Z - planeNormal.Z * planeOrigin.Z)) / (Math.Pow(planeNormal.X, 2) + Math.Pow(planeNormal.Y, 2) + Math.Pow(planeNormal.Z, 2));

			return new Point3d(resultX, resultY, resultZ);
		}
	}	
}
