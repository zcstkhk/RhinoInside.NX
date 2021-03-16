using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using RhinoInside.NX.Core;
using DB = NXOpen;

namespace RhinoInside.NX.GH.Components
{
    public abstract class GH_Component : Grasshopper.Kernel.GH_Component
    {
        protected GH_Component(string name, string nickname, string description, string category, string subCategory)
        : base(name, nickname, description, category, subCategory) { }

        // Grasshopper default implementation has a bug, it checks inputs instead of outputs
        public override bool IsBakeCapable => Params?.Output.OfType<IGH_BakeAwareObject>().Where(x => x.IsBakeCapable).Any() ?? false;

        protected override Bitmap Icon => ((Bitmap)Properties.Resources.ResourceManager.GetObject(GetType().Name)) ??
                                          ImageBuilder.BuildIcon(IconTag, Properties.Resources.UnknownIcon);

        protected virtual string IconTag => GetType().Name.Substring(0, 1);
    }

    public abstract class Component : GH_Component, Kernel.IGH_ElementIdComponent
    {
        protected Component(string name, string nickname, string description, string category, string subCategory)
        : base(name, nickname, description, category, subCategory)
        {
            if (Obsolete)
            {
                foreach (var obsolete in GetType().GetCustomAttributes(typeof(ObsoleteAttribute), false).Cast<ObsoleteAttribute>())
                {
                    if (obsolete.Message != string.Empty)
                        Description = $"{obsolete.Message}\n{Description}";
                }
            }
        }

        static string[] keywords = new string[] { "Revit" };
        public override IEnumerable<string> Keywords => base.Keywords is null ? keywords : Enumerable.Concat(base.Keywords, keywords);

        protected virtual NXOpen.Selection.MaskTriple ElementFilter { get; }

        public override sealed void ComputeData() =>
          Rhinoceros.InvokeInHostContext(() => base.ComputeData());

        protected override sealed void SolveInstance(IGH_DataAccess DA)
        {
            //try
            //{
            //    TrySolveInstance(DA);
            //}
            //catch (Exceptions.CancelException e)
            //{
            //    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"{e.Source}: {e.Message}");
            //}
            //catch (Autodesk.Revit.Exceptions.ApplicationException e)
            //{
            //    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"{e.Source}: {e.Message}");
            //}
            //catch (System.Exception e)
            //{
            //    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"{e.Source}: {e.Message}");
            //    DA.AbortComponentSolution();
            //}
        }
        protected abstract void TrySolveInstance(IGH_DataAccess DA);

        public override Rhino.Geometry.BoundingBox ClippingBox
        {
            get
            {
                var clippingBox = Rhino.Geometry.BoundingBox.Empty;

                foreach (var param in Params)
                {
                    if (param.SourceCount > 0)
                        continue;

                    if (param is IGH_PreviewObject previewObject)
                    {
                        if (!previewObject.Hidden && previewObject.IsPreviewCapable)
                            clippingBox.Union(previewObject.ClippingBox);
                    }
                }

                return clippingBox;
            }
        }
    }
}
