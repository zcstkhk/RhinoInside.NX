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
using Rhino.Geometry;
using Rhino.Input;
using Rhino.PlugIns;
using Rhino.Runtime.InProcess;
using static System.Math;
using static Rhino.RhinoMath;
using NXOpen;
using Microsoft.Win32;
using System.IO;
using static NXOpen.Extensions.Globals;
using NXOpen.Extensions;

namespace RhinoInside.NX.Core
{
    public static class Rhinoceros
    {
        #region ×Ö¶Î
        public static WindowHandle MainWindow = WindowHandle.Zero;
        public static IntPtr MainWindowHandle => MainWindow.Handle;

        static RhinoCore Core;

        internal static string[] StartupLog;

        static List<GuestInfo> guests;

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

        public static readonly string SystemDir = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\McNeel\Rhinoceros\7.0\Install", "Path", null) as string ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Rhino WIP", "System");
        #endregion

        static Rhinoceros()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Loader.Loader.AssemblyResolveHandler;
        }

        public static MenuBarManager.CallbackStatus RhinoStartup()
        {
            if (Core is null)
            {
                // Load RhinoCore
                try
                {
                    Core = new RhinoCore
                    (
                      new string[]
                      {
                              "/nosplash",
                              "/notemplate",
                              "/captureprintcalls",
                              "/stopwatch",
                              $"/language=2052"
                      },
                      WindowStyle.Hidden
                    );
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

                //RunScript(script: Environment.GetEnvironmentVariable("RhinoInside_RunScript"), activate: false);

                // Reset document units
                //UpdateDocumentUnits(RhinoDoc.ActiveDoc);
                //UpdateDocumentUnits(RhinoDoc.ActiveDoc, Revit.ActiveDBDocument);

                Type[] types = default;
                try { types = Assembly.GetCallingAssembly().GetTypes(); }
                catch (ReflectionTypeLoadException ex) { types = ex.Types?.Where(x => x is object).ToArray(); }

                // Look for Guests
                guests = types.Where(x => typeof(IGuest).IsAssignableFrom(x)).Where(x => !x.IsInterface).Select(x => new GuestInfo(x)).ToList();

                CheckInGuests();

                StartGrasshopper();

                WindowHandle.ActiveWindow = NX.MainWindow;
            }

            return MenuBarManager.CallbackStatus.Continue;
        }

        static void StartGrasshopper()
        {
            Guest.LoadEditor();
        }

        static void CheckInGuests()
        {
            if (guests is null)
                return;

            foreach (var guestInfo in guests)
            {
                if (guestInfo.Guest is object)
                    continue;

                bool load = true;
                foreach (var guestPlugIn in guestInfo.ClassType.GetCustomAttributes(typeof(GuestPlugInIdAttribute), false).Cast<GuestPlugInIdAttribute>())
                    load |= PlugIn.GetPlugInInfo(guestPlugIn.PlugInId).IsLoaded;
                                
                if (!load)
                    continue;

                guestInfo.Guest = Activator.CreateInstance(guestInfo.ClassType) as IGuest;

                string complainMessage = string.Empty;
                try { guestInfo.LoadReturnCode = guestInfo.Guest.OnCheckIn(ref complainMessage); }
                catch (Exception e)
                {
                    guestInfo.LoadReturnCode = LoadReturnCode.ErrorShowDialog;
                    complainMessage = e.Message;
                }

                if (guestInfo.LoadReturnCode == LoadReturnCode.ErrorShowDialog)
                {
                    TheUI.NXMessageBox.Show(guestInfo.Guest.Name, NXMessageBox.DialogType.Error, $"{guestInfo.Guest.Name} failed to load");
                }
            }
        }

        static void CheckOutGuests()
        {
            if (guests is null)
                return;

            foreach (var guestInfo in Enumerable.Reverse(guests))
            {
                if (guestInfo.Guest is null)
                    continue;

                if (guestInfo.LoadReturnCode == LoadReturnCode.Success)
                    continue;

                try { guestInfo.Guest.OnCheckOut(); guestInfo.LoadReturnCode = LoadReturnCode.ErrorNoDialog; }
                catch (Exception) { }
            }
        }

        internal static MenuBarManager.CallbackStatus Shutdown()
        {
            CheckOutGuests();

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

        #region Rhino »Øµ÷
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