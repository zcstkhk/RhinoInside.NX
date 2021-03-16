using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RhinoInside.NX.Extensions.Globals;

namespace RhinoInside.NX.Extensions
{
    public static class FaceEx
    {
        /// <summary>
        /// 测量面的面积
        /// </summary>
        /// <param name="face"></param>
        /// <param name="tolerance">测量公差，越接近1.0，结果越精确，但同时可能会花费较长时间</param>
        /// <returns></returns>
        public static double GetArea(this Face face, double tolerance = 0.999) => WorkPart.MeasureManager.MeasureFace(face, tolerance).Area;

        /// <summary>
        /// 测量面的周长
        /// </summary>
        /// <param name="face"></param>
        /// <param name="tolerance">测量公差，越接近1.0，结果越精确，但同时可能会花费较长时间</param>
        /// <returns></returns>
        public static double GetPerimeter(this Face face, double tolerance = 0.999) => WorkPart.MeasureManager.MeasureFace(face, tolerance).Perimeter;

        /// <summary>
        /// 获取面的UV范围
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static BoundingBox2D GetUVBoundingBox(this Face face)
        {
            double[] uvMinMax = new double[4];
            TheUfSession.Modl.AskFaceUvMinmax(face.Tag, uvMinMax);

            return new BoundingBox2D(uvMinMax[0], uvMinMax[1], uvMinMax[2], uvMinMax[3]);
        }

        [Obsolete("2020-12-20,直接使用不带参数的GetPoint()方法")]
        public static Point3d GetUVMiddlePoint(this Face face)
        {
            var faceMiddleUV = face.GetUVBoundingBox().Middle;

            return face.GetProperties(faceMiddleUV.U, faceMiddleUV.V).Point;
        }

        /// <summary>
        /// 获取面的相关信息
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static FaceData GetData(this Face face)
        {
            double[] facePt = new double[3];
            double[] direction = new double[3];
            double[] box = new double[6];
            TheUfSession.Modl.AskFaceData(face.Tag, out int type, facePt, direction, box, out double radius, out double radData, out int normDir);

            return new FaceData()
            {
                Box = new BoundingBox3D(new Point3d(box[0], box[1], box[2]), new Point3d(box[3], box[4], box[5])),
                Direction = direction.ToVector3d(),
                FaceType = face.SolidFaceType,
                NormalReversed = normDir == -1,
                Point = facePt.ToPoint3d(),
                Radius = radius,
                RadiusData = radData
            };
        }

        /// <summary>
        /// 获取面上指定UV位置的属性
        /// </summary>
        /// <param name="face"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static FaceProperties GetProperties(this Face face, double u, double v)
        {
            double[] pt = new double[3];
            double[] u1 = new double[3];
            double[] v1 = new double[3];
            double[] u2 = new double[3];
            double[] v2 = new double[3];
            double[] unitNormal = new double[3];
            double[] radii = new double[2];
            TheUfSession.Modl.AskFaceProps(face.Tag, new double[] { u, v }, pt, u1, v1, u2, v2, unitNormal, radii);

            return new FaceProperties()
            {
                Point = pt.ToPoint3d(),
                RadiusU = radii[0],
                RadiusV = radii[1],
                U1 = u1.ToVector3d(),
                V1 = v1.ToVector3d(),
                U2 = u2.ToVector3d(),
                V2 = v2.ToVector3d(),
                UnitNormal = unitNormal.ToVector3d()
            };
        }

        /// <summary>
        /// 获取面在UV值0.5，0.5处的法向
        /// </summary>
        /// <param name="face">要计算的面</param>
        /// <returns></returns>
        public static Vector3d GetNormal(this Face face)
        {
            double[] pt, u1, v1, u2, v2, unit_norm;
            pt = u1 = v1 = u2 = v2 = unit_norm = new double[3];
            double[] radii = new double[2];
            TheUfSession.Modl.AskFaceProps(face.Tag, new double[] { 0.5, 0.5 }, pt, u1, v1, u2, v2, unit_norm, radii);

            return unit_norm.ToVector3d();
        }

        /// <summary>
        /// 获取面在UV值0.5，0.5处的点
        /// </summary>
        /// <param name="face">面</param>
        /// <returns></returns>
        // 通过参数访问面上的点
        public static Point3d GetPoint(this Face face)
        {
            Part workPart = TheSession.Parts.Work;
            Scalar scalar = workPart.Scalars.CreateScalar(0.5, Scalar.DimensionalityType.None, SmartObject.UpdateOption.WithinModeling);

            Point middlePoint = workPart.Points.CreatePoint(face, scalar, scalar, SmartObject.UpdateOption.WithinModeling);
            Point3d middlePoint3d = middlePoint.Coordinates;
            middlePoint.Delete();
            return middlePoint3d;
        }

        /// <summary>
        /// 获取面在指定 UV 值处的点
        /// </summary>
        /// <param name="face">面</param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        // 通过参数访问面上的点
        public static Point3d GetPoint(this Face face, double u, double v)
        {
            Part workPart = TheSession.Parts.Work;
            Scalar scalarU = workPart.Scalars.CreateScalar(u, Scalar.DimensionalityType.None, SmartObject.UpdateOption.WithinModeling);
            Scalar scalarV = workPart.Scalars.CreateScalar(v, Scalar.DimensionalityType.None, SmartObject.UpdateOption.WithinModeling);

            Point middlePoint = workPart.Points.CreatePoint(face, scalarU, scalarV, SmartObject.UpdateOption.WithinModeling);
            Point3d middlePoint3d = middlePoint.Coordinates;
            middlePoint.Delete();
            return middlePoint3d;
        }

        /// <summary>
        /// 获取面在指定 UV 值处的点
        /// </summary>
        /// <param name="face">面</param>
        /// <param name="uv"></param>
        /// <returns></returns>
        // 通过参数访问面上的点
        public static Point3d GetPoint(this Face face, UV uv) => GetPoint(face, uv.U, uv.V);

        public struct FaceProperties
        {
            /// <summary>
            /// Point at parameter
            /// </summary>
            public Point3d Point;

            /// <summary>
            /// First derivative in U
            /// </summary>
            public Vector3d U1;

            /// <summary>
            /// First derivative in V
            /// </summary>
            public Vector3d V1;

            /// <summary>
            /// Second derivative in U
            /// </summary>
            public Vector3d U2;

            /// <summary>
            /// Second derivative in V
            /// </summary>
            public Vector3d V2;

            /// <summary>
            /// Unit face normal
            /// </summary>
            public Vector3d UnitNormal;

            /// <summary>
            /// Principal radii of curvature in U
            /// </summary>
            public double RadiusU;

            /// <summary>
            /// Principal radii of curvature in V
            /// </summary>
            public double RadiusV;
        }

        public struct FaceData
        {
            public Face.FaceType FaceType;
            /// <summary>
            /// Point information is returned according to the value of type as follows.
            /// <br>Plane = Position in plane</br>
            /// <br>Cylinder = Position on axis</br>
            /// <br>Cone = Position on axis</br>
            /// <br>Sphere = Center position</br>
            /// <br>Torus = Center position</br>
            /// <br>Revolved = Position on axis</br>
            /// </summary>
            public Point3d Point;

            /// <summary>
            /// Direction information is returned according to the value of type as follows.
            /// <br>Plane = Normal direction</br>
            /// <br>Cylinder= Axis direction</br>
            /// <br>Cone = Axis direction</br>
            /// <br>Torus = Axis direction</br>
            /// <br>Revolved = Axis direction</br>
            /// </summary>
            public Vector3d Direction;

            /// <summary>
            /// Face boundary.The coordinates of the opposite corners of a rectangular box with sides parallel to X, Y, and Z axes(Absolute Coordinate System) are returned.The box contains the specified face and isusually close to the minimum possible size, but this is not guaranteed.
            /// </summary>
            public BoundingBox3D Box;

            /// <summary>
            /// Face major radius:
            /// <br>For a cone, the radius is taken at the point[3] position on the axis.</br>
            /// <br>For a torus, the radius is taken at the major axis.</br>
            /// </summary>
            public double Radius;

            /// <summary>
            /// Face minor radius: only a torus or cone has rad_data as a minor radius.
            /// <br>For a cone, rad_data is the half angle in radians.</br>
            /// <br>For a torus, rad_data is taken at the minor axis.</br>
            /// </summary>
            public double RadiusData;

            /// <summary>
            /// Face normal direction: +1 if the face normal is in the same direction as the surface normal(cross product of the U- and V-derivative vectors), -1 if reversed.
            /// </summary>
            public bool NormalReversed;
        }
    }
}
