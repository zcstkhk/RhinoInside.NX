using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel;
using RhinoInside.NX.GH.Types;
using System.Drawing;
using NXOpen;
using RhinoInside.NX.Extensions;
using NXOpen.UF;
using System.Windows.Forms;
using RhinoInside.NX.GH.Properties;

namespace RhinoInside.NX.GH.Parameters
{
    public class Param_NXEdge : NX_DisplayableParam<NX_Edge>
    {
        public override Guid ComponentGuid => new Guid("DE2B4E54-48D8-4B8C-9758-ED9EE19F1ED2");

        public Param_NXEdge() : base("NX Edge", "Edge", Languages.GetString("NXEdgeParamDesc"), "NX", Languages.GetString("GrasshopperGeometrySubCategory"))
        {

        }

        protected override Bitmap Icon => Properties.Icons.NX_Edge;

        protected override GH_GetterResult Prompt_Plural(ref List<NX_Edge> values)
        {
            SelectionPreparation();
            var selectedObjects = Select_Plural(MaskTripleEx.Edge);
            if (selectedObjects !=null)
            {
                values = new List<NX_Edge>();
                for (int i = 0; i < selectedObjects.Length; i++)
                {
                    values.Add(new NX_Edge(selectedObjects[i].Tag, Attributes.Selected));
                }

                return GH_GetterResult.success;
            }

            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref NX_Edge value)
        {
            SelectionPreparation();

            var selectedObject = Select_Singular(MaskTripleEx.Edge);

            if (selectedObject != null)
            {
                value = new NX_Edge(selectedObject.Tag, Attributes.Selected);

                return GH_GetterResult.success;
            }

            return GH_GetterResult.cancel;
        }


    }
}