using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using NXOpen.Extensions;
using RhinoInside.NX.GH.Properties;
using RhinoInside.NX.GH.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.GH.Parameters.Param_DisplayableObject
{
    public class Param_NXBody : Param_NXDisplayableObject<NX_Body>
    {
        public Param_NXBody() : base(Languages.GetString("NXBodyParamName"), Languages.GetString("NXBodyParamNickName"), Languages.GetString("NXBodyParamDesc"), "NX", Languages.GetString("GrasshopperGeometrySubCategory"))
        {

        }

        protected override Bitmap Icon => Icons.NX_Body;

        public override Guid ComponentGuid => new Guid("6DF3B05A-83D6-4B5A-BFFB-779EB28D6D44");

        protected override GH_GetterResult Prompt_Plural(ref List<NX_Body> values)
        {
            SelectionPreparation();

            var selectedObjects = Select_Plural(MaskTripleEx.Body);
            if (selectedObjects != null)
            {
                values = new List<NX_Body>();
                for (int i = 0; i < selectedObjects.Length; i++)
                {
                    values.Add(new NX_Body(selectedObjects[i].Tag, Attributes.Selected));
                }

                return GH_GetterResult.success;
            }

            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref NX_Body value)
        {
            SelectionPreparation();

            var selectedObject = Select_Singular(MaskTripleEx.Body);

            if (selectedObject != null)
            {
                value = new NX_Body(selectedObject.Tag, Attributes.Selected);

                return GH_GetterResult.success;
            }

            return GH_GetterResult.cancel;
        }
    }
}


