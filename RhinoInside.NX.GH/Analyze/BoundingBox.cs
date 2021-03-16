using Grasshopper.Kernel;
using Rhino.Geometry;
using RhinoInside.NX.GH.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RhinoInside.NX.GH
{
    public class BoundingBox : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BoundingBox class.
        /// </summary>
        public BoundingBox() : base("BoundingBox", "Bounding Box",
              "Get Boundingbox of a NX body",
              "NX", "Debug")
        {
            AppDomain.CurrentDomain.AssemblyResolve += Initializer.AssemblyResolveHandler;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Object", "O", "Wireframe and Solid type objects", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Box", "B", "Object Bounding Box Parameters", GH_ParamAccess.item);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "test");

            base.AppendAdditionalComponentMenuItems(menu);
        }



        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Resources.bounding_volume;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("b27c7b8a-ccae-424a-8253-12f3dc9c57cf"); }
        }
    }
}