using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Forms.InteropExtension;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.PlugIns;
using RhinoInside.NX.Translator;
using NXOpen.UF;
using NXOpen;
using NXOpen.MenuBar;
using RhinoInside.NX.Core;
using RhinoInside.NX.Translator.Geometry;
using System.Reflection;
using Microsoft.Win32;
using NXOpen.Extensions;
using RhinoInside.NX.Extensions;

namespace RhinoInside.NX.Core
{
    public class CommandGrasshopperPlayer
    {
        private static UFSession theUfSession = UFSession.GetUFSession();
        private static Session theSession = Session.GetSession();
        private static UI theUI = UI.GetUI();
        private static Part workPart = theSession.Parts.Work;
        private static Part dispPart = theSession.Parts.Display;

        public static MenuBarManager.CallbackStatus StartGrasshopperPlayer(MenuButtonEvent buttonEvent)
        {
            if (theSession.Parts.Work == null)
            {
                "请先打开或创建一个部件后再使用此工具".ShowInNXMessageBox(NXMessageBox.DialogType.Error);
                return MenuBarManager.CallbackStatus.Error;
            }

            var result = MenuBarManager.CallbackStatus.Error;

            Logger.Info("Start Grasshopper Player");

            if ((result = BrowseForFile(out var filePath)) == MenuBarManager.CallbackStatus.Continue)
            {
                Logger.Info($"Selected {filePath} for Player");
                string message = "";
                result = Execute(filePath, ref message);
            }

            return result;
        }

        public static MenuBarManager.CallbackStatus BrowseForFile(out string filePath)
        {
            using (var openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.Filter = "Grasshopper Binary (*.gh)|*.gh|Grasshopper Xml (*.ghx)|*.ghx";
#if DEBUG
                openFileDialog.FilterIndex = 2;
#else
        openFileDialog.FilterIndex = 1;
#endif
                openFileDialog.RestoreDirectory = true;

                switch (openFileDialog.ShowDialog(NX.MainWindowHandle))
                {
                    case System.Windows.Forms.DialogResult.OK: filePath = openFileDialog.FileName; return MenuBarManager.CallbackStatus.Continue;
                    case System.Windows.Forms.DialogResult.Cancel: filePath = null; return MenuBarManager.CallbackStatus.Cancel;
                    default: filePath = null; return MenuBarManager.CallbackStatus.Error;
                }
            }
        }

        public static MenuBarManager.CallbackStatus Execute
        (
          string filePath,
          ref string message
        )
        {
            var result = MenuBarManager.CallbackStatus.Error;
            if ((result = ReadFromFile(filePath, out var definition)) == MenuBarManager.CallbackStatus.Continue)
            {
                using (definition)
                {
                    bool enableSolutions = GH_Document.EnableSolutions;

                    try
                    {
                        GH_Document.EnableSolutions = true;
                        definition.Enabled = true;
                        definition.ExpireSolution();

                        var inputs = GetInputParams(definition);
                        result = PromptForInputs(inputs, out var values);
                        if (result != MenuBarManager.CallbackStatus.Continue)
                            return result;

                        foreach (var value in values)
                            value.Key.AddVolatileDataList(new global::Grasshopper.Kernel.Data.GH_Path(0), value.Value);

                        using (var modal = new Rhinoceros.ModalScope())
                        {
                            definition.NewSolution(false, GH_SolutionMode.Silent);

                            do
                            {
                                if (modal.Run(false, false) == MenuBarManager.CallbackStatus.Error)
                                    return MenuBarManager.CallbackStatus.Error;

                            } while (definition.ScheduleDelay >= GH_Document.ScheduleRecursive);
                        }

                        if (definition.SolutionState == GH_ProcessStep.Aborted)
                        {
                            message = $"Solution aborted by user after ~{ definition.SolutionSpan.TotalSeconds} seconds";
                            return MenuBarManager.CallbackStatus.Cancel;
                        }
                        else
                        {
                            var ss = definition.Objects;

                            Logger.Info($"Definition SolutionState:{definition.SolutionState}");

                            foreach (var item in ss)
                            {
                                Console.WriteLine(item.GetType());
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        message = e.Message;
                        return MenuBarManager.CallbackStatus.Error;
                    }
                    finally
                    {
                        GH_Document.EnableSolutions = enableSolutions;
                    }
                }
            }

            return result;
        }

        public static MenuBarManager.CallbackStatus ReadFromFile(string filePath, out GH_Document definition)
        {
            definition = null;

            try
            {
                var archive = new GH_Archive();
                if (!archive.ReadFromFile(filePath))
                    return MenuBarManager.CallbackStatus.Error;

                definition = new GH_Document();
                if (archive.ExtractObject(definition, "Definition"))
                    return MenuBarManager.CallbackStatus.Continue;

                definition?.Dispose();
                definition = null;
                return MenuBarManager.CallbackStatus.Error;
            }
            catch (Exception)
            {
                return MenuBarManager.CallbackStatus.Error;
            }
        }

        internal static IList<IGH_Param> GetInputParams(GH_Document definition)
        {
            var inputs = new List<IGH_Param>();

            Logger.Info("开始获取输入参数。");

            // 获取输入参数
            foreach (var obj in definition.Objects)
            {
                Logger.Info($"Name:{obj.Name}, Type:{obj.GetType()}");

                if (!(obj is IGH_Param param))
                    continue;

                Logger.Info($"\t Source Count:{param.Sources.Count}, Recipient Count:{param.Recipients.Count}");

                if (param.Sources.Count != 0 || param.Recipients.Count == 0)
                    continue;

                Logger.Info($"\t VolatileDataCount:{param.VolatileDataCount}");

                if (param.VolatileDataCount > 0)
                    continue;

                Logger.Info($"\t Locked:{param.Locked}");
                if (param.Locked)
                    continue;

                inputs.Add(param);
            }

            return inputs;
        }

        internal static MenuBarManager.CallbackStatus PromptForInputs(IList<IGH_Param> inputs, out Dictionary<IGH_Param, IEnumerable<IGH_Goo>> values)
        {
            Logger.Info("提示用户进行输入");
            values = new Dictionary<IGH_Param, IEnumerable<IGH_Goo>>();
            foreach (IGH_Param input in inputs.OrderBy((x) => x.Attributes.Pivot.Y))
            {
                Logger.Info(input.GetType().ToString());

                switch (input)
                {
                    case Param_Box box:
                        var boxes = PromptBox(input.NickName);
                        if (boxes == null)
                            return MenuBarManager.CallbackStatus.Cancel;
                        values.Add(input, boxes);
                        break;
                    case Param_Point point:
                        var points = PromptPoint(input.NickName);
                        if (points == null)
                            return MenuBarManager.CallbackStatus.Cancel;
                        values.Add(input, points);
                        break;
                    case Param_Line line:
                        var lines = PromptLine(input.NickName);
                        if (lines == null)
                            return MenuBarManager.CallbackStatus.Cancel;
                        values.Add(input, lines);
                        break;
                    case Param_Curve curve:
                        var curves = PromptEdge(input.NickName);
                        if (curves == null)
                            return MenuBarManager.CallbackStatus.Cancel;
                        values.Add(input, curves);
                        break;
                    case Param_Surface surface:
                        var surfaces = PromptSurface(input.NickName);
                        if (surfaces == null)
                            return MenuBarManager.CallbackStatus.Cancel;
                        values.Add(input, surfaces);
                        break;
                    case Param_Brep brep:
                        var breps = PromptSurface(input.NickName);
                        if (breps == null)
                            return MenuBarManager.CallbackStatus.Cancel;
                        values.Add(input, breps);
                        break;
                }
            }

            return MenuBarManager.CallbackStatus.Continue;
        }

        internal static bool PickPointOnFace(string prompt, out NXOpen.Point3d point)
        {
            point = default;

            double[] pt = new double[3];
            if (theUfSession.Ui.PointSubfunction("Please pick a point on the face", new int[] { 14, 0 }, 0, pt) == 5)
            {
                point = new NXOpen.Point3d(pt[0], pt[1], pt[2]);
                return true;
            }
            else
                return false;
        }

        internal static bool PickPoint(string prompt, out NXOpen.Point3d point)
        {
            point = default;

            double[] pt = new double[3];

            if (theUfSession.Ui.PointSubfunction(prompt + " : First box corner - ", new int[] { 0, 1 }, 0, pt) == 5)
            {
                point = new NXOpen.Point3d(pt[0], pt[1], pt[2]);
                return true;
            }
            else
                return false;
        }

        internal static IEnumerable<IGH_Goo> PromptPoint(string prompt)
        {
            IGH_Goo goo = null;

            if (PickPoint(prompt + " : ", out var point))
                goo = new GH_Point(point.ToRhino());

            yield return goo;
        }

        internal static IEnumerable<IGH_Goo> PromptLine(string prompt)
        {
            IGH_Goo goo = null;

            if
            (
              PickPoint(prompt + " : Start point - ", out var from) &&
              PickPoint(prompt + " : End pont - ", out var to)
            )
            {
                goo = new GH_Line(new Rhino.Geometry.Line(from.ToRhino(), to.ToRhino()));
            }

            yield return goo;
        }

        internal static IEnumerable<IGH_Goo> PromptBox(string prompt)
        {
            IGH_Goo goo = null;

            if (PickPointOnFace(prompt + " : First box corner - ", out var from)
            && PickPointOnFace(prompt + " : Second box corner - ", out var to))
            {
                var min = new NXOpen.Point3d(Math.Min(from.X, to.X), Math.Min(from.Y, to.Y), Math.Min(from.Z, to.Z));
                var max = new NXOpen.Point3d(Math.Max(from.X, to.X), Math.Max(from.Y, to.Y), Math.Max(from.Z, to.Z));

                goo = new GH_Box(new Rhino.Geometry.BoundingBox(min.ToRhino(), max.ToRhino()));
            }

            yield return goo;
        }

        internal static IEnumerable<IGH_Goo> PromptEdge(string prompt)
        {
            IGH_Goo goo = null;

            try
            {
                var selectEdgeResult = theUI.SelectionManager.SelectTaggedObject(prompt, prompt, Selection.SelectionScope.WorkPart, Selection.SelectionAction.AllAndDisableSpecific, false, false, new Selection.MaskTriple[] { MaskTripleEx.Edge }, out var reference, out _);

                if (selectEdgeResult == Selection.Response.Ok)
                {
                    var edge = reference as Edge;

                    var extractedCurve = workPart.Features.CreateExtractGeometry(edge);

                    var nxCurve = extractedCurve.GetEntities()[0] as NXOpen.Curve;

                    nxCurve.RemoveParameters();

                    var curve = nxCurve.ToRhino();

                    goo = new GH_Curve(curve);
                }
            }
            catch (Exception) { }

            yield return goo;
        }

        internal static IEnumerable<IGH_Goo> PromptSurface(string prompt)
        {
            try
            {
                var selectEdgeResult = theUI.SelectionManager.SelectTaggedObject(prompt, prompt, Selection.SelectionScope.WorkPart, Selection.SelectionAction.AllAndDisableSpecific, false, false, new Selection.MaskTriple[] { MaskTripleEx.Face }, out var reference, out _);

                if (reference != null)
                {
                    var face = reference as Face;
                    var surface = face.ToBrep();
                    return new GH_Surface[] { new GH_Surface(surface) };
                }
            }
            catch (Exception) { }

            return null;
        }


    }
}
