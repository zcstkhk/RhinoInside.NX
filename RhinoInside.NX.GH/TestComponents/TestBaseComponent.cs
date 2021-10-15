using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using RhinoInside.NX.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.GH.TestComponents
{
    public abstract class TestBaseComponent : GH_Component
    {
        public override Guid ComponentGuid
        {
            get { return Guid.NewGuid(); }
        }

        public TestBaseComponent(string name) : base(name, name, name, "NX", "Test")
        {

        }

        
    }
}
