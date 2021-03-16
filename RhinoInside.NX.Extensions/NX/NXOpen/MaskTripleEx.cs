using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;

namespace RhinoInside.NX.Extensions
{
    /// <summary>
    /// 定义常用的选项过滤
    /// </summary>
    public class MaskTripleEx
    {
        public static Selection.MaskTriple Body => new Selection.MaskTriple(UFConstants.UF_solid_type, UFConstants.UF_solid_body_subtype, UFConstants.UF_UI_SEL_FEATURE_BODY);

        public static Selection.MaskTriple SolidBody => new Selection.MaskTriple(UFConstants.UF_solid_type, UFConstants.UF_solid_section_type, UFConstants.UF_UI_SEL_FEATURE_SOLID_BODY);

        public static Selection.MaskTriple SheetBody => new Selection.MaskTriple(UFConstants.UF_solid_type, UFConstants.UF_solid_body_subtype, UFConstants.UF_UI_SEL_FEATURE_SHEET_BODY);

        public static Selection.MaskTriple Edge => new Selection.MaskTriple(UFConstants.UF_solid_type, UFConstants.UF_solid_body_subtype, UFConstants.UF_UI_SEL_FEATURE_ANY_EDGE);

        public static Selection.MaskTriple Face => new Selection.MaskTriple(UFConstants.UF_solid_type, UFConstants.UF_solid_body_subtype, UFConstants.UF_UI_SEL_FEATURE_ANY_FACE);

        public static Selection.MaskTriple Component => new Selection.MaskTriple(UFConstants.UF_component_type, UFConstants.UF_component_subtype, 0);

        public static Selection.MaskTriple Circle => new Selection.MaskTriple(UFConstants.UF_circle_type, UFConstants.UF_all_subtype, 0);

        public static Selection.MaskTriple Line => new Selection.MaskTriple(UFConstants.UF_line_type, UFConstants.UF_all_subtype, 0);

        public static Selection.MaskTriple DatumAxis => new Selection.MaskTriple(UFConstants.UF_datum_axis_type, UFConstants.UF_all_subtype, 0);

        public static Selection.MaskTriple Spline => new Selection.MaskTriple(UFConstants.UF_spline_type, UFConstants.UF_all_subtype, 0);

        public static Selection.MaskTriple Direction => new Selection.MaskTriple(UFConstants.UF_direction_type, UFConstants.UF_all_subtype, 0);

        public static Selection.MaskTriple[] Curve => new Selection.MaskTriple[] { Circle, Spline, Line };
    }
}
