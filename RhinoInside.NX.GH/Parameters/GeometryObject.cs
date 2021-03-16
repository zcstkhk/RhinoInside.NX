using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using RhinoInside.NX.Core;
using DB = NXOpen;

namespace RhinoInside.NX.GH.Parameters
{
    public class Edge : ElementIdWithPreviewParam<Types.Edge, DB.Edge>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("B79FD0FD-63AE-4776-A0A7-6392A3A58B0D");
        public Edge() : base("Edge", "Edge", "Represents a Revit edge.", "Params", "Revit Primitives") { }

        #region UI methods
        protected override GH_GetterResult Prompt_Plural(ref List<Types.Edge> value)
        {
            var uiDocument = DB.Session.GetSession().Parts.Work;
            switch (uiDocument.PickObjects(out var references, ObjectType.Edge))
            {
                case Autodesk.Revit.UI.Result.Succeeded:
                    value = references.Select((x) => new Types.Edge(uiDocument.Document, x)).ToList();
                    return GH_GetterResult.success;
                case Autodesk.Revit.UI.Result.Cancelled:
                    return GH_GetterResult.cancel;
            }

            // If PickObject failed reset the Param content to Null.
            value = default;
            return GH_GetterResult.success;
        }
        protected override GH_GetterResult Prompt_Singular(ref Types.Edge value)
        {
            var uiDocument = Revit.ActiveUIDocument;
            switch (uiDocument.PickObject(out var reference, ObjectType.Edge))
            {
                case Autodesk.Revit.UI.Result.Succeeded:
                    value = new Types.Edge(uiDocument.Document, reference);
                    return GH_GetterResult.success;
                case Autodesk.Revit.UI.Result.Cancelled:
                    return GH_GetterResult.cancel;
            }

            // If PickObject failed reset the Param content to Null.
            value = default;
            return GH_GetterResult.success;
        }
        #endregion
    }

    public class Face : ElementIdWithPreviewParam<Types.Face, DB.Face>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("759700ED-BC79-4986-A6AB-84921A7C9293");
        public Face() : base("Face", "Face", "Represents a Revit face.", "Params", "Revit Primitives") { }

        #region UI methods
        protected override GH_GetterResult Prompt_Plural(ref List<Types.Face> value)
        {
            var uiDocument = Revit.ActiveUIDocument;
            switch (uiDocument.PickObjects(out var references, ObjectTypes.Face))
            {
                case Autodesk.Revit.UI.Result.Succeeded:
                    value = references.Select((x) => new Types.Face(uiDocument.Document, x)).ToList();
                    return GH_GetterResult.success;
                case Autodesk.Revit.UI.Result.Cancelled:
                    return GH_GetterResult.cancel;
            }

            // If PickObject failed reset the Param content to Null.
            value = default;
            return GH_GetterResult.success;
        }
        protected override GH_GetterResult Prompt_Singular(ref Types.Face value)
        {
            var uiDocument = Revit.ActiveUIDocument;
            switch (uiDocument.PickObject(out var reference, ObjectType.Face))
            {
                case Autodesk.Revit.UI.Result.Succeeded:
                    value = new Types.Face(uiDocument.Document, reference);
                    return GH_GetterResult.success;
                case Autodesk.Revit.UI.Result.Cancelled:
                    return GH_GetterResult.cancel;
            }

            // If PickObject failed reset the Param content to Null.
            value = default;
            return GH_GetterResult.success;
        }
        #endregion
    }
}
