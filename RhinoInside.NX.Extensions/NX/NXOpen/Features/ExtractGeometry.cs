using NXOpen.Features;
using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
    public static partial class FeatureCollectionEx
    {
        /// <summary>
        /// 抽取曲线
        /// </summary>
        /// <param name="features"></param>
        /// <param name="curve"></param>
        /// <param name="associative"></param>
        /// <returns></returns>
        public static CompositeCurve CreateExtractGeometry(this FeatureCollection features, IBaseCurve curve, bool associative = false) => CreateExtractGeometry(features, new IBaseCurve[] { curve }, associative);

        /// <summary>
        /// 抽取曲线
        /// </summary>
        /// <param name="features"></param>
        /// <param name="curves"></param>
        /// <param name="associative"></param>
        /// <returns></returns>
        public static CompositeCurve CreateExtractGeometry(this FeatureCollection features, IBaseCurve[] curves, bool associative = false)
        {
            NXOpen.Features.CompositeCurveBuilder compositeCurveBuilder1 = _workPart.Features.CreateCompositeCurveBuilder(null);
            compositeCurveBuilder1.Tolerance = 0.01;
            compositeCurveBuilder1.Associative = associative;
            compositeCurveBuilder1.FixAtCurrentTimestamp = true;
            compositeCurveBuilder1.JoinOption = NXOpen.Features.CompositeCurveBuilder.JoinMethod.Genernal;
            compositeCurveBuilder1.ParentPart = NXOpen.Features.CompositeCurveBuilder.PartType.WorkPart;
            compositeCurveBuilder1.Section.DistanceTolerance = 0.01;
            compositeCurveBuilder1.Section.ChainingTolerance = 0.0095;
            compositeCurveBuilder1.HideOriginal = false;
            compositeCurveBuilder1.InheritDisplayProperties = false;
            compositeCurveBuilder1.Section.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.CurvesAndPoints);

			CurveDumbRule curveDumbRule1 = _workPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves);

            compositeCurveBuilder1.Section.AllowSelfIntersection(false);
            SelectionIntentRule[] rules1 = new SelectionIntentRule[1] { curveDumbRule1 };
            Point3d helpPoint1 = new Point3d(1885.57193958882, -793.938033503939, 819.818721208804);
            compositeCurveBuilder1.Section.AddToSection(rules1, curves[0] as NXObject, null, null, helpPoint1, NXOpen.Section.Mode.Create, false);
            CompositeCurve compositeCurve = (CompositeCurve)compositeCurveBuilder1.CommitFeature();
            compositeCurveBuilder1.Destroy();
            return compositeCurve;
        }

        /// <summary>
        /// 抽取体
        /// </summary>
        /// <param name="features"></param>
        /// <param name="bodies"></param>
        /// <param name="associative"></param>
        /// <returns></returns>
        public static ExtractFace CreateExtractGeometry(this FeatureCollection features, Body[] bodies, bool associative = false)
        {
            ExtractFaceBuilder extractFaceBuilder = _workPart.Features.CreateExtractFaceBuilder(null);

            extractFaceBuilder.ParentPart = ExtractFaceBuilder.ParentPartType.WorkPart;

            extractFaceBuilder.Associative = associative;

            extractFaceBuilder.FixAtCurrentTimestamp = true;

            extractFaceBuilder.HideOriginal = false;

            extractFaceBuilder.InheritDisplayProperties = false;

            extractFaceBuilder.Type = ExtractFaceBuilder.ExtractType.Body;

            extractFaceBuilder.CopyThreads = false;

            extractFaceBuilder.FeatureOption = ExtractFaceBuilder.FeatureOptionType.SeparateFeatureForEachBody;

			BodyDumbRule bodyDumbRule1 = _workPart.ScRuleFactory.CreateRuleBodyDumb(bodies, true);

            SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
            rules1[0] = bodyDumbRule1;
            extractFaceBuilder.ExtractBodyCollector.ReplaceRules(rules1, false);

            ExtractFace nXObject1 = (ExtractFace)extractFaceBuilder.Commit();

            extractFaceBuilder.Destroy();

            return nXObject1;
        }

        /// <summary>
        /// 抽取面，返回面
        /// </summary>
        /// <param name="features"></param>
        /// <param name="face"></param>
        /// <param name="associative"></param>
        /// <returns></returns>
        public static ExtractFace CreateExtractGeometry(this FeatureCollection features, Face face, bool associative = false)
        {
            NXOpen.Features.ExtractFaceBuilder extractFaceBuilder = _workPart.Features.CreateExtractFaceBuilder(null);
            extractFaceBuilder.ParentPart = NXOpen.Features.ExtractFaceBuilder.ParentPartType.WorkPart;
            extractFaceBuilder.Associative = associative;
            extractFaceBuilder.FixAtCurrentTimestamp = true;
            extractFaceBuilder.HideOriginal = false;
            extractFaceBuilder.DeleteHoles = false;
            extractFaceBuilder.InheritDisplayProperties = false;
            extractFaceBuilder.Type = NXOpen.Features.ExtractFaceBuilder.ExtractType.Face;
            bool added;
            added = extractFaceBuilder.ObjectToExtract.Add(face);
            ExtractFace extractedFace = (ExtractFace)extractFaceBuilder.Commit();
            extractFaceBuilder.Destroy();
            return extractedFace;
        }
    }
}