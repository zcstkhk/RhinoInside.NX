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
	/// <summary>A loop is a connected sequence of fins that form part of the boundary of a face</summary>
	/// <remarks>
	/// Typically, a face will have one peripheral loop and perhaps some hole loops in its interior.
	/// <para>
	/// Within each loop, the fins have a consistent direction,
	/// and, when walking along the loop in this direction, the loop's face
	/// lies on your left. In simple cases, this means that
	/// peripheral loops will have a counterclockwise direction, and hole loops
	/// will have a clockwise direction, when viewed from outside the body.
	/// </para>
	/// <para>
	/// Note that you can not use "loop" as the name of a variable in your code,
	/// because it is a reserved word in the Visual Basic language.
	/// </para>
	/// </remarks>
	public class Loop : Topo
	{
		internal LOOP_t PsLoop
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

		internal Loop(LOOP_t psLoop) : base(psLoop)
		{
			this.PsLoop = psLoop;
		}

		/// <summary>The body on which this loop lies</summary>
		public unsafe Body Body
		{
			get
			{
				BODY_t psBody;
				LOOP.ask_body(this.PsLoop, &psBody);
				return psBody.GetBody();
			}
		}

		/// <summary>The face on which this loop lies</summary>
		public unsafe Face Face
		{
			get
			{
				FACE_t psFace;
				LOOP.ask_face(this.PsLoop, &psFace);
				return psFace.GetFace();
			}
		}

		/// <summary>The array of vertices in the loop</summary>
		/// <remarks>
		/// If the loop has no vertices (a loop containing
		/// a single circular edge, for example), 
		/// this property will return an array of length zero.
		/// </remarks>
		public unsafe Vertex[] Vertices
		{
			get
			{
				int num = 0;
				VERTEX_t* ptr;
				LOOP.ask_vertices(this.PsLoop, &num, &ptr);
				Vertex[] array = new Vertex[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = new Vertex(ptr[i]);
				}
				if (num > 0)
				{
					MEMORY.free((void*)ptr);
				}
				return array;
			}
		}

		/// <summary>The array of fins in the loop</summary>
		/// <remarks>
		/// The fins are ordered nose-to-tail around the loop, so that the end of each fin coincides with
		/// the beginning of the next fin in the list, and the end of the last fin
		/// coincides with the start of the first one.
		/// <para>
		/// If you were to walk along the fin, with the face normal pointing from your 
		/// feet up towards your head, then the face that owns the fin would be on your left.
		/// </para>
		/// <para>
		/// If you prefer to get the fins of a loop one at a time, rather than
		/// getting them all at once, you can use the Next or Previous properties
		/// to cycle through them.
		/// </para>
		/// </remarks>
		public unsafe Fin[] Fins
		{
			get
			{
				int num = 0;
				FIN_t* ptr;
				LOOP.ask_fins(this.PsLoop, &num, &ptr);
				Fin[] array = new Fin[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = new Fin(ptr[i]);
				}
				if (num > 0)
				{
					MEMORY.free((void*)ptr);
				}
				return array;
			}
		}

		/// <summary>The array of edges in the loop</summary>
		/// <remarks>
		/// There are cases where a loop has no edges. For example, at the tip of
		/// cone, there is a loop that contains one vertex and zero edges. In this
		/// case, this function will return an array with length zero.
		/// </remarks>
		public unsafe Edge[] Edges
		{
			get
			{
				int num = 0;
				EDGE_t* ptr;
				LOOP.ask_edges(this.PsLoop, &num, &ptr);
				Edge[] array = new Edge[num];
				for (int i = 0; i < num; i++)
				{
					Edge edge = ptr[i].GetEdge();
					array[i] = edge;
				}
				if (num > 0)
				{
					MEMORY.free((void*)ptr);
				}
				return array;
			}
		}

		/// <summary>The type of the loop (peripheral, hole, etc.)</summary>
		public unsafe LoopType Type
		{
			get
			{
				LOOP.type_t type_t;
				LOOP.ask_type(this.PsLoop, &type_t);
				LoopType result = (LoopType)type_t;
				if (type_t == LOOP.type_t.vertex_c)
				{
					result = LoopType.Unknown;
				}
				if (type_t == LOOP.type_t.wire_c)
				{
					result = LoopType.Unknown;
				}
				if (type_t == LOOP.type_t.error_c)
				{
					result = LoopType.Unknown;
				}
				return result;
			}
		}

		/// <summary>
		/// Returns a string representation of the loop
		/// </summary>
		/// <returns>A string containing the object type and its Parasolid tag</returns>
		/// <remarks>
		/// Returns an empty zero-length string if this object is <c>Nothing</c>
		/// </remarks>
		public override string ToString()
		{
			string result = "";
			if (this != null && base.PsTopology != TOPOL_t.@null)
			{
				result = "Loop:" + base.PsTag.ToString();
			}
			return result;
		}
	}

	/// <summary>
	/// The type of a Loop object.
	/// </summary>
	/// <remarks>
	/// For planes and other non-periodic surfaces, all loops are either
	/// of type Outer or Inner. Each face has one 
	/// peripheral loop (type Outer) and possibly some holes (type Inner).
	/// <para>
	/// For cylindrical topology, i.e. surfaces that are periodic in one
	/// parameter direction only, closed loops that wind once around the periodic
	/// parameter space are of type Winding. To define the
	/// periphery of a face in this situation, you
	/// need either an Outer loop or a matched pair of Winding loops.
	/// </para>
	/// <para>
	/// A winding loop that completely surrounds the tip (singularity) of a cone
	/// has type InnerSingular.
	/// </para>
	/// <para>
	/// On spheres and donut tori, it is sometimes not clear whether
	/// a loop represents a periphery or a hole. This is the reason
	/// for the loop types LikelyOuter, LikelyInner, and Unknown.
	/// </para>
	/// </remarks>
	public enum LoopType
	{
		/// <summary>A simple peripheral loop</summary>    
		Outer = 5412,
		/// <summary>A simple hole loop</summary>    
		Inner,
		/// <summary>A winding loop on a periodic surface e.g. a circle on a cylinder or doughnut</summary>    
		Winding,
		/// <summary>A hole loop around a surface singularity, e.g. chopping the top off a cone</summary>    
		InnerSingular,
		/// <summary>An apparently peripheral loop on a doubly closed surface</summary>    
		LikelyOuter,
		/// <summary>An apparent hole in a doubly closed surface</summary>    
		LikelyInner,
		/// <summary>Unknown loop type </summary>    
		Unknown
	}
}
