using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Extensions
{
    public static partial class Globals
    {
        public static Assembly ManagedLibraryResolver(object sender, ResolveEventArgs args)
        {
            string[] nxManagedAssemblies = new string[] { "pskernel_net" };

            var assemblyName = new AssemblyName(args.Name).Name;

            if (nxManagedAssemblies.Contains(assemblyName))
            {
                var mainModule = Process.GetCurrentProcess().MainModule;

                var assemblyFullPath = Path.Combine(Path.GetDirectoryName(mainModule.FileName), "managed", assemblyName + ".dll");

                return Assembly.LoadFrom(assemblyFullPath);
            }
            else
                return null;
        }

        private unsafe static string TranslateVariable(string s)
        {
            byte* ptr = ENV_translate_variable(s);
            if (ptr == null)
                return null;

            int num = strlen(ptr);
            byte[] array = new byte[num];
            if (num > 0)
            {
                Marshal.Copy((IntPtr)((void*)ptr), array, 0, num);
            }
            return Encoding.UTF8.GetString(array);
        }

        private unsafe static string LocateManagedDir()
        {
            byte* ptr = LocateDir();
            if (ptr != null)
            {
                int num = strlen(ptr);
                byte[] array = new byte[num];
                if (num > 0)
                    Marshal.Copy((IntPtr)((void*)ptr), array, 0, num);

                SM_free((IntPtr)((void*)ptr));
                return Encoding.UTF8.GetString(array);
            }
            return null;
        }

        private unsafe static int strlen(byte* s)
        {
            byte* ptr = s;
            while (*ptr != 0)
                ptr++;

            return (int)((long)(ptr - s));
        }

        [DllImport("libjam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "JAM_LocateManagedDir")]
        private unsafe static extern byte* LocateDir();

        [DllImport("libjam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "JAM_env_translate_variable")]
        private unsafe static extern byte* ENV_translate_variable(string s);

        [DllImport("libjam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "JAM_sm_free")]
        private static extern void SM_free(IntPtr ptr);
    }
}
