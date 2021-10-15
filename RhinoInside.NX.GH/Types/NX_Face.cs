using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RhinoInside.NX.Translator.Geometry;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using RhinoInside.NX.Translator;

namespace RhinoInside.NX.GH.Types
{
    public class NX_Face : NX_DisplayableObject<Face>
    {
        public override string TypeName => "NX Face";

        public override string TypeDescription => Properties.Languages.GetString("NXFaceTypeDesc");

        public NX_Face() : base()
        {

        }

        /// <summary>
        /// 存放于当前工作部件中的对象
        /// </summary>
        /// <param name="handle"></param>
        public NX_Face(Tag tag) : base(tag)
        {

        }

        public NX_Face(Tag tag, bool highlight) : base(tag, highlight)
        {

        }

        public bool CastToOld<Q>(ref Q target)
        {
            if (typeof(Q) == typeof(GH_Brep))
            {
                target = (Q)(object)new GH_Brep(Value.ToBrep());
                return true;
            }
            else if (typeof(Q) == typeof(GH_Surface))
            {
                var rhino = Value.ToRhinoSurface();

                if (rhino == null)
                    return false;

                target = (Q)(object)new GH_Surface(rhino);
                return true;
            }
            else
            {
                Console.WriteLine(typeof(Q));
                return base.CastTo(ref target);
            }
        }

        public override bool CastTo<Q>(ref Q target)
        {
            if (typeof(Q) == typeof(GH_Brep))
            {
                var exportedStp = SolidExchanger.NXExport(Value);

                var import = SolidExchanger.GrasshopperImport(exportedStp);

                target = (Q)(object)new GH_Brep(import);

                return true;
            }
            else if (typeof(Q) == typeof(GH_Surface))
            {
                var rhino = Value.ToRhinoSurface();

                if (rhino == null)
                    return false;

                target = (Q)(object)new GH_Surface(rhino);
                return true;
            }
            else
            {
                Console.WriteLine(typeof(Q));
                return base.CastTo(ref target);
            }
        }
    }
}

namespace RhinoInside.NX.GH.Parameters.Hints
{
    public class NX_FaceHint : IGH_TypeHint
    {
        public string TypeName => "NXOpen.Face";

        public Guid HintID => new Guid("0A761653-D67A-4168-A7E5-06D4AD5FD4CB");

        public bool Cast(object data, out object target)
        {
            target = data;

            return true;
        }
    }
}
