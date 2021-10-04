using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using PLMComponents.Parasolid.PK_.Unsafe;
using RhinoInside.NX.Extensions.Parasolid;

namespace RhinoInside.NX.Extensions.Topology
{
    /// <summary>A shell is a connected set of faces forming part of the boundary of a body</summary>
    /// <remarks>
    /// Two faces can belong to the same shell only if they are "connected"
    /// in the sense that they share a common edge.
    /// <para>
    /// A typical solid body has a single outer shell and perhaps some inner shells
    /// that define internal voids.
    /// </para>
    /// </remarks>
    public class Shell : Topo
    {
        internal SHELL_t PsShell
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

        internal Shell(SHELL_t psShell) : base(psShell)
        {
            this.PsShell = psShell;
        }

        /// <summary>The array of faces in the shell</summary>&gt;
        public unsafe Face[] Faces
        {
            get
            {
                int num = 0;
                FACE_t* ptr;
                LOGICAL_t* pointer;
                SHELL.ask_oriented_faces(PsShell, &num, &ptr, &pointer);
                Face[] array = new Face[num];
                for (int i = 0; i < num; i++)
                {
                    Face face = ptr[i].GetFace();
                    array[i] = face;
                }
                if (num > 0)
                {
                    MEMORY.free((void*)ptr);
                }
                if (num > 0)
                {
                    MEMORY.free((void*)pointer);
                }
                return array;
            }
        }

        /// <summary>The type of the shell (Outer, Inner, or Open)</summary>
        public unsafe ShellType Type
        {
            get
            {
                ShellType result = ShellType.Open;
                SHELL.sign_t sign_t;
                SHELL.find_sign(this.PsShell, &sign_t);
                if (sign_t == SHELL.sign_t.positive_c)
                {
                    result = ShellType.Outer;
                }
                if (sign_t == SHELL.sign_t.negative_c)
                {
                    result = ShellType.Inner;
                }
                return result;
            }
        }

        /// <summary>The body that contains this shell</summary>
        public unsafe Body Body
        {
            get
            {
                BODY_t psBody;
                SHELL.ask_body(this.PsShell, &psBody);
                return psBody.GetBody();
            }
        }

        /// <summary>
        /// Returns a string representation of the shell
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
                result = "Shell: " + base.PsTag.ToString();
            }
            return result;
        }
    }

    /// <summary>
    /// The type of a 
    /// <see cref="T:NXOpen.Extensions.Topology.Shell">Shell</see> object.
    /// </summary>
    public enum ShellType
    {
        /// <summary>The shell represents the exterior surface of a solid body</summary>
        Outer = 1,
        /// <summary>The shell represents a void inside a solid body</summary>
        Inner = -1,
        /// <summary>The shell represents a sheet body</summary>
        Open
    }
}