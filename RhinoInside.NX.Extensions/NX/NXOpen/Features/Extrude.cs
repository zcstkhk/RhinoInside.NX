using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen.Features;
using NXOpen;
using static RhinoInside.NX.Extensions.Globals;

namespace RhinoInside.NX.Extensions
{
	public static partial class FeatureCollectionEx
	{
        /// <summary>
        /// 创建拉伸片体
        /// </summary>
        /// <param name="features"></param>
        /// <param name="curves"></param>
        /// <param name="extrudeDirection"></param>
        /// <param name="startDistance"></param>
        /// <param name="endDistance"></param>
        /// <returns></returns>
        public static Extrude CreateExtrude(this FeatureCollection features, IBaseCurve[] curves, Vector3d extrudeDirection, double startDistance, double endDistance)
		{
			NXOpen.Features.ExtrudeBuilder extrudeBuilder = features.CreateExtrudeBuilder(null);

			Section section1 = WorkPart.Sections.CreateSection(0.0095, 0.01, 0.5);

			extrudeBuilder.Section = section1;

			extrudeBuilder.AllowSelfIntersectingSection(true);

			extrudeBuilder.BooleanOperation.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;

			extrudeBuilder.Offset.Option = NXOpen.GeometricUtilities.Type.NoOffset;

			extrudeBuilder.Draft.DraftOption = NXOpen.GeometricUtilities.SimpleDraft.SimpleDraftType.NoDraft;

			section1.DistanceTolerance = 0.01;

			section1.ChainingTolerance = 0.0095;

			section1.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.OnlyCurves);

			extrudeBuilder.Direction = WorkPart.Directions.CreateDirection(new Point3d(0.0, 0.0, 0.0), extrudeDirection, NXOpen.SmartObject.UpdateOption.WithinModeling);

#if NX12
			CurveDumbRule curveDumbRule = (WorkPart as BasePart).ScRuleFactory.CreateRuleBaseCurveDumb(curves);
#else
			CurveDumbRule curveDumbRule = WorkPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves);
#endif
			section1.AllowSelfIntersection(true);

			SelectionIntentRule[] rules1 = new SelectionIntentRule[1] { curveDumbRule };

			section1.AddToSection(rules1, (NXObject)curves[0], null, null, new Point3d(0, 0, 0), NXOpen.Section.Mode.Create, false);

			extrudeBuilder.Limits.StartExtend.TrimType = NXOpen.GeometricUtilities.Extend.ExtendType.Value;

			extrudeBuilder.Limits.EndExtend.TrimType = NXOpen.GeometricUtilities.Extend.ExtendType.Value;

			extrudeBuilder.Limits.EndExtend.Value.RightHandSide = endDistance.ToString();

			extrudeBuilder.Limits.StartExtend.Value.RightHandSide = startDistance.ToString();

			extrudeBuilder.FeatureOptions.BodyType = NXOpen.GeometricUtilities.FeatureOptions.BodyStyle.Sheet;

			extrudeBuilder.ParentFeatureInternal = false;

			Extrude extrude = (Extrude)extrudeBuilder.CommitFeature();
			extrudeBuilder.Destroy();
			return extrude;
		}

        /// <summary>
        /// 创建拉伸
        /// </summary>
        /// <param name="features"></param>
        /// <param name="curves"></param>
        /// <param name="extrudeDirection"></param>
        /// <param name="startDistance"></param>
        /// <param name="endDistance"></param>
        /// <param name="bodyStyle"></param>
        /// <returns></returns>
        public static Extrude CreateExtrude(this FeatureCollection features, IBaseCurve[] curves, Vector3d extrudeDirection, double startDistance, double endDistance, NXOpen.GeometricUtilities.FeatureOptions.BodyStyle bodyStyle)
		{
			NXOpen.Features.ExtrudeBuilder extrudeBuilder = features.CreateExtrudeBuilder(null);

			Section section1 = WorkPart.Sections.CreateSection(0.0095, 0.01, 0.5);

			extrudeBuilder.Section = section1;

			extrudeBuilder.AllowSelfIntersectingSection(true);

			extrudeBuilder.BooleanOperation.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;

			extrudeBuilder.Offset.Option = NXOpen.GeometricUtilities.Type.NoOffset;

			extrudeBuilder.Draft.DraftOption = NXOpen.GeometricUtilities.SimpleDraft.SimpleDraftType.NoDraft;

			extrudeBuilder.FeatureOptions.BodyType = bodyStyle;

			section1.DistanceTolerance = 0.01;

			section1.ChainingTolerance = 0.0095;

			section1.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.OnlyCurves);

			extrudeBuilder.Direction = WorkPart.Directions.CreateDirection(new Point3d(0.0, 0.0, 0.0), extrudeDirection, NXOpen.SmartObject.UpdateOption.WithinModeling);

#if NX12
			CurveDumbRule curveDumbRule = (WorkPart as BasePart).ScRuleFactory.CreateRuleBaseCurveDumb(curves);
#else
			CurveDumbRule curveDumbRule = WorkPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves);
#endif
			section1.AllowSelfIntersection(true);

			SelectionIntentRule[] rules1 = new SelectionIntentRule[1] { curveDumbRule };

			section1.AddToSection(rules1, (NXObject)curves[0], null, null, new Point3d(0, 0, 0), NXOpen.Section.Mode.Create, false);

			extrudeBuilder.Limits.StartExtend.TrimType = NXOpen.GeometricUtilities.Extend.ExtendType.Value;

			extrudeBuilder.Limits.EndExtend.TrimType = NXOpen.GeometricUtilities.Extend.ExtendType.Value;

			extrudeBuilder.Limits.EndExtend.Value.RightHandSide = endDistance.ToString();

			extrudeBuilder.Limits.StartExtend.Value.RightHandSide = startDistance.ToString();

			extrudeBuilder.ParentFeatureInternal = false;

			Extrude extrude = (Extrude)extrudeBuilder.CommitFeature();
			extrudeBuilder.Destroy();
			return extrude;
		}

		/// <summary>
		/// 创建单边偏置的拉伸
		/// </summary>
		/// <param name="features"></param>
		/// <param name="curves"></param>
		/// <param name="extrudeDirection"></param>
		/// <param name="startDistance"></param>
		/// <param name="endDistance"></param>
		/// <param name="bodyStyle"></param>
		/// <param name="offsetValue"></param>
		/// <returns></returns>
		public static Extrude CreateExtrude(this FeatureCollection features, IBaseCurve[] curves, Vector3d extrudeDirection, double startDistance, double endDistance, NXOpen.GeometricUtilities.FeatureOptions.BodyStyle bodyStyle, double offsetValue)
		{
			NXOpen.Features.ExtrudeBuilder extrudeBuilder = features.CreateExtrudeBuilder(null);

			Section section1 = WorkPart.Sections.CreateSection(0.0095, 0.01, 0.5);

			extrudeBuilder.Section = section1;

			extrudeBuilder.AllowSelfIntersectingSection(true);

			extrudeBuilder.BooleanOperation.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;

			extrudeBuilder.Offset.Option = NXOpen.GeometricUtilities.Type.SingleOffset;

			extrudeBuilder.Offset.EndOffset.Value = offsetValue;

			extrudeBuilder.Draft.DraftOption = NXOpen.GeometricUtilities.SimpleDraft.SimpleDraftType.NoDraft;

			extrudeBuilder.FeatureOptions.BodyType = bodyStyle;

			section1.DistanceTolerance = 0.01;

			section1.ChainingTolerance = 0.0095;

			section1.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.OnlyCurves);

			extrudeBuilder.Direction = WorkPart.Directions.CreateDirection(new Point3d(0.0, 0.0, 0.0), extrudeDirection, NXOpen.SmartObject.UpdateOption.WithinModeling);

#if NX12
			CurveDumbRule curveDumbRule = (WorkPart as BasePart).ScRuleFactory.CreateRuleBaseCurveDumb(curves);
#else
			CurveDumbRule curveDumbRule = WorkPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves);
#endif
			section1.AllowSelfIntersection(true);

			SelectionIntentRule[] rules1 = new SelectionIntentRule[1] { curveDumbRule };

			section1.AddToSection(rules1, (NXObject)curves[0], null, null, new Point3d(0, 0, 0), NXOpen.Section.Mode.Create, false);

			extrudeBuilder.Limits.StartExtend.TrimType = NXOpen.GeometricUtilities.Extend.ExtendType.Value;

			extrudeBuilder.Limits.EndExtend.TrimType = NXOpen.GeometricUtilities.Extend.ExtendType.Value;

			extrudeBuilder.Limits.EndExtend.Value.RightHandSide = endDistance.ToString();

			extrudeBuilder.Limits.StartExtend.Value.RightHandSide = startDistance.ToString();

			extrudeBuilder.ParentFeatureInternal = false;

			Extrude extrude = (Extrude)extrudeBuilder.CommitFeature();
			extrudeBuilder.Destroy();
			return extrude;
		}
	}
}
