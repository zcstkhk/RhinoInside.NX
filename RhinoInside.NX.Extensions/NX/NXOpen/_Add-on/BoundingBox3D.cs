using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
    /// <summary>
    /// 立方体
    /// </summary>
    public class BoundingBox3D
    {
        #region 属性
        public bool Enabled;

        public Matrix4x4 Transform;

        /// <summary>
        /// 最小角点
        /// </summary>
        public Point3d MinCorner;

        /// <summary>
        /// 最大角点
        /// </summary>
        public Point3d MaxCorner;

        /// <summary>
        /// 中心点
        /// </summary>
        public Point3d Center { get => MinCorner.MidPoint3d(MaxCorner); }

        /// <summary>
        /// 方位
        /// </summary>
        public Matrix3x3 Orientation { get; }

        public Point3d V1 { get => MinCorner; }

        public Point3d V2 { get => MinCorner.Move(Orientation.GetAxisX(), Length); }

        public Point3d V3 { get => V2.Move(Orientation.GetAxisY(), Width); }

        public Point3d V4 { get => MinCorner.Move(Orientation.GetAxisY(), Width); }

        public Point3d V5 { get => MinCorner.Move(Orientation.GetAxisZ(), Height); }

        public Point3d V6 { get => V2.Move(Orientation.GetAxisZ(), Height); }

        public Point3d V7 { get => MaxCorner; }

        public Point3d V8 { get => V4.Move(Orientation.GetAxisZ(), Height); }

        public Rectangle Top { get => new Rectangle(V5, V6, V7, V8); }

        public Rectangle Bottom { get => new Rectangle(V1, V2, V3, V4); }

        public Rectangle Left { get => new Rectangle(V1, V5, V8, V4); }

        public Rectangle Right { get => new Rectangle(V2, V6, V7, V3); }

        public Rectangle Front { get => new Rectangle(V1, V5, V6, V2); }

        public Rectangle Back { get => new Rectangle(V4, V3, V7, V8); }

        /// <summary>
        /// 顶点
        /// </summary>
        private Point3d[] Vertices { get; set; }

        /// <summary>
        /// Box 的长度/X 方向距离
        /// </summary>
        public double Length
        {
            get => Vector3dEx.Create(MaxCorner, MinCorner).DotProduct(Orientation.GetAxisX());
        }

        /// <summary>
        /// Box 的宽度/Y 方向距离
        /// </summary>
        public double Width
        {
            get => Vector3dEx.Create(MaxCorner, MinCorner).DotProduct(Orientation.GetAxisY());
        }

        /// <summary>
        /// Box 的高度/Z 方向距离
        /// </summary>
        public double Height
        {
            get => Vector3dEx.Create(MaxCorner, MinCorner).DotProduct(Orientation.GetAxisZ());
        }

        /// <summary>
        /// 长度/X 方向
        /// </summary>
        public Vector3d LengthDirection { get => Orientation.GetAxisX(); }

        /// <summary>
        /// 宽度/Y 方向
        /// </summary>
        public Vector3d WidthDirection { get => Orientation.GetAxisY(); }

        /// <summary>
        /// 高度/Z 方向
        /// </summary>
        public Vector3d HeightDirection { get => Orientation.GetAxisZ(); }
        #endregion

        #region 构造函数
        public BoundingBox3D(Point3d min, Point3d max)
        {
            MinCorner = min;
            MaxCorner = max;
            Orientation = Matrix3x3Ex.Identity;
        }

        /// <summary>
        /// 使用两个角点和方位来初始化 Box
        /// </summary>
        /// <param name="minCorner"></param>
        /// <param name="maxCorner"></param>
        /// <param name="orientation"></param>
        public BoundingBox3D(Point3d minCorner, Point3d maxCorner, Matrix3x3 orientation)
        {
            MinCorner = minCorner;
            MaxCorner = maxCorner;
            Orientation = orientation;
        }

        /// <summary>
        /// 使用最小角点和长宽高来初始化 Box
        /// </summary>
        /// <param name="minCorner"></param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public BoundingBox3D(Point3d minCorner, double length, double width, double height)
        {
            MinCorner = minCorner;
            MaxCorner = minCorner.Move(new Vector3d(length, width, height));
            Orientation = Matrix3x3Ex.Identity;
        }

        /// <summary>
        /// 使用最小角点、方位，以及长宽高来初始化 Box
        /// </summary>
        /// <param name="minCorner"></param>
        /// <param name="orientation"></param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public BoundingBox3D(Point3d minCorner, Matrix3x3 orientation, double length, double width, double height)
        {
            MinCorner = minCorner;
            Orientation = orientation;
            MaxCorner = minCorner.Move(orientation.GetAxisX(), length).Move(orientation.GetAxisY(), width).Move(orientation.GetAxisZ(), height);
        }

        public BoundingBox3D(Point3d minCorner, Point3d maxCorner, Matrix4x4 transform)
        {
            MinCorner = minCorner;
            MaxCorner = maxCorner;
            Transform = transform;
        }
        #endregion

        #region 类方法
        /// <summary>
        /// 创建方块
        /// </summary>
        public Body CreateBlock()
        {
            var bottomEdges = Bottom.CreateEdges();

            var extrudeFeature = Globals.WorkPart.Features.CreateExtrude(bottomEdges, Orientation.GetAxisZ(), 0.0, Height);

            var blockBody = extrudeFeature.GetBodies()[0];

            blockBody.RemoveParameters();

            bottomEdges.Delete();

            return blockBody;
        }

        // Token: 0x06000160 RID: 352 RVA: 0x0000DF18 File Offset: 0x0000C118
        public void Join(BoundingBox3D box)
        {
            Point3d maxCorner = MaxCorner;
            Point3d maxCorner2 = box.MaxCorner;
            MinCorner.X = Math.Min(MinCorner.X, box.MinCorner.X);
            MinCorner.Y = Math.Min(MinCorner.Y, box.MinCorner.Y);
            MinCorner.Z = Math.Min(MinCorner.Z, box.MinCorner.Z);
            double num = Math.Max(maxCorner.X, maxCorner2.X);
            double num2 = Math.Max(maxCorner.Y, maxCorner2.Y);
            double num3 = Math.Max(maxCorner.Z, maxCorner2.Z);
        }

        // Token: 0x06000161 RID: 353 RVA: 0x0000E01C File Offset: 0x0000C21C
        public bool IsIntersect(BoundingBox3D box)
        {
            bool result = box.MinCorner.X > MaxCorner.X || box.MaxCorner.X < MinCorner.X || box.MinCorner.Y > MaxCorner.Y || box.MaxCorner.Y < MinCorner.Y || box.MinCorner.Z > MaxCorner.Z || box.MaxCorner.Z < MinCorner.Z;
            return !result;
        }

        public bool IsEmpty()
        {
            return MinCorner.DistanceTo(new Point3d()) <= Globals.DistanceTolerance && MaxCorner.DistanceTo(new Point3d()) <= Globals.DistanceTolerance;
        }

        // Token: 0x06000163 RID: 355 RVA: 0x0000E11C File Offset: 0x0000C31C
        public double SquareMagnitude(BoundingBox3D box)
        {
            double num = Center.X - box.Center.X;
            double num2 = Center.Y - box.Center.Y;
            double num3 = Center.Z - box.Center.Z;
            return num * num + num2 * num2 + num3 * num3;
        }

        // Token: 0x06000164 RID: 356 RVA: 0x0000E184 File Offset: 0x0000C384
        public double SquareMinDistance(BoundingBox3D box)
        {
            double[] array = new double[3];
            double[] array2 = MinCorner.ToArray();
            double[] array3 = MaxCorner.ToArray();
            double[] array4 = box.MinCorner.ToArray();
            double[] array5 = box.MaxCorner.ToArray();
            for (int i = 0; i < 3; i++)
            {
                if ((array2[i] <= array5[i] && array2[i] >= array4[i]) || (array3[i] <= array5[i] && array3[i] >= array4[i]) || (array4[i] <= array3[i] && array4[i] >= array2[i]) || (array5[i] <= array3[i] && array5[i] >= array2[i]))
                    array[i] = 0.0;
                else
                    array[i] = Math.Min(Math.Min(Math.Abs(array2[i] - array4[i]), Math.Abs(array2[i] - array5[i])), Math.Min(Math.Abs(array3[i] - array5[i]), Math.Abs(array3[i] - array4[i])));
            }
            return array[0] * array[0] + array[1] * array[1] + array[2] * array[2];
        }

        // Token: 0x06000165 RID: 357 RVA: 0x0000E2D0 File Offset: 0x0000C4D0
        public double SquareMaxDistance(BoundingBox3D box)
        {
            double[] array = new double[3];
            double[] array2 = MinCorner.ToArray();
            double[] array3 = MaxCorner.ToArray();
            double[] array4 = box.MinCorner.ToArray();
            double[] array5 = box.MaxCorner.ToArray();
            for (int i = 0; i < 3; i++)
            {
                if ((array2[i] <= array5[i] && array2[i] >= array4[i]) || (array3[i] <= array5[i] && array3[i] >= array4[i]) || (array4[i] <= array3[i] && array4[i] >= array2[i]) || (array5[i] <= array3[i] && array5[i] >= array2[i]))
                    array[i] = 0.0;
                else
                    array[i] = Math.Max(Math.Max(Math.Abs(array2[i] - array4[i]), Math.Abs(array2[i] - array5[i])), Math.Max(Math.Abs(array3[i] - array5[i]), Math.Abs(array3[i] - array4[i])));
            }
            return array[0] * array[0] + array[1] * array[1] + array[2] * array[2];
        }

        // Token: 0x06000166 RID: 358 RVA: 0x0000E41C File Offset: 0x0000C61C
        public double CubeVolume()
        {
            return (MaxCorner.X - MinCorner.X) * (MaxCorner.Y - MinCorner.Y) * (MaxCorner.Z - MinCorner.Z);
        }

        // Token: 0x06000167 RID: 359 RVA: 0x0000E478 File Offset: 0x0000C678
        public BoxPositionType CalXType(BoundingBox3D box)
        {
            BoxPositionType result;
            if ((MinCorner.X <= box.MaxCorner.X && MinCorner.X >= box.MinCorner.X) || (MaxCorner.X <= box.MaxCorner.X && MaxCorner.X >= box.MinCorner.X) || (box.MinCorner.X <= MaxCorner.X && box.MinCorner.X >= MinCorner.X) || (box.MaxCorner.X <= MaxCorner.X && box.MaxCorner.X >= MinCorner.X))
                result = BoxPositionType.Intersect;
            else
            {
                if (MaxCorner.X <= box.MinCorner.X)
                    result = BoxPositionType.AB;
                else
                    result = BoxPositionType.BA;
            }
            return result;
        }

        // Token: 0x06000168 RID: 360 RVA: 0x0000E584 File Offset: 0x0000C784
        public BoxPositionType CalYType(BoundingBox3D box)
        {
            BoxPositionType result;
            if ((MinCorner.Y <= box.MaxCorner.Y && MinCorner.Y >= box.MinCorner.Y) || (MaxCorner.Y <= box.MaxCorner.Y && MaxCorner.Y >= box.MinCorner.Y) || (box.MinCorner.Y <= MaxCorner.Y && box.MinCorner.Y >= MinCorner.Y) || (box.MaxCorner.Y <= MaxCorner.Y && box.MaxCorner.Y >= MinCorner.Y))
                result = BoxPositionType.Intersect;
            else
            {
                if (MaxCorner.Y <= box.MinCorner.Y)
                    result = BoxPositionType.AB;
                else
                    result = BoxPositionType.BA;
            }
            return result;
        }

        // Token: 0x06000169 RID: 361 RVA: 0x0000E690 File Offset: 0x0000C890
        public BoxPositionType CalZType(BoundingBox3D box)
        {
            BoxPositionType result;
            if ((MinCorner.Z <= box.MaxCorner.Z && MinCorner.Z >= box.MinCorner.Z) || (MaxCorner.Z <= box.MaxCorner.Z && MaxCorner.Z >= box.MinCorner.Z) || (box.MinCorner.Z <= MaxCorner.Z && box.MinCorner.Z >= MinCorner.Z) || (box.MaxCorner.Z <= MaxCorner.Z && box.MaxCorner.Z >= MinCorner.Z))
                result = BoxPositionType.Intersect;
            else
            {
                if (MaxCorner.Z <= box.MinCorner.Z)
                    result = BoxPositionType.AB;
                else
                    result = BoxPositionType.BA;
            }
            return result;
        }

        // Token: 0x0600016B RID: 363 RVA: 0x0000E7D0 File Offset: 0x0000C9D0
        public Vector3d CalXZLimit(Vector3d origin, Vector3d refPoint)
        {
            double num = ((origin.X > MaxCorner.X) ? (Math.Abs(refPoint.Z - MaxCorner.Z) < Math.Abs(refPoint.Z - MinCorner.Z) ? (origin.Z < MaxCorner.Z) : (origin.Z > MinCorner.Z)) : (Math.Abs(refPoint.Z - MaxCorner.Z) < Math.Abs(refPoint.Z - MinCorner.Z) ? (origin.Z > MaxCorner.Z) : (origin.Z < MinCorner.Z))) ? MaxCorner.X : MinCorner.X;
            double num2 = Math.Abs(refPoint.Z - MaxCorner.Z) < Math.Abs(refPoint.Z - MinCorner.Z) ? MaxCorner.Z : MinCorner.Z;
            Vector3d result = new Vector3d(num - origin.X, 0.0, num2 - origin.Z);
            result.GetUnitVector();
            return result;
        }

        // Token: 0x0600016E RID: 366 RVA: 0x0000EB98 File Offset: 0x0000CD98
        public BoundingBox3D Multiply(Matrix3x3 matrix)
        {
            BoundingBox3D simpleBox = new BoundingBox3D(MinCorner, Length, Width, Height);
            simpleBox.MinCorner = matrix.Multiply(simpleBox.MinCorner);
            return simpleBox;
        }

        // Token: 0x0600016F RID: 367 RVA: 0x0000EBDC File Offset: 0x0000CDDC
        // Note: this type is marked as 'beforefieldinit'.
        static BoundingBox3D()
        {
        }

        // Token: 0x02000036 RID: 54
    }

    // Token: 0x02000020 RID: 32
    public enum BoxPositionType
    {
        // Token: 0x04000070 RID: 112
        Intersect,
        // Token: 0x04000071 RID: 113
        AB,
        // Token: 0x04000072 RID: 114
        BA
    }
    #endregion
}