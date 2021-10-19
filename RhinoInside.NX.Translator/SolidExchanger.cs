using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using static NXOpen.Extensions.Globals;
using NXOpen.Extensions;
using System.IO;
using Grasshopper.Kernel;

namespace RhinoInside.NX.Translator
{
    public static class SolidExchanger
    {
        public static Brep GrasshopperImport(string filePath)
        {
            RhinoDoc.ActiveDoc.Objects.UnselectAll();

            string importCmd = $"!_-Import \"{filePath}\" _Enter";

            Rhino.RhinoApp.RunScript(importCmd, false);

            List<RhinoObject> selectedObjects = RhinoDoc.ActiveDoc.Objects.GetSelectedObjects(false, false).ToList();

            List<Brep> results = new List<Brep>();

            foreach (var obj in selectedObjects)
            {
                Brep geo = obj.Geometry as Brep;
                results.Add(geo);
                RhinoDoc.ActiveDoc.Objects.Delete(obj, true);
            }

            return results[0];
        }

        /// <summary>
        /// NX 1953 中可能无法正确导入 STEP，返回 null
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static NXOpen.Body NXImport(string filePath)
        {
            var originalBodies = WorkPart.Bodies.ToArray();

            var preferenceFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "RhinoInside Preferences.config");

            var currentNXProcessFile = new System.IO.FileInfo(Process.GetCurrentProcess().MainModule.FileName);        // NXBIN\ugraf.exe

            var nxRootPath = currentNXProcessFile.Directory.Parent.FullName;

            //if (!System.IO.File.Exists(preferenceFilePath) || System.IO.File.ReadAllLines(preferenceFilePath).First(obj => obj.Contains("STEPMode")).Split('=')[1] == "Internal")
            if (true)
            {
                NXOpen.Step242Importer step242Importer = TheSession.DexManager.CreateStep242Importer();

                step242Importer.SimplifyGeometry = true;

                step242Importer.Messages = NXOpen.Step242Importer.MessageEnum.None;

                step242Importer.ObjectTypes.Surfaces = true;

                step242Importer.ObjectTypes.Solids = true;

                step242Importer.SimplifyGeometry = false;

                step242Importer.SmoothBSurfaces = false;

                step242Importer.SettingsFile = Path.Combine(nxRootPath, "translators", "step242", "step242ug.def");

                step242Importer.ImportToTeamcenter = false;

#if !NX1847 && !NX1872 && !NX1899 && !NX1926
                step242Importer.SetMode(NXOpen.BaseImporter.Mode.NativeFileSystem);
#endif

                step242Importer.OutputFile = GetTempFileName(".prt");

                step242Importer.InputFile = filePath;

                step242Importer.ObjectTypes.Curves = false;

                step242Importer.ObjectTypes.Surfaces = true;

                step242Importer.ObjectTypes.Solids = true;

                step242Importer.ObjectTypes.Csys = false;

                step242Importer.ObjectTypes.PmiData = false;

#if !NX1847 && !NX1872 && !NX1899 && !NX1926
                step242Importer.ObjectTypes.Kinematic = false;

                step242Importer.ObjectTypes.Attributes = false;
#endif

                step242Importer.SewSurfaces = true;

                step242Importer.SimplifyGeometry = true;

                step242Importer.Optimize = true;

                step242Importer.SmoothBSurfaces = true;

                step242Importer.FlattenAssembly = false;

                step242Importer.FileOpenFlag = false;

                step242Importer.ProcessHoldFlag = false;

                NXOpen.NXObject nXObject = step242Importer.Commit();

                step242Importer.Destroy();
            }
            else
            {
                string outputFile = GetTempFileName("prt");

                using (Process p = new Process())
                {
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                    p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                    p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                    p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                    p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
                    p.Start();      //启动程序

                    //向cmd窗口写入命令
                    p.StandardInput.AutoFlush = true;

                    p.StandardInput.WriteLine(Path.Combine(nxRootPath, "UGII", "nxcommand.bat") + "&exit");

                    string cmd = Path.Combine(nxRootPath, @"step214ug\step214ug.cmd") + $" {filePath} o={outputFile} d=" + Path.Combine(nxRootPath, @"STEP214UG\step214ug.def") + $" l={outputFile}.log" + "&exit";

                    p.StandardInput.WriteLine(cmd);

                    //获取cmd窗口的输出信息
                    p.StandardOutput.ReadToEnd();
                    p.WaitForExit();//等待程序执行完退出进程
                    p.Close();
                }

                PartEx.ImportPart(WorkPart, outputFile, Matrix4x4Ex.Identity);
            }

            if (WorkPart.Bodies.ToArray().Length == originalBodies.Length)
                return null;
            else
            {
                var resultBody = WorkPart.Bodies.ToArray().Last();
                resultBody.SetBooleanUserAttribute("GH_Baked_Object", -1, false, NXOpen.Update.Option.Now);
                return resultBody;
            }

        }

        /// <summary>
        /// 将 NX 体导出为 STEP 242 格式
        /// </summary>
        /// <param name="bodyToExport"></param>
        /// <returns></returns>
        public static string NXExport(NXOpen.Body bodyToExport, bool forceUpate = false)
        {
            var bodyHashCode = bodyToExport.GetHandle().GetHashCode();

            string targetFile = Path.Combine(Path.GetTempPath(), bodyHashCode.ToString() + ".stp");

            if (System.IO.File.Exists(targetFile) && !forceUpate)
                return targetFile;

            NXOpen.StepCreator stepCreator = TheSession.DexManager.CreateStepCreator();

            stepCreator.ExportAs = NXOpen.StepCreator.ExportAsOption.Ap242;

            stepCreator.ColorAndLayers = true;

            var currentNXProcessFile = new System.IO.FileInfo(Process.GetCurrentProcess().MainModule.FileName);        // NXBIN\ugraf.exe

            var nxRootPath = currentNXProcessFile.Directory.Parent;

            stepCreator.SettingsFile = System.IO.Path.Combine(nxRootPath.FullName, "translators", "step242", "ugstep242.def");

            stepCreator.ExportSelectionBlock.SelectionScope = NXOpen.ObjectSelector.Scope.SelectedObjects;

            stepCreator.ExportSelectionBlock.SelectionComp.Add(bodyToExport);

            stepCreator.OutputFile = targetFile;

            stepCreator.FileSaveFlag = false;

            stepCreator.LayerMask = "1-256";

            stepCreator.ProcessHoldFlag = true;

            stepCreator.ExportSolidsAndSurfacesAs = NXOpen.StepCreator.ExportSolidsAndSurfacesAsOption.Precise;

#if !NX1847 && !NX1872 && !NX1899 && ! NX1926
            stepCreator.ExportConvergentAs = NXOpen.StepCreator.ExportConvergentAsOption.SplitPreciseAndTessellated;
#endif

            NXOpen.NXObject nXObject1 = stepCreator.Commit();

            stepCreator.Destroy();

            return targetFile;
        }

        public static string NXExport(NXOpen.Face faceToExport)
        {
            var copiedFace = faceToExport.CopyAndPaste()[0] as NXOpen.Body;

            var exportedStpFile = NXExport(copiedFace);

            copiedFace.Delete();

            return exportedStpFile;
        }

        public static string GrasshopperExport(Brep brep)
        {
            RhinoDoc.ActiveDoc.Objects.UnselectAll();

            var gh_brep = new GH_Brep(brep);

            Guid brepGuid = Guid.Empty;

            gh_brep.BakeGeometry(RhinoDoc.ActiveDoc, new ObjectAttributes(), ref brepGuid);

            RhinoDoc.ActiveDoc.Objects.Select(brepGuid, true);

            string exportName = GetTempFileName("stp");

            string exportCmd = "!_-Export _SaveSmall=Yes _GeometryOnly=Yes _SaveTextures=No _SavePlugInData=Yes " + "\"" + exportName + "\"" + " _Schema=AP214AutomotiveDesign _ExportParameterSpaceCurves=Yes _SplitClosedSurfaces=No _LetImportingApplicationSetColorForBlackObjects=Yes " + " _Enter";

            RhinoApp.RunScript(exportCmd, false);

            RhinoDoc.ActiveDoc.Objects.Delete(brepGuid, true);

            return exportName;
        }

        private static string GetTempFileName(string extension = null)
        {
            string fileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString().Substring(1, 7));

            if (string.IsNullOrEmpty(extension))
                return fileName;
            else
                return fileName + "." + extension;
        }
    }
}
