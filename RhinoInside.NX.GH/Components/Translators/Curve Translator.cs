using Grasshopper.Kernel;
using Rhino;
using Rhino.Commands;
using RhinoInside.NX.GH.Parameters;
using RhinoInside.NX.GH.Types;
using System;
using RhinoInside.NX.Translator.Geometry;
using System.Drawing;
using Grasshopper.Kernel.Types;
using System.Linq;

namespace RhinoInside.NX.GH.Translators
{
    public class CurveTranslator : GH_Component
    {
        public override Guid ComponentGuid => new Guid("17CC45D9-D014-46F7-9603-13ABD299AB28");

        protected override Bitmap Icon => Properties.Icons.NX_Curve;

        public CurveTranslator() : base("Curve 转换器", "Curve", Properties.Languages.GetString("CurveTranslatorDesc"), "NX", Properties.Languages.GetString("GrasshopperTranslatorSubCategory"))
        {

        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_NXCurve(), "NX Curve", "N", "选择 NX 曲线", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Rhino Curve", "R", "转换后的 Rhino 曲线", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            System.Collections.Generic.List<NX_Curve> nxCurves = new System.Collections.Generic.List<NX_Curve>();

            if (DA.GetDataList(0, nxCurves))
            {
                System.Collections.Generic.List<Rhino.Geometry.Curve> rhinoCurves = nxCurves.Select(obj => obj.Value.ToRhino()).ToList();

                DA.SetDataList(0, rhinoCurves);
            }
        }
    }
}
