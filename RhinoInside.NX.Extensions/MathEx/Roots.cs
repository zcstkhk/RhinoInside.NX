using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Extensions
{
    public static class Roots
    {
        /// <summary>Finds a single real root of a given function f in an interval [x1, x2] </summary>
        /// <param name="func">The function whose root we want to find</param>
        /// <param name="data">Data to be passed to the function f</param>
        /// <param name="x1">Lower limit of interval</param>
        /// <param name="x2">Upper limit of interval</param>
        /// <param name="tol">Tolerance</param>
        /// <param name="maxiter">Maximum number of iterations allowed</param>
        /// <returns>A root x with x1 &lt; x &lt; x2</returns>
        /// <remarks>
        /// The function must have different signs at the end-points opf the given interval.
        /// In other words, f(x1) and f(x2) must have different signs.
        /// </remarks>
        /// <exception cref="T:System.ArgumentException">f(x1) and f(x2) have the same sign</exception>
        /// <exception cref="T:System.InvalidOperationException">Maximum number of iterations exceeded</exception>
        // Token: 0x06000320 RID: 800 RVA: 0x00031AF8 File Offset: 0x0002FCF8
        public static double FindRealRoot(MathEx.DoubleFunction func, object data, double x1, double x2, double tol, int maxiter)
        {
            double num = 3E-08;
            double num2 = x1;
            double num3 = x2;
            double num4 = x2;
            double num5 = 0.0;
            double num6 = 0.0;
            double num7 = func(data, num2);
            double num8 = func(data, num3);
            if ((num7 > 0.0 && num8 > 0.0) || (num7 < 0.0 && num8 < 0.0))
            {
                throw new ArgumentException("f(x1) and f(x2) have the same sign");
            }
            double num9 = num8;
            for (int i = 1; i <= maxiter; i++)
            {
                if ((num8 > 0.0 && num9 > 0.0) || (num8 < 0.0 && num9 < 0.0))
                {
                    num4 = num2;
                    num9 = num7;
                    num5 = (num6 = num3 - num2);
                }
                if (Math.Abs(num9) < Math.Abs(num8))
                {
                    num2 = num3;
                    num3 = num4;
                    num4 = num2;
                    num7 = num8;
                    num8 = num9;
                    num9 = num7;
                }
                double num10 = 2.0 * num * Math.Abs(num3) + 0.5 * tol;
                double num11 = 0.5 * (num4 - num3);
                if (Math.Abs(num11) <= num10 || num8 == 0.0)
                {
                    return num3;
                }
                if (Math.Abs(num6) >= num10 && Math.Abs(num7) > Math.Abs(num8))
                {
                    double num12 = num8 / num7;
                    double num13;
                    double num14;
                    if (num2 == num4)
                    {
                        num13 = 2.0 * num11 * num12;
                        num14 = 1.0 - num12;
                    }
                    else
                    {
                        num14 = num7 / num9;
                        double num15 = num8 / num9;
                        num13 = num12 * (2.0 * num11 * num14 * (num14 - num15) - (num3 - num2) * (num15 - 1.0));
                        num14 = (num14 - 1.0) * (num15 - 1.0) * (num12 - 1.0);
                    }
                    if (num13 > 0.0)
                    {
                        num14 = -num14;
                    }
                    num13 = Math.Abs(num13);
                    double num16 = 3.0 * num11 * num14 - Math.Abs(num10 * num14);
                    double num17 = Math.Abs(num6 * num14);
                    if (2.0 * num13 < ((num16 < num17) ? num16 : num17))
                    {
                        num6 = num5;
                        num5 = num13 / num14;
                    }
                    else
                    {
                        num5 = num11;
                        num6 = num5;
                    }
                }
                else
                {
                    num5 = num11;
                    num6 = num5;
                }
                num2 = num3;
                num7 = num8;
                if (Math.Abs(num5) > num10)
                {
                    num3 += num5;
                }
                else
                {
                    num3 += (double)Math.Sign(num10) * num11;
                }
                num8 = func(data, num3);
            }
            throw new InvalidOperationException("Maximum number of iterations exceeded");
        }

        /// <summary>Find the real roots of a quadratic equation a*x^2 + b*x + c = 0</summary>
        /// <param name="a">Coefficient of x^2</param>
        /// <param name="b">Coefficient of x</param>
        /// <param name="c">Constant term</param>
        /// <returns>The real roots</returns>
        /// <remarks>
        /// The returned array may have length zero, one, or two. 
        /// <para>
        /// One root will be returned only if the coefficient a is zero. In other cases, either zero
        /// or two roots will be returned. If the equation has two equal roots, then these will 
        /// be returned as two individual elements of the array.
        /// </para>
        /// <para>
        /// When two roots are returned, the first one will be the smaller of the two.
        /// </para>
        /// </remarks>
        /// <exception cref="T:System.ArgumentException">The coefficients a and b are both zero</exception>
        // Token: 0x06000321 RID: 801 RVA: 0x00031DB0 File Offset: 0x0002FFB0
        public static double[] FindRealRoots(double a, double b, double c)
        {
            if (Math.Abs(a) < 4.94065645841247E-323)
            {
                if (Math.Abs(b) < 4.94065645841247E-323)
                {
                    throw new ArgumentException("Coefficients a and b are both zero");
                }
                return new double[]
                {
                        -c / b
                };
            }
            else
            {
                double num = b * b - 4.0 * a * c;
                if (num < 0.0)
                {
                    return new double[0];
                }
                double num2 = Math.Sqrt(num);
                double num3;
                double num4;
                if (b > 0.0)
                {
                    num3 = (-b - num2) / (2.0 * a);
                    num4 = c / num3;
                }
                else
                {
                    num3 = (-b + num2) / (2.0 * a);
                    num4 = c / num3;
                }
                double[] array = new double[]
                {
                        num3,
                        num4
                };
                Array.Sort<double>(array);
                return array;
            }
        }
    }
}
