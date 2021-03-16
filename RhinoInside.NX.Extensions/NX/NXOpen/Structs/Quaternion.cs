using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
	public struct Quaternion
	{
		#region 字段
		public double Q1;

		public double Q2;

		public double Q3;

		public double Q4;

		public static readonly Quaternion Identity = new Quaternion(1.0, 0.0, 0.0, 0.0);

		#endregion
		#region 构造函数
		public Quaternion(double _q1, double _q2, double _q3, double _q4)
		{
			Q1 = _q1;
			Q2 = _q2;
			Q3 = _q3;
			Q4 = _q4;
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000DE83 File Offset: 0x0000C083
		public Quaternion(double w, Vector4d vec)
		{
			Q1 = w;
			Q2 = vec.X;
			Q3 = vec.Y;
			Q4 = vec.Z;
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000DEB1 File Offset: 0x0000C0B1
		public Quaternion(Vector3d axis, double angle)
		{
			this = Quaternion.Identity;
			AxisAngle = new Vector4d(axis, angle);
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000DEEA File Offset: 0x0000C0EA
		public Quaternion(Matrix4x4 m)
		{
			this = Identity;
			Matrix = m.GetRotation();
		}
		#endregion

		#region 属性
		public double Scalar
		{
			get
			{
				return Q1;
			}
			set
			{
				Q1 = value;
			}
		}

		public Vector3d Vector
		{
			get
			{
				return new Vector3d(Q2, Q3, Q4);
			}
			set
			{
				Q2 = value.X;
				Q3 = value.Y;
				Q4 = value.Z;
			}
		}

		public Vector4d AxisAngle
		{
			get
			{
				double num = Math.Sqrt(1.0 - Q1 * Q1);
				if (num < 1E-10)
					num = 1.0;

				return new Vector4d(Q2 / num, Q3 / num, Q4 / num, 2.0 * Math.Acos(Clamp(Q1)));
			}
			set
			{
				if (value.Length < Globals.DistanceTolerance)
				{
					this = Quaternion.Identity;
				}
				else
				{
					if (double.IsNaN(value.X) || double.IsNaN(value.Y) || double.IsNaN(value.Z) || value.X * value.X + value.Y * value.Y + value.Z * value.Z == 0.0)
					{
						throw new ArgumentException("Invalid rotation axis");
					}
					double num = value.W * 0.5;
					double num2 = Math.Sin(num);
					double q = Math.Cos(num);
					Q1 = q;
					Q2 = value.X * num2;
					Q3 = value.Y * num2;
					Q4 = value.Z * num2;
				}
			}
		}
		#endregion

		#region 基本重载函数
		public double this[int index]
		{
			get
			{
				double result;
				if (index == 0)
					result = Q1;
				else
				{
					if (index == 1)
						result = Q2;
					else
					{
						if (index != 2)
							result = Q4;
						else
							result = Q3;
					}
				}
				return result;
			}
			set
			{
				if (index == 0)
					Q1 = value;
				else
				{
					if (index == 1)
						Q2 = value;
					else
					{
						if (index == 2)
							Q3 = value;
						else
							Q4 = value;
					}
				}
			}
		}
		#endregion

		public Matrix3x3 Matrix
		{
			get
			{
				Matrix3x3 identity = Matrix3x3Ex.Identity;
				double num = 2.0 / Norm();
				identity.Xx = 1.0 - num * (Q3 * Q3 + Q4 * Q4);
				identity.Xy = num * (Q2 * Q3 + Q1 * Q4);
				identity.Xz = num * (Q2 * Q4 - Q1 * Q3);
				identity.Yx = num * (Q2 * Q3 - Q1 * Q4);
				identity.Yy = 1.0 - num * (Q2 * Q2 + Q4 * Q4);
				identity.Yz = num * (Q3 * Q4 + Q1 * Q2);
				identity.Zx = num * (Q2 * Q4 + Q1 * Q3);
				identity.Zy = num * (Q3 * Q4 - Q1 * Q2);
				identity.Zz = 1.0 - num * (Q2 * Q2 + Q3 * Q3);
				return identity;
			}
			set
			{
				double num = value.Xx + value.Yy + value.Zz + 1.0;
				if (num > 1E-10)
				{
					double num2 = 0.5 / Math.Sqrt(num);
					Q1 = 0.25 / num2;
					Q2 = (value.Yz - value.Zy) * num2;
					Q3 = (value.Zx - value.Xz) * num2;
					Q4 = (value.Xy - value.Yx) * num2;
				}
				else
				{
					if (value.Xx > value.Yy && value.Xx > value.Zz)
					{
						double num3 = 0.5 / Math.Sqrt(1.0 + value.Xx - value.Yy- value.Zz);
						Q1 = (value.Zy - value.Yz) * num3;
						Q2 = 0.25 / num3;
						Q3 = (value.Yx + value.Xy) * num3;
						Q4 = (value.Zx+ value.Xz) * num3;
					}
					else
					{
						if (value.Yy > value.Zz)
						{
							double num4 = 0.5 / Math.Sqrt(1.0 + value.Yy- value.Xx - value.Zz);
							Q1 = (value.Zx- value.Xz) * num4;
							Q2 = (value.Yx + value.Xy) * num4;
							Q3 = 0.25 / num4;
							Q4 = (value.Zy + value.Yz) * num4;
						}
						else
						{
							double num5 = 0.5 / Math.Sqrt(1.0 + value.Zz - value.Xx - value.Yy);
							Q1 = (value.Yx - value.Xy) * num5;
							Q2 = (value.Zx+ value.Xz) * num5;
							Q3 = (value.Zy + value.Yz) * num5;
							Q4 = 0.25 / num5;
						}
					}
				}
				Normalize();
			}
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000DE63 File Offset: 0x0000C063


		// Token: 0x0600020F RID: 527 RVA: 0x0000DF24 File Offset: 0x0000C124
		public static bool operator ==(Quaternion lhs, Quaternion rhs)
		{
			return lhs.Q1 == rhs.Q1 && lhs.Q2 == rhs.Q2 && lhs.Q3 == rhs.Q3 && lhs.Q4 == rhs.Q4;
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000DF74 File Offset: 0x0000C174
		public static bool operator !=(Quaternion lhs, Quaternion rhs)
		{
			return !(lhs == rhs);
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000DF90 File Offset: 0x0000C190
		public override bool Equals(object obj)
		{
			return obj is Quaternion && this == (Quaternion)obj;
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000DFC0 File Offset: 0x0000C1C0
		public override int GetHashCode()
		{
			return Q1.GetHashCode() ^ Q2.GetHashCode() ^ Q3.GetHashCode() ^ Q4.GetHashCode();
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000E004 File Offset: 0x0000C204
		public static Quaternion operator +(Quaternion lhs, Quaternion rhs)
		{
			return new Quaternion(lhs.Q1 + rhs.Q1, lhs.Q2 + rhs.Q2, lhs.Q3 + rhs.Q3, lhs.Q4 + rhs.Q4);
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000E050 File Offset: 0x0000C250
		public Quaternion Add(Quaternion rhs)
		{
			return new Quaternion(Q1 + rhs.Q1, Q2 + rhs.Q2, Q3 + rhs.Q3, Q4 + rhs.Q4);
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000E09C File Offset: 0x0000C29C
		public static Quaternion operator -(Quaternion lhs, Quaternion rhs)
		{
			return new Quaternion(lhs.Q1 - rhs.Q1, lhs.Q2 - rhs.Q2, lhs.Q3 - rhs.Q3, lhs.Q4 - rhs.Q4);
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000E0E8 File Offset: 0x0000C2E8
		public Quaternion Subtract(Quaternion rhs)
		{
			return new Quaternion(Q1 - rhs.Q1, Q2 - rhs.Q2, Q3 - rhs.Q3, Q4 - rhs.Q4);
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000E134 File Offset: 0x0000C334
		public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
		{
			return lhs.Multiply(rhs);
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000E150 File Offset: 0x0000C350
		public Quaternion Multiply(Quaternion rhs)
		{
			return new Quaternion(Q1 * rhs.Q1 - Q2 * rhs.Q2 - Q3 * rhs.Q3 - Q4 * rhs.Q4, Q1 * rhs.Q2 + Q2 * rhs.Q1 + Q3 * rhs.Q4 - Q4 * rhs.Q3, Q1 * rhs.Q3 - Q2 * rhs.Q4 + Q3 * rhs.Q1 + Q4 * rhs.Q2, Q1 * rhs.Q4 + Q2 * rhs.Q3 - Q3 * rhs.Q2 + Q4 * rhs.Q1);
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000E244 File Offset: 0x0000C444
		public static Quaternion operator *(Quaternion lhs, double rhs)
		{
			return new Quaternion(lhs.Q1 * rhs, lhs.Q2 * rhs, lhs.Q3 * rhs, lhs.Q4 * rhs);
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000E27C File Offset: 0x0000C47C
		public Quaternion Multiply(double rhs)
		{
			return new Quaternion(Q1 * rhs, Q2 * rhs, Q3 * rhs, Q4 * rhs);
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000E2B4 File Offset: 0x0000C4B4
		public static Quaternion operator *(double lhs, Quaternion rhs)
		{
			return new Quaternion(lhs * rhs.Q1, lhs * rhs.Q2, lhs * rhs.Q3, lhs * rhs.Q4);
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000E2EC File Offset: 0x0000C4EC
		public static Quaternion operator /(Quaternion lhs, double rhs)
		{
			return new Quaternion(lhs.Q1 / rhs, lhs.Q2 / rhs, lhs.Q3 / rhs, lhs.Q4 / rhs);
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000E324 File Offset: 0x0000C524
		public Quaternion Divide(double rhs)
		{
			return new Quaternion(Q1 / rhs, Q2 / rhs, Q3 / rhs, Q4 / rhs);
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000E35C File Offset: 0x0000C55C
		public static Quaternion operator -(Quaternion rhs)
		{
			return new Quaternion(-rhs.Q1, -rhs.Q2, -rhs.Q3, -rhs.Q4);
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000E390 File Offset: 0x0000C590
		public double Norm()
		{
			return Q1 * Q1 + Q2 * Q2 + Q3 * Q3 + Q4 * Q4;
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000E3DC File Offset: 0x0000C5DC
		public double Magnitude()
		{
			return Math.Sqrt(Q1 * Q1 + Q2 * Q2 + Q3 * Q3 + Q4 * Q4);
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000E42C File Offset: 0x0000C62C
		public void Normalize()
		{
			double num = Magnitude();
			Q1 /= num;
			Q2 /= num;
			Q3 /= num;
			Q4 /= num;
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000E47C File Offset: 0x0000C67C
		public void Invert()
		{
			double num = 1.0 / Norm();
			Q1 *= num;
			Q2 = -Q2 * num;
			Q3 = -Q3 * num;
			Q4 = -Q4 * num;
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000E4D8 File Offset: 0x0000C6D8
		public Quaternion Inverse()
		{
			double num = 1.0 / Norm();
			return new Quaternion(Q1 * num, -Q2 * num, -Q3 * num, -Q4 * num);
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000E524 File Offset: 0x0000C724
		public Quaternion Conjugate()
		{
			return new Quaternion(Q1, -Q2, -Q3, -Q4);
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000E558 File Offset: 0x0000C758
		public double Dot(Quaternion rhs)
		{
			return Q1 * rhs.Q1 + Q2 * rhs.Q2 + Q3 * rhs.Q3 + Q4 * rhs.Q4;
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000E5A4 File Offset: 0x0000C7A4
		public Quaternion Interpolate(Quaternion quat, double slerp)
		{
			double num = Dot(quat);
			Quaternion result;
			if (num < 0.0)
				result = Interpolate(-quat, slerp);
			else
			{
				double rhs;
				double rhs2;
				if (num > -0.9999999999 && num < 0.9999999999)
				{
					double num2 = Math.Acos(num);
					double num3 = Math.Sin(num2);
					rhs = Math.Sin((1.0 - slerp) * num2) / num3;
					rhs2 = Math.Sin(slerp * num2) / num3;
				}
				else
				{
					rhs = 1.0 - slerp;
					rhs2 = slerp;
				}
				result = this * rhs + quat * rhs2;
			}
			return result;
		}

		public override string ToString()
		{
			return string.Format("[{0} {1} {2} {3}]", new object[]
			{
				Q1,
				Q2,
				Q3,
				Q4
			});
		}

		internal static double Clamp(double d)
		{
			double result;
			if (d < -1.0)
				result = -1.0;
			else
			{
				if (d > 1.0)
					result = 1.0;
				else
					result = d;
			}
			return result;
		}
	}
}
