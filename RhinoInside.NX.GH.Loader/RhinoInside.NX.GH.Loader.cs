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
//using RhinoInside.NX.Extensions;
using System.Windows.Forms;

namespace RhinoInside.NX.GH
{
    public class Loader : GH_AssemblyPriority
    {
        public static string NXBinPath;

        public static string RhinoInsideDirectory => Environment.GetEnvironmentVariable("UGII_RhinoInside_Dir");

        static Loader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveHandler;

            var nxModuleFileName = new FileInfo(Process.GetCurrentProcess().MainModule.FileName);

            NXBinPath = Path.Combine(nxModuleFileName.DirectoryName, "managed");
        }

        public static HashSet<string> NXOpenAssemblies = new HashSet<string>
        {
            "NXOpen",
            "NXOpen.UF",
            "NXOpen.Utilities",
            "NXOpenUI",
            "pskernel_net",
            "Snap"
        };

        internal static Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args)
        {
            int length = args.Name.IndexOf(',');
            string assemblyName = args.Name.Substring(0, length);

            if (NXOpenAssemblies.Contains(assemblyName))
            {
                return Assembly.LoadFrom(Path.Combine(NXBinPath, assemblyName + ".dll"));
            }
            else if (assemblyName == "NXOpen.Extensions" || assemblyName == "RhinoInside.NX.Core" || assemblyName == "RhinoInside.NX.Translator")
            {
                return Assembly.LoadFrom(Path.Combine(RhinoInsideDirectory, "Startup", assemblyName + ".dll"));
            }
            else
            {
                //Logger.Error($"无法加载{assemblyName}");
                Console.WriteLine($"无法加载{assemblyName}");
                return null;
            }
        }

        /// <summary>
        /// 加载过程中的回调函数
        /// </summary>
        /// <returns></returns>
        public override GH_LoadingInstruction PriorityLoad()
        {
            if (Process.GetCurrentProcess().ProcessName == "ugraf")
            {
                LoadRhinoInsideNXGHComponent();
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

        bool LoadRhinoInsideNXGHComponent()
        {
            {
                var bCoff = Instances.Settings.GetValue("Assemblies:COFF", false);
                try
                {
                    Instances.Settings.SetValue("Assemblies:COFF", false);

                    var location = Path.Combine(RhinoInsideDirectory, "Application", "RhinoInside.NX.GH.dll");
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
