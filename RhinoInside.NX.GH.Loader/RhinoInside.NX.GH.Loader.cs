using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Grasshopper;

namespace RhinoInside.NX.GH
{ 
    public class Loader : GH_AssemblyPriority
    {
        private static string NXBinPath;

        static Loader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveHandler;

            var nxModuleFileName = new FileInfo(Process.GetCurrentProcess().MainModule.FileName);

            NXBinPath = Path.Combine(nxModuleFileName.DirectoryName, "managed");
        }

        private static HashSet<string> NXOpenAssemblies = new HashSet<string>
        {
            "NXOpen",
            "NXOpen.UF",
            "NXOpen.Utilities",
            "NXOpenUI"
        };

        internal static Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args)
        {
            int length = args.Name.IndexOf(',');
            string assemblyName = args.Name.Substring(0, length);

            if (NXOpenAssemblies.Contains(assemblyName))
            {
                return Assembly.LoadFrom(Path.Combine(NXBinPath, assemblyName + ".dll"));
            }
            else if (assemblyName == "RhinoInside.NX.Extensions")
            {
                var rhinoInsideStartupDirectory = Environment.GetEnvironmentVariable("RhinoInside_Startup_Directory");
                return Assembly.LoadFrom(Path.Combine(rhinoInsideStartupDirectory, assemblyName + ".dll"));
            }
            else
            {
                Console.WriteLine($"无法加载{assemblyName}");
                return null;
            }
        }

        public override GH_LoadingInstruction PriorityLoad()
        {
            if (Process.GetCurrentProcess().ProcessName == "ugraf")
            {
                LoadComponent();
                return GH_LoadingInstruction.Proceed;
            }
            else
            {
                return GH_LoadingInstruction.Abort;
            }
        }

        public Loader()
        {

        }

        bool LoadComponent()
        {
            {
                var bCoff = Instances.Settings.GetValue("Assemblies:COFF", false);
                try
                {
                    Instances.Settings.SetValue("Assemblies:COFF", false);

                   var rhinoInsideProgramDirectory = Environment.GetEnvironmentVariable("RhinoInside_Program_Directory");

                    var location = Path.Combine(rhinoInsideProgramDirectory, "Startup", "RhinoInside.NX.GH.gha");
                    if (!LoadGHA(location))
                    {
                        if (!File.Exists(location))
                            throw new FileNotFoundException("File Not Found.", location);

                        if (CentralSettings.IsLoadProtected(location))
                            throw new InvalidOperationException($"Assembly '{location}' is load protected.");

                        return false;
                    }
                }
                finally
                {
                    Instances.Settings.SetValue("Assemblies:COFF", bCoff);
                }
            }
            GH_ComponentServer.UpdateRibbonUI();
            return true;
        }

        static bool LoadGHA(string filePath)
        {
            var LoadGHAProc = typeof(GH_ComponentServer).GetMethod("LoadGHA", BindingFlags.NonPublic | BindingFlags.Instance);
            if (LoadGHAProc == null)
            {
                var message = new StringBuilder();
                message.AppendLine("An attempt is made to invoke an invalid target method.");
                message.AppendLine();
                var assembly = typeof(GH_ComponentServer).Assembly;
                var assemblyName = assembly.GetName();

                message.AppendLine($"Assembly Version={assemblyName.Version}");
                message.AppendLine($"{assembly.Location.Replace(' ', (char)0xA0)}");

                throw new TargetException(message.ToString());
            }

            try
            {
                return (bool)LoadGHAProc.Invoke
                (
                  Instances.ComponentServer,
                  new object[] { new GH_ExternalFile(filePath), false }
                );
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }
    }
}
