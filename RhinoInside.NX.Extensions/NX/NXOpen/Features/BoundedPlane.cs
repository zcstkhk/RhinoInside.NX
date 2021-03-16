using NXOpen;
using NXOpen.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Extensions
{
    public static partial class FeatureCollectionEx
    {
        /// <summary>
        /// 创建有界平面
        /// </summary>
        /// <param name="featureCollection"></param>
        /// <param name="boundingCurves"></param>
        /// <returns></returns>
        public static BoundedPlane CreateBoundedPlane(this FeatureCollection featureCollection, IBaseCurve[] boundingCurves)
        {
            if (boundingCurves.Length != 0)
            {
                BoundedPlaneBuilder boundedPlaneBuilder = _workPart.Features.CreateBoundedPlaneBuilder(null);

                boundedPlaneBuilder.BoundingCurves.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.OnlyCurves);

                CurveDumbRule curveDumbRule1 = _workPart.ScRuleFactory.CreateRuleBaseCurveDumb(boundingCurves);

                boundedPlaneBuilder.BoundingCurves.AllowSelfIntersection(true);

                NXOpen.SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = curveDumbRule1;
                boundedPlaneBuilder.BoundingCurves.AddToSection(rules1, boundingCurves[0] as NXObject, null, null, new Point3d(), Section.Mode.Create, false);

                BoundedPlane nXObject1 = (BoundedPlane)boundedPlaneBuilder.Commit();

                boundedPlaneBuilder.Destroy();

                return nXObject1;
            }
            return null;
        }
    }
}
