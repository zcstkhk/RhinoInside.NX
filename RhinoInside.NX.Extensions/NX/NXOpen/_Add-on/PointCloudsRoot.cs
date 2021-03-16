using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
    // Token: 0x0200001C RID: 28
    public class PointCloudsRoot
    {
        // Token: 0x1700001D RID: 29
        // (get) Token: 0x0600010A RID: 266 RVA: 0x0000BEA1 File Offset: 0x0000A0A1
        // (set) Token: 0x0600010B RID: 267 RVA: 0x0000BEA9 File Offset: 0x0000A0A9
        internal Dictionary<int, Point3d> PointDic { get; set; }

        // Token: 0x1700001E RID: 30
        // (get) Token: 0x0600010C RID: 268 RVA: 0x0000BEB2 File Offset: 0x0000A0B2
        // (set) Token: 0x0600010D RID: 269 RVA: 0x0000BEBA File Offset: 0x0000A0BA
        private List<int> XCollection { get; set; }

        // Token: 0x1700001F RID: 31
        // (get) Token: 0x0600010E RID: 270 RVA: 0x0000BEC3 File Offset: 0x0000A0C3
        // (set) Token: 0x0600010F RID: 271 RVA: 0x0000BECB File Offset: 0x0000A0CB
        private List<int> YCollection { get; set; }

        // Token: 0x17000020 RID: 32
        // (get) Token: 0x06000110 RID: 272 RVA: 0x0000BED4 File Offset: 0x0000A0D4
        // (set) Token: 0x06000111 RID: 273 RVA: 0x0000BEDC File Offset: 0x0000A0DC
        private List<int> ZCollection { get; set; }

        // Token: 0x06000112 RID: 274 RVA: 0x0000BEE8 File Offset: 0x0000A0E8
        public PointCloudsRoot(int minx, int miny, int minz, int maxx, int maxy, int maxz)
        {
            _minx = minx;
            _miny = miny;
            _minz = minz;
            _maxx = maxx;
            _maxy = maxy;
            _maxz = maxz;
            PointDic = new Dictionary<int, Point3d>();
            _lastIndex = -1;
            _xCollection = new List<int>[maxx - minx + 1];
            _yCollection = new List<int>[maxy - miny + 1];
            _zCollection = new List<int>[maxz - minz + 1];
        }

        public void AddPoint(Point3d point)
        {
            _lastIndex++;
            PointDic.Add(_lastIndex, point);
            //point.ToString().ConsoleWriteLine();
            AddX(point);
            AddY(point);
            AddZ(point);
        }

        private void AddX(Point3d point)
        {
            int num = (int)point.X - _minx;
            //$"X: {num}".ConsoleWriteLine();
            List<int> list = _xCollection[num];
            if (list == null)
            {
                list = new List<int>();
                _xCollection[num] = list;
            }
            list.Add(_lastIndex);
        }

        private void AddY(Point3d point)
        {
            int num = (int)point.Y - _miny;
            //$"Y: {num}".ConsoleWriteLine();
            List<int> list = _yCollection[num];
            if (list == null)
            {
                list = new List<int>();
                _yCollection[num] = list;
            }
            list.Add(_lastIndex);
        }

        private void AddZ(Point3d point)
        {
            int num = (int)point.Z - _minz;
            //$"Z: {num}".ConsoleWriteLine();
            List<int> list = _zCollection[num];
            if (list == null)
            {
                list = new List<int>();
                _zCollection[num] = list;
            }
            list.Add(_lastIndex);
        }

        public void FinalizeInput()
        {
            XCollection = new List<int>();
            YCollection = new List<int>();
            ZCollection = new List<int>();
            foreach (List<int> list in _xCollection)
            {
                if (list != null)
                    XCollection.AddRange(list);
            }
            foreach (List<int> list2 in _yCollection)
            {
                if (list2 != null)
                    YCollection.AddRange(list2);
            }
            foreach (List<int> list3 in _zCollection)
            {
                if (list3 != null)
                    ZCollection.AddRange(list3);
            }
        }

        public PointClouds GetPointClouds()
        {
            List<int> xc = new List<int>(XCollection);
            List<int> yc = new List<int>(YCollection);
            List<int> zc = new List<int>(ZCollection);

            return new PointClouds(xc, yc, zc, this);
        }

        public static PointCloudsRoot Create(List<Point3d> points)
        {
            double num = double.MaxValue;
            double num2 = double.MaxValue;
            double num3 = double.MaxValue;
            double num4 = double.MinValue;
            double num5 = double.MinValue;
            double num6 = double.MinValue;
            foreach (Point3d pt in points)
            {
                num = Math.Min(num, pt.X);
                num4 = Math.Max(num4, pt.X);
                num2 = Math.Min(num2, pt.Y);
                num5 = Math.Max(num5, pt.Y);
                num3 = Math.Min(num3, pt.Z);
                num6 = Math.Max(num6, pt.Z);
            }
            PointCloudsRoot pointCloudsRoot = new PointCloudsRoot((int)num, (int)num2, (int)num3, (int)num4, (int)num5, (int)num6);
            foreach (Point3d point in points)
            {
                pointCloudsRoot.AddPoint(point);
            }
            pointCloudsRoot.FinalizeInput();
            return pointCloudsRoot;
        }

        // Token: 0x0600011A RID: 282 RVA: 0x0000C320 File Offset: 0x0000A520
        public static double CalMinDistance(List<Point3d> points1, List<Point3d> points2)
        {
            double num = double.MaxValue;
            double num2 = double.MaxValue;
            double num3 = double.MaxValue;
            double num4 = double.MinValue;
            double num5 = double.MinValue;
            double num6 = double.MinValue;
            foreach (Point3d Vector3d in points1)
            {
                num = Math.Min(num, Vector3d.X);
                num4 = Math.Max(num4, Vector3d.X);
                num2 = Math.Min(num2, Vector3d.Y);
                num5 = Math.Max(num5, Vector3d.Y);
                num3 = Math.Min(num3, Vector3d.Z);
                num6 = Math.Max(num6, Vector3d.Z);
            }
            PointCloudsRoot pointCloudsRoot = new PointCloudsRoot((int)num, (int)num2, (int)num3, (int)num4, (int)num5, (int)num6);
            foreach (Point3d point in points1)
            {
                pointCloudsRoot.AddPoint(point);
            }
            pointCloudsRoot.FinalizeInput();
            foreach (Point3d Vector3d2 in points2)
            {
                num = Math.Min(num, Vector3d2.X);
                num4 = Math.Max(num4, Vector3d2.X);
                num2 = Math.Min(num2, Vector3d2.Y);
                num5 = Math.Max(num5, Vector3d2.Y);
                num3 = Math.Min(num3, Vector3d2.Z);
                num6 = Math.Max(num6, Vector3d2.Z);
            }
            PointCloudsRoot pointCloudsRoot2 = new PointCloudsRoot((int)num, (int)num2, (int)num3, (int)num4, (int)num5, (int)num6);
            foreach (Point3d point2 in points2)
            {
                pointCloudsRoot2.AddPoint(point2);
            }
            pointCloudsRoot2.FinalizeInput();
            PointClouds pointClouds = pointCloudsRoot.GetPointClouds();
            PointClouds pointClouds2 = pointCloudsRoot2.GetPointClouds();
            BoundingBox3D box = pointClouds.GetBox();
            BoundingBox3D box2 = pointClouds2.GetBox();
            points1 = pointClouds.GetPoints();
            points2 = pointClouds2.GetPoints();
            int count = points1.Count;
            int count2 = points2.Count;
            double result;
            if (count == 0 || count2 == 0)
                result = -1.0;            
            else
            {
                BoxPositionType boxPositionType = box.CalXType(box2);
                BoxPositionType boxPositionType2 = box.CalYType(box2);
                BoxPositionType boxPositionType3 = box.CalZType(box2);
                while (count > 100 || count2 > 100)
                {
                    if (boxPositionType == BoxPositionType.Intersect && boxPositionType2 == BoxPositionType.Intersect && boxPositionType3 == BoxPositionType.Intersect)
                    {
                        return 0.0;
                    }
                    if (boxPositionType > BoxPositionType.Intersect)
                    {
                        PointClouds pointClouds3;
                        PointClouds pointClouds4;
                        pointClouds.DivideXByCount(out pointClouds3, out pointClouds4);
                        PointClouds pointClouds5;
                        PointClouds pointClouds6;
                        pointClouds2.DivideXByCount(out pointClouds5, out pointClouds6);
                        if (boxPositionType == BoxPositionType.AB)
                        {
                            pointClouds = ((pointClouds4 == null) ? pointClouds3 : pointClouds4);
                            pointClouds2 = ((pointClouds5 == null) ? pointClouds6 : pointClouds5);
                        }
                        else
                        {
                            if (boxPositionType == BoxPositionType.BA)
                            {
                                pointClouds = ((pointClouds3 == null) ? pointClouds4 : pointClouds3);
                                pointClouds2 = ((pointClouds6 == null) ? pointClouds5 : pointClouds6);
                            }
                        }
                    }
                    else
                    {
                        if (boxPositionType2 > BoxPositionType.Intersect)
                        {
                            PointClouds pointClouds3;
                            PointClouds pointClouds4;
                            pointClouds.DivideYByCount(out pointClouds3, out pointClouds4);
                            PointClouds pointClouds5;
                            PointClouds pointClouds6;
                            pointClouds2.DivideYByCount(out pointClouds5, out pointClouds6);
                            if (boxPositionType2 == BoxPositionType.AB)
                            {
                                pointClouds = ((pointClouds4 == null) ? pointClouds3 : pointClouds4);
                                pointClouds2 = ((pointClouds5 == null) ? pointClouds6 : pointClouds5);
                            }
                            else
                            {
                                if (boxPositionType2 == BoxPositionType.BA)
                                {
                                    pointClouds = ((pointClouds3 == null) ? pointClouds4 : pointClouds3);
                                    pointClouds2 = ((pointClouds6 == null) ? pointClouds5 : pointClouds6);
                                }
                            }
                        }
                        else
                        {
                            if (boxPositionType3 > BoxPositionType.Intersect)
                            {
                                PointClouds pointClouds3;
                                PointClouds pointClouds4;
                                pointClouds.DivideZByCount(out pointClouds3, out pointClouds4);
                                PointClouds pointClouds5;
                                PointClouds pointClouds6;
                                pointClouds2.DivideZByCount(out pointClouds5, out pointClouds6);
                                if (boxPositionType3 == BoxPositionType.AB)
                                {
                                    pointClouds = ((pointClouds4 == null) ? pointClouds3 : pointClouds4);
                                    pointClouds2 = ((pointClouds5 == null) ? pointClouds6 : pointClouds5);
                                }
                                else
                                {
                                    if (boxPositionType3 == BoxPositionType.BA)
                                    {
                                        pointClouds = ((pointClouds3 == null) ? pointClouds4 : pointClouds3);
                                        pointClouds2 = ((pointClouds6 == null) ? pointClouds5 : pointClouds6);
                                    }
                                }
                            }
                        }
                    }
                    box = pointClouds.GetBox();
                    box2 = pointClouds2.GetBox();
                    points1 = pointClouds.GetPoints();
                    points2 = pointClouds2.GetPoints();
                    count = points1.Count;
                    count2 = points2.Count;
                    boxPositionType = box.CalXType(box2);
                    boxPositionType2 = box.CalYType(box2);
                    boxPositionType3 = box.CalZType(box2);
                }

                var measure = Globals.WorkPart.MeasureManager.MeasureDistance(points1, points2);
                result = measure.Distance;
            }
            return result;
        }

        private int _lastIndex;

        private List<int>[] _xCollection;

        private List<int>[] _yCollection;

        private List<int>[] _zCollection;

        private int _minx;

        private int _miny;

        private int _minz;

        private int _maxx;

        private int _maxy;

        private int _maxz;
    }
}