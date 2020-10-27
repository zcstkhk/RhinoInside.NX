using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using NXOpen.Extensions;

namespace RhinoInside.NX.Loader
{
    public class Loader : MarshalByRefObject
    {
        private static Assembly CoreAssembly;

        private static string CoreAssemblyName = Path.Combine(Globals.StartupPath, "RhinoInside.NX.Core.dll");

        private static TraceListener traceListener;

        private static bool loadedNetAssemblies = false;

        private static HashSet<string> RhinoAssemblies = new HashSet<string>
        {
        "RhinoCommon.dll",
        "Grasshopper.dll",
        "GH_IO.dll",
        "Eto.dll",
        "Rhino.UI.dll"
        };

        public static readonly string RhinoInstallPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\McNeel\Rhinoceros\7.0\Install", "InstallPath", null) as string ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Rhino WIP", "System");

        public static readonly string RhinoSystemPath = Path.Combine(RhinoInstallPath, "System");

        public static readonly string GrasshopperPath = Path.Combine(RhinoInstallPath, "Plug-ins", "Grasshopper");

        static Loader()
        {
            traceListener = new LogTraceListener();
            Trace.Listeners.Add(traceListener);

            Globals.TheUfSession.UF.GetRelease(out string release);
            if (System.Convert.ToInt32(release.Split(new char[] { '.' })[0].Replace("NX V", "")) < 1847)
            {
                Trace.WriteLine("需要 NX 版本 1847 以上才能运行RhinoInside.NX，请先升级");
                return;
            }

            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveHandler;
            AppDomain.CurrentDomain.AssemblyLoad += AssemblyLoadHandler;
            AppDomain.CurrentDomain.TypeResolve += TypeResolveHandler;
        }

        public static int Startup()
        {
            Globals.TheUfSession.UF.GetRelease(out string release);
            if (System.Convert.ToInt32(release.Split(new char[] { '.' })[0].Replace("NX V", "")) < 1847)
            {
                Trace.WriteLine("需要 NX 版本 1847 以上才能运行RhinoInside.NX，请先升级");
                return 0;
            }

            Trace.WriteLine(string.Concat(new object[]
            {
            "RhinoInside.Loader.Load: ",
            CoreAssemblyName,
            " ",
            AppDomain.CurrentDomain
            }));
            Trace.WriteLine("AppBase: " + AppDomain.CurrentDomain.BaseDirectory);

            try
            {
                CoreAssembly = Assembly.LoadFrom(CoreAssemblyName);

                Run("Register", "", out var ss, out int rs);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return 1;
            }
            return 0;
        }

        public static void AssemblyLoadHandler(object sender, AssemblyLoadEventArgs args)
        {
            Trace.WriteLine("RhinoInside.Loader Loaded assembly: " + args.LoadedAssembly.FullName + " from " + args.LoadedAssembly.Location);
        }

        public static Assembly ResourceResolveHandler(object sender, ResolveEventArgs args)
        {
            Console.WriteLine("Resource Not Found: " + args.Name);
            return null;
        }

        public static Assembly TypeResolveHandler(object sender, ResolveEventArgs args)
        {
            if (!loadedNetAssemblies)
            {
                loadedNetAssemblies = true;

                Trace.WriteLine("TypeResolveHandler : " + args.Name);

                foreach (string assemblyName in RhinoAssemblies)
                {
                    string assemblyPath = GetAssemblyPath(assemblyName);
                    if (!string.IsNullOrEmpty(assemblyPath))
                    {
                        Assembly assembly = Assembly.LoadFrom(assemblyPath);
                        if (assembly == null)
                        {
                            Trace.WriteLine("RhinoInside Failed to load: " + assemblyPath);
                        }
                        else
                        {
                            AssemblyName name = assembly.GetName(false);
                            Trace.WriteLine("RhinoInside Loaded: " + name.ToString());
                        }
                    }
                }
            }
            return null;
        }

        private static string GetAssemblyPath(string assemblyName)
        {
            if (!assemblyName.EndsWith(".dll"))
                assemblyName += ".dll";

            string assemFullPath = null;
            if (assemblyName.StartsWith("Grasshopper") || assemblyName.StartsWith("GH"))
                assemFullPath = Path.Combine(GrasshopperPath, assemblyName);
            else
                assemFullPath = Path.Combine(RhinoSystemPath, assemblyName);

            if (!File.Exists(assemFullPath))
            {
                Console.WriteLine("RhinoInside Failed to find: " + assemblyName);
                return null;
            }
            else
                return assemFullPath;
        }

        public static Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args)
        {
            Trace.WriteLine("RhinoInside Resolve failed: " + args.Name);
            int length = args.Name.IndexOf(',');
            string assemblyName = args.Name.Substring(0, length);

            if (assemblyName.EndsWith(".resources"))
                assemblyName = assemblyName.Substring(0, assemblyName.Length - 10);

            string assemblyPath = GetAssemblyPath(assemblyName);
            if (string.IsNullOrEmpty(assemblyPath))
                return null;

            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            if (assembly == null)
            {
                Trace.WriteLine("RhinoInside Failed to load: " + assemblyName);

                return assembly;
            }
            AssemblyName name = assembly.GetName(false);
            Trace.WriteLine("RhinoInside Loaded: " + name.ToString());

            return assembly;
        }

        private static MethodInfo GetMethod(string methodName)
        {
            MethodInfo entryPoint = CoreAssembly.EntryPoint;
            if (entryPoint != null && entryPoint.Name == methodName)
            {
                Trace.WriteLine("Using entryPoint method " + methodName + "\n");
                return entryPoint;
            }
            Type[] types = CoreAssembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                foreach (MethodInfo methodInfo in types[i].GetMethods(BindingFlags.Static | BindingFlags.Public))
                {
                    if (methodInfo.Name == methodName)
                    {
                        if (methodInfo.Name == "UnloadLibrary")
                        {
                            if (!(methodInfo.ReturnType != typeof(int)) || !(methodInfo.ReturnType != typeof(void)))
                            {
                                return methodInfo;
                            }
                            Trace.WriteLine("Found method, but return type is neither int nor void");
                        }
                        else
                        {
                            ParameterInfo[] parameters = methodInfo.GetParameters();
                            if (parameters.Length > 2)
                            {
                                Trace.WriteLine("Found method, but it takes " + parameters.Length + " parameters");
                            }
                            else if (methodInfo.Name == "Main")
                            {
                                if (parameters.Length != 0)
                                {
                                    Type parameterType = parameters[0].ParameterType;
                                    if (!parameterType.IsArray)
                                    {
                                        Trace.WriteLine("Got Main method, but arg is not an array\n");
                                        goto IL_251;
                                    }
                                    if (parameterType.GetElementType() != typeof(string))
                                    {
                                        Trace.WriteLine("Got Main method, but arg is not an array of String\n");
                                        goto IL_251;
                                    }
                                }
                                if (!(methodInfo.ReturnType != typeof(int)) || !(methodInfo.ReturnType != typeof(void)))
                                {
                                    return methodInfo;
                                }
                                Trace.WriteLine("Got Main method, but return type is not int or void\n");
                            }
                            else if (parameters.Length > 1)
                            {
                                Trace.WriteLine("Found method, but it takes " + parameters.Length + " parameters\n");
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
                                        Trace.WriteLine("Found method, but parameter is not a String or array of Strings");
                                        goto IL_251;
                                    }
                                }
                                if (!(methodInfo.ReturnType != typeof(int)) || !(methodInfo.ReturnType != typeof(string)))
                                {
                                    return methodInfo;
                                }
                                Trace.WriteLine("Found method, but return type is not an integer or string");
                            }
                        }
                    }
                    IL_251:;
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
            Loader.CheckTypesEqual(parameterType, argument);
            return Loader.ConvertToParameterTypeInternal(parameterType, argument);
        }

        private MethodInfo GetMethod(string className, string methodName, object[] args)
        {
            foreach (Type type in CoreAssembly.GetTypes())
            {
                if (type.Name.Equals(className))
                {
                    string text = null;
                    foreach (MethodInfo methodInfo in type.GetMethods())
                    {
                        if (methodInfo.Name.Equals(methodName))
                        {
                            if (!methodInfo.IsStatic)
                            {
                                text = string.Format("Method {0} is not static", methodInfo);
                            }
                            else
                            {
                                ParameterInfo[] parameters = methodInfo.GetParameters();
                                if (parameters.Length == args.Length)
                                {
                                    try
                                    {
                                        for (int k = 0; k < parameters.Length; k++)
                                        {
                                            Type parameterType = parameters[k].ParameterType;
                                            args[k] = Loader.convertToParameterType(parameterType, args[k]);
                                        }
                                        return methodInfo;
                                    }
                                    catch (ArgumentException ex)
                                    {
                                        text = ex.Message;
                                        goto IL_D7;
                                    }
                                }
                                text = string.Format("Method {0} has {1} parameters, {2} arguments were supplied\n", methodInfo, parameters.Length, args.Length);
                            }
                        }
                        IL_D7:;
                    }
                    if (text == null)
                    {
                        text = string.Format("Cannot find method named {0} in class {1}", methodName, className);
                    }
                    throw new MethodAccessException(text);
                }
            }
            throw new MethodAccessException(string.Format("Cannot find class named {0} in assembly {1}", className, CoreAssembly));
        }

        public string RunVariant(string className, string methodName, object[] args, out object result)
        {
            result = null;
            if (CoreAssembly == null)
            {
                return "Assembly " + CoreAssemblyName + " not loaded";
            }
            string result2 = null;
            try
            {
                MethodInfo method = this.GetMethod(className, methodName, args);
                if (method.ReturnType == typeof(void))
                {
                    method.Invoke(null, args);
                }
                else
                {
                    result = method.Invoke(null, args);
                }
            }
            catch (TargetInvocationException ex)
            {
                Trace.WriteLine("Caught exception while running: " + methodName);
                try
                {
                    Trace.WriteLine(ex.InnerException.ToString());
                }
                catch (Exception ex2)
                {
                    Trace.WriteLine("Exception while trying to print stack trace: " + ex2.Message);
                    Trace.WriteLine("Original exception: " + ex.InnerException.Message);
                }
                result2 = ex.InnerException.Message;
            }
            catch (ArgumentException ex3)
            {
                Trace.WriteLine("Invalid arguments");
                Trace.WriteLine(ex3.Message);
                result2 = ex3.Message;
            }
            catch (MethodAccessException ex4)
            {
                Trace.WriteLine("Cannot find method");
                Trace.WriteLine(ex4.Message);
                result2 = ex4.Message;
            }
            return result2;
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
                Trace.WriteLine("Caught exception while running: " + methodName);
                try
                {
                    Trace.WriteLine(ex.InnerException.ToString());
                }
                catch (Exception ex2)
                {
                    Trace.WriteLine("Exception while trying to print stack trace: " + ex2.Message);
                    Trace.WriteLine("Original exception: " + ex.InnerException.Message);
                }
                result2 = ex.InnerException.Message;
            }
            catch (Exception ex3)
            {
                Trace.WriteLine("Caught unexpected exception");
                Trace.WriteLine(ex3.Message);
                result2 = ex3.Message;
            }
            return result2;
        }

        public string Run(string methodName, string[] arg, out string outArg, out int result)
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
                        string[] array2 = new string[arg.Length];
                        Array.Copy(arg, array2, arg.Length);
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
                            string[] array3 = new string[arg.Length];
                            Array.Copy(arg, array3, arg.Length);
                            array[0] = array3;
                            flag = true;
                        }
                        else
                        {
                            Console.WriteLine("Main signature doesn't accept arrays .... so just passing in first argument.\n");
                            array[0] = arg[0];
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
                        if (array4[0] != arg[0])
                        {
                            outArg = array4[0];
                        }
                    }
                }
            }
            catch (TargetInvocationException ex)
            {
                Trace.WriteLine("Caught exception while running: " + methodName);
                try
                {
                    Trace.WriteLine(ex.InnerException.ToString());
                }
                catch (Exception ex2)
                {
                    Trace.WriteLine("Exception while trying to print stack trace: " + ex2.Message);
                    Trace.WriteLine("Original exception: " + ex.InnerException.Message);
                }
                result2 = ex.InnerException.Message;
            }
            catch (Exception ex3)
            {
                Trace.WriteLine("Caught unexpected exception");
                Trace.WriteLine(ex3.Message);
                result2 = ex3.Message;
            }
            return result2;
        }

        public int RunStringArray(string methodName, string[] args, out string[] results)
        {
            Type[] types = CoreAssembly.GetTypes();
            try
            {
                Type[] array = types;
                for (int i = 0; i < array.Length; i++)
                {
                    foreach (MethodInfo methodInfo in array[i].GetMethods(BindingFlags.Static | BindingFlags.Public))
                    {
                        if (methodInfo.Name == methodName)
                        {
                            ParameterInfo[] parameters = methodInfo.GetParameters();
                            if (parameters.Length == 1)
                            {
                                Type parameterType = parameters[0].ParameterType;
                                Type returnType = methodInfo.ReturnType;
                                if (parameterType.IsArray && parameterType.GetElementType() == typeof(string) && returnType.IsArray && returnType.GetElementType() == typeof(string))
                                {
                                    results = (string[])methodInfo.Invoke(null, new object[]
                                    {
                                    args
                                    });
                                    return 0;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                results = new string[]
                {
                ex.ToString()
                };
                return 1;
            }
            results = null;
            return 2;
        }

        public int CheckMethod(string methodName)
        {
            if (CoreAssembly == null)
            {
                return 0;
            }
            if (!(GetMethod(methodName) == null))
            {
                return 1;
            }
            return 0;
        }

        private class LogTraceListener : TraceListener, IDisposable
        {
            StreamWriter logWriter;

            public LogTraceListener()
            {
                if (!Directory.Exists(Path.Combine(Globals.RootPath, "Logs")))
                    Directory.CreateDirectory(Path.Combine(Globals.RootPath, "Logs"));

                logWriter = new StreamWriter(Path.Combine(Globals.RootPath, "Logs", "aaa.syslog"));
            }

            public override void Write(string s)
            {
                logWriter.Write(s);
            }

            public override void WriteLine(string s)
            {
                logWriter.WriteLine(s + "\n");
            }


        }

        internal class CoreArrayType
        {
            // Token: 0x06000032 RID: 50 RVA: 0x000037C8 File Offset: 0x000019C8
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
    }
}