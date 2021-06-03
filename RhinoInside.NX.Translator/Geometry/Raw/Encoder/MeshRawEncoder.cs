using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Translator.Geometry.Raw.Encoder
{
    static partial class RawEncoder
    {
        #region Mesh
        //public static NXOpen.Facet.FacetedBody ToHost(Mesh mesh)
        //{
        //  if (mesh is null)
        //    return null;

        //        //theUfSession.Facet.CreateModel(WorkPart.Tag, out Tag facetModelTag);



        //  using
        //  (
        //    var builder = new NXOpen.TessellatedShapeBuilder()
        //    {
        //      GraphicsStyleId = GeometryEncoder.Context.Peek.GraphicsStyleId,
        //      Target = NXOpen.TessellatedShapeBuilderTarget.Mesh,
        //      Fallback = NXOpen.TessellatedShapeBuilderFallback.Salvage
        //    }
        //  )
        //  {
        //    var isSolid = mesh.SolidOrientation() != 0;
        //    builder.OpenConnectedFaceSet(isSolid);

        //    var vertices = mesh.Vertices.ToPoint3dArray();
        //    var triangle = new NXOpen.Point3d[3];
        //    var quad = new NXOpen.Point3d[4];

        //    foreach (var face in mesh.Faces)
        //    {
        //      if (face.IsQuad)
        //      {
        //        quad[0] = AsXYZ(vertices[face.A]);
        //        quad[1] = AsXYZ(vertices[face.B]);
        //        quad[2] = AsXYZ(vertices[face.C]);
        //        quad[3] = AsXYZ(vertices[face.D]);

        //        builder.AddFace(new NXOpen.TessellatedFace(quad, GeometryEncoder.Context.Peek.MaterialId));
        //      }
        //      else
        //      {
        //        triangle[0] = AsXYZ(vertices[face.A]);
        //        triangle[1] = AsXYZ(vertices[face.B]);
        //        triangle[2] = AsXYZ(vertices[face.C]);

        //        builder.AddFace(new NXOpen.TessellatedFace(triangle, GeometryEncoder.Context.Peek.MaterialId));
        //      }
        //    }
        //    builder.CloseConnectedFaceSet();

        //    builder.Build();
        //    using (var result = builder.GetBuildResult())
        //    {
        //      if (result.Outcome != NXOpen.TessellatedShapeBuilderOutcome.Nothing)
        //      {
        //        var geometries = result.GetGeometricalObjects();
        //        if (geometries.Count == 1)
        //        {
        //          return geometries[0] as NXOpen.Mesh;
        //        }
        //      }
        //    }
        //  }

        //  return null;
        //}
        #endregion
    }
}
