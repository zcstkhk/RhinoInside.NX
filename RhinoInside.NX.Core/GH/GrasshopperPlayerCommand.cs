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
using RhinoInside.NX.Convert;
using NXOpen.UF;
using NXOpen;
using NXOpen.MenuBar;
using RhinoInside.NX.Core;
using RhinoInside.NX.Extensions.NX;
using NXOpen.Extensions;
using RhinoInside.NX.Convert.Geometry;
using System.Reflection;
using Microsoft.Win32;

namespace RhinoInside.NX.Core
{
    public class CommandGrasshopperPlayer
    {
        private static UFSession theUfSession = UFSession.GetUFSession();
        private static Session theSession = Session.GetSession();
        private static UI theUI = UI.GetUI();
        private static Part workPart = theSession.Parts.Work;
        private static Part dispPart = theSession.Parts.Display;

        public static readonly string SystemDir = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\McNeel\Rhinoceros\7.0\Install", "Path", null) as string ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Rhino WIP", "System");
        static CommandGrasshopperPlayer()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Loader.Loader.AssemblyResolveHandler;
            AppDomain.CurrentDomain.AssemblyLoad += Loader.Loader.AssemblyLoadHandler;
            AppDomain.CurrentDomain.TypeResolve += Loader.Loader.TypeResolveHandler;
            AppDomain.CurrentDomain.ResourceResolve += Loader.Loader.ResourceResolveHandler;
           //System.IO.FileNotFoundException.
        }

        public static MenuBarManager.CallbackStatus StartGrasshopperPlayer(MenuButtonEvent buttonEvent)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            var result = MenuBarManager.CallbackStatus.Error;

            if ((result = BrowseForFile(out var filePath)) == MenuBarManager.CallbackStatus.Continue)
            {
                string message = "";
                result = Execute(filePath, ref message);
            }

            return result;

            //Rhinoceros.RunScript("_GrasshopperPlayer", activate: true);

            //Rhinoceros.MainWindow.BringToFront();

            //return MenuBarManager.CallbackStatus.Continue;
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

                        // Update input volatile data values
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
                            Console.WriteLine(definition.SolutionState);
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

            // Collect input params
            foreach (var obj in definition.Objects)
            {
                if (!(obj is IGH_Param param))
                    continue;

                if (param.Sources.Count != 0 || param.Recipients.Count == 0)
                    continue;

                if (param.VolatileDataCount > 0)
                    continue;

                if (param.Locked)
                    continue;

                inputs.Add(param);
            }

            return inputs;
        }

        internal static MenuBarManager.CallbackStatus PromptForInputs(IList<IGH_Param> inputs, out Dictionary<IGH_Param, IEnumerable<IGH_Goo>> values)
        {
            values = new Dictionary<IGH_Param, IEnumerable<IGH_Goo>>();
            foreach (IGH_Param input in inputs.OrderBy((x) => x.Attributes.Pivot.Y))
            {
                //if (input.Type.Name == "GH_Box")
                //{
                //    var boxes = PromptBox(input.NickName);
                //    if (boxes == null)
                //        return MenuBarManager.CallbackStatus.Cancel;
                //    values.Add(input, boxes);
                //}
                //else if (input.Type.Name == "GH_Point")
                //{
                //    var points = PromptPoint(input.NickName);
                //    if (points == null)
                //        return MenuBarManager.CallbackStatus.Cancel;
                //    values.Add(input, points);
                //}
                //else
                //{
                //    Console.WriteLine(input.Type.Name);
                //}

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
                goo = new GH_Point(point.ToPoint3d());

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
                goo = new GH_Line(new Rhino.Geometry.Line(from.ToPoint3d(), to.ToPoint3d()));
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

                goo = new GH_Box(new Rhino.Geometry.BoundingBox(min.ToPoint3d(), max.ToPoint3d()));
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

                    nxCurve.RemoveParameter();

                    var curve = nxCurve.ToCurve();

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
