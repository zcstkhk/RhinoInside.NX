using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using RhinoInside.Revit.External.DB.Extensions;
using DB = Autodesk.Revit.DB;

namespace RhinoInside.Revit.Convert.Geometry
{
  /// <summary>
  /// This class is obsolete but is here as reference of a possible alternative SolidDecoder
  /// <para><see cref="Raw.RawDecoder.ToRhino(DB.Solid)"/> seems to be robust enough to make this class obsolete</para>
  /// </summary>
  [Obsolete("Please use Raw.RawDecoder.ToRhino(DB.Solid)")]
  static class SolidDecoder
  {
    static IEnumerable<Curve> ToCurveMany(IEnumerable<DB.CurveLoop> loops)
    {
      foreach (var loop in loops)
      {
        var curves = Curve.JoinCurves(loop.Select(x => Raw.RawDecoder.ToRhino(x)), Revit.ShortCurveTolerance, false);
        if (curves.Length != 1)
          throw new ConversionException("Failed to found one and only one closed loop.");

        yield return curves[0];
      }
    }

    static Brep JoinAndMerge(ICollection<Brep> brepFaces, double tolerance)
    {
      if (brepFaces.Count == 0)
        return null;

      if (brepFaces.Count == 1)
        return brepFaces.First();

      var joinedBreps = Brep.JoinBreps(brepFaces.OfType<Brep>(), tolerance) ?? brepFaces;
      if (joinedBreps.Count == 1)
        return joinedBreps.First();

      var merged = Brep.MergeBreps(joinedBreps, Rhino.RhinoMath.UnsetValue);
      if (merged?.IsValid == false)
        merged.Repair(tolerance);

      return merged;
    }

    static Brep TrimFaces(Brep brep, IEnumerable<Curve> loops)
    {
      var brepFaces = new List<Brep>();

      foreach (var brepFace in brep?.Faces ?? Enumerable.Empty<BrepFace>())
      {
        var trimmedBrep = brepFace.Split(loops, Revit.VertexTolerance);

        if (trimmedBrep is object)
        {
          // Remove ears, faces with edges not over 'loops'
          foreach (var trimmedFace in trimmedBrep.Faces.OrderByDescending(x => x.FaceIndex))
          {
            var boundaryEdges = trimmedFace.Loops.
                                SelectMany(loop => loop.Trims).
                                Where(trim => trim.TrimType == BrepTrimType.Boundary).
                                Select(trim => trim.Edge);

            foreach (var edge in boundaryEdges)
            {
              var midPoint = edge.PointAt(edge.Domain.Mid);

              var midPointOnAnyLoop = loops.Where(x => x.ClosestPoint(midPoint, out var _, Revit.VertexTolerance)).Any();
              if (!midPointOnAnyLoop)
              {
                trimmedBrep.Faces.RemoveAt(trimmedFace.FaceIndex);
                break;
              }
            }
          }

          // Remove holes, faces with no boundary edges
          foreach (var trimmedFace in trimmedBrep.Faces.OrderByDescending(x => x.FaceIndex))
          {
            var boundaryTrims = trimmedFace.Loops.
                                SelectMany(loop => loop.Trims).
                                Where(trim => trim.TrimType == BrepTrimType.Boundary);

            if (!boundaryTrims.Any())
            {
              trimmedBrep.Faces.RemoveAt(trimmedFace.FaceIndex);
              continue;
            }
          }

          if (!trimmedBrep.IsValid)
            trimmedBrep.Repair(Revit.VertexTolerance);

          trimmedBrep.Compact();
          brepFaces.Add(trimmedBrep);
        }
      }

      return brepFaces.Count == 0 ?
             brep.DuplicateBrep() :
             JoinAndMerge(brepFaces, Revit.VertexTolerance);
    }

    internal static Brep ToBrep(DB.Face face)
    {
      var surface = Raw.RawDecoder.ToRhinoSurface(face, out var _, 1.0);
      if (surface is null)
        return null;

      var brep = Brep.CreateFromSurface(surface);
      if (brep is null)
        return null;

      if (!face.MatchesSurfaceOrientation())
        brep.Flip();

      var loops = ToCurveMany(face.GetEdgesAsCurveLoops()).ToArray();

      try { return TrimFaces(brep, loops); }
      finally { brep.Dispose(); }
    }

    internal static Brep ToBrep(DB.Solid solid)
    {
      return JoinAndMerge
      (
        solid.Faces.
        Cast<DB.Face>().
        Select(x => ToBrep(x)).
        ToArray(),
        Revit.VertexTolerance
      );
    }
  }
}
