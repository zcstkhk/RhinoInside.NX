using System;
using NXOpen.UF;
using static RhinoInside.NX.Extensions.Globals;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
    /// <summary>Holds the results of a mass properties calculation for a collection of bodies</summary>
    public class MassProperties
    {
        public MassProperties(double[] massProp)
        {
            Area = massProp[0];
            Volume = massProp[1];
            Mass = massProp[2];
            Centroid = new Point3d(massProp[3], massProp[4], massProp[5]);
            MomentsOfInertia = new double[]
            {
                massProp[9],
                massProp[10],
                massProp[11]
            };
            ProductsOfInertia = new double[]
            {
                massProp[16],
                massProp[17],
                massProp[18]
            };
            Vector3d vector = new Vector3d(massProp[22], massProp[23], massProp[24]);
            Vector3d vector2 = new Vector3d(massProp[25], massProp[26], massProp[27]);
            Vector3d vector3 = new Vector3d(massProp[28], massProp[29], massProp[30]);
            PrincipalAxes = new Vector3d[]
            {
                vector,
                vector2,
                vector3
            };
            PrincipalMoments = new double[]
            {
                massProp[31],
                massProp[32],
                massProp[33]
            };
            RadiusOfGyration = massProp[40];
            Density = massProp[46];
        }

        public MassProperties(UFWeight.Properties wp)
        {
            Area = wp.area;
            Centroid = wp.center_of_mass.ToPoint3d();
            Density = wp.density;
            Mass = wp.mass;
            MomentsOfInertia = wp.moments_of_inertia;
            ProductsOfInertia = wp.products_of_inertia;
            Units = wp.units;
            Volume = wp.volume;
            RadiusOfGyration = -1.0;
            if (wp.volume < 1E-10)
            {
                Density = Mass / Area;
            }
        }

        /// <summary>The total surface area of the collection of bodies</summary>
        public double Area { get; internal set; }

        /// <summary>The total volume of the collection of bodies</summary>
        public double Volume { get; internal set; }

        /// <summary>The total mass of the collection of bodies</summary>
        public double Mass { get; internal set; }

        /// <summary>The centroid of the collection of bodies</summary>
        public Point3d Centroid { get; internal set; }

        /// <summary>The principal axes of the collection of bodies at the centroid, in same order as principal moments</summary>
        /// <remarks>
        /// <para>
        /// </para>
        /// The axes are unit vectors
        /// </remarks>
        public Vector3d[] PrincipalAxes { get; internal set; }

        /// <summary>The principal moments of the collection of bodies at the centroid, in order, largest first</summary>
        public double[] PrincipalMoments { get; internal set; }

        /// <summary>The radius of gyration of the collection of bodies</summary>
        /// <remarks>This is returned only when the input is an array of solid bodies</remarks>
        public double RadiusOfGyration { get; internal set; }

        /// <summary>The density of the overall collection of bodies (total mass divided by total volume or area)</summary>
        /// <remarks>
        /// <para>
        /// The meaning and units of this property depend on the types of bodies being measured:
        /// <list type="bullet">
        /// <item>If the given bodies are all solids, the unit is gm/mm^3;</item>
        /// <item>If the given bodies are all sheets, the unit is gm/mm^2;</item>
        /// <item>If the given bodies are a mixture of solids and sheets, this value has no meaning.</item>
        /// </list>
        /// </para>
        /// </remarks>
        public double Density { get; internal set; }

        /// <summary>The inertia tensor at the centroid</summary>
        /// <remarks>
        /// <para>
        /// The inertia tensor is a symmetric 3×3 matrix whose entries are moments
        /// and products of inertia. It can be used to compute the moment of inertia
        /// around any axis through the centroid of the collection of bodies, although the
        /// <see cref="M:Snap.Compute.MomentOfInertia(Snap.Position,Snap.Vector,Snap.NX.Body[])">"Snap.Compute.MomentOfInertia"</see> function
        /// provides an easier way to do this.
        /// </para>
        /// <para>
        /// If we denote the inertia tensor by M, then we can immediately obtain moments of 
        /// inertia around certain specific lines through the centroid:
        /// <list type="bullet">
        /// <item>M[0,0] is the moment of inertia around a line parallel to the x-axis</item>
        /// <item>M[1,1] is the moment of inertia around a line parallel to the y-axis</item>
        /// <item>M[2,2] is the moment of inertia around a line parallel to the z-axis</item>
        /// </list>
        /// </para>
        /// </remarks>
        public double[,] InertiaTensor { get; internal set; }

        /// <summary>Moments of inertia about X, Y, Z axes (Ixx, Iyy, Izz)</summary>
        internal double[] MomentsOfInertia { get; set; }

        /// <summary>Products of inertia about X, Y, Z axes (Myz, Mzx, Mzy, in that order)</summary>
        internal double[] ProductsOfInertia { get; set; }

        internal UFWeight.UnitsType Units;

        /// <summary>Fills in missing information for principal moments and principal axes</summary>
        /// <returns>Results with principal moments and principal axes added/corrected</returns>
        internal void CompleteResults()
        {
            double[,] array = new double[3, 3];
            array[0, 0] = MomentsOfInertia[0];
            array[1, 1] = MomentsOfInertia[1];
            array[2, 2] = MomentsOfInertia[2];
            array[0, 1] = -ProductsOfInertia[2];
            array[0, 2] = -ProductsOfInertia[1];
            array[1, 2] = -ProductsOfInertia[0];
            double mass = Mass;
            Vector3d vector = Centroid.ToVector3d();
            double num = Math.Pow(vector.GetLength(), 2);
            double[,] array2 = new double[3, 3];
            array2[0, 0] = array[0, 0] - mass * (num - vector.X * vector.X);
            array2[1, 1] = array[1, 1] - mass * (num - vector.Y * vector.Y);
            array2[2, 2] = array[2, 2] - mass * (num - vector.Z * vector.Z);
            array2[0, 1] = array[0, 1] + mass * vector.X * vector.Y;
            array2[1, 0] = array2[0, 1];
            array2[0, 2] = array[0, 2] + mass * vector.X * vector.Z;
            array2[2, 0] = array2[0, 2];
            array2[1, 2] = array[1, 2] + mass * vector.Y * vector.Z;
            array2[2, 1] = array2[1, 2];
            LinearAlgebra.EigenSystemResult[] array3 = LinearAlgebra.EigenSystem(array2);
            PrincipalMoments = new double[]
            {
            array3[0].Eigenvalue,
            array3[1].Eigenvalue,
            array3[2].Eigenvalue
            };
            PrincipalAxes = new Vector3d[]
            {
            array3[0].Eigenvector.ToVector3d(),
            array3[1].Eigenvector.ToVector3d(),
            array3[2].Eigenvector.ToVector3d()
            };
            for (int i = 0; i < 3; i++)
            {
                PrincipalAxes[i] = PrincipalAxes[i].Reverse();
            }
            InertiaTensor = array2;
        }

        public static MassProperties Combine(params MassProperties[] inputResults)
        {
            UFWeight.Properties[] array = new UFWeight.Properties[inputResults.Length];

            for (int i = 0; i < inputResults.Length; i++)
                array[i] = inputResults[i].ToWeightProps();

            TheUfSession.Weight.SumProps(inputResults.Length, array, out UFWeight.Properties wp);

            return new MassProperties(wp);
        }

        public UFWeight.Properties ToWeightProps()
        {
            return new UFWeight.Properties
            {
                cache_state = UFWeight.StateType.Cached,
                accuracy = 0.99,
                area = Area,
                area_state = UFWeight.StateType.Cached,
                center_of_mass = Centroid.ToArray(),
                cofm_state = UFWeight.StateType.Cached,
                density = Density,
                density_state = UFWeight.StateType.Cached,
                mass = Mass,
                mass_state = UFWeight.StateType.Cached,
                moments_of_inertia = MomentsOfInertia,
                mofi_state = UFWeight.StateType.Cached,
                volume = Volume,
                volume_state = UFWeight.StateType.Cached,
                products_of_inertia = ProductsOfInertia,
                units = Units
            };
        }
    }
}