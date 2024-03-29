﻿using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using RhinoInside.NX.Translator;
using RhinoInside.NX.Translator.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.GH.Types
{
    public class NX_Body : NX_DisplayableObject<NXOpen.Body>
    {
        public override string TypeName => "NX Body";

        public override string TypeDescription => Properties.Languages.GetString("NXBodyTypeDesc");

        public NX_Body() : base()
        {

        }

        /// <summary>
        /// 存放于当前工作部件中的对象
        /// </summary>
        /// <param name="handle"></param>
        public NX_Body(NXOpen.Tag tag) : base(tag)
        {

        }

        public NX_Body(NXOpen.Tag tag, bool highlight) : base(tag, highlight)
        {

        }

        public bool CastToOld<Q>(ref Q target)
        {
            if (typeof(Q) == typeof(GH_Brep))
            {
                target = (Q)(object)new GH_Brep(Value.ToBrep());
                return true;
            }
            else
            {
                Console.WriteLine(typeof(Q));
                return base.CastTo(ref target);
            }
        }

        /// <summary>
        /// 中间 STEP 文件
        /// </summary>
        internal string InterFile;

        public override bool CastTo<Q>(ref Q target)
        {
            if (typeof(Q) == typeof(GH_Brep))
            {
                var body = Value;

                InterFile = SolidExchanger.NXExport(body);

                var s = SolidExchanger.GrasshopperImport(InterFile);

                target = (Q)(object)new GH_Brep(s);

                return true;
            }
            else
            {
                Console.WriteLine($"无法将 NXOpen.Body 转换为 {typeof(Q)}");
                return base.CastTo(ref target);
            }
        }

        public override bool CastFrom(object source)
        {
            if (source is GH_Brep gh_brep)
            {
                var interFile = SolidExchanger.GrasshopperExport(gh_brep.Value);

                Value = SolidExchanger.NXImport(interFile);

                return true;
            }
            else
            {
                Console.WriteLine($"无法将 {source.GetType()} 转换为 NXOpen.Body");
                return base.CastFrom(source);
            }
        }

        public override string ToString()
        {
            if (Value != null)
                return base.ToString() + $"Solid Body: {Value.IsSolidBody}";

            return string.Empty;
        }
    }
}

namespace RhinoInside.NX.GH.Parameters.Hints
{
    public class NX_BodyHint : IGH_TypeHint
    {
        public string TypeName => "NXOpen.Body";

        public Guid HintID => new Guid("96FA89B5-2370-4A0E-AD5C-7145A37D8E9A");

        public bool Cast(object data, out object target)
        {
            target = data;

            return true;
        }
    }
}