using PLMComponents.Parasolid.PK_.Unsafe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Extensions.Topology
{
	/// <summary>An abstract class representing any topology object</summary>
	/// <remarks>
	/// You will probably never use a Topo object directly. Instead, you will
	/// use more concrete objects like fins, loops, shells, and vertices, which inherit
	/// from the Topo class.
	/// <para>
	/// In some sense, Face and Edge objects also represent "topology".
	/// But the NX objects are persistent (stored in NX part files), whereas the
	/// Topology objects are transient. As soon as your program has finished executing,
	/// all of your fins, loops, shells, and vertices are gone.
	/// </para>
	/// </remarks>
	public abstract class Topo
	{
		/// <summary>Gets the corresponding Parasolid topology object</summary>
		internal TOPOL_t PsTopology { get; set; }

		/// <summary>Gets the Parasolid tag of the corresponding Parasolid topology object</summary>
		public int PsTag
		{
			get
			{
				return PsTopology.Value;
			}
		}

		/// <summary>Create a new Topology.Topo object</summary>
		/// <param name="psTopo"></param>
		internal Topo(TOPOL_t psTopo)
		{
			PsTopology = psTopo;
		}

		internal Topo()
		{
			PsTopology = TOPOL_t.@null;
		}

		/// <summary>
		/// Returns a string representation of the object
		/// </summary>
		/// <returns>A string containing the object type and its Parasolid tag</returns>
		/// <remarks>
		/// Returns an empty zero-length string if this object is <c>Nothing</c>
		/// </remarks>
		public override string ToString()
		{
			string result = "";
			if (this != null && this.PsTopology != TOPOL_t.@null)
			{
				result = "Topo:" + this.PsTag.ToString();
			}
			return result;
		}
	}

}
