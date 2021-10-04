using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.PlugIns;
using Microsoft.Win32.SafeHandles;
using System.Transactions;
using NXOpen.MenuBar;
using RhinoInside.NX.Core;
using NXOpen.UF;
using NXOpen;
using System.Reflection;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;
using NXOpen.Extensions;
using RhinoInside.NX.Translator;
using RhinoInside.NX.Extensions;

namespace RhinoInside.NX.Core
{
    public class GrasshopperCommands
    {
        private static UFSession theUfSession = UFSession.GetUFSession();
        private static UI theUI = UI.GetUI();
        private static Session theSession = Session.GetSession();
        private static Part workPart = theSession.Parts.Work;
        private static Part dispPart = theSession.Parts.Display;

        public static readonly string SystemDir = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\McNeel\Rhinoceros\7.0\Install", "Path", null) as string ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Rhino 7", "System");

        public static MenuBarManager.CallbackStatus StartGrasshopper(MenuButtonEvent buttonEvent)
        {
            Logger.Info("Starting Grasshopper");

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            Rhinoceros.RunScript("_Grasshopper", activate: true);

            //Instances.DocumentEditor?.GetHashCode().ToString().ConsoleWriteLine();

            //Guest.ShowEditor();

            Rhinoceros.MainWindow.BringToFront();

            Logger.Info("Start Grasshopper Succesfully.");

            return MenuBarManager.CallbackStatus.Continue;
        }

        private static void DocumentEditor_Load(object sender, EventArgs e)
        {
            Logger.Info($"Document Editor AppDomain ID: {AppDomain.CurrentDomain.Id}");
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
                var loadResult = (bool)LoadGHAProc.Invoke
                (
                  Instances.ComponentServer,
                  new object[] { new GH_ExternalFile(filePath), false }
                );

                return loadResult;
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        public static MenuBarManager.CallbackStatus GrasshopperSolver(MenuButtonEvent buttonEvent)
        {
            GH_Document.EnableSolutions = !GH_Document.EnableSolutions;

            if (GH_Document.EnableSolutions)
            {
                if (Instances.ActiveCanvas?.Document is GH_Document definition)
                    definition.NewSolution(false);
            }
            else
            {
                theUfSession.Disp.RegenerateDisplay();
            }

            return MenuBarManager.CallbackStatus.Continue;
        }

        public static MenuBarManager.CallbackStatus GrasshopperRecompute(MenuButtonEvent buttonEvent)
        {
            if (Instances.ActiveCanvas?.Document is GH_Document definition)
            {
                if (GH_Document.EnableSolutions) definition.NewSolution(true);
                else
                {
                    GH_Document.EnableSolutions = true;
                    try { definition.NewSolution(false); }
                    finally { GH_Document.EnableSolutions = false; }
                }

                // If there are no scheduled solutions return control back to NX now
                if (definition.ScheduleDelay > GH_Document.ScheduleRecursive)
                    WindowHandle.ActiveWindow = Rhinoceros.MainWindow;

                if (definition.SolutionState == GH_ProcessStep.PostProcess)
                    return MenuBarManager.CallbackStatus.Continue;
                else
                    return MenuBarManager.CallbackStatus.Cancel;
            }

            return MenuBarManager.CallbackStatus.Error;
        }

        public static MenuBarManager.CallbackStatus GrasshopperBake(MenuButtonEvent buttonEvent)
        {
            if (Instances.ActiveCanvas?.Document is GH_Document definition)
            {
                bool groupResult = (System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Control) != System.Windows.Forms.Keys.None;

                var options = new BakeOptions()
                {
                    Part = theSession.Parts.Work,
                    View = theSession.Parts.Work.Views.WorkView,
                    Material = default
                };

                var resultingElementIds = new List<NXOpen.Tag>();

                var bakedElementIds = new List<NXOpen.Tag>();
                foreach (var obj in GetObjectsToBake(definition, options))
                {
                    if (obj.Bake(options, out var partial))
                        bakedElementIds.AddRange(partial);
                }

                {
                    //var activeDesignOptionId = DB.DesignOption.GetActiveDesignOptionId(options.Document);
                    //var elementIdsToAssignDO = new List<NXOpen.Tag>();
                    //foreach (var elementId in bakedElementIds)
                    //{
                    //    if
                    //    (
                    //      options.Part.GetElement(elementId) is DB.Element element &&
                    //      element.DesignOption?.Id is DB.ElementId elementDesignOptionId &&
                    //      elementDesignOptionId != activeDesignOptionId
                    //    )
                    //    {
                    //        elementIdsToAssignDO.Add(elementId);
                    //    }
                    //    else resultingElementIds?.Add(elementId);
                    //}

                    //if (elementIdsToAssignDO.Count > 0)
                    //{
                    //    // Move elements to Active Design Option
                    //    var elementIdsCopied = DB.ElementTransformUtils.CopyElements(options.Document, elementIdsToAssignDO, DB.XYZ.Zero);
                    //    options.Document.Delete(elementIdsToAssignDO);
                    //    resultingElementIds?.AddRange(elementIdsCopied);
                    //}
                }

                if (groupResult)
                {
                    //var group = options.Document.Create.NewGroup(resultingElementIds);

                    //resultingElementIds = new List<DB.ElementId>();
                    //resultingElementIds.Add(group.Id);
                }


                //data.Application.ActiveUIDocument.Selection.SetElementIds(resultingElementIds);
                //Instances.RedrawCanvas();
            }

            return MenuBarManager.CallbackStatus.Continue;
        }

        public static IEnumerable<IGH_ElementIdBakeAwareObject> GetObjectsToBake(GH_Document definition, BakeOptions options) => ElementIdBakeAwareObject.OfType(definition.SelectedObjects().OfType<IGH_ActiveObject>().Where(x => !x.Locked)).Where(x => x.CanBake(options));

        class ElementIdBakeAwareObject : IGH_ElementIdBakeAwareObject
        {
            public static IEnumerable<IGH_ElementIdBakeAwareObject> OfType(IEnumerable<IGH_ActiveObject> values)
            {
                foreach (var value in values)
                {
                    if (value is IGH_ElementIdBakeAwareObject bakeId)
                        yield return bakeId;

                    else if (value is IGH_BakeAwareObject bake)
                        yield return new ElementIdBakeAwareObject(bake);
                }
            }

            readonly IGH_BakeAwareObject activeObject;
            public ElementIdBakeAwareObject(IGH_BakeAwareObject value) { activeObject = value; }
            bool IGH_ElementIdBakeAwareObject.CanBake(BakeOptions options) => activeObject.IsBakeCapable;

            bool IGH_ElementIdBakeAwareObject.Bake(BakeOptions options, out List<NXOpen.Tag> ids)
            {
                bool result = false;

                if (activeObject is IGH_Param param)
                    result = Bake(param, options, out ids);

                else if (activeObject is IGH_Component component)
                {
                    var list = new List<NXOpen.Tag>();
                    foreach (var outParam in component.Params.Output)
                    {
                        if (Bake(outParam, options, out var partial))
                        {
                            result = true;
                            list.AddRange(partial);
                        }
                    }

                    ids = result ? list : default;
                }
                else ids = default;

                return result;
            }

            bool Bake(IGH_Param param, BakeOptions options, out List<NXOpen.Tag> ids)
            {
                var geometryToBake = param.VolatileData.AllData(true).Select(x => x.ScriptVariable()).
                Select(x =>
                {
                    switch (x)
                    {
                        case Rhino.Geometry.Point3d point: return new Rhino.Geometry.Point(point);
                        case Rhino.Geometry.GeometryBase geometry: return geometry;
                    }

                    return null;
                });

                if (geometryToBake.Any())
                {
                    ids = new List<NXOpen.Tag>();
                    foreach (var geometry in geometryToBake)
                    {

                        var shape = geometry.ToShape().ToList();
                        ids.AddRange(shape.Select(obj => obj.Tag));
                    }

                    return true;
                }

                ids = default;
                return false;
            }
        }




        //#region Preview
        //abstract class CommandGrasshopperPreview : GrasshopperCommand
        //{
        //    public static void CreateUI(RibbonPanel ribbonPanel)
        //    {
        //        var radioData = new RadioButtonGroupData("GrasshopperPreview");

        //        if (ribbonPanel.AddItem(radioData) is RadioButtonGroup radioButton)
        //        {
        //            CommandGrasshopperPreviewOff.CreateUI(radioButton);
        //            CommandGrasshopperPreviewWireframe.CreateUI(radioButton);
        //            CommandGrasshopperPreviewShaded.CreateUI(radioButton);
        //        }
        //    }

        //    protected new class Availability : NeedsActiveDocument<GrasshopperCommand.Availability>
        //    {
        //        public override bool IsCommandAvailable(UIApplication _, DB.CategorySet selectedCategories) =>
        //          base.IsCommandAvailable(_, selectedCategories) &&
        //          Revit.ActiveUIDocument?.Document.IsFamilyDocument == false;
        //    }
        //}

        //[Transaction(TransactionMode.ReadOnly), Regeneration(RegenerationOption.Manual)]
        //class CommandGrasshopperPreviewOff : CommandGrasshopperPreview
        //{
        //    public static void CreateUI(RadioButtonGroup radioButtonGroup)
        //    {
        //        var buttonData = NewToggleButtonData<CommandGrasshopperPreviewOff, Availability>("Off");

        //        if (radioButtonGroup.AddItem(buttonData) is ToggleButton pushButton)
        //        {
        //            pushButton.ToolTip = "Don't draw any preview geometry";
        //            pushButton.Image = ImageBuilder.LoadBitmapImage("Resources.Ribbon.Grasshopper.Preview_Off.png", true);
        //            pushButton.LargeImage = ImageBuilder.LoadBitmapImage("Resources.Ribbon.Grasshopper.Preview_Off.png");
        //            pushButton.Visible = PlugIn.PlugInExists(PluginId, out bool _, out bool _);

        //            if (GH.PreviewServer.PreviewMode == GH_PreviewMode.Disabled)
        //                radioButtonGroup.Current = pushButton;
        //        }
        //    }

        //    public override Result Execute(ExternalCommandData data, ref string message, DB.ElementSet elements)
        //    {
        //        GH.PreviewServer.PreviewMode = GH_PreviewMode.Disabled;
        //        data.Application.ActiveUIDocument.RefreshActiveView();
        //        return Result.Succeeded;
        //    }
        //}

        //[Transaction(TransactionMode.ReadOnly), Regeneration(RegenerationOption.Manual)]
        //class CommandGrasshopperPreviewWireframe : CommandGrasshopperPreview
        //{
        //    public static void CreateUI(RadioButtonGroup radioButtonGroup)
        //    {
        //        var buttonData = NewToggleButtonData<CommandGrasshopperPreviewWireframe, Availability>("Wireframe");

        //        if (radioButtonGroup.AddItem(buttonData) is ToggleButton pushButton)
        //        {
        //            pushButton.ToolTip = "Draw wireframe preview geometry";
        //            pushButton.Image = ImageBuilder.LoadBitmapImage("Resources.Ribbon.Grasshopper.Preview_Wireframe.png", true);
        //            pushButton.LargeImage = ImageBuilder.LoadBitmapImage("Resources.Ribbon.Grasshopper.Preview_Wireframe.png");
        //            pushButton.Visible = PlugIn.PlugInExists(PluginId, out bool _, out bool _);

        //            if (GH.PreviewServer.PreviewMode == GH_PreviewMode.Wireframe)
        //                radioButtonGroup.Current = pushButton;
        //        }
        //    }

        //    public override Result Execute(ExternalCommandData data, ref string message, DB.ElementSet elements)
        //    {
        //        GH.PreviewServer.PreviewMode = GH_PreviewMode.Wireframe;
        //        data.Application.ActiveUIDocument.RefreshActiveView();
        //        return Result.Succeeded;
        //    }
        //}

        //[Transaction(TransactionMode.ReadOnly), Regeneration(RegenerationOption.Manual)]
        //class CommandGrasshopperPreviewShaded : CommandGrasshopperPreview
        //{
        //    public static void CreateUI(RadioButtonGroup radioButtonGroup)
        //    {
        //        var buttonData = NewToggleButtonData<CommandGrasshopperPreviewShaded, Availability>("Shaded");

        //        if (radioButtonGroup.AddItem(buttonData) is ToggleButton pushButton)
        //        {
        //            pushButton.ToolTip = "Draw shaded preview geometry";
        //            pushButton.Image = ImageBuilder.LoadBitmapImage("Resources.Ribbon.Grasshopper.Preview_Shaded.png", true);
        //            pushButton.LargeImage = ImageBuilder.LoadBitmapImage("Resources.Ribbon.Grasshopper.Preview_Shaded.png");
        //            pushButton.Visible = PlugIn.PlugInExists(PluginId, out bool _, out bool _);

        //            if (GH.PreviewServer.PreviewMode == GH_PreviewMode.Shaded)
        //                radioButtonGroup.Current = pushButton;
        //        }
        //    }

        //    public override Result Execute(ExternalCommandData data, ref string message, DB.ElementSet elements)
        //    {
        //        GH.PreviewServer.PreviewMode = GH_PreviewMode.Shaded;
        //        data.Application.ActiveUIDocument.RefreshActiveView();
        //        return Result.Succeeded;
        //    }
        //}
        //#endregion
    }
}
