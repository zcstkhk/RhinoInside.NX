using PLMComponents.Parasolid.PK_.Unsafe;
using System;
using static RhinoInside.NX.Extensions.Globals;
using NXOpen;

namespace RhinoInside.NX.Extensions.Parasolid
{
    public static class TOPOLEx
    {
        static TOPOLEx() => AppDomain.CurrentDomain.AssemblyResolve += Globals.ManagedLibraryResolver;

        internal unsafe static TOPOL.facet_2_r_t facet2(PKTag[] bodies, double minLength, bool minLogical, double maxLength, bool maxLogical)
        {
            TOPOL_t[] topols = new TOPOL_t[bodies.Length];
            for (int i = 0; i < topols.Length; i++)
                topols[i] = new TOPOL_t(bodies[i].PKValue);

            checked
            {
                TOPOL.facet_2_o_t facet_o_t = new TOPOL.facet_2_o_t(true);
                facet_o_t.control.min_facet_width = minLogical ? minLength : 0.0;
                facet_o_t.control.is_min_facet_width = minLogical ? LOGICAL_t.@true : LOGICAL_t.@false;
                facet_o_t.control.max_facet_width = maxLogical ? maxLength : 0.0;
                facet_o_t.control.is_max_facet_width = maxLogical ? LOGICAL_t.@true : LOGICAL_t.@false;

                TRANSF_sf_t transf_sf_t = new TRANSF_sf_t(1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0);
                TOPOL.facet_2_r_t result = default(TOPOL.facet_2_r_t);
                TOPOL.facet_2(bodies.Length, topols, null, &facet_o_t, &result);

                return result;
            }
        }

        internal unsafe static TOPOL.facet_r_t facet(PKTag[] bodies, double minLength, bool minLogical, double maxLength, bool maxLogical)
        {
            TOPOL_t[] topols = new TOPOL_t[bodies.Length];
            for (int i = 0; i < topols.Length; i++)
                topols[i] = new TOPOL_t(bodies[i].PKValue);

            TOPOL.facet_o_t facet_o_t = new TOPOL.facet_o_t(true);

            facet_o_t.control.is_min_facet_width = minLogical ? LOGICAL_t.@true : LOGICAL_t.@false;
            facet_o_t.control.min_facet_width = minLogical ? minLength / 1000.0 : 0.0;

            facet_o_t.control.is_max_facet_width = maxLogical ? LOGICAL_t.@true : LOGICAL_t.@false;
            facet_o_t.control.max_facet_width = maxLogical ? maxLength / 1000.0 : 0.0;

            TRANSF_sf_t transf_sf = new TRANSF_sf_t(1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0);
            TRANSF_t transf = new TRANSF_t();
            TRANSF.create(&transf_sf, &transf);

            TOPOL.facet_r_t result = new TOPOL.facet_r_t();
            TOPOL.facet(bodies.Length, topols, null, transf, &facet_o_t, &result);

            return result;

        }

        internal unsafe static void range_array_vector(Point3d point, PKTag[] topols, out range_result_t rangeResult, out range_1_r_t range)
        {
            checked
            {
                TOPOL_t[] pkTopols = new TOPOL_t[topols.Length];
                for (int i = 0; i < topols.Length; i++)
                    pkTopols[i] = new TOPOL_t(topols[i].PKValue);

                TOPOL.range_array_vector_o_t option = new TOPOL.range_array_vector_o_t(true);
                option.have_tolerance = LOGICAL_t.@true;
                option.tolerance = DistanceTolerance / 1000.0;
                option.opt_level = range_opt_t.performance_c;

                range_result_t result = new range_result_t();
                range_1_r_t result1 = new range_1_r_t();

                VECTOR_t pt = new VECTOR_t(point.X / 1000.0, point.Y / 1000.0, point.Z / 1000.0);

                TOPOL.range_array_vector(topols.Length, pkTopols, pt, &option, &result, &result1);

                rangeResult = result;
                range = result1;
            }
        }
    }
}
