using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RhinoInside.NX.Extensions
{
    /// <summary>Object representing coordinates in 2-dimensional space.</summary>
    /// <remarks>Usually this means parameters on a surface. In actual use, it could be
    /// interpreted as either point or vector in 2-dimensional space.</remarks>
    // Token: 0x020004C2 RID: 1218
    public class UV
    {
        /// <summary>Creates a UV with the supplied coordinates.</summary>
        /// <param name="u">The first coordinate.</param>
        /// <param name="v">The second coordinate.</param>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentException">
        /// Thrown when setting an infinite number to the U or V property.
        /// </exception>
        // Token: 0x06006587 RID: 25991 RVA: 0x001EA6B8 File Offset: 0x001E9AB8
        public UV(double u, double v)
        {
            U = u;
            V = v;
        }

        /// <summary>Gets the first coordinate.</summary>
        // Token: 0x1700005A RID: 90
        // (get) Token: 0x0600658A RID: 25994 RVA: 0x001E98C8 File Offset: 0x001E8CC8
        public double U
        {
            get;
            set;
        }

        /// <summary>Gets the second coordinate.</summary>
        // Token: 0x17000059 RID: 89
        // (get) Token: 0x0600658B RID: 25995 RVA: 0x001E98E0 File Offset: 0x001E8CE0
        public double V
        {
            get;
            set;
        }

        // Token: 0x17000058 RID: 88
        public unsafe double this[int idx]
        {
            get
            {
                if (idx == 0)
                    return U;

                if (idx != 1)
                    throw new Exception("idx can be only 0 or 1.");

                return V;
            }
        }

        /// <summary>The coordinate origin or zero 2-D vector.</summary>
        /// <remarks>The zero vector is (0,0).</remarks>
        // Token: 0x17000057 RID: 87
        // (get) Token: 0x0600658D RID: 25997 RVA: 0x001EADB8 File Offset: 0x001EA1B8
        public static UV Zero
        {
            get => new UV(0.0, 0.0);
        }

        /// <summary>The basis of the U axis.</summary>
        /// <remarks>The basis of the U axis is the vector (1,0), the unit vector on the U axis.</remarks>
        // Token: 0x17000056 RID: 86
        // (get) Token: 0x0600658E RID: 25998 RVA: 0x001EADDC File Offset: 0x001EA1DC
        public static UV BasisU
        {
            get => new UV(1.0, 0.0);
        }

        /// <summary>The basis of the V axis.</summary>
        /// <remarks>The basis of the V axis is the vector (0,1), the unit vector on the V axis.</remarks>
        // Token: 0x17000055 RID: 85
        // (get) Token: 0x0600658F RID: 25999 RVA: 0x001EAE00 File Offset: 0x001EA200
        public static UV BasisV
        {
            get => new UV(0.0, 1.0);
        }

        /// <summary>Returns a new UV whose coordinates are the normalized values from this vector.</summary>
        /// <remarks> 
        /// Normalized indicates that the length of this vector equals one 
        /// (a unit vector).
        /// </remarks>
        /// <returns>The normalized UV or zero if the vector is almost Zero.</returns>
        // Token: 0x06006590 RID: 26000 RVA: 0x001E98F8 File Offset: 0x001E8CF8
        public UV Normalize()
        {
            if (this.DotProduct(this) < 1E-09)
            {
                return new UV(0.0, 0.0);
            }
            return this / this.GetLength();
        }

        /// <summary>The length of this 2-D vector.</summary>
        /// <remarks> 
        /// In 2-D Euclidean space, the length of the vector is the square root of the sum
        /// of the two coordinates squared.
        /// </remarks>
        // Token: 0x06006591 RID: 26001 RVA: 0x001E9918 File Offset: 0x001E8D18
        public double GetLength()
        {
            return Math.Sqrt(V * V + U * U);
        }

        /// <summary>The boolean value indicates whether this 2-D vector is a zero vector.</summary>
        /// <remarks>The zero vector's each component is zero within the tolerance (1.0e-09).</remarks>
        public bool IsZeroLength => Math.Abs(U) < 1E-09 && Math.Abs(V) < 1E-09;

        /// <summary>The boolean value indicates whether this 2-D vector is of unit length.</summary>
        /// <remarks> 
        /// A unit length vector has a length of one, and is considered normalized.
        /// </remarks>
        // Token: 0x06006593 RID: 26003 RVA: 0x001E9948 File Offset: 0x001E8D48
        [return: MarshalAs(UnmanagedType.U1)]
        public bool IsUnitLength() => (Math.Abs(GetLength() - 1.0) >= 1E-09) ? false : true;

        /// <summary>Adds the two specified 2-D vectors and returns the result.</summary>
        /// <param name="left">The first vector.</param>
        /// <param name="right">The second vector.</param>
        /// <returns>The 2-D vector equal to the sum of the two source vectors.</returns>
        /// <remarks>
        /// The added vector is obtained by adding each coordinate of the right vector
        /// to the corresponding coordinate of the left vector.
        /// </remarks>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentNullException">
        /// Thrown when left or right is <see langword="null" />.
        /// </exception>
        // Token: 0x06006594 RID: 26004 RVA: 0x001ED370 File Offset: 0x001EC770
        public static UV operator +(UV left, UV right)
        {
            if (null == left)
                throw new Exception("left value is null.");

            if (null == right)
                throw new Exception("right value is null.");

            return left.Add(right);
        }

        /// <summary>Negates this 2-D vector and returns the result.</summary>
        /// <returns>The 2-D vector opposite to this vector.</returns>
        /// <remarks>
        /// The negated vector is obtained by changing the sign of each coordinate 
        /// of the specified vector.
        /// </remarks>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentNullException">
        /// Thrown when source is <see langword="null" />.
        /// </exception>
        // Token: 0x06006595 RID: 26005 RVA: 0x001ED428 File Offset: 0x001EC828
        public static UV operator -(UV source)
        {
            if (null == source)
                throw new Exception("source value is null.");

            return source.Negate();
        }

        /// <summary>Subtracts the two specified 2-D vectors and returns the result.</summary>
        /// <param name="left">The first vector.</param>
        /// <param name="right">The second vector.</param>
        /// <returns>The 2-D vector equal to the difference between the two source vectors.</returns>
        /// <remarks>
        /// The subtracted vector is obtained by subtracting each coordinate of 
        /// the right vector from the corresponding coordinate of the left vector.
        /// </remarks>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentNullException">
        /// Thrown when left or right is <see langword="null" />.
        /// </exception>
        // Token: 0x06006596 RID: 26006 RVA: 0x001ED3CC File Offset: 0x001EC7CC
        public static UV operator -(UV left, UV right)
        {
            if (null == left)
                throw new Exception("left value is null.");

            if (null == right)
                throw new Exception("right value is null.");

            return left.Subtract(right);
        }

        /// <summary>The product of the specified number and the specified 2-D vector.</summary>
        /// <param name="left">The vector to multiply with the value.</param>
        /// <param name="value">The value to multiply with the specified vector.</param>
        /// <returns>The multiplied 2-D vector.</returns>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentException">
        /// Thrown when the specified value is an infinite number.
        /// </exception>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentNullException">
        /// Thrown when left is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// The multiplied vector is obtained by multiplying each coordinate of 
        /// the specified vector by the specified value.
        /// </remarks>
        // Token: 0x06006597 RID: 26007 RVA: 0x001ED4C4 File Offset: 0x001EC8C4
        public static UV operator *(UV left, double value)
        {
            if (null == left)
                throw new Exception("left value is null.");

            return left.Multiply(value);
        }

        /// <summary>The product of the specified number and the specified 2-D vector.</summary>
        /// <param name="value">The value to multiply with the specified vector.</param>
        /// <param name="right">The vector to multiply with the value.</param>
        /// <returns>The multiplied 2-D vector.</returns>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentException">
        /// Thrown when the specified value is an infinite number.
        /// </exception>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentNullException">
        /// Thrown when right is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// The multiplied vector is obtained by multiplying each coordinate of 
        /// the specified vector by the specified value.
        /// </remarks>
        // Token: 0x06006598 RID: 26008 RVA: 0x001ED460 File Offset: 0x001EC860
        public static UV operator *(double value, UV right)
        {
            if (null == right)
                throw new Exception("right value is null.");

            return right.Multiply(value);
        }

        /// <summary>Divides the specified 2-D vector by the specified value.</summary>
        /// <param name="left">The value to divide the vector by.</param>
        /// <param name="value">The vector to divide by the value.</param>
        /// <returns>The divided 2-D vector.</returns>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentException">
        /// Thrown when the specified value is an infinite number or zero.
        /// </exception>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentNullException">
        /// Thrown when left is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// The divided vector is obtained by dividing each coordinate of 
        /// the specified vector by the specified value.
        /// </remarks>
        // Token: 0x06006599 RID: 26009 RVA: 0x001ED528 File Offset: 0x001EC928
        public static UV operator /(UV left, double value)
        {
            if (null == left)
                throw new Exception("left value is null.");

            if (Math.Abs(value) < 1E-09)
                throw new Exception("Cannot devide by zero.");

            return left.Divide(value);
        }

        /// <summary>The dot product of this 2-D vector and the specified 2-D vector.</summary>
        /// <param name="source">The vector to multiply with this vector.</param>
        /// <returns>The real number equal to the dot product.</returns>
        /// <remarks>
        /// The dot product is the sum of the respective coordinates of the two vectors: Pu * Ru + Pv * Rv.
        /// </remarks>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentNullException">
        /// Thrown when source is <see langword="null" />.
        /// </exception>
        // Token: 0x0600659A RID: 26010 RVA: 0x001EAE24 File Offset: 0x001EA224
        public unsafe double DotProduct(UV source)
        {
            if (null == source)
                throw new Exception("source value is null.");

            return V * V + U * U;
        }

        /// <summary>The cross product of this 2-D vector and the specified 2-D vector.</summary>
        /// <param name="source">The vector to multiply with this vector.</param>
        /// <returns>The real number equal to the cross product.</returns>
        /// <remarks>
        /// The cross product of the two vectors in 2-D space is equivalent to the area of the parallelogram they span.
        /// </remarks>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentNullException">
        /// Thrown when source is <see langword="null" />.
        /// </exception>
        // Token: 0x0600659B RID: 26011 RVA: 0x001EAF9C File Offset: 0x001EA39C
        public unsafe double CrossProduct(UV source)
        {
            if (null == source)
                throw new Exception("source value is null.");

            return source.V * this.U - source.U * this.V;
        }

        /// <summary>Adds the specified 2-D vector to this 2-D vector and returns the result.</summary>
        /// <param name="source">The vector to add to this vector.</param>
        /// <returns>The 2-D vector equal to the sum of the two vectors.</returns>
        /// <remarks>
        /// The added vector is obtained by adding each coordinate of the specified vector
        /// to the corresponding coordinate of this vector.
        /// </remarks>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentNullException">
        /// Thrown when source is <see langword="null" />.
        /// </exception>
        // Token: 0x0600659C RID: 26012 RVA: 0x001EB114 File Offset: 0x001EA514
        public unsafe UV Add(UV source)
        {
            if (null == source)
                throw new Exception("source value is null.");

            return new UV(source.U + U, source.V + V);
        }

        /// <summary>Subtracts the specified 2-D vector from this 2-D vector and returns the result.</summary>
        /// <param name="source">The vector to subtract from this vector.</param>
        /// <returns>The 2-D vector equal to the difference between the two vectors.</returns>
        /// <remarks>
        /// The subtracted vector is obtained by subtracting each coordinate of 
        /// the specified vector from the corresponding coordinate of this vector.
        /// </remarks>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentNullException">
        /// Thrown when left is <see langword="null" />.
        /// </exception>
        // Token: 0x0600659D RID: 26013 RVA: 0x001EB288 File Offset: 0x001EA688
        public unsafe UV Subtract(UV source)
        {
            if (null == source)
                throw new Exception("source value is null.");

            return new UV(U - source.U, V - source.V);
        }

        /// <summary>Negates this 2-D vector.</summary>
        /// <returns>The 2-D vector opposite to this vector.</returns>
        /// <remarks>
        /// The negated vector is obtained by changing the sign of each coordinate 
        /// of this vector.
        /// </remarks>
        // Token: 0x0600659E RID: 26014 RVA: 0x001EB3FC File Offset: 0x001EA7FC
        public UV Negate()
        {
            return new UV(-U, -V);
        }

        /// <summary>Multiplies this 2-D vector by the specified value and returns the result.</summary>
        /// <param name="value">The value to multiply with this vector.</param>
        /// <returns>The multiplied 2-D vector.</returns>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentException">
        /// Thrown when the specified value is an infinite number.
        /// </exception>
        /// <remarks>
        /// The multiplied vector is obtained by multiplying each coordinate of 
        /// this vector by the specified value.
        /// </remarks>
        // Token: 0x0600659F RID: 26015 RVA: 0x001EB42C File Offset: 0x001EA82C
        public unsafe UV Multiply(double value)
        {
            return new UV(U * value, V * value);
        }

        /// <summary>Divides this 2-D vector by the specified value and returns the result.</summary>
        /// <param name="value">The value to divide this vector by.</param>
        /// <returns>The divided 2-D vector.</returns>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentException">
        /// Thrown when the specified value is an infinite number.
        /// </exception>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentException">
        /// Thrown when the specified value is zero.
        /// </exception>
        /// <remarks>
        /// The divided vector is obtained by dividing each coordinate of 
        /// this vector by the specified value.
        /// </remarks>
        // Token: 0x060065A0 RID: 26016 RVA: 0x001EB574 File Offset: 0x001EA974
        public unsafe UV Divide(double value)
        {
            if (Math.Abs(value) < 1E-09)
                throw new Exception("Cannot divide by zero.");

            return new UV(U / value, V / value);
        }

        /// <summary>Determines whether this 2-D vector and the specified 2-D vector are the same within a specified tolerance.</summary>
        /// <param name="source">The vector to compare with this vector.</param>
        /// <param name="tolerance">The tolerance for equality check.</param>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentNullException">
        /// Thrown when source is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentException">
        /// Thrown when tolerance is less than 0.
        /// </exception>
        /// <returns>True if the vectors are the same; otherwise, false.</returns>
        // Token: 0x060065A1 RID: 26017 RVA: 0x001EA938 File Offset: 0x001E9D38
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe bool IsAlmostEqualTo(UV source, double tolerance)
        {
            if (null == source)
                throw new Exception("source value is null.");

            if (tolerance < 0.0)
                throw new Exception("tolerance must be no less than zero.");

            double num = tolerance * tolerance;
            double num2 = source.DotProduct(source) + this.DotProduct(this);
            if (num2 < num)
            {
                return true;
            }
            UV uvproxy = this - source;
            return (uvproxy.DotProduct(uvproxy) >= num2 * num) ? false : true;
        }

        /// <summary>Determines whether this 2-D vector and the specified 2-D vector are the same within the tolerance (1.0e-09).</summary>
        /// <param name="source">The vector to compare with this vector.</param>
        /// <returns>True if the vectors are the same; otherwise, false.</returns>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentNullException">
        /// Thrown when left is <see langword="null" />.
        /// </exception>
        // Token: 0x060065A2 RID: 26018 RVA: 0x001EAAA0 File Offset: 0x001E9EA0
        [return: MarshalAs(UnmanagedType.U1)]
        public bool IsAlmostEqualTo(UV source)=> IsAlmostEqualTo(source, 1E-09);

        /// <summary>Returns the distance from this 2-D point to the specified 2-D point.</summary>
        /// <param name="source">The specified point.</param>
        /// <returns>The real number equal to the distance between the two points.</returns>
        /// <remarks>
        /// The distance between the two points is equal to the length of the vector
        /// that joins the two points.
        /// </remarks>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentNullException">
        /// Thrown when source is <see langword="null" />.
        /// </exception>
        // Token: 0x060065A3 RID: 26019 RVA: 0x001EAAE0 File Offset: 0x001E9EE0
        public unsafe double DistanceTo(UV source)
        {            
            if (null == source)
                throw new Exception("source value is null.");

            return (this - source).GetLength();
        }

        /// <summary>Returns the angle between this vector and the specified vector.</summary>
        /// <param name="source">The specified vector.</param>
        /// <returns>The real number between 0 and 2*PI equal to the angle between the two vectors in radians.</returns>
        /// <exception cref="T:Autodesk.Revit.Exceptions.ArgumentNullException">
        /// Thrown when source is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// The angle is measured counterclockwise.
        /// </remarks>
        // Token: 0x060065A4 RID: 26020 RVA: 0x001EAC4C File Offset: 0x001EA04C
        public unsafe double AngleTo(UV source)
        {
            if (null == source)
                throw new Exception("source value is null.");

            double x = this.DotProduct(source);
            double num = Math.Atan2(this.CrossProduct(source), x);
            double result;
            if (num < 0.0)
            {
                result = num + 6.2831853071795862;
            }
            else
            {
                result = num;
            }
            return result;
        }

        /// <summary>
        /// Gets formatted string showing (U, V) with values formatted to 9 decimal places. 
        /// </summary>
        // Token: 0x060065A5 RID: 26021 RVA: 0x001E9960 File Offset: 0x001E8D60
        public override string ToString()
        {
            if (IsZeroLength)
                return "(0.000000000, 0.000000000)";
            else
                return $"{U.ToString("0.000000000")},{V.ToString("0.000000000")}";
        }
    }
}
