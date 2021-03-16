using System.Collections.Generic;
using Grasshopper.Kernel;
using DB = NXOpen;

namespace RhinoInside.NX.GH.Kernel
{
    /// <summary>
    /// Base interface for all Parameter types in RhinoInside.NX.GH that reference NX objects.
    /// </summary>
    /// <remarks>
    /// Do not implement this interface from scratch, derive from <c>RhinoInside.NX.GH.Types.ElementIdParam</c> instead.
    /// </remarks>
    /// <seealso cref="RhinoInside.Revit.GH.Types.ElementIdParam"/>
    public interface IGH_ElementIdParam : IGH_Param
    {
        bool NeedsToBeExpired
        (
          DB.Part doc,
          ICollection<DB.Tag> added,
          ICollection<DB.Tag> deleted,
          ICollection<DB.Tag> modified
        );
    }

    /// <summary>
    /// Base interface for all Component types in RhinoInside.NX.GH that reference NX Objects.
    /// </summary>
    /// <remarks>
    /// Do not implement this interface from scratch, derive from <c>RhinoInside.Revit.GH.Components.Component</c> instead.
    /// </remarks>
    /// <seealso cref="RhinoInside.Revit.GH.Components.Component"/>
    public interface IGH_ElementIdComponent : IGH_Component
    {
        //bool NeedsToBeExpired(DB.Events.DocumentChangedEventArgs args);
    }
}
