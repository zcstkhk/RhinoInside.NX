using NXOpen.Assemblies;
using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using static RhinoInside.NX.Extensions.Globals;
using RhinoInside.NX.Extensions.Parasolid;
using PLMComponents.Parasolid.PK_.Unsafe;
using System.Runtime.InteropServices;

namespace RhinoInside.NX.Extensions
{
    public static class BodyEx
    {
        static BodyEx() => AppDomain.CurrentDomain.AssemblyResolve += Globals.ManagedLibraryResolver;

        static UFSession _theUfSession = UFSession.GetUFSession();

        /// <summary>
        /// 将体小平面化
        /// </summary>
        /// <param name="body"></param>
        /// <returns>体小平面化后得到的三角片</returns>
        public static List<Triangle> GetTriangles(this Body body)
        {
            _theUfSession.Facet.AskDefaultParameters(out UFFacet.Parameters faceting_params);

            _theUfSession.Facet.FacetSolid(body.Tag, ref faceting_params, out Tag model_Tag);
            List<Triangle> list = new List<Triangle>();
            int num2 = UFConstants.UF_FACET_NULL_FACET_ID;
            double[,] array2 = new double[3, 3];

            _theUfSession.Facet.CycleFacets(model_Tag, ref num2);
            while (num2 != -1)
            {
                _theUfSession.Facet.AskVerticesOfFacet(model_Tag, num2, out _, array2);
                Triangle item = new Triangle(array2[0, 0], array2[0, 1], array2[0, 2], array2[1, 0], array2[1, 1], array2[1, 2], array2[2, 0], array2[2, 1], array2[2, 2]);
                //if (item.IsValid())
                list.Add(item);

                _theUfSession.Facet.CycleFacets(model_Tag, ref num2);
            }
            return list;
        }

        /// <summary>
        /// 将体小平面化
        /// </summary>
        /// <param name="body"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<Triangle> GetTriangles(this Body body, double tolerance)
        {
            _theUfSession.Facet.AskDefaultParameters(out UFFacet.Parameters faceting_params);

            //faceting_params.specify_parameters = true;
            faceting_params.specify_surface_tolerance = true;
            faceting_params.surface_dist_tolerance = tolerance;

            faceting_params.specify_curve_tolerance = true;
            faceting_params.curve_dist_tolerance = tolerance;

            //faceting_params.number_storage_type = UFConstants.UF_FACET_TYPE_DOUBLE;

            //faceting_params.silh_chord_tolerance = tolerance;

            //faceting_params.curve_max_length = 2;

            //faceting_params.max_facet_size = 5;

            //faceting_params.specify_max_facet_size = true;

            _theUfSession.Facet.FacetSolid(body.Tag, ref faceting_params, out Tag model_Tag);
            List<Triangle> list = new List<Triangle>();
            int num2 = UFConstants.UF_FACET_NULL_FACET_ID;
            double[,] array2 = new double[3, 3];

            _theUfSession.Facet.CycleFacets(model_Tag, ref num2);
            while (num2 != -1)
            {
                _theUfSession.Facet.AskVerticesOfFacet(model_Tag, num2, out _, array2);
                Triangle item = new Triangle(array2[0, 0], array2[0, 1], array2[0, 2], array2[1, 0], array2[1, 1], array2[1, 2], array2[2, 0], array2[2, 1], array2[2, 2]);
                list.Add(item);

                _theUfSession.Facet.CycleFacets(model_Tag, ref num2);
            }
            return list;
        }

        //public unsafe static List<Point3d> FacetToPoints(this Body body, double maxDistance = 5.0)
        //{
        //	Tag tag;
        //	BodyEx._theUfSession.Ps.AskPsTagOfObject(body.Tag, out tag);
        //	double* ptr = null;
        //	int num;

        //	PK.TRANSF_t trans = new PK.TRANSF_t() { Value =  }

        //	PK.TOPOL.facet(1, new PK.TOPOL_t[] { body.ToPKBody() }, new PK.TRANSF_t[] {  } )

        //	GLB.TOPOL_facet_Points(&tag, 1, 0.0, false, maxDistance, true, &num, &ptr);
        //	List<Point3d> list = new List<Point3d>();
        //	for (int i = 0; i < num; i++)
        //	{
        //		Point3d item = new Point3d(ptr[i * 3], ptr[i * 3 + 1], ptr[i * 3 + 2]);
        //		list.Add(item);
        //	}
        //	bool isOccurrence = body.IsOccurrence;
        //	if (isOccurrence)
        //	{
        //		Component owningComponent = body.OwningComponent;
        //		Matrix4x4 matrix = owningComponent.GetPosition();
        //		for (int j = 0; j < list.Count; j++)
        //		{
        //			list[j] = matrix.MultiplyPoint(list[j]);
        //		}
        //	}
        //	return list;
        //}

        /// <summary>
        /// 转换为 PKBody
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public static BODY_t ToPKBody(this Body body)
        {
            _theUfSession.Ps.AskPsTagOfObject(body.Tag, out Tag value);
            BODY_t result = new BODY_t((int)value);
            return result;
        }

        //public static void GetMassProperty(this Body body)
        //{
        //    Part WorkPart = theSession.Parts.Work;

        //    // 先获取用户默认设置中重量测量的精度值
        //    double weightCalcAccuracy = Convert.ToDouble(GetCustomerDefaults("Assemblies_WeightDataAccuracy"));

        //    double[] massProps = new double[47];
        //    double[] statics = new double[13];
        //    _theUfSession.Modl.AskMassProps3d(new Tag[] { currentBody.Tag }, 1, 1, 4, 1.0, 1, new double[] { 0.99, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, massProps, statics);

        //    return massProps[2];



        //    Unit[] units = WorkPart.UnitCollection.ToArray();
        //    MeasureBodies massProperty = theSession.Parts.Work.MeasureManager.NewMassProperties(units, 0.01, new IBody[] { body });
        //    return massProperty;
        //}

        /// <summary>
        /// 获取体相对于 WCS 的 Bounding Box
        /// </summary>
        /// <param name="body"></param>
        /// <param name="alignToComponentWCS">若是组件，是否需要与组件的坐标系对齐</param>
        /// <returns></returns>
        public static BoundingBox3D GetBoundingBox(this Body body, bool alignToComponentWCS = false)
        {
            if (alignToComponentWCS && body.IsOccurrence)
            {
                body.OwningComponent.GetPosition(out Point3d origin, out Matrix3x3 orientation);

                NXMatrix matrix = WorkPart.NXMatrices.Create(orientation);

                _theUfSession.Csys.CreateTempCsys(origin.ToArray(), matrix.Tag, out Tag csysTag);

                double[] minCorner = new double[3];
                double[,] directions = new double[3, 3];
                double[] distances = new double[3];
                _theUfSession.Modl.AskBoundingBoxAligned(body.Tag, csysTag, false, minCorner, directions, distances);

                return new BoundingBox3D(minCorner.ToPoint3d(), Matrix3x3Ex.Create(directions), distances[0], distances[1], distances[2]);

                //Component owningComponent = body.OwningComponent;
                //var orientation = owningComponent.GetPosition();
                //minCorner = orientation.Multiply(minCorner.Move(orientation.Translation.Reverse()));
                //maxCorner = orientation.Multiply(maxCorner.Move(orientation.Translation.Reverse()));
                //return new Box(minCorner, maxCorner, orientation.Rotation);
            }
            else
            {
                double[] boundingBox = new double[6];
                _theUfSession.Modl.AskBoundingBox(body.Tag, boundingBox);
                Point3d minCorner = new Point3d(boundingBox[0], boundingBox[1], boundingBox[2]);
                Point3d maxCorner = new Point3d(boundingBox[3], boundingBox[4], boundingBox[5]);

                return new BoundingBox3D(minCorner, maxCorner);
            }
            //bool isOccurrence = body.IsOccurrence;
            //if (isOccurrence)
            //{

            //}

        }

        [Obsolete("2021-01-15，请使用 GetBoundingBox")]
        public static BoundingBox3D GetBox(this Body body, Matrix4x4 refFrame)
        => GetBoundingBox(body, refFrame);

        /// <summary>
        /// 获取体的最小包围盒
        /// </summary>
        /// <param name="body"></param>
        /// <param name="refFrame"></param>
        /// <returns></returns>
        public static BoundingBox3D GetBoundingBox(this Body body, Matrix4x4 refFrame)
        {
            Tag csysTag = refFrame.CreateCsys(false);
            double[] minCorner = new double[3];
            double[,] directions = new double[3, 3];
            double[] distances = new double[3];
            _theUfSession.Modl.AskBoundingBoxAligned(body.Tag, csysTag, false, minCorner, directions, distances);

            var orientation = Matrix3x3Ex.Create(directions.GetRow(0), directions.GetRow(1), directions.GetRow(2));

            return new BoundingBox3D(minCorner.ToPoint3d(), orientation, distances[0], distances[1], distances[2]);

            //UFSession ufsession = UFSession.GetUFSession();
            //Matrix4x4 matrix = refFrame.Inverse();
            //Tag csysTag = refFrame.CreateCsys(false);
            //Point3d vec3d = new Point3d(double.MaxValue, double.MaxValue, double.MaxValue);
            //Vector3d vec3d2 = new Vector3d(double.MinValue, double.MinValue, double.MinValue);
            //foreach (Tag BodyTag in bodies)
            //{
            //    double[] minCorner = new double[3];
            //    double[,] directions = new double[3, 3];
            //    double[] distances = new double[3];
            //    ufsession.Modl.AskBoundingBoxAligned(BodyTag, csysTag, false, minCorner, directions, distances);
            //    corner = minCorner.ToPoint3d();
            //    corner = matrix.Multiply(corner);
            //    vec3d.X = Math.Min(vec3d.X, corner.X);
            //    vec3d.Y = Math.Min(vec3d.Y, corner.Y);
            //    vec3d.Z = Math.Min(vec3d.Z, corner.Z);
            //    vec3d2.X = Math.Max(vec3d2.X, corner.X + distances[0]);
            //    vec3d2.Y = Math.Max(vec3d2.Y, corner.Y + distances[1]);
            //    vec3d2.Z = Math.Max(vec3d2.Z, corner.Z + distances[2]);
            //}
            //corner = vec3d;
            //size.X = vec3d2.X - vec3d.X;
            //size.Y = vec3d2.Y - vec3d.Y;
            //size.Z = vec3d2.Z - vec3d.Z;
        }

        /// <summary>
        /// 获取一组体的最小包围盒
        /// </summary>
        /// <param name="bodies"></param>
        /// <param name="refFrame"></param>
        /// <returns></returns>
        public static BoundingBox3D GetBoundingBox(this IEnumerable<Body> bodies, Matrix4x4 refFrame) => GetBoundingBox(bodies.Select(obj => obj.Tag), refFrame);

        internal static BoundingBox3D GetBoundingBox(IEnumerable<Tag> bodies, Matrix4x4 refFrame)
        {
            UFSession ufsession = UFSession.GetUFSession();
            Matrix4x4 matrix = refFrame.GetInvert();
            Tag csysTag = refFrame.CreateCsys(false);
            Point3d tempCornerPoint3d = new Point3d(double.MaxValue, double.MaxValue, double.MaxValue);
            Point3d tempExtremePoint3d = new Point3d(double.MinValue, double.MinValue, double.MinValue);
            foreach (Tag bodyTag in bodies)
            {
                double[] minCorner = new double[3];
                double[,] directions = new double[3, 3];
                double[] distance = new double[3];
                ufsession.Modl.AskBoundingBoxExact(bodyTag, csysTag, minCorner, directions, distance);
                Point3d corner = minCorner.ToPoint3d();
                corner = matrix.Multiply(corner);
                tempCornerPoint3d.X = Math.Min(tempCornerPoint3d.X, corner.X);
                tempCornerPoint3d.Y = Math.Min(tempCornerPoint3d.Y, corner.Y);
                tempCornerPoint3d.Z = Math.Min(tempCornerPoint3d.Z, corner.Z);
                tempExtremePoint3d.X = Math.Max(tempExtremePoint3d.X, corner.X + distance[0]);
                tempExtremePoint3d.Y = Math.Max(tempExtremePoint3d.Y, corner.Y + distance[1]);
                tempExtremePoint3d.Z = Math.Max(tempExtremePoint3d.Z, corner.Z + distance[2]);
            }

            return new BoundingBox3D(tempCornerPoint3d.Multiply(refFrame), tempExtremePoint3d.Multiply(refFrame), refFrame.GetRotation());
        }

        public unsafe static (bool Success, double Distance, Point3d PointOnObjects) DistanceTo(this Body[] bodies, Point3d targetPoint)
        {
            PKTag[] bodyPKTags = new PKTag[bodies.Length];
            for (int i = 0; i < bodies.Length; i++)
                bodyPKTags[i] = new PKTag(bodies[i]);

            TOPOLEx.range_array_vector(targetPoint, bodyPKTags, out var rangeResult, out var range);

            if (rangeResult == range_result_t.found_c)
                return (Success: true, Distance: range.distance, PointOnObjects: new Point3d(range.end.vector.coord[0] * 1000.0, range.end.vector.coord[1] * 1000.0, range.end.vector.coord[2] * 1000.0));
            else
                return (false, 0, new Point3d());
        }

        /// <summary>
        /// 测量体的表面积
        /// </summary>
        /// <param name="body"></param>
        /// <param name="tolerance">测量公差，越接近1.0，结果越精确，但同时可能会花费较长时间</param>
        /// <returns></returns>
        public static double GetArea(this Body body, double tolerance = 0.999) => body.GetMassProperties(tolerance).Area;

        public static PointClouds SimplifyPointClouds(this Body body, double pointPointMaxDistance) => SimplifyPointClouds(new Body[] { body }, pointPointMaxDistance);

        public static PointClouds SimplifyPointClouds(this Body[] bodies, double pointPointMaxDistance)
        {
            List<Point3d> list = new List<Point3d>();
            foreach (Body body in bodies)
                list.AddRange(body.FacetToPoints(pointPointMaxDistance));

            PointCloudsRoot pointCloudsRoot = new PointCloudsRoot(-10000, -10000, -10000, 10000, 10000, 10000);
            for (int i = 0; i < list.Count; i++)
            {
                _theUfSession.Ui.SetStatus($"Processing Point {i + 1} / {list.Count}");
                pointCloudsRoot.AddPoint(list[i]);
            }

            pointCloudsRoot.FinalizeInput();
            return pointCloudsRoot.GetPointClouds();
        }

        public unsafe static Point3d[] FacetToPoints(this Body body, double maxDistance = 5.0)
        {
            var facetResult = TOPOLEx.facet(new PKTag[] { new PKTag(body.Tag) }, 0.0, false, maxDistance, true);

            Point3d[] vertices = new Point3d[facetResult.point_vec.length];
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = new Point3d(facetResult.point_vec.vec[i].coord[0] * 1000.0, facetResult.point_vec.vec[i].coord[1] * 1000.0, facetResult.point_vec.vec[i].coord[2] * 1000.0);

            bool isOccurrence = body.IsOccurrence;
            if (isOccurrence)
            {
                Component owningComponent = body.OwningComponent;
                Matrix4x4 matrix = owningComponent.GetPosition();
                for (int j = 0; j < vertices.Length; j++)
                {
                    vertices[j] = vertices[j].Multiply(matrix);
                }
            }
            return vertices;
        }

        internal unsafe static List<Point3d> FacetToPoints_Legacy(this Body body, double maxDistance = 5.0)
        {
            Tag tag;
            _theUfSession.Ps.AskPsTagOfObject(body.Tag, out tag);
            double* ptr = null;
            int num;
            TOPOL_facet_Points(&tag, 1, 0.0, false, maxDistance, true, &num, &ptr);
            //num.ToString().ListingWindowWriteLine();
            List<Point3d> list = new List<Point3d>();
            for (int i = 0; i < num; i++)
            {
                Point3d item = new Point3d(ptr[i * 3], ptr[i * 3 + 1], ptr[i * 3 + 2]);
                list.Add(item);
            }
            bool isOccurrence = body.IsOccurrence;
            if (isOccurrence)
            {
                Component owningComponent = body.OwningComponent;
                Matrix4x4 matrix = owningComponent.GetPosition();
                for (int j = 0; j < list.Count; j++)
                    list[j] = list[j].Multiply(matrix);
            }
            return list;
        }

        [DllImport("NXOpen.Kernel.dll")]
        internal unsafe static extern void TOPOL_facet_Points(Tag* topols, int count, double minLength, bool minLogical, double maxLength, bool maxLogical, int* pointNum, double** points);

        /// <summary>
        /// 测量体的质量属性
        /// </summary>
        /// <param name="body"></param>
        /// <param name="tolerance">测量公差，越接近1.0，结果越精确，但同时可能会花费较长时间</param>
        /// <returns></returns>
        public static MassProperties GetMassProperties(this Body body, double tolerance = 0.999)
        {
            int type = body.IsSolidBody ? 1 : 2;
            double density = UnitType == BasePart.Units.Millimeters ? body.Density * 100.0 : body.Density;
            int units = UnitType == BasePart.Units.Millimeters ? 3 : 1;
            int accuracy = 1;
            double[] array = new double[11];
            array[0] = tolerance;
            double[] massProps = new double[47];
            double[] statistics = new double[13];
            Tag[] bodyTags = new Tag[] { body.Tag };

            _theUfSession.Modl.AskMassProps3d(bodyTags, 1, type, units, density, accuracy, array, massProps, statistics);

            MassProperties massPropertiesResult = new MassProperties(massProps);

            MassProperties result = null;

            if (UnitType == BasePart.Units.Millimeters)
            {
                massPropertiesResult.Units = UFWeight.UnitsType.UnitsGc;
                result = ConvertUnits(massPropertiesResult, UFWeight.UnitsType.UnitsGm, body.GetObjectType().SubType);
            }
            else
            {
                massPropertiesResult.Units = UFWeight.UnitsType.UnitsLi;
                result = massPropertiesResult;
            }
            return result;
        }

        /// <summary>
        /// 测量体的质量属性
        /// </summary>
        /// <param name="bodies"></param>
        /// <param name="tolerance">测量公差，越接近1.0，结果越精确，但同时可能会花费较长时间</param>
        /// <returns></returns>
        public static MassProperties GetMassProperties(this Body[] bodies, double tolerance = 0.999)
        {
            Body[] solidBodies = bodies.Where(obj => obj.IsSolidBody).ToArray();

            Body[] sheetBodies = bodies.Where(obj => obj.IsSheetBody).ToArray();

            List<MassProperties> list = new List<MassProperties>();
            if (solidBodies.Length != 0)
                list.Add(GetMassProperties(solidBodies, ObjectSubType.BodySolid, tolerance));

            foreach (var body in sheetBodies)
                list.Add(GetMassProperties(new Body[] { body }, ObjectSubType.BodySheet, tolerance));

            MassProperties massPropertiesResult = null;
            if (list.Count > 1)
                massPropertiesResult = MassProperties.Combine(list.ToArray());
            else
                massPropertiesResult = list[0];

            massPropertiesResult.CompleteResults();

            return massPropertiesResult;
        }

        public static MassProperties GetMassProperties(this Body[] bodies, ObjectSubType bodySubType, double tolerance = 0.999)
        {
            int type = bodySubType == ObjectSubType.BodySolid ? 1 : 2;
            double density = UnitType == BasePart.Units.Millimeters ? bodies[0].Density * 100.0 : bodies[0].Density;
            int units = UnitType == BasePart.Units.Millimeters ? 3 : 1;
            int accuracy = 1;
            double[] array = new double[11];
            array[0] = tolerance;
            double[] massProps = new double[47];
            double[] statistics = new double[13];
            Tag[] bodyTags = bodies.Select(obj => obj.Tag).ToArray();

            _theUfSession.Modl.AskMassProps3d(bodyTags, bodies.Length, type, units, density, accuracy, array, massProps, statistics);

            MassProperties massPropertiesResult = new MassProperties(massProps);

            MassProperties result = null;

            if (UnitType == BasePart.Units.Millimeters)
            {
                massPropertiesResult.Units = UFWeight.UnitsType.UnitsGc;
                result = ConvertUnits(massPropertiesResult, UFWeight.UnitsType.UnitsGm, bodies[0].GetObjectType().SubType);
            }
            else
            {
                massPropertiesResult.Units = UFWeight.UnitsType.UnitsLi;
                result = massPropertiesResult;
            }
            return result;
        }

        private static MassProperties ConvertUnits(MassProperties input, UFWeight.UnitsType outUnits, ObjectSubType subType)
        {
            UFWeight.Properties originalProperties = input.ToWeightProps();
            UFWeight.Properties convertedProperties = new UFWeight.Properties();
            _theUfSession.Weight.ConvertPropUnits(ref originalProperties, outUnits, out convertedProperties);
            MassProperties massPropertiesResult = new MassProperties(convertedProperties);

            if (UnitType == BasePart.Units.Millimeters)
                massPropertiesResult.RadiusOfGyration = 10.0 * input.RadiusOfGyration;
            else
                massPropertiesResult.RadiusOfGyration = input.RadiusOfGyration;

            return massPropertiesResult;
        }
    }
}
