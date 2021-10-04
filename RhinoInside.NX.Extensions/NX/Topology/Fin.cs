using System;
using NXOpen;
using PLMComponents.Parasolid.PK_.Unsafe;
using RhinoInside.NX.Extensions.Parasolid;

namespace RhinoInside.NX.Extensions.Topology
{
	/// <summary>A fin is "half" of an edge -- an oriented use of an edge by a loop</summary>
	/// <remarks>
	/// A fin has:
	/// <list type="bullet">
	/// <item>
	/// A logical 'sense' indicating whether the fin's orientation, 
	/// and therefore the orientation of its owning loop, is the same as its owning edge
	/// </item>
	/// <item>A curve, which may be <c>Nothing</c> if geometry is attached to the fin's edge, instead. 
	/// </item>
	/// </list>
	/// <para>
	/// In some software systems, fins are called "half-edges" or "co-edges".
	/// </para>
	/// <para>
	/// The picture below shows the fins and edges on three faces of a cube.
	/// The thick blue lines are edges, and the thin red ones are fins.
	/// </para>
	/// <img src="../Images/fins.png" />
	/// <para>
	/// Note how each edge has two corresponding fins, which have opposite
	/// senses. On the vertical edge, E3, for example, fin F4 has a positive 
	/// sense (i.e. the same direction as E3), and fin F5 has a negative sense.
	/// </para>
	/// <para>
	/// Within each loop, the fins have a consistent direction,
	/// and, when walking along the loop in this direction, the loop's face
	/// lies on your left. In simple cases, this means that
	/// peripheral loops will have a counterclockwise direction, and hole loops
	/// will have a clockwise direction, when viewed from outside the body.
	/// </para>
	/// <para>
	/// The red vertex is the start vertex of fins F2, F3 and F5, and it is the
	/// end vertex of fins F1, F4, and F6.
	/// </para>
	/// </remarks>
	public class Fin : Topo
	{
		internal FIN_t PsFin
		{
			get
			{
				return base.PsTopology;
			}
			private set
			{
				base.PsTopology = value;
			}
		}

		internal Fin(FIN_t psFin) : base(psFin)
		{
			this.PsFin = psFin;
		}

		/// <summary>The body on which this fin lies</summary>
		public unsafe Body Body
		{
			get
			{
				BODY_t psBody;
				FIN.ask_body(this.PsFin, &psBody);
				return psBody.GetBody();
			}
		}

		/// <summary>The loop to which this fin belongs</summary>
		public unsafe Loop Loop
		{
			get
			{
				LOOP_t psLoop;
				FIN.ask_loop(this.PsFin, &psLoop);
				return new Loop(psLoop);
			}
		}

		/// <summary>The face to which this fin belongs</summary>
		/// <remarks>
		/// Actually, the relationship between the fin and the face is indirect:
		/// the fin belongs to a loop, and that loop then belongs to a face.
		/// This property just provides a shortcut that allows you to get to the face
		/// in one step instead of two.
		/// </remarks>
		public unsafe Face Face
		{
			get
			{
				FACE_t psFace;
				FIN.ask_face(this.PsFin, &psFace);
				return psFace.GetFace();
			}
		}

		/// <summary>The edge to which this fin belongs</summary>
		public unsafe Edge Edge
		{
			get
			{
				EDGE_t psEdge;
				FIN.ask_edge(this.PsFin, &psEdge);
				return psEdge.GetEdge();
			}
		}

		/// <summary>The next fin in the loop</summary>
		/// <remarks>
		/// You can use this function to cycle through the fins in a loop, one at a time, 
		/// in the "forward" direction (i.e. in the order returned by Fins
		/// The ordering is cyclic, so the first fin is returned again after the last one.
		/// <para>
		/// If there is only one fin in the loop, then this property returns the current fin.
		/// </para>
		/// </remarks>
		public unsafe Fin Next
		{
			get
			{
				FIN_t psFin;
				FIN.ask_next_in_loop(this.PsFin, &psFin);
				return new Fin(psFin);
			}
		}

		/// <summary>The previous fin in the loop</summary>
		/// <remarks>
		/// You can use this function to cycle through the fins in a loop, one at a time, 
		/// in the "reverse" direction (i.e. the opposite of the order returned by Fins
		/// The ordering is cyclic, so the last fin is returned again after the first one.
		/// <para>
		/// If there is only one fin in the loop, then this property returns the current fin.
		/// </para>
		/// </remarks>
		public unsafe Fin Previous
		{
			get
			{
				FIN_t psFin;
				FIN.ask_previous_in_loop(this.PsFin, &psFin);
				return new Fin(psFin);
			}
		}

		/// <summary>The sense of the fin with respect to its owning edge</summary>
		/// <remarks>
		/// If a given edge has two fins, then these two fins will have opposite senses.
		/// <para>
		/// The fin sense allows you to determine the "forward" direction of the fin
		/// and its loop.
		/// This is important because you can then find the "left" side
		/// of the fin, which is where its face lies.
		/// </para>
		/// </remarks>
		public unsafe Sense Sense
		{
			get
			{
				LOGICAL_t x;
				FIN.is_positive(PsFin, &x);
				Sense result = Sense.Forward;
				if (x == LOGICAL_t.@false)
				{
					result = Sense.Reverse;
				}
				return result;
			}
		}

		/// <summary>The start vertex of the fin</summary>
		/// <remarks>
		/// There are three possible cases:
		/// <list type="bullet">
		/// <item>
		/// If the fin has no vertices, this property returns <c>Nothing</c>
		/// </item>
		/// <item>
		/// If the fin has one vertex (because its start and end points are coincident), 
		/// then this property returns the common start/end vertex. This case is rare.
		/// </item>
		/// <item>
		/// If the fin has two vertices, this property returns the one that is shared
		/// by this fin and the previous fin in the loop.
		/// </item>
		/// </list>
		/// </remarks>
		public Vertex StartVertex
		{
			get
			{
				Vertex result = null;
				Vertex[] vertices = Edge.GetParasolidVertices();
				int vertexCount = vertices.Length;
				if (vertexCount == 1)
					result = vertices[0];
				
				if (vertexCount == 2)
				{
					int index = 0;
					if (Sense == Sense.Reverse)
						index = 1;
					
					result = vertices[index];
				}
				return result;
			}
		}

		/// <summary>The end vertex of the fin</summary>
		/// <remarks>
		/// There are three possible cases:
		/// <list type="bullet">
		/// <item>
		/// If the fin has no vertices, this property returns <c>Nothing</c>
		/// </item>
		/// <item>
		/// If the fin has one vertex (because its start and end points are coincident), 
		/// then this property returns the common start/end vertex. This case is rare.
		/// </item>
		/// <item>
		/// If the fin has two vertices, this property returns the one that is shared
		/// by this fin and the next fin in the loop.
		/// </item>
		/// </list>
		/// </remarks>
		public Vertex EndVertex
		{
			get
			{
				Vertex result = null;
				Vertex[] vertices = Edge.GetParasolidVertices();
				int vertexCount = vertices.Length;
				if (vertexCount == 1)
					result = vertices[0];
				
				if (vertexCount == 2)
				{
					int index = 1;
					if (Sense == Sense.Reverse)
						index = 0;
					
					result = vertices[index];
				}
				return result;
			}
		}

		/// <summary>
		/// Returns a string representation of the fin
		/// </summary>
		/// <returns>A string containing the object type and its Parasolid tag</returns>
		/// <remarks>
		/// Returns an empty zero-length string if this object is <c>Nothing</c>
		/// </remarks>
		public override string ToString()
		{
			string result = "";
			if (PsTopology != TOPOL_t.@null)
			{
				result = "Fin:" + base.PsTag.ToString();
			}
			return result;
		}
	}
}
