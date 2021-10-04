using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using NXOpen.Extensions;
using RhinoInside.NX.GH.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RhinoInside.NX.GH.Properties;

namespace RhinoInside.NX.GH.Parameters
{
    public class Param_NXCurve : Param_NXDisplayableObject<NX_Curve>
    {
        public Param_NXCurve() : base(Languages.GetString("NXCurveParamName"), Languages.GetString("NXCurveParamNickName"), Languages.GetString("NXCurveParamDesc"), "NX", Languages.GetString("GrasshopperGeometrySubCategory"))
        {

        }

        protected override Bitmap Icon => Icons.NX_Curve;

        public override Guid ComponentGuid => new Guid("50FB26CD-B4ED-4478-8576-BF800CE9A407");

        protected override GH_GetterResult Prompt_Plural(ref List<NX_Curve> values)
        {
            SelectionPreparation();

            var selectedObjects = Select_Plural(MaskTripleEx.Curve);
            if (selectedObjects != null)
            {
                values = new List<NX_Curve>();
                for (int i = 0; i < selectedObjects.Length; i++)
                {
                    values.Add(new NX_Curve(selectedObjects[i].Tag, Attributes.Selected));
                }

                return GH_GetterResult.success;
            }

            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref NX_Curve value)
        {
            SelectionPreparation();

            var selectedObject = Select_Singular(MaskTripleEx.Curve);

            if (selectedObject != null)
            {
                value = new NX_Curve(selectedObject.Tag, Attributes.Selected);

                return GH_GetterResult.success;
            }

            return GH_GetterResult.cancel;
        }
    }
}
