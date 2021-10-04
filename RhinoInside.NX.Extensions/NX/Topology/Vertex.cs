using PLMComponents.Parasolid.PK_.Unsafe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RhinoInside.NX.Extensions.Parasolid;
using NXOpen;

namespace RhinoInside.NX.Extensions.Topology
{
	/// <summary>A vertex is a place where several edges meet</summary>
	/// <remarks>
	/// An edge may have zero, one, or two vertices.
	/// <list type="bullet">
	/// <item>An edge with zero vertices is a closed ring shape. Its start and end points coincide,
	/// and the start/end junction is smooth. 
	/// Examples are edges E1 and E2 in the image below.</item>
	/// <item>An edge with one vertex is again a closed ring shape whose start and end points coincide.
	/// But it has a distinct corner at the start/end junction point.
	/// Examples are edges E3 and E4 in the image below.</item>
	/// <item>An edge with two vertices is open; its start and end points are different. 
	/// An example is edge E5 in the image below.</item>      
	/// </list>
	/// <para>
	/// Edges with one vertex are fairly unusual; most edges have either zero or two vertices.
	/// </para>
	/// </remarks>
	public class Vertex : Topo
	{
		internal VERTEX_t PsVertex
		{
			get
			{
				return PsTopology;
			}
			private set
			{
				base.PsTopology = value;
			}
		}

		internal Vertex(VERTEX_t psVertex) : base(psVertex)
		{
			this.PsVertex = psVertex;
		}

		/// <summary>The body on which this vertex lies</summary>
		public unsafe Body Body
		{
			get
			{
				BODY_t psBody;
				VERTEX.ask_body(PsVertex, &psBody);
				return psBody.GetBody();
			}
		}

		/// <summary>The shell that the vertex belongs to</summary>
		/// <remarks>
		/// In NX objects, a vertex always belongs to exactly one shell.
		/// </remarks>
		public unsafe Shell Shell
		{
			get
			{
				int num = 0;
				SHELL_t* ptr;
				VERTEX.ask_shells(this.PsVertex, &num, &ptr);
				Shell result = new Shell(*ptr);
				if (num > 0)
				{
					MEMORY.free((void*)ptr);
				}
				return result;
			}
		}

		/// <summary>The faces on which the vertex lies</summary>
		/// <remarks>
		/// A vertex might lie on one, two, or more faces. The most common
		/// situation is a vertex lying on three faces. 
		/// </remarks>
		public unsafe Face[] Faces
		{
			get
			{
				int numFaces = 0;
				FACE_t* faces;
				VERTEX.ask_faces(PsVertex, &numFaces, &faces);
				Face[] results = new Face[numFaces];
				for (int i = 0; i < numFaces; i++)
				{
					Face face = faces[i].GetFace();
					results[i] = face;
				}

				if (numFaces > 0)
				{
					MEMORY.free((void*)faces);
				}
				return results;
			}
		}

		/// <summary>The edges on which the vertex lies</summary>
		/// <remarks>
		/// A vertex might lie on zero, one, two, or more edges.
		/// <para>
		/// If the vertex lies on zero edges, this property will return an array of length zero.
		/// </para>
		/// </remarks>
		public unsafe Edge[] Edges
		{
			get
			{
				int numEdges = 0;
				EDGE_t* edges;
				LOGICAL_t* orients;
				VERTEX.ask_oriented_edges(PsVertex, &numEdges, &edges, &orients);
				Edge[] results = new Edge[numEdges];
				for (int i = 0; i < numEdges; i++)
				{
					Edge edge = edges[i].GetEdge();
					results[i] = edge;
				}
				return results;
			}
		}

		/// <summary>The position of the vertex</summary>
		public unsafe Point3d Position
		{
			get
			{
				POINT_t point;
				VERTEX.ask_point(PsVertex, &point);
				POINT_sf_t point_sf_t;
				POINT.ask(point, &point_sf_t);
				VECTOR_t position = point_sf_t.position;

				double* ptr = position.coord;
				double metersToPartUnits = UnitConversion.MetersToPartUnits;
				double x = metersToPartUnits * *ptr;
				double y = metersToPartUnits * ptr[1];
				double z = metersToPartUnits * ptr[2];
				return new Point3d(x, y, z);
			}
		}

		/// <summary>
		/// Returns a string representation of the vertex
		/// </summary>
		/// <returns>A string containing the object type and its Parasolid tag</returns>
		/// <remarks>
		/// Returns an empty zero-length string if this object is <c>Nothing</c>
		/// </remarks>
		public override string ToString()
		{
			string result = "";
			if (base.PsTopology != TOPOL_t.@null)
			{
				result = "Vertex:" + base.PsTag.ToString();
			}
			return result;
		}
	}
}