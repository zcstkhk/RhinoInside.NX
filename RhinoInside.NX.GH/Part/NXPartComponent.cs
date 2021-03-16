using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using NXOpen;
using static RhinoInside.NX.Extensions.Globals;

namespace RhinoInside.NX.GH
{
    public abstract class NXPartComponent : GH_Component
    {
        public Part Part;

        public NXPartComponent(string name, string nickName, string desc) : base(name, nickName, desc, "NX", "Part")
        {

        }

        public void MakeWork() => TheSession.Parts.SetWork(Part);

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry", "G", "Geometry that want to Created In NX", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("NX Geonetry", "G", "Geometry Created In NX", GH_ParamAccess.item);
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

            DA.SetData(0, obj.GetType().ToString());
        }
    }
}
