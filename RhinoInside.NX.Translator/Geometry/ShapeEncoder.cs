using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rhino;
using Rhino.Geometry;
using static NXOpen.Extensions.Globals;

namespace RhinoInside.NX.Translator
{

    /// <summary>
    /// This class is used to convert geometry to be stored in a <see cref="NXOpen.DirectShape"/>.
    /// </summary>
    public static class ShapeEncoder
    {
        public static NXOpen.DisplayableObject[] ToShape(this GeometryBase geometry) => ToShape(geometry, UnitConverter.RhinoToNXUnitsRatio);
        public static NXOpen.DisplayableObject[] ToShape(this GeometryBase geometry, double factor)
        {
            switch (geometry)
            {
                case Point point:
                    return new NXOpen.DisplayableObject[] { point.ToPoint(factor) };

                case PointCloud pointCloud:
                    return pointCloud.ToPoints(factor);

                case Curve curve:
                    return CurveEncoder.ToNXCurves(curve, factor).OfType<NXOpen.DisplayableObject>().ToArray();

                //case Brep brep:
                //  return ToGeometryObjectMany(BrepEncoder.ToRawBrep(brep, factor)).OfType<NXOpen.GeometryObject>().ToArray();

                //case Extrusion extrusion:
                //  return ToGeometryObjectMany(ExtrusionEncoder.ToRawBrep(extrusion, factor)).OfType<NXOpen.GeometryObject>().ToArray();

                //case SubD subD:
                //  return ToGeometryObjectMany(SubDEncoder.ToRawBrep(subD, factor)).OfType<NXOpen.GeometryObject>().ToArray(); ;

                //case Mesh mesh:
                //  return new NXOpen.GeometryObject[] { MeshEncoder.ToMesh(MeshEncoder.ToRawMesh(mesh, factor)) };

                default:
                    //if (geometry.HasBrepForm)
                    //{
                    //  var brepForm = Brep.TryConvertBrep(geometry);
                    //  if (BrepEncoder.EncodeRaw(ref brepForm, factor))
                    //    return ToGeometryObjectMany(brepForm).OfType<NXOpen.GeometryObject>().ToArray();
                    //}

                    return new NXOpen.DisplayableObject[0];
            }
        }

        //internal static IEnumerable<NXOpen.DisplayableObject> ToGeometryObjectMany(Brep brep)
        //{
        //    var solid = BrepEncoder.ToSolid(brep);
        //    if (solid is object)
        //    {
        //        yield return solid;
        //        yield break;
        //    }

        //    if (brep.Faces.Count > 1)
        //    {
        //        Debug.WriteLine("Try exploding the brep and converting face by face.");

        //        var breps = brep.UnjoinEdges(brep.Edges.Select(x => x.EdgeIndex));
        //        foreach (var face in breps.SelectMany(x => ToGeometryObjectMany(x)))
        //            yield return face;
        //    }
        //    else
        //    {
        //        Debug.WriteLine("Try meshing the brep.");

        //        // Emergency result as a mesh
        //        var mp = MeshingParameters.Default;
        //        mp.MinimumEdgeLength = DistanceTolerance;
        //        mp.ClosedObjectPostProcess = true;
        //        mp.JaggedSeams = false;

        //        var brepMesh = new Mesh();
        //        if (Mesh.CreateFromBrep(brep, mp) is Mesh[] meshes)
        //            brepMesh.Append(meshes);

        //        yield return brepMesh.ToMesh(UnitConverter.NoScale);
        //    }
        //}
    };
}
