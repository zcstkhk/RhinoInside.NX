using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using PLMComponents.Parasolid.PK_.Unsafe;
using RhinoInside.NX.Extensions.Topology;

namespace RhinoInside.NX.Extensions
{
   public static class EdgeEx
    {
        public static Vertex[] GetParasolidVertices(this Edge edge)
        {
			unsafe
            {
				EDGE_t psEdge = (int)edge.GetParasolidTag();
				List<Vertex> verticesList = new List<Vertex>();
				VERTEX_t[] array = new VERTEX_t[2];
				fixed (VERTEX_t* ptr = array)
				{
					EDGE.ask_vertices(psEdge, ptr);
				}
				VERTEX_t vertex_t = array[0];
				VERTEX_t vertex_t2 = array[1];
				if (vertex_t != VERTEX_t.@null)
				{
					verticesList.Add(new Vertex(vertex_t));
				}
				if (vertex_t2 != VERTEX_t.@null && vertex_t2 != vertex_t)
				{
					verticesList.Add(new Vertex(vertex_t2));
				}
				return verticesList.ToArray();
			}
		}

		public static Tag GetParasolidTag(this Edge edge)
        {
			Tag result;
			Globals.TheUfSession.Ps.AskPsTagOfObject(edge.Tag, out result);
			return result;
		}
    }
}
