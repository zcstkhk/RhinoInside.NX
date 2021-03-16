using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Microsoft.Win32.SafeHandles;
using NXOpen.MenuBar;
using Rhino;
using Rhino.Commands;
using Rhino.Input;
using Rhino.Runtime.InProcess;
using NXOpen;
using Microsoft.Win32;
using System.IO;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper;
using static RhinoInside.NX.Extensions.Globals;
using NXOpen.UF;
using RhinoInside.NX.Extensions;

namespace RhinoInside.NX.Core
{
    public static class Rhinoceros
    {
        static Rhinoceros()
        {
            _theUI = UI.GetUI();
            _theUfSession = UFSession.GetUFSession();
        }

        #region 字段
        static UI _theUI;
        static UFSession _theUfSession;

        public static WindowHandle MainWindow = WindowHandle.Zero;
        public static IntPtr MainWindowHandle => MainWindow.Handle;

        public static RhinoCore Core;

        internal static string[] StartupLog;

        //static List<GuestInfo> guests;

        /*internal*/
        public static void InvokeInHostContext(Action action) => Core.InvokeInHostContext(action);
        /*internal*/
        public static T InvokeInHostContext<T>(Func<T> func) => Core.InvokeInHostContext(func);

        public static bool Exposed
        {
            get => MainWindow.Visible && MainWindow.WindowStyle != ProcessWindowStyle.Minimized;
            set
            {
                MainWindow.Visible = value;

                if (value && MainWindow.WindowStyle == ProcessWindowStyle.Minimized)
                    MainWindow.WindowStyle = ProcessWindowStyle.Normal;
            }
        }

        static ExposureSnapshot QuiescentExposure;

        public static readonly string SystemDir = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\McNeel\Rhinoceros\7.0\Install", "Path", null) as string ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Rhino 7", "System");
        #endregion

        public static MenuBarManager.CallbackStatus RhinoStartup()
        {
            if (Core is null)
            {
                // Load RhinoCore
                try
                {
                    int languageCode = 2052;    // 默认为中文
                    switch (NX.CurrentLanguage)
                    {
                        case Language.Simple_Chinese:
                            languageCode = 2052;
                            break;
                        case Language.English:
                            languageCode = 1033;
                            break;
                        case Language.French:
                            languageCode = 1036;
                            break;
                        case Language.German:
                            languageCode = 1031;
                            break;
                        case Language.Japanese:
                            languageCode = 1041;
                            break;
                        case Language.Italian:
                            languageCode = 1040;
                            break;
                        case Language.Russian:
                            languageCode = 1049;
                            break;
                        case Language.Korean:
                            languageCode = 1042;
                            break;
                        case Language.Trad_Chinese:
                            languageCode = 1028;
                            break;
                        default:
                            break;
                    }

                    Core = new RhinoCore(new string[] { "/nosplash", "/notemplate", "/captureprintcalls", "/stopwatch", $"/language={languageCode}" }, WindowStyle.Hidden);
                }
                catch (Exception ex)
                {
                    ex.ToString().ShowNXMessageBox(NXOpen.NXMessageBox.DialogType.Error);
                    return MenuBarManager.CallbackStatus.Error;
                }
                finally
                {
                    StartupLog = RhinoApp.CapturedCommandWindowStrings(true);
                    RhinoApp.CommandWindowCaptureEnabled = false;
                }

                MainWindow = new WindowHandle(RhinoApp.MainWindowHandle());

                RhinoApp.MainLoop += MainLoop;
                RhinoDoc.NewDocument += OnNewDocument;
                RhinoDoc.EndOpenDocumentInitialViewUpdate += EndOpenDocumentInitialViewUpdate;

                Command.BeginCommand += BeginCommand;
                Command.EndCommand += EndCommand;

                WindowHandle.ActiveWindow = NX.MainWindow;
            }

            return MenuBarManager.CallbackStatus.Continue;
        }

        internal static MenuBarManager.CallbackStatus Shutdown()
        {
            //CheckOutGuests();

            Command.EndCommand -= EndCommand;
            Command.BeginCommand -= BeginCommand;

            RhinoDoc.EndOpenDocumentInitialViewUpdate -= EndOpenDocumentInitialViewUpdate;
            RhinoDoc.NewDocument -= OnNewDocument;

            RhinoApp.MainLoop -= MainLoop;

            try
            {
                Core.Dispose();
                Core = null;
            }
            catch (Exception ex)
            {
                ex.ToString().ConsoleWriteLine();
                return MenuBarManager.CallbackStatus.Error;
            }

            return MenuBarManager.CallbackStatus.Continue;
        }

        #region Rhino 回调
        static void MainLoop(object sender, EventArgs e)
        {
            if (!Command.InScriptRunnerCommand())
            {
                if (RhinoDoc.ActiveDoc is RhinoDoc rhinoDoc)
                {
                    // Keep Rhino window exposed to user while in a get operation
                    if (RhinoGet.InGet(rhinoDoc))
                    {
                        // if there is no floating viewport visible...
                        if (!rhinoDoc.Views.Where(x => x.Floating).Any())
                        {
                            if (!Exposed)
                                Exposed = true;
                        }
                    }
                }
            }
        }

        static void OnNewDocument(object sender, DocumentEventArgs e)
        {
            // If a new document is created without template it is updated from Revit.ActiveDBDocument
            Debug.Assert(string.IsNullOrEmpty(e.Document.TemplateFileUsed));

            UpdateDocumentUnits(e.Document);
        }

        static void EndOpenDocumentInitialViewUpdate(object sender, DocumentEventArgs e)
        {
            if (e.Document.IsOpening)
                AuditUnits(e.Document);
        }

        static void BeginCommand(object sender, CommandEventArgs e)
        {
            if (!Command.InScriptRunnerCommand())
            {
                // Capture Rhino Main Window exposure to restore it when user ends picking
                QuiescentExposure = new ExposureSnapshot();

                // Disable Revit Main Window while in Command
                NX.MainWindow.Enabled = false;
            }
        }

        static void EndCommand(object sender, CommandEventArgs e)
        {
            if (!Command.InScriptRunnerCommand())
            {
                // Reenable Revit main window
                NX.MainWindow.Enabled = true;

                if (MainWindow.WindowStyle != ProcessWindowStyle.Maximized)
                {
                    // Restore Rhino Main Window exposure
                    QuiescentExposure?.Restore();
                    QuiescentExposure = null;
                    RhinoApp.SetFocusToMainWindow();
                }
            }
        }
        #endregion

        static void AuditUnits(RhinoDoc doc)
        {
            if (Command.InScriptRunnerCommand())
                return;
        }

        static void UpdateDocumentUnits(RhinoDoc rhinoDoc, Part nxPart = null)
        {
            bool docModified = rhinoDoc.Modified;
            try
            {
                if (nxPart is null)
                {
                    rhinoDoc.ModelUnitSystem = UnitSystem.None;
                    rhinoDoc.ModelAbsoluteTolerance = DistanceTolerance;
                    rhinoDoc.ModelAngleToleranceRadians = AngleTolerance;
                }
            }
            finally
            {
                rhinoDoc.Modified = docModified;
            }
        }

        static bool idlePending = true;

        internal static bool Run()
        {
            if (idlePending)
                idlePending = Core.DoIdle();

            var active = Core.DoEvents();
            if (active)
                idlePending = true;

            Core.RaiseIdle();

            return active;
        }

        class ExposureSnapshot
        {
            readonly bool Visible = MainWindow.Visible;
            readonly ProcessWindowStyle Style = MainWindow.WindowStyle;

            public void Restore()
            {
                MainWindow.WindowStyle = Style;
                MainWindow.Visible = Visible;
            }
        }

        public static void RunScript(string script, bool activate)
        {
            if (string.IsNullOrEmpty(script))
                return;

            if (activate)
                RhinoApp.SetFocusToMainWindow();

            RhinoApp.RunScript(script, false);
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

        static bool LoadComponents()
        {
            // Load This Assembly as a GHA in Grasshopper
            {
                var bCoff = Instances.Settings.GetValue("Assemblies:COFF", false);
                try
                {
                    Instances.Settings.SetValue("Assemblies:COFF", false);

                    var location = Assembly.GetExecutingAssembly().Location;
                    location = @"E:\Documents\Programming\Repos\RhinoInside\_参考项目\Rhino.Inside\HelloGrassHopper\bin\HelloGrasshopper.gha";
                    if (!LoadGHA(location))
                    {
                        if (!File.Exists(location))
                            throw new FileNotFoundException("File Not Found.", location);

                        if (CentralSettings.IsLoadProtected(location))
                            throw new InvalidOperationException($"Assembly '{location}' is load protected.");

                        return false;
                    }
                }
                catch (Exception e)
                {
                    var mainContent = e.Message;

                    _theUI.NXMessageBox.Show("Grasshopper Assembly Failure", NXOpen.NXMessageBox.DialogType.Error, $"Grasshopper cannot load the external assembly RhinoInside.NX.GH.gha. Please contact the provider for assistance.\n{mainContent}");

                    return false;
                }
                finally
                {
                    Instances.Settings.SetValue("Assemblies:COFF", bCoff);
                }
            }

            GH_ComponentServer.UpdateRibbonUI();
            return true;
        }

        /// <summary>
        /// Represents a Pseudo-modal loop
        /// This class implements IDisposable, it's been designed to be used in a using statement.
        /// </summary>
        internal sealed class ModalScope : IDisposable
        {
            static bool wasExposed = false;
            readonly bool wasEnabled = NX.MainWindow.Enabled;

            public ModalScope() => NX.MainWindow.Enabled = false;

            void IDisposable.Dispose() => NX.MainWindow.Enabled = wasEnabled;

            public MenuBarManager.CallbackStatus Run(bool exposeMainWindow)
            {
                return Run(exposeMainWindow, !Keyboard.IsKeyDown(Key.LeftCtrl));
            }

            public MenuBarManager.CallbackStatus Run(bool exposeMainWindow, bool restorePopups)
            {
                try
                {
                    if (exposeMainWindow) Exposed = true;
                    else if (restorePopups) Exposed = wasExposed || MainWindow.WindowStyle == ProcessWindowStyle.Minimized;

                    if (restorePopups)
                        MainWindow.ShowOwnedPopups();

                    // Activate a Rhino window to keep the loop running
                    var activePopup = MainWindow.ActivePopup;
                    if (activePopup.IsInvalid || exposeMainWindow)
                    {
                        if (!Exposed)
                            return MenuBarManager.CallbackStatus.Cancel;

                        RhinoApp.SetFocusToMainWindow();
                    }
                    else
                    {
                        activePopup.BringToFront();
                    }

                    while (Rhinoceros.Run())
                    {
                        if (!Exposed && MainWindow.ActivePopup.IsInvalid)
                            break;
                    }

                    return MenuBarManager.CallbackStatus.Continue;
                }
                finally
                {
                    wasExposed = Exposed;

                    NX.MainWindow.Enabled = true;
                    WindowHandle.ActiveWindow = NX.MainWindow;
                    MainWindow.HideOwnedPopups();
                    Exposed = false;
                }
            }
        }
    }
}