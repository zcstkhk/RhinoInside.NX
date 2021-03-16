using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using RhinoInside.NX.Convert.Geometry;
using RhinoInside.NX.External.DB.Extensions;
using RhinoInside.NX.Geometry.Extensions;
using DB = NXOpen;

namespace RhinoInside.NX.GH.Types
{
  /// <summary>
  /// Iterface that represents any <see cref="DB.Element"/> that has a Graphical representation in Revit
  /// </summary>
  public interface IGH_GraphicalElement : IGH_Element { }

  public class GraphicalElement :
    Element,
    IGH_GraphicalElement, 
    IGH_GeometricGoo,
    IGH_PreviewData
  {
    public GraphicalElement() { }
    public GraphicalElement(DB.Element element) : base(element) { }

    protected override bool SetValue(DB.Element element) => IsValidElement(element) && base.SetValue(element);
    public static bool IsValidElement(DB.Element element)
    {
      if (element is DB.ElementType)
        return false;

      if (element is DB.View)
        return false;

      if (element.Location is object)
        return true;

      return
      (
        element is DB.DirectShape ||
        element is DB.CurveElement ||
        element is DB.CombinableElement ||
        element is DB.Architecture.TopographySurface ||
        element is DB.Opening ||
        element is DB.Part ||
        InstanceElement.IsValidElement(element)
      );
    }

    #region IGH_GeometricGoo
    public BoundingBox Boundingbox => ClippingBox;
    Guid IGH_GeometricGoo.ReferenceID
    {
      get => Guid.Empty;
      set { if (value != Guid.Empty) throw new InvalidOperationException(); }
    }
    bool IGH_GeometricGoo.IsReferencedGeometry => IsReferencedElement;
    bool IGH_GeometricGoo.IsGeometryLoaded => IsElementLoaded;

    void IGH_GeometricGoo.ClearCaches() => UnloadElement();
    IGH_GeometricGoo IGH_GeometricGoo.DuplicateGeometry() => (IGH_GeometricGoo) MemberwiseClone();
    public virtual BoundingBox GetBoundingBox(Transform xform) => ClippingBox.GetBoundingBox(xform);
    bool IGH_GeometricGoo.LoadGeometry() => IsElementLoaded || LoadElement();
    bool IGH_GeometricGoo.LoadGeometry(Rhino.RhinoDoc doc) => IsElementLoaded || LoadElement();
    IGH_GeometricGoo IGH_GeometricGoo.Transform(Transform xform) => null;
    IGH_GeometricGoo IGH_GeometricGoo.Morph(SpaceMorph xmorph) => null;
    #endregion

    #region IGH_PreviewData
    protected BoundingBox? clippingBox;
    public virtual BoundingBox ClippingBox
    {
      get
      {
        if (!clippingBox.HasValue)
          clippingBox = APIElement?.get_BoundingBox(null).ToBoundingBox() ?? BoundingBox.Unset;

        return clippingBox.Value;
      }
    }

    public virtual void DrawViewportWires(GH_PreviewWireArgs args) { }
    public virtual void DrawViewportMeshes(GH_PreviewMeshArgs args) { }
    #endregion

    public override bool CastTo<Q>(ref Q target)
    {
      if (base.CastTo<Q>(ref target))
        return true;

      if (typeof(Q).IsAssignableFrom(typeof(GH_Plane)))
      {
        try
        {
          var plane = Location;
          if (!plane.IsValid || !plane.Origin.IsValid)
            return false;

          target = (Q) (object) new GH_Plane(plane);
          return true;
        }
        catch (Autodesk.Revit.Exceptions.InvalidOperationException) { return false; }
      }

      if (typeof(Q).IsAssignableFrom(typeof(GH_Point)))
      {
        var location = Location.Origin;
        if (!location.IsValid)
          return false;

        target = (Q) (object) new GH_Point(location);
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(GH_Line)))
      {
        var curve = Curve;
        if (curve?.IsValid != true)
          return false;

        if (!curve.TryGetLine(out var line, Revit.VertexTolerance * Revit.ModelUnits))
          return false;

        target = (Q) (object) new GH_Line(line);
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(GH_Vector)))
      {
        var orientation = Orientation;
        if (!orientation.IsValid || orientation.IsZero)
          return false;

        target = (Q) (object) new GH_Vector(orientation);
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(GH_Transform)))
      {
        var plane = Location;
        if (!plane.IsValid || !plane.Origin.IsValid)
          return false;

        target = (Q) (object) new GH_Transform(Transform.PlaneToPlane(Plane.WorldXY, plane));
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(GH_Box)))
      {
        var box = Box;
        if (!box.IsValid)
          return false;

        target = (Q) (object) new GH_Box(box);
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
      {
        var axis = Curve;
        if (axis is null)
          return false;

        target = (Q) (object) new GH_Curve(axis);
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(GH_Surface)))
      {
        var surface = Surface;
        if (surface is null)
          return false;

        target = (Q) (object) new GH_Surface(surface);
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(GH_Brep)))
      {
        var surface = Surface;
        if (surface is null)
          return false;

        target = (Q) (object) new GH_Brep(surface);
        return true;
      }

      return false;
    }

    #region Location
    public virtual Box Box
    {
      get
      {
        if (APIElement is DB.Element element)
        {
          var plane = Location;
          if (!Location.IsValid)
            return element.get_BoundingBox(null).ToBox();

          var xform = Transform.ChangeBasis(Plane.WorldXY, plane);
          var bbox = GetBoundingBox(xform);

          return new Box
          (
            plane,
            new Interval(bbox.Min.X, bbox.Max.X),
            new Interval(bbox.Min.Y, bbox.Max.Y),
            new Interval(bbox.Min.Z, bbox.Max.Z)
          );
        }

        return new Box(ClippingBox);
      }
    }
    public virtual Level Level => default;

    public virtual Plane Location
    {
      get
      {
        if (!ClippingBox.IsValid) return new Plane
        (
          new Point3d(double.NaN, double.NaN, double.NaN),
          new Vector3d(double.NaN, double.NaN, double.NaN),
          new Vector3d(double.NaN, double.NaN, double.NaN)
        );

        var origin = ClippingBox.Center;
        var axis = Vector3d.XAxis;
        var perp = Vector3d.YAxis;

        if (APIElement is DB.Element element)
        {
          switch (element.Location)
          {
            case DB.LocationPoint pointLocation:
              origin = pointLocation.Point.ToPoint3d();
              try
              {
                axis.Rotate(pointLocation.Rotation, Vector3d.ZAxis);
                perp.Rotate(pointLocation.Rotation, Vector3d.ZAxis);
              }
              catch { }

              break;
            case DB.LocationCurve curveLocation:
              var curve = curveLocation.Curve;
              if (curve.IsBound)
              {
                var start = curve.Evaluate(0.0, normalized: true).ToPoint3d();
                var end = curve.Evaluate(1.0, normalized: true).ToPoint3d();
                axis = end - start;
                origin = start + (axis * 0.5);
                perp = axis.PerpVector();
              }
              else if(curve is DB.Arc || curve is DB.Ellipse)
              {
                var start = curve.Evaluate(0.0, normalized: false).ToPoint3d();
                var end = curve.Evaluate(Math.PI, normalized: false).ToPoint3d();
                axis = end - start;
                origin = start + (axis * 0.5);
                perp = axis.PerpVector();
              }

              break;
          }
        }

        return new Plane(origin, axis, perp);
      }
    }

    public virtual Vector3d Orientation => Location.YAxis;

    public virtual Vector3d Handing => Location.XAxis;

    public virtual Curve Curve
    {
      get
      {
        if (!(APIElement is DB.Element element))
          return default;

        if (element is DB.ModelCurve modelCurve)
          return modelCurve.GeometryCurve.ToCurve();

        return element?.Location is DB.LocationCurve curveLocation ?
          curveLocation.Curve.ToCurve() :
          null;
      }
    }

    public virtual Brep Surface => null;
    #endregion
  }
}
