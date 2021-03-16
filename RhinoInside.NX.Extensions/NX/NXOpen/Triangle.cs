using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
    /// <summary>
    /// 表示三角形的结构体，包含三个顶点
    /// </summary>
    public class Triangle
    {
        #region 字段
        public Point3d V1 { get; set; }

        public Point3d V2 { get; set; }

        public Point3d V3 { get; set; }

        public List<Point3d> Vertices
        {
            get
            {
                return new Point3d[] { V1, V2, V3 }.ToList();
            }
        }
        #endregion

        #region 构造函数

        public Triangle(Point3d p1, Point3d p2, Point3d p3)
        {
            V1 = p1;
            V2 = p2;
            V3 = p3;
        }

        public Triangle(double p11, double p12, double p13, double p21, double p22, double p23, double p31, double p32, double p33)
        {
            V1 = new Point3d(p11, p12, p13);
            V2 = new Point3d(p21, p22, p23);
            V3 = new Point3d(p31, p32, p33);
        }
        #endregion

        #region 类方法
        public Tag CreateModel()
        {
            Session session = Session.GetSession();
            Tag tag = session.Parts.Work.Tag;
            UFSession ufsession = UFSession.GetUFSession();
            ufsession.Facet.CreateModel(tag, out Tag tag2);
            ufsession.Facet.AddFacetToModel(tag2, 3, ToArray(), null, null, out int num);
            ufsession.Facet.ModelEditsDone(tag2);
            return tag2;
        }

        /// <summary>
        /// 连接两个三角形
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public List<Triangle> ConnectTriangles(Triangle target)
        {
            List<Triangle> list = new List<Triangle>();
            Triangle item1 = new Triangle(V1, V2, target.V1);
            list.Add(item1);

            Triangle item2 = new Triangle(target.V1, target.V2, V2);
            list.Add(item2);

            Triangle item3 = new Triangle(V1, V3, target.V1);
            list.Add(item3);

            Triangle item4 = new Triangle(target.V1, target.V3, V3);
            list.Add(item4);

            Triangle item5 = new Triangle(V2, V3, target.V2);
            list.Add(item5);

            Triangle item6 = new Triangle(target.V2, target.V3, V3);
            list.Add(item6);

            return list;
        }

        public List<Triangle> ConnectTriangles(Triangle target, bool validate)
        {
            List<Triangle> list = new List<Triangle>();
            Triangle item1 = new Triangle(V1, V2, target.V1);
            if (validate && item1.IsValid())
                list.Add(item1);

            Triangle item2 = new Triangle(target.V1, target.V2, V2);
            if (validate && item2.IsValid())
                list.Add(item2);

            Triangle item3 = new Triangle(V1, V3, target.V1);
            if (validate && item3.IsValid())
                list.Add(item3);

            Triangle item4 = new Triangle(target.V1, target.V3, V3);
            if (validate && item4.IsValid())
                list.Add(item4);

            Triangle item5 = new Triangle(V2, V3, target.V2);
            if (validate && item5.IsValid())
                list.Add(item5);

            Triangle item6 = new Triangle(target.V2, target.V3, V3);
            //if (item6.IsValid())
            list.Add(item6);

            return list;
        }

        public bool IsValid()
        {
            bool result;
            if (V1.IsSame(V2) || V1.IsSame(V3) || V2.IsSame(V3))
                result = false;
            else
            {
                Vector3d vector = V1.Subtract(V2);
                Vector3d target = V1.Subtract(V3);
                result = !vector.IsParallel(target);
            }
            return result;
        }

        /// <summary>
        /// 将三角形的三个点依次平移
        /// </summary>
        /// <param name="translation"></param>
        /// <returns></returns>
        public Triangle Translate(Vector3d translation)
        {
            return new Triangle(V1.Move(translation), V2.Move(translation), V3.Move(translation));
        }

        /// <summary>
        /// 将三角形的三个点进行平移
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public Triangle Translate(Vector3d translation, double distance)
        {
            return new Triangle(V1.Move(translation, distance), V2.Move(translation, distance), V3.Move(translation, distance));
        }

        public Vector3d GetNormal()
        {
            Vector3d vector = V1.Subtract(V2);
            Vector3d vector2 = V1.Subtract(V3);
            return vector.CrossProduct(vector2);
        }

        public Triangle Multiply(Matrix4x4 trans)
        {
            var p1 = V1.Multiply(trans);
            var p2 = V2.Multiply(trans);
            var p3 = V3.Multiply(trans);
            return new Triangle(p1, p2, p3);
        }

        /// <summary>
        /// 转换为 3x3 数组，第一维表示顶点序号，第二维表示 X、Y、Z
        /// </summary>
        /// <returns></returns>
        public double[,] ToArray()
        {
            double[,] array = new double[3, 3];
            array[0, 0] = V1.X;
            array[0, 1] = V1.Y;
            array[0, 2] = V1.Z;
            array[1, 0] = V2.X;
            array[1, 1] = V2.Y;
            array[1, 2] = V2.Z;
            array[2, 0] = V3.X;
            array[2, 1] = V3.Y;
            array[2, 2] = V3.Z;
            return array;
        }

        /// <summary>
        /// 转换为 Point3d 数组
        /// </summary>
        /// <returns></returns>
        public Point3d[] ToPointArray()
        {
            return new Point3d[]
            {
                V1,
                V2,
                V3
            };
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("V1:" + V1.ToString() + ";");
            stringBuilder.Append("V2:" + V2.ToString() + ";");
            stringBuilder.Append("V3:" + V3.ToString());
            return stringBuilder.ToString();
        }

        public Triangle ScaleByDistance(double distance)
        {
            Point3d middlePoint = Vertices.ToArray().MidPoint3d();
            Vector3d vec = middlePoint.Subtract(V1);
            vec = vec.GetUnitVector();
            Point3d p2 = V1.Add(vec.Multiply(distance));
            Vector3d vec2 = middlePoint.Subtract(V2);
            vec2 = vec2.GetUnitVector();
            Point3d p3 = V2.Add(vec2.Multiply(distance));
            Vector3d vec3 = middlePoint.Subtract(V3);
            vec3 = vec3.GetUnitVector();
            Point3d p4 = V3.Add(vec3.Multiply(distance));
            Triangle result = new Triangle(p2, p3, p4);
            return result;
        }

        public List<Triangle> Facet(double maxLength)
        {
            if (maxLength <= 0.0)
                throw new Exception("maxLength必须为正数!");

            List<Triangle> list = new List<Triangle>();
            List<Triangle> list2 = new List<Triangle>();
            list2.Add(this);
            while (list2.Count != list.Count)
            {
                list.Clear();
                list.AddRange(list2);
                foreach (Triangle triangle in list)
                {
                    double num = triangle.V1.DistanceTo(triangle.V2);
                    if (num > maxLength)
                    {
                        list2.Remove(triangle);
                        Point3d p = triangle.V1.MidPoint3d(triangle.V2);
                        Triangle item = new Triangle(triangle.V1, p, triangle.V3);
                        Triangle item2 = new Triangle(triangle.V2, p, triangle.V3);
                        list2.Add(item);
                        list2.Add(item2);
                    }
                    else
                    {
                        double num2 = triangle.V1.DistanceTo(triangle.V3);
                        if (num2 > maxLength)
                        {
                            list2.Remove(triangle);
                            Point3d p2 = triangle.V1.MidPoint3d(triangle.V3);
                            Triangle item3 = new Triangle(triangle.V1, p2, triangle.V2);
                            Triangle item4 = new Triangle(triangle.V3, p2, triangle.V2);
                            list2.Add(item3);
                            list2.Add(item4);
                        }
                        else
                        {
                            double num3 = triangle.V2.DistanceTo(triangle.V3);
                            if (num3 > maxLength)
                            {
                                list2.Remove(triangle);
                                Point3d p3 = triangle.V2.MidPoint3d(triangle.V3);
                                Triangle item5 = new Triangle(triangle.V2, p3, triangle.V1);
                                Triangle item6 = new Triangle(triangle.V3, p3, triangle.V1);
                                list2.Add(item5);
                                list2.Add(item6);
                            }
                        }
                    }
                }
            }
            return list2;
        }

        public Tag CreateSheet()
        {
            UFSession ufsession = UFSession.GetUFSession();
            Point3d point3d = V2.MidPoint3d(V3);
            Session session = Session.GetSession();
            Part work = session.Parts.Work;
            RuledBuilder ruledBuilder = work.Features.CreateRuledBuilder(null);
#if NX12
            CurveDumbRule curveDumbRule = (work as BasePart).ScRuleFactory.CreateRuleCurveDumbFromPoints(new Point[] { work.Points.CreatePoint(V1) });
#else
            CurveDumbRule curveDumbRule = work.ScRuleFactory.CreateRuleCurveDumbFromPoints(new Point[] { work.Points.CreatePoint(V1) });
#endif

            ruledBuilder.FirstSection.AddToSection(new SelectionIntentRule[]
            {
                curveDumbRule
            }, null, null, null, default(Point3d), Section.Mode.Create, false);
            Line line = work.Curves.CreateLine(V2, V3);
#if NX12
            CurveDumbRule curveDumbRule2 = (work as BasePart).ScRuleFactory.CreateRuleCurveDumb(new Curve[] { line });
#else
            CurveDumbRule curveDumbRule2 = work.ScRuleFactory.CreateRuleCurveDumb(new Curve[]
            { line });
#endif
            ruledBuilder.SecondSection.AddToSection(new SelectionIntentRule[]
            {
                curveDumbRule2
            }, null, null, null, default(Point3d), Section.Mode.Create, false);
            ruledBuilder.PositionTolerance = 0.001;
            ruledBuilder.AlignmentMethod.AlignType = AlignmentMethodBuilder.Type.SpineCurve;
            ruledBuilder.AlignmentMethod.AlignCurve.DistanceTolerance = 0.01;
            ruledBuilder.FirstSection.DistanceTolerance = 0.01;
            ruledBuilder.SecondSection.DistanceTolerance = 0.01;
            ruledBuilder.FirstSection.SetAllowedEntityTypes(Section.AllowTypes.CurvesAndPoints);
            ruledBuilder.SecondSection.SetAllowedEntityTypes(Section.AllowTypes.CurvesAndPoints);
            NXObject nxobject = ruledBuilder.Commit();
            Ruled ruled = nxobject as Ruled;
            Body body = (ruled != null) ? ruled.GetBodies()[0] : null;
            ufsession.Modl.DeleteBodyParms(new Tag[]
            {
                body.Tag
            });
            line.Delete();
            return body.Tag;
        }
        #endregion

        #region 其它扩展方法

        #endregion
    }
}
