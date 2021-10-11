using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.GH.TestComponents
{
    public abstract class TestComponent : GH_Component
    {
        public override Guid ComponentGuid
        {
            get { return Guid.NewGuid(); }
        }

        public TestComponent(string name) : base(name, name, name, "NX", "Test")
        {

        }
    }
}
