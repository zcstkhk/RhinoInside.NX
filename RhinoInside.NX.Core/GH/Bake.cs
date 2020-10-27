using System.Collections.Generic;
using NXOpen;

namespace RhinoInside.NX.Core
{
  public struct BakeOptions
  {
    public Part Part;
    public View View;
    public Material Material;
  }

  public interface IGH_ElementIdBakeAwareObject
  {
    bool CanBake(BakeOptions options);
    bool Bake(BakeOptions options, out List<Tag> ids);
  }

  public interface IGH_ElementIdBakeAwareData
  {
    bool Bake(BakeOptions options, out Tag id);
  }
}
