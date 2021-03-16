using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
	public class PointClouds
	{
		private List<int> _xCollection;

		private List<int> _yCollection;

		// Token: 0x04000048 RID: 72
		private List<int> _zCollection;

		// Token: 0x04000049 RID: 73
		private BoundingBox3D _box = null;

		// Token: 0x0400004A RID: 74
		private List<Point3d> _points = null;

		public PointCloudsRoot Root { get; internal set; }

		// Token: 0x060000E4 RID: 228 RVA: 0x0000A503 File Offset: 0x00008703
		internal PointClouds(List<int> xc, List<int> yc, List<int> zc, PointCloudsRoot root)
		{
			_xCollection = xc;
			_yCollection = yc;
			_zCollection = zc;
			Root = root;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x0000A53C File Offset: 0x0000873C
		public void DivideXByCount(out PointClouds min, out PointClouds max)
		{
			min = null;
			max = null;
			int num = _xCollection.Count / 2;
			if (num == 0)
				max = this;
			else
			{
				List<int> range = _xCollection.GetRange(0, num + 1);
				List<int> range2 = _yCollection.GetRange(0, _yCollection.Count);
				List<int> range3 = _zCollection.GetRange(0, _zCollection.Count);
				if (num == _xCollection.Count - 1)
					min = this;
				else
				{
					List<int> range4 = _xCollection.GetRange(num + 1, _xCollection.Count - num - 1);
					List<int> range5 = _yCollection.GetRange(0, _yCollection.Count);
					List<int> range6 = _zCollection.GetRange(0, _zCollection.Count);
					foreach (int item in range4)
					{
						range2.Remove(item);
						range3.Remove(item);
					}
					foreach (int item2 in range)
					{
						range5.Remove(item2);
						range6.Remove(item2);
					}
					min = new PointClouds(range, range2, range3, Root);
					max = new PointClouds(range4, range5, range6, Root);
				}
			}
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x0000A6E8 File Offset: 0x000088E8
		public void DivideYByCount(out PointClouds min, out PointClouds max)
		{
			min = null;
			max = null;
			int num = _yCollection.Count / 2;
			if (num == 0)
				max = this;
			else
			{
				List<int> range = _yCollection.GetRange(0, num + 1);
				List<int> range2 = _xCollection.GetRange(0, _xCollection.Count);
				List<int> range3 = _zCollection.GetRange(0, _zCollection.Count);
				if (num == _yCollection.Count - 1)
				{
					min = this;
				}
				else
				{
					List<int> range4 = _yCollection.GetRange(num + 1, _yCollection.Count - num - 1);
					List<int> range5 = _xCollection.GetRange(0, _xCollection.Count);
					List<int> range6 = _zCollection.GetRange(0, _zCollection.Count);
					foreach (int item in range4)
					{
						range2.Remove(item);
						range3.Remove(item);
					}
					foreach (int item2 in range)
					{
						range5.Remove(item2);
						range6.Remove(item2);
					}
					min = new PointClouds(range2, range, range3, Root);
					max = new PointClouds(range5, range4, range6, Root);
				}
			}
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x0000A894 File Offset: 0x00008A94
		public void DivideZByCount(out PointClouds min, out PointClouds max)
		{
			min = null;
			max = null;
			int num = _zCollection.Count / 2;
			if (num == 0)
				max = this;
			else
			{
				List<int> range = _zCollection.GetRange(0, num + 1);
				List<int> range2 = _yCollection.GetRange(0, _yCollection.Count);
				List<int> range3 = _xCollection.GetRange(0, _xCollection.Count);
				if (num == _zCollection.Count - 1)
					min = this;
				else
				{
					List<int> range4 = _zCollection.GetRange(num + 1, _zCollection.Count - num - 1);
					List<int> range5 = _yCollection.GetRange(0, _yCollection.Count);
					List<int> range6 = _xCollection.GetRange(0, _xCollection.Count);
					foreach (int item in range4)
					{
						range2.Remove(item);
						range3.Remove(item);
					}
					foreach (int item2 in range)
					{
						range5.Remove(item2);
						range6.Remove(item2);
					}
					min = new PointClouds(range3, range2, range, Root);
					max = new PointClouds(range6, range5, range4, Root);
				}
			}
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x0000AA40 File Offset: 0x00008C40
		public void DivideXByDistance(out PointClouds min, out PointClouds max)
		{
			min = null;
			max = null;
			int num = (_xCollection.Last() - _xCollection.First()) / 2;
			int num2 = 0;
			foreach (int num3 in _xCollection)
			{
				if (num > num3)
					num2++;
			}

			if (num2 == 0)
				max = this;
			else
			{
				List<int> range = _xCollection.GetRange(0, num2 + 1);
				List<int> range2 = _yCollection.GetRange(0, _yCollection.Count);
				List<int> range3 = _zCollection.GetRange(0, _zCollection.Count);
				if (num2 == _xCollection.Count - 1)
					min = this;
				else
				{
					List<int> range4 = _xCollection.GetRange(num2 + 1, _xCollection.Count - num2 - 1);
					List<int> range5 = _yCollection.GetRange(0, _yCollection.Count);
					List<int> range6 = _zCollection.GetRange(0, _zCollection.Count);
					foreach (int item in range4)
					{
						range2.Remove(item);
						range3.Remove(item);
					}
					foreach (int item2 in range)
					{
						range5.Remove(item2);
						range6.Remove(item2);
					}
					min = new PointClouds(range, range2, range3, Root);
					max = new PointClouds(range4, range5, range6, Root);
				}
			}
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x0000AC50 File Offset: 0x00008E50
		public void DivideYByDistance(out PointClouds min, out PointClouds max)
		{
			min = null;
			max = null;
			int num = (_yCollection.Last<int>() - _yCollection.First<int>()) / 2;
			int num2 = 0;
			foreach (int num3 in _yCollection)
			{
				if (num > num3)
					num2++;
			}
			if (num2 == 0)
				max = this;
			else
			{
				List<int> range = _yCollection.GetRange(0, num2 + 1);
				List<int> range2 = _xCollection.GetRange(0, _xCollection.Count);
				List<int> range3 = _zCollection.GetRange(0, _zCollection.Count);
				if (num2 == _yCollection.Count - 1)
					min = this;
				else
				{
					List<int> range4 = _yCollection.GetRange(num2 + 1, _yCollection.Count - num2 - 1);
					List<int> range5 = _xCollection.GetRange(0, _xCollection.Count);
					List<int> range6 = _zCollection.GetRange(0, _zCollection.Count);
					foreach (int item in range4)
					{
						range2.Remove(item);
						range3.Remove(item);
					}
					foreach (int item2 in range)
					{
						range5.Remove(item2);
						range6.Remove(item2);
					}
					min = new PointClouds(range2, range, range3, Root);
					max = new PointClouds(range5, range4, range6, Root);
				}
			}
		}

		// Token: 0x060000EA RID: 234 RVA: 0x0000AE60 File Offset: 0x00009060
		public void DivideZByDistance(out PointClouds min, out PointClouds max)
		{
			min = null;
			max = null;
			int num = (_zCollection.Last<int>() - _zCollection.First<int>()) / 2;
			int num2 = 0;
			foreach (int num3 in _zCollection)
			{
				if (num > num3)
					num2++;
			}

			if (num2 == 0)
				max = this;
			else
			{
				List<int> range = _zCollection.GetRange(0, num2 + 1);
				List<int> range2 = _yCollection.GetRange(0, _yCollection.Count);
				List<int> range3 = _xCollection.GetRange(0, _xCollection.Count);
				if (num2 == _zCollection.Count - 1)
					min = this;
				else
				{
					List<int> range4 = _zCollection.GetRange(num2 + 1, _zCollection.Count - num2 - 1);
					List<int> range5 = _yCollection.GetRange(0, _yCollection.Count);
					List<int> range6 = _xCollection.GetRange(0, _xCollection.Count);
					foreach (int item in range4)
					{
						range2.Remove(item);
						range3.Remove(item);
					}
					foreach (int item2 in range)
					{
						range5.Remove(item2);
						range6.Remove(item2);
					}
					min = new PointClouds(range3, range2, range, Root);
					max = new PointClouds(range6, range5, range4, Root);
				}
			}
		}

		public List<PointClouds> DivideByCount()
		{
			PointClouds pointClouds;
			PointClouds pointClouds2;
			DivideXByCount(out pointClouds, out pointClouds2);
			List<PointClouds> list = new List<PointClouds>();
			if (pointClouds != null)
			{
				PointClouds pointClouds3;
				PointClouds pointClouds4;
				pointClouds.DivideYByCount(out pointClouds3, out pointClouds4);
				if (pointClouds3 != null)
				{
					PointClouds pointClouds5;
					PointClouds pointClouds6;
					pointClouds3.DivideZByCount(out pointClouds5, out pointClouds6);
					if (pointClouds5 != null)
						list.Add(pointClouds5);

					if (pointClouds6 != null)
						list.Add(pointClouds6);
				}

				if (pointClouds4 != null)
				{
					PointClouds pointClouds5;
					PointClouds pointClouds6;
					pointClouds4.DivideZByCount(out pointClouds5, out pointClouds6);
					if (pointClouds5 != null)
						list.Add(pointClouds5);

					if (pointClouds6 != null)
						list.Add(pointClouds6);
				}
			}

			if (pointClouds2 != null)
			{
				PointClouds pointClouds3;
				PointClouds pointClouds4;
				pointClouds2.DivideYByCount(out pointClouds3, out pointClouds4);
				if (pointClouds3 != null)
				{
					PointClouds pointClouds5;
					PointClouds pointClouds6;
					pointClouds3.DivideZByCount(out pointClouds5, out pointClouds6);
					if (pointClouds5 != null)
						list.Add(pointClouds5);

					if (pointClouds6 != null)
						list.Add(pointClouds6);
				}

				if (pointClouds4 != null)
				{
					PointClouds pointClouds5;
					PointClouds pointClouds6;
					pointClouds4.DivideZByCount(out pointClouds5, out pointClouds6);
					if (pointClouds5 != null)
						list.Add(pointClouds5);

					if (pointClouds6 != null)
						list.Add(pointClouds6);
				}
			}
			return list;
		}

		public List<PointClouds> Divide8ByDistance()
		{
			PointClouds pointClouds;
			PointClouds pointClouds2;
			DivideXByCount(out pointClouds, out pointClouds2);
			List<PointClouds> list = new List<PointClouds>();
			if (pointClouds != null)
			{
				PointClouds pointClouds3;
				PointClouds pointClouds4;
				pointClouds.DivideYByDistance(out pointClouds3, out pointClouds4);
				if (pointClouds3 != null)
				{
					PointClouds pointClouds5;
					PointClouds pointClouds6;
					pointClouds3.DivideZByDistance(out pointClouds5, out pointClouds6);
					if (pointClouds5 != null)
						list.Add(pointClouds5);

					if (pointClouds6 != null)
						list.Add(pointClouds6);
				}

				if (pointClouds4 != null)
				{
					PointClouds pointClouds5;
					PointClouds pointClouds6;
					pointClouds4.DivideZByDistance(out pointClouds5, out pointClouds6);
					if (pointClouds5 != null)
						list.Add(pointClouds5);

					if (pointClouds6 != null)
						list.Add(pointClouds6);
				}
			}
			if (pointClouds2 != null)
			{
				PointClouds pointClouds3;
				PointClouds pointClouds4;
				pointClouds2.DivideYByDistance(out pointClouds3, out pointClouds4);
				if (pointClouds3 != null)
				{
					PointClouds pointClouds5;
					PointClouds pointClouds6;
					pointClouds3.DivideZByDistance(out pointClouds5, out pointClouds6);
					if (pointClouds5 != null)
						list.Add(pointClouds5);

					if (pointClouds6 != null)
						list.Add(pointClouds6);
				}

				if (pointClouds4 != null)
				{
					PointClouds pointClouds5;
					PointClouds pointClouds6;
					pointClouds4.DivideZByCount(out pointClouds5, out pointClouds6);
					if (pointClouds5 != null)
						list.Add(pointClouds5);

					if (pointClouds6 != null)
						list.Add(pointClouds6);
				}
			}
			return list;
		}

		public List<Point3d> GetPoints()
		{
			if (_points == null)
			{
				List<Point3d> list = new List<Point3d>();
				foreach (int key in _xCollection)
				{
					Point3d item;
					Root.PointDic.TryGetValue(key, out item);
					list.Add(item);
				}
				_points = list;
			}
			return _points;
		}

		public BoundingBox3D GetBox()
		{
			if (_box == null)
			{
				Dictionary<int, Point3d> pointDic = Root.PointDic;
				Point3d point;
				pointDic.TryGetValue(_xCollection.First<int>(), out point);
				int num = (int)point.X;
				pointDic.TryGetValue(_xCollection.Last<int>(), out point);
				int num2 = (int)point.X;
				pointDic.TryGetValue(_yCollection.First<int>(), out point);
				int num3 = (int)point.Y;
				pointDic.TryGetValue(_yCollection.Last<int>(), out point);
				int num4 = (int)point.Y;
				pointDic.TryGetValue(_zCollection.First<int>(), out point);
				int num5 = (int)point.Z;
				pointDic.TryGetValue(_zCollection.Last<int>(), out point);
				int num6 = (int)point.Z;
				_box = new BoundingBox3D(new Point3d((double)num, (double)num3, (double)num5), new Point3d((double)num2, (double)num4, (double)num6));
			}
			return _box;
		}

	}
}