using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using NXOpen;
using NXOpen.MenuBar;
using NXOpen.UF;
using NXOpen.Extensions;
using static NXOpen.Extensions.Globals;
using System.Globalization;
using RhinoInside.NX.Extensions;
using static RhinoInside.NX.Extensions.Globals;

namespace RhinoInside.NX.Core
{
    public enum Language
    {
        Simple_Chinese,
        English,
        French,
        German,
        Japanese,
        Italian,
        Russian,
        Korean,
        Trad_Chinese,
    }

    /// <summary>
    /// ����Ϊ NX �������Զ����ص��࣬����ע�� Rhino ��ť����������������
    /// </summary>
    public partial class NX
    {
        #region �ֶ�
        private static UI theUI = UI.GetUI();
        public static Language CurrentLanguage;
        public static NX theProgram;
        public static bool isDisposeCalled;

        public static WindowHandle MainWindow { get; private set; } = WindowHandle.Zero;
        public static IntPtr MainWindowHandle => MainWindow.Handle;
        #endregion

        #region ���캯��
        static NX()
        {
            TheUfSession.UF.TranslateVariable("UGII_LANG", out var language);

            if (language == "simpl_chinese")
                CurrentLanguage = Language.Simple_Chinese;
            else if (language == "english")
                CurrentLanguage = Language.English;
            else if (language == "french")
                CurrentLanguage = Language.French;
            else if (language == "german")
                CurrentLanguage = Language.German;
            else if (language == "japanese")
                CurrentLanguage = Language.Japanese;
            else if (language == "italian")
                CurrentLanguage = Language.Italian;
            else if (language == "russian")
                CurrentLanguage = Language.Russian;
            else if (language == "korean")
                CurrentLanguage = Language.Korean;
            else if (language == "trad_chinese")
                CurrentLanguage = Language.Trad_Chinese;
        }
        #endregion

        /// <summary>
        /// �������������е���
        /// </summary>
        /// <returns></returns>
        public static int Register()
        {
            int retValue = 0;
            try
            {
                isDisposeCalled = false;

                theProgram = new NX();

                Logger.Info("Starting RhinoInside.");

                var result = Rhinoceros.RhinoStartup();

                if (result != MenuBarManager.CallbackStatus.Continue)
                {
                    "Can't Start Rhino Enviroment.".ShowInNXMessageBox(NXMessageBox.DialogType.Error);
                    Logger.Error("Can't Start Rhino Enviroment.");
                    return 0;
                }

                GrassHopperDefaultAssemblyFolder = Grasshopper.Folders.DefaultAssemblyFolder;

                File.Copy(Path.Combine(ApplicationPath, "RhinoInside.NX.GH.Loader.dll"), Path.Combine(GrassHopperDefaultAssemblyFolder, "RhinoInside.NX.GH.Loader.gha"), true);

#if DEBUG
                File.Copy(Path.Combine(ApplicationPath, "RhinoInside.NX.GH.Loader.pdb"), Path.Combine(GrassHopperDefaultAssemblyFolder, "RhinoInside.NX.GH.Loader.pdb"), true);
#endif

                theUI.MenuBarManager.AddMenuAction("STARTRHINO", new MenuBarManager.ActionCallback(RhinoCommands.StartRhino));
                theUI.MenuBarManager.AddMenuAction("IMPORTRHINO", new MenuBarManager.ActionCallback(ImportCommand.ImportRhino));

                theUI.MenuBarManager.AddMenuAction("STARTIRONPYTHON", new MenuBarManager.ActionCallback(RhinoCommands.StartIronPython));

                theUI.MenuBarManager.AddMenuAction("STARTGRASSHOPPER", new MenuBarManager.ActionCallback(GrasshopperCommands.StartGrasshopper));
                theUI.MenuBarManager.AddMenuAction("STARTGRASSHOPPERPLAYER", new MenuBarManager.ActionCallback(CommandGrasshopperPlayer.StartGrasshopperPlayer));
                theUI.MenuBarManager.AddMenuAction("GRASSHOPPERSOLVER", new MenuBarManager.ActionCallback(GrasshopperCommands.StartGrasshopper));
                theUI.MenuBarManager.AddMenuAction("GRASSHOPPERRECOMPUTE", new MenuBarManager.ActionCallback(GrasshopperCommands.GrasshopperRecompute));
                theUI.MenuBarManager.AddMenuAction("GRASSHOPPERBAKE", new MenuBarManager.ActionCallback(GrasshopperCommands.GrasshopperBake));

                theUI.MenuBarManager.AddMenuAction("RHINOINSIDEPREFERENCES", new MenuBarManager.ActionCallback(SetRhinoInsidePreferences));
                theUI.MenuBarManager.AddMenuAction("RHINOINSIDEINFORMATION", new MenuBarManager.ActionCallback(ShowRhinoInsideInformation));
                theUI.MenuBarManager.AddMenuAction("RHINOINSIDEHELP", new MenuBarManager.ActionCallback(ShowRhinoInsideHelp));

                MainWindow = new WindowHandle(NXOpenUI.FormUtilities.GetDefaultParentWindowHandle());

                Environment.SetEnvironmentVariable("RhinoInside_Program_Directory", RootPath);

                Logger.Info("Start RhinoInside Succesfully.");
            }
            catch (NXOpen.NXException ex)
            {
                ex.ToString().ShowInNXMessageBox(NXMessageBox.DialogType.Error);
                Logger.Error($"Start RhinoInside Failed. {ex}");
                retValue = 1;
            }
            return retValue;
        }

        private static MenuBarManager.CallbackStatus ShowRhinoInsideInformation(MenuButtonEvent buttonEvent)
        {
            DateTime latestBuildTime = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            theUI.NXMessageBox.Show("About", NXMessageBox.DialogType.Information, "Version:\t" + version.Major + "." + version.Minor + "." + version.Build + "." + version.Revision + "\n" + "Last Build At: " + latestBuildTime.ToString());

            return MenuBarManager.CallbackStatus.Continue;
        }

        private static MenuBarManager.CallbackStatus ShowRhinoInsideHelp(MenuButtonEvent buttonEvent)
        {
            "Not Ready yet!".ShowInNXMessageBox(NXMessageBox.DialogType.Warning);
            return MenuBarManager.CallbackStatus.Continue;
        }

        private static MenuBarManager.CallbackStatus SetRhinoInsidePreferences(MenuButtonEvent buttonEvent)
        {
            RhinoInsidePreferences theRhinoInside_Preferences = null;
            try
            {
                "Nothing need to set now.".ShowInNXMessageBox(NXMessageBox.DialogType.Information);
                return MenuBarManager.CallbackStatus.Continue;

                if (WorkPart == null)
                {
                    "Open a part first!".ShowInNXMessageBox(NXMessageBox.DialogType.Error);
                    return MenuBarManager.CallbackStatus.Error;
                }

                theRhinoInside_Preferences = new RhinoInsidePreferences();

                theRhinoInside_Preferences.Show();
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                TheUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            finally
            {
                if (theRhinoInside_Preferences != null)
                    theRhinoInside_Preferences.Dispose();
                theRhinoInside_Preferences = null;
            }

            return MenuBarManager.CallbackStatus.Continue;
        }

        public static int GetUnloadOption(string arg)
        {
            //Unloads the image explicitly, via an unload dialog
            return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
        }

        public static void UnloadLibrary(string arg)
        {
            try
            {
                Logger.Info($"Unloading RhinoInside.NX.Core {arg}");

                Rhinoceros.Core.Dispose();
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                Logger.Error(ex.ToString());
            }
        }
    }
}