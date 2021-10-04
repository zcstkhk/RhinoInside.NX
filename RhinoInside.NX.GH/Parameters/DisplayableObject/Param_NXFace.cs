using Grasshopper.Kernel;
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
    public class Param_NXFace : Param_NXDisplayableObject<NX_Face>
    {
        public Param_NXFace() : base(Languages.GetString("NXFaceParamName"), Languages.GetString("NXFaceParamNickName"), Languages.GetString("NXFaceParamDesc"), "NX", Languages.GetString("GrasshopperGeometrySubCategory"))
        {

        }

        protected override Bitmap Icon => Icons.NX_Face;

        public override Guid ComponentGuid => new Guid("2A512990-BC09-4151-97EC-FD53937C71B4");

        protected override GH_GetterResult Prompt_Plural(ref List<NX_Face> values)
        {
            SelectionPreparation();

            var selectedObjects = Select_Plural(MaskTripleEx.Face);
            if (selectedObjects != null)
            {
                values = new List<NX_Face>();
                for (int i = 0; i < selectedObjects.Length; i++)
                {
                    values.Add(new NX_Face(selectedObjects[i].Tag, Attributes.Selected));
                }

                return GH_GetterResult.success;
            }

            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref NX_Face value)
        {
            SelectionPreparation();

            var selectedObject = Select_Singular(MaskTripleEx.Face);

            if (selectedObject != null)
            {
                value = new NX_Face(selectedObject.Tag, Attributes.Selected);

                return GH_GetterResult.success;
            }

            return GH_GetterResult.cancel;
        }
    }
}
