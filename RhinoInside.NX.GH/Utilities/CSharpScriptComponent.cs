using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using Grasshopper;
using Grasshopper.GUI.HTML;
using Grasshopper.GUI.Script;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Parameters.Hints;
using Microsoft.CSharp;
using RhinoInside.NX.GH.Parameters.Hints;
using ScriptComponents;

namespace RhinoInside.NX.GH.Utilities
{
    public class Component_NXOpen_CSNET_Script : Component_CSNET_Script
    {
        public Component_NXOpen_CSNET_Script() : base()
        {
            base.Name = "NXOpen C# Script";
            base.Category = "NX";
            base.SubCategory = "Utilities";

            foreach (var nxopenAssembly in Loader.NXOpenAssemblies)
            {
                ScriptSource.References.Add(Path.Combine(Loader.NXBinPath, nxopenAssembly + ".dll"));
            }

            ScriptSource.References.Add(Path.Combine(Loader.RhinoInsideDirectory, "Startup", "RhinoInside.NX.Extensions.dll"));

            ScriptSource.References.Add(Path.Combine(Loader.RhinoInsideDirectory, "Startup", "RhinoInside.NX.GH.dll"));

            ScriptSource.References.Add(Path.Combine(Loader.RhinoInsideDirectory, "Startup", "RhinoInside.NX.Core.dll"));

            ScriptSource.References.Add(Path.Combine(Loader.RhinoInsideDirectory, "Application", "RhinoInside.NX.GH.Loader.dll"));

            ScriptSource.References.Add(Path.Combine(Loader.RhinoInsideDirectory, "Startup", "RhinoInside.NX.Translator.dll"));
        }

        protected override List<IGH_TypeHint> AvailableTypeHints
        {
            get
            {
                var originalTypeHints = base.AvailableTypeHints;

                originalTypeHints.Add(new NX_BodyHint());
                originalTypeHints.Add(new NX_FaceHint());

                return originalTypeHints;
            }
        }

        public override Guid ComponentGuid => new Guid("774E8C94-3979-4837-B4D1-568298087453");

        protected override string CreateSourceForEdit(ScriptSource script)
        {
            var originalSource = base.CreateSourceForEdit(script);

            //originalSource = originalSource.Insert(0, "using RhinoInside.NX.GH.Types;\r\n");

            originalSource = originalSource.Insert(0, "using NXOpen;\r\n");

            return originalSource;
        }

        protected override CompilerResults CompileSource(string source)
        {
            var compileResult = base.CompileSource(source);
            return compileResult;
        }
    }
}
