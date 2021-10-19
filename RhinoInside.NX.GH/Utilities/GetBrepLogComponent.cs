using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.GH
{
    public abstract class GetBrepLogComponent : GH_Component
    {
        public override Guid ComponentGuid
        {
            get => Guid.NewGuid();
        }

        public GetBrepLogComponent() : base("Brep Log", "Log", "Get Brep Log", "NX", "Utilities")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "Brep to Ask Log", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Log", "L", "Log of Brep", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object obj = null;
            if (!DA.GetData(0, ref obj))
                return;

            if (obj is GH_Brep brep)
            {
                brep.Value.IsValidWithLog(out string brepLog);

                DA.SetData(0, brepLog);
            }
            else
                return;
        }
    }
}
