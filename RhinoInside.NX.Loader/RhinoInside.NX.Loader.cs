using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using NXOpen;
using System.Linq;
using System.Threading.Tasks;
using RhinoInside.NX.Extensions;
using static RhinoInside.NX.Extensions.Globals;

namespace RhinoInside.NX
{
    public class Loader : MarshalByRefObject
    {
        private static Assembly CoreAssembly;

        private static string CoreAssemblyName = Path.Combine(StartupPath, "RhinoInside.NX.Core.dll");

        private static bool loadedNetAssemblies = false;

        public static int Tess = 199;

        private static HashSet<string> RhinoAssemblies = new HashSet<string>
        {
            "RhinoCommon",
            "Eto",
            "Rhino.UI",
            "RhinoWindows"
        };

        private static HashSet<string> GrasshopperAssemblies = new HashSet<string>
        {
            "Grasshopper",
            "GH_IO",
        };

        public static readonly string RhinoInstallPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\McNeel\Rhinoceros\7.0\Install", "InstallPath", null) as string ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Rhino 7", "System");

        public static readonly string RhinoSystemPath = Path.Combine(RhinoInstallPath, "System");

        public static readonly string GrasshopperPath = Path.Combine(RhinoInstallPath, "Plug-ins", "Grasshopper");

        static Loader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveHandler;
            AppDomain.CurrentDomain.AssemblyLoad += AssemblyLoadHandler;
            AppDomain.CurrentDomain.TypeResolve += TypeResolveHandler;
            AppDomain.CurrentDomain.ResourceResolve += ResourceResolveHandler;

            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        }

        public static int Startup()
        {
            NXOpen.UF.UFSession.GetUFSession().UF.GetRelease(out string release);

            if (System.Convert.ToInt32(release.Split(new char[] { '.' })[0].Replace("NX V", "")) < 1847)
            {
                "需要 NX 版本 1847 以上才能运行RhinoInside.NX，请先升级".ShowNXMessageBox(NXMessageBox.DialogType.Error);
                Logger.Error("需要 NX 版本 1847 以上才能运行RhinoInside.NX，请先升级");
                return 0;
            }

            try
            {
                CoreAssembly = Assembly.LoadFrom(CoreAssemblyName);

                Run("Register", "", out var ss, out int rs);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.ToString());
                return 1;
            }
            return 0;
        }

        public static void AssemblyLoadHandler(object sender, AssemblyLoadEventArgs args)
        {
            Logger.Info("RhinoInside.Loader Loaded assembly: " + args.LoadedAssembly.FullName + " from " + args.LoadedAssembly.Location);
        }

        public static Assembly ResourceResolveHandler(object sender, ResolveEventArgs args)
        {
            Logger.Error("Resource Not Found: " + args.Name);
            return null;
        }

        public static Assembly TypeResolveHandler(object sender, ResolveEventArgs args)
        {
            if (!loadedNetAssemblies)
            {
                loadedNetAssemblies = true;

                Logger.Info("TypeResolveHandler : " + args.Name);

                foreach (string assemblyName in RhinoAssemblies)
                {
                    string assemblyPath = GetAssemblyPath(assemblyName);
                    if (!string.IsNullOrEmpty(assemblyPath))
                    {
                        Assembly assembly = Assembly.LoadFrom(assemblyPath);
                        if (assembly == null)
                        {
                            Logger.Error("RhinoInside Failed to load: " + assemblyPath);
                        }
                        else
                        {
                            AssemblyName name = assembly.GetName(false);
                            Logger.Info("RhinoInside Loaded: " + name.ToString());
                        }
                    }
                }
            }
            return null;
        }

        private static string GetAssemblyPath(string assemblyName)
        {
            string assemFullPath = null;
            if (GrasshopperAssemblies.Contains(assemblyName))
                assemFullPath = Path.Combine(GrasshopperPath, assemblyName + ".dll");
            else if (RhinoAssemblies.Contains(assemblyName))
                assemFullPath = Path.Combine(RhinoSystemPath, assemblyName + ".dll");
            else
            {
                Logger.Error($"Undefined Assembly Name {assemblyName}");
                return null;
            }

            if (!File.Exists(assemFullPath))
            {
                Logger.Error("RhinoInside Failed to find: " + assemblyName);
                return null;
            }
            else
                return assemFullPath;
        }

        public static Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args)
        {
            Logger.Info("RhinoInside Resolve failed: " + args.Name);
            int length = args.Name.IndexOf(',');
            string assemblyName = args.Name.Substring(0, length);

            string assemblyPath = GetAssemblyPath(assemblyName);
            if (string.IsNullOrEmpty(assemblyPath))
                return null;

            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            if (assembly == null)
            {
                Logger.Error("RhinoInside Failed to load: " + assemblyName);

                return null;
            }
            AssemblyName name = assembly.GetName(false);
            Logger.Info("RhinoInside Loaded: " + name.ToString());

            return assembly;
        }

        private static MethodInfo GetMethod(string methodName)
        {
            MethodInfo entryPoint = CoreAssembly.EntryPoint;
            if (entryPoint != null && entryPoint.Name == methodName)
            {
                Logger.Info("Using entryPoint method " + methodName + "\n");
                return entryPoint;
            }
            Type[] types = CoreAssembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                foreach (MethodInfo methodInfo in types[i].GetMethods(BindingFlags.Static | BindingFlags.Public))
                {
                    while (methodInfo.Name == methodName)
                    {
                        if (methodInfo.Name == "UnloadLibrary")
                        {
                            if (!(methodInfo.ReturnType != typeof(int)) || !(methodInfo.ReturnType != typeof(void)))
                            {
                                return methodInfo;
                            }
                            Logger.Warn("Found method, but return type is neither int nor void");
                        }
                        else
                        {
                            ParameterInfo[] parameters = methodInfo.GetParameters();
                            if (parameters.Length > 2)
                            {
                                Logger.Warn("Found method, but it takes " + parameters.Length + " parameters");
                            }
                            else if (methodInfo.Name == "Main")
                            {
                                if (parameters.Length != 0)
                                {
                                    Type parameterType = parameters[0].ParameterType;
                                    if (!parameterType.IsArray)
                                    {
                                        Logger.Warn("Got Main method, but arg is not an array\n");
                                        break;
                                    }
                                    if (parameterType.GetElementType() != typeof(string))
                                    {
                                        Logger.Warn("Got Main method, but arg is not an array of String\n");
                                        break;
                                    }
                                }
                                if (!(methodInfo.ReturnType != typeof(int)) || !(methodInfo.ReturnType != typeof(void)))
                                {
                                    return methodInfo;
                                }
                                Logger.Warn("Got Main method, but return type is not int or void\n");
                            }
                            else if (parameters.Length > 1)
                            {
                                Logger.Warn("Found method, but it takes " + parameters.Length + " parameters\n");
                            }
                            else
                            {
                                if (parameters.Length == 1)
                                {
                                    Type type = parameters[0].ParameterType;
                                    if (type.IsArray)
                                    {
                                        type = type.GetElementType();
                                    }
                                    if (type != typeof(string))
                                    {
                                        Logger.Warn("Found method, but parameter is not a String or array of Strings");
                                        break;
                                    }
                                }
                                if (!(methodInfo.ReturnType != typeof(int)) || !(methodInfo.ReturnType != typeof(string)))
                                {
                                    return methodInfo;
                                }
                                Logger.Warn("Found method, but return type is not an integer or string");
                            }
                        }
                        break;
                    }
                }
            }
            return null;
        }

        private static CoreArrayType GetCoreTypeOfType(Type type)
        {
            CoreArrayType coreArrayType = new Loader.CoreArrayType();
            while (type.IsArray)
            {
                type = type.GetElementType();
                coreArrayType.dimensionality++;
            }
            coreArrayType.coreType = type;
            return coreArrayType;
        }

        private static void CheckTypesEqual(Type parameterType, object argument)
        {
            if (parameterType == typeof(object))
            {
                return;
            }
            if (argument == null)
            {
                if (parameterType == Type.GetType("System.Double") || parameterType == Type.GetType("System.Int32") || parameterType == Type.GetType("System.Boolean"))
                {
                    throw new ArgumentException("Cannot pass a null argument to primitive type " + parameterType);
                }
                return;
            }
            else
            {
                Type type = argument.GetType();
                Loader.CoreArrayType coreTypeOfType = Loader.GetCoreTypeOfType(parameterType);
                Loader.CoreArrayType coreTypeOfType2 = Loader.GetCoreTypeOfType(type);
                if (coreTypeOfType.dimensionality == coreTypeOfType2.dimensionality)
                {
                    if (coreTypeOfType.dimensionality > 0)
                    {
                        return;
                    }
                    if (parameterType.IsAssignableFrom(type))
                    {
                        return;
                    }
                    if (parameterType == typeof(double) && type == typeof(int))
                    {
                        return;
                    }
                    if (Type.GetType("NXOpen.TaggedObject, NXOpen.Utilities").IsAssignableFrom(parameterType) && type == typeof(uint))
                    {
                        return;
                    }
                    throw new ArgumentException(string.Concat(new object[]
                    {
                    "parameter type ",
                    parameterType,
                    " and supplied type ",
                    type,
                    " are not compatible"
                    }));
                }
                else
                {
                    if (coreTypeOfType.dimensionality == 0)
                    {
                        throw new ArgumentException(string.Concat(new object[]
                        {
                        "Cannot pass array argument ",
                        argument,
                        " to non-array parameter type ",
                        parameterType
                        }));
                    }
                    int num = coreTypeOfType2.dimensionality;
                    if (num == 0)
                    {
                        num = (argument.GetType().IsArray ? 1 : 0);
                    }
                    if (num == 0)
                    {
                        throw new ArgumentException(string.Concat(new object[]
                        {
                        "Cannot pass non-array argument ",
                        argument,
                        " to array parameter type ",
                        parameterType
                        }));
                    }
                    return;
                }
            }
        }

        internal static object ConvertToParameterTypeInternal(Type parameterType, object argument)
        {
            if (argument == null)
            {
                return argument;
            }
            Type type = argument.GetType();
            if (!parameterType.IsArray)
            {
                if (Type.GetType("NXOpen.TaggedObject, NXOpen.Utilities").IsAssignableFrom(parameterType) && type == typeof(uint))
                {
                    return Type.GetType("NXOpen.Utilities.NXObjectManager, NXOpen.Utilities").GetMethod("GetObjectFromUInt").Invoke(null, new object[]
                    {
                    argument
                    });
                }
                return argument;
            }
            else
            {
                if (!type.IsArray)
                {
                    throw new ArgumentException(string.Concat(new object[]
                    {
                    "Cannot convert non-array object ",
                    argument,
                    " to array type ",
                    parameterType
                    }));
                }
                if (parameterType.IsAssignableFrom(type))
                {
                    return argument;
                }
                object obj = null;
                try
                {
                    parameterType.GetElementType();
                    type.GetElementType();
                    Array array = (Array)argument;
                    Array array2 = Array.CreateInstance(parameterType.GetElementType(), array.Length);
                    for (int i = 0; i < array2.Length; i++)
                    {
                        object value = array.GetValue(i);
                        object obj2 = Loader.convertToParameterType(parameterType.GetElementType(), value);
                        if (obj2 == null && value != null)
                        {
                            return null;
                        }
                        array2.SetValue(obj2, i);
                    }
                    obj = array2;
                }
                catch (ArrayTypeMismatchException value2)
                {
                    Trace.WriteLine(value2);
                }
                catch (InvalidCastException value3)
                {
                    Trace.WriteLine(value3);
                }
                if (obj == null)
                {
                    throw new ArgumentException(string.Concat(new object[]
                    {
                    "Cannot convert argument ",
                    argument,
                    " to parameter type ",
                    parameterType
                    }));
                }
                return obj;
            }
        }

        internal static object convertToParameterType(Type parameterType, object argument)
        {
            CheckTypesEqual(parameterType, argument);
            return ConvertToParameterTypeInternal(parameterType, argument);
        }

        public static string Run(string methodName, string arg, out string outArg, out int result)
        {
            result = 0;
            outArg = null;
            if (CoreAssembly == null)
                return "Assembly " + CoreAssemblyName + " not loaded";

            MethodInfo method = GetMethod(methodName);
            if (method == null)
                return "Cannot find method: " + methodName;

            string result2 = null;
            try
            {
                object[] array = null;
                if (method.Name == "Main")
                {
                    if (method.GetParameters().Length != 0)
                    {
                        array = new object[1];
                        string[] array2 = new string[]
                        {
                        arg
                        };
                        array[0] = array2;
                    }
                    if (method.ReturnType == typeof(int))
                    {
                        result = (int)method.Invoke(null, array);
                    }
                    else
                    {
                        method.Invoke(null, array);
                    }
                }
                else if (method.ReturnType == typeof(string))
                {
                    result2 = "Method signature not recognised - expecting integer return value";
                }
                else
                {
                    bool flag = false;
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length != 0)
                    {
                        array = new object[1];
                        if (parameters[0].ParameterType.IsArray)
                        {
                            string[] array3 = new string[]
                            {
                            arg
                            };
                            array[0] = array3;
                            flag = true;
                        }
                        else
                        {
                            array[0] = arg;
                        }
                    }
                    if (method.ReturnType == typeof(void))
                    {
                        method.Invoke(null, array);
                        result = 0;
                    }
                    else
                    {
                        result = (int)method.Invoke(null, array);
                    }
                    if (flag)
                    {
                        string[] array4 = (string[])array[0];
                        if (array4[0] != arg)
                        {
                            outArg = array4[0];
                        }
                    }
                }
            }
            catch (TargetInvocationException ex)
            {
                Logger.Error("Caught exception while running: " + methodName);
                try
                {
                    Logger.Error(ex.InnerException.ToString());
                }
                catch (Exception ex2)
                {
                    Logger.Error("Exception while trying to print stack trace: " + ex2.Message);
                    Logger.Error("Original exception: " + ex.InnerException.Message);
                }
                result2 = ex.InnerException.Message;
            }
            catch (Exception ex3)
            {
                Logger.Error("Caught unexpected exception");
                Logger.Error(ex3.Message);
                result2 = ex3.Message;
            }
            return result2;
        }

        internal class CoreArrayType
        {
            public override string ToString()
            {
                return string.Concat(new object[]
                {
                "{core type ",
                this.coreType,
                ", dimensionality ",
                this.dimensionality,
                "}"
                });
            }

            // Token: 0x0400000B RID: 11
            public int dimensionality;

            // Token: 0x0400000C RID: 12
            public Type coreType;
        }

        //------------------------------------------------------------------------------
        // This method specifies how a shared image is unloaded from memory
        // within NX. This method gives you the capability to unload an
        // internal NX Open application or user  exit from NX. Specify any
        // one of the three constants as a return value to determine the type
        // of unload to perform:
        //
        //
        //    Immediately : unload the library as soon as the automation program has completed
        //    Explicitly  : unload the library from the "Unload Shared Image" dialog
        //    AtTermination : unload the library when the NX session terminates
        //
        //
        // NOTE:  A program which associates NX Open applications with the menubar
        // MUST NOT use this option since it will UNLOAD your NX Open application image
        // from the menubar.
        //------------------------------------------------------------------------------
        public static int GetUnloadOption(string arg)
        {
            //return System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);
            return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
            // return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
        }

        //------------------------------------------------------------------------------
        // Following method cleanup any housekeeping chores that may be needed.
        // This method is automatically called by NX.
        //------------------------------------------------------------------------------
        public static void UnloadLibrary(string arg)
        {
            try
            {
                Console.WriteLine("Unloading image.");
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                TheUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
        }
    }
}