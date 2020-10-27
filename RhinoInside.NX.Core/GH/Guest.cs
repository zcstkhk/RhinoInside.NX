using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using Rhino;
using Rhino.PlugIns;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.GUI.Canvas;
using NXOpen.Extensions;
using RhinoInside.NX.Core;

using static NXOpen.Extensions.Globals;
using System.Threading;
using System.Globalization;

namespace RhinoInside.NX.Core
{
    [GuestPlugInId("B45A29B1-4343-4035-989E-044E8580D9CF")]
    internal class Guest : IGuest
    {
        public static Grasshopper.Plugin.GH_RhinoScriptInterface Script = new Grasshopper.Plugin.GH_RhinoScriptInterface();
        public string Name => "Grasshopper";
        LoadReturnCode IGuest.OnCheckIn(ref string errorMessage)
        {
            string message = null;
            try
            {
                if (!LoadComponents())
                    message = "Failed to load NX Grasshopper components.";
            }
            catch (FileNotFoundException e)
            {
                message = $"{e.Message}{Environment.NewLine}{e.FileName}";
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            if (!(message is null))
            {
                errorMessage = message;
                return LoadReturnCode.ErrorShowDialog;
            }

            RhinoDoc.BeginOpenDocument += BeginOpenDocument;
            RhinoDoc.EndOpenDocumentInitialViewUpdate += EndOpenDocumentInitialViewUpdate;

            return LoadReturnCode.Success;
        }

        static Rhino.UnitSystem modelUnitSystem = Rhino.UnitSystem.Unset;
        public static Rhino.UnitSystem ModelUnitSystem
        {
            get => Instances.ActiveCanvas is null ? Rhino.UnitSystem.Unset : modelUnitSystem;
            private set => modelUnitSystem = value;
        }

        void IGuest.OnCheckOut()
        {
            RhinoDoc.EndOpenDocumentInitialViewUpdate -= EndOpenDocumentInitialViewUpdate;
            RhinoDoc.BeginOpenDocument -= BeginOpenDocument;
        }

        /// <summary>
        /// Returns the loaded state of the Grasshopper Main window.
        /// </summary>
        /// <returns>True if the Main Grasshopper Window has been loaded.</returns>
        public static bool IsEditorLoaded() => Script.IsEditorLoaded();

        /// <summary>
        /// Load the main Grasshopper Editor. If the editor has already been loaded nothing
        /// will happen.
        /// </summary>
        public static void LoadEditor()
        {
            Script.LoadEditor();
            if (!Script.IsEditorLoaded())
                throw new Exception("Failed to startup Grasshopper");
        }

        /// <summary>
        /// Returns the visible state of the Grasshopper Main window.
        /// </summary>
        /// <returns>True if the Main Grasshopper Window has been loaded and is visible.</returns>
        public static bool IsEditorVisible() => Script.IsEditorVisible();

        /// <summary>
        /// Show the main Grasshopper Editor. The editor will be loaded first if needed.
        /// If the Editor is already on screen, nothing will happen.
        /// </summary>
        public static void ShowEditor()
        {
            Script.ShowEditor();
            Rhinoceros.MainWindow.BringToFront();
        }

        /// <summary>
        /// Hide the main Grasshopper Editor. If the editor hasn't been loaded or if the
        /// Editor is already hidden, nothing will happen.
        /// </summary>
        public static void HideEditor() => Script.HideEditor();

        /// <summary>
        /// Open a Grasshopper document. The editor will be loaded if necessary, but it will not be automatically shown.
        /// </summary>
        /// <param name="filename">Path of file to open (must be a *.gh or *.ghx extension).</param>
        /// <returns>True on success, false on failure.</returns>
        public static bool OpenDocument(string filename) => Script.OpenDocument(filename);

        /// <summary>
        /// Open a Grasshopper document. The editor will be loaded and shown if necessary.
        /// </summary>
        /// <param name="filename">Full path to GH definition file</param>
        /// <param name="showEditor">True to force the Main Grasshopper Window visible.</param>
        public static void OpenDocumentAsync(string filename, bool showEditor = true)
        {
            if (showEditor)
                ShowEditor();

            OpenDocument(filename);
        }

        static bool LoadGHA(string filePath)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

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

        bool LoadComponents()
        {
            // Load This Assembly as a GHA in Grasshopper
            {
                var bCoff = Instances.Settings.GetValue("Assemblies:COFF", false);
                try
                {
                    Instances.Settings.SetValue("Assemblies:COFF", false);

                    var location = Assembly.GetExecutingAssembly().Location;
                    location = Path.Combine(Path.GetDirectoryName(location), "HelloGrasshopper.gha");
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

                    TheUI.NXMessageBox.Show("Grasshopper Assembly Failure", NXOpen.NXMessageBox.DialogType.Error, $"Grasshopper cannot load the external assembly RhinoInside.NX.GH.gha. Please contact the provider for assistance.\n{mainContent}");

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

        private void ModalScope_Enter(object sender, EventArgs e)
        {
            if (Instances.ActiveCanvas?.Document is GH_Document definition)
                definition.Enabled = true;
        }

        private void ModalScope_Exit(object sender, EventArgs e)
        {
            if (Instances.ActiveCanvas?.Document is GH_Document definition)
                definition.Enabled = false;
        }

        bool activeDefinitionWasEnabled = false;
        void BeginOpenDocument(object sender, DocumentOpenEventArgs e)
        {
            if (Instances.ActiveCanvas?.Document is GH_Document definition)
            {
                activeDefinitionWasEnabled = definition.Enabled;
                definition.Enabled = false;
            }
        }

        void EndOpenDocumentInitialViewUpdate(object sender, DocumentOpenEventArgs e)
        {
            if (Instances.ActiveCanvas?.Document is GH_Document definition)
            {
                definition.Enabled = activeDefinitionWasEnabled;
                definition.NewSolution(false);
            }
        }

        /// <summary>
        /// 更改工作部件时引发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ufcwp(string param)
        {
            Console.WriteLine("Changed Work Part");
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class GuestPlugInIdAttribute : Attribute
    {
        public readonly Guid PlugInId;
        public GuestPlugInIdAttribute(string plugInId) => PlugInId = Guid.Parse(plugInId);
    }

    class GuestInfo
    {
        public readonly Type ClassType;
        public IGuest Guest;
        public LoadReturnCode LoadReturnCode;

        public GuestInfo(Type type) => ClassType = type;
    }

    public interface IGuest
    {
        string Name { get; }

        LoadReturnCode OnCheckIn(ref string complainMessage);

        void OnCheckOut();
    }
}
