using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using RhinoInside.NX.GH.TestComponents;
using RhinoInside.NX.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.GH
{
    public class ExportTest : TestBaseComponent
    {
        public ExportTest() : base("Export Test")
        {

        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "Brep Geometry", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // pManager.AddGenericParameter("NX Geonetry", "G", "Geometry Created In NX", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object obj = null;
            if (!DA.GetData(0, ref obj))
                return;

            if (obj is GH_Brep brep)
                SolidExchanger.GrasshopperExport(brep.Value);
            else
                return;
        }
    }
}
