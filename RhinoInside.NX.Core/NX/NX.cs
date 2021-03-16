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
using RhinoInside.NX.Extensions;
using static RhinoInside.NX.Extensions.Globals;
using System.Globalization;

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
    /// 此类为 NX 启动后自动加载的类，包含注册 Rhino 按钮，创建工具条功能
    /// </summary>
    public partial class NX
    {
        #region 字段
        private static UI theUI = UI.GetUI();
        public static Language CurrentLanguage;
        public static NX theProgram;
        public static bool isDisposeCalled;

        public static WindowHandle MainWindow { get; private set; } = WindowHandle.Zero;
        public static IntPtr MainWindowHandle => MainWindow.Handle;
        #endregion

        #region 构造函数
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
        /// 将被启动器进行调用
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
                    "Can't Start Rhino Enviroment.".ShowNXMessageBox(NXMessageBox.DialogType.Error);
                    Logger.Error("Can't Start Rhino Enviroment.");
                    return 0;
                }

                GrassHopperDefaultAssemblyFolder = Grasshopper.Folders.DefaultAssemblyFolder;

                for (int i = 0; i < FilesToCopyToLibrary.Length; i++)
                {
                    File.Copy(Path.Combine(ApplicationPath, FilesToCopyToLibrary[i]), Path.Combine(GrassHopperDefaultAssemblyFolder, FilesToCopyToLibrary[i]), true);
                }

                theUI.MenuBarManager.AddMenuAction("STARTRHINOINSIDE", new MenuBarManager.ActionCallback(RhinoCommands.StartRhinoInside));
                theUI.MenuBarManager.AddMenuAction("STARTRHINO", new MenuBarManager.ActionCallback(RhinoCommands.StartRhino));
                theUI.MenuBarManager.AddMenuAction("IMPORTRHINO", new MenuBarManager.ActionCallback(ImportCommand.ImportRhino));
                theUI.MenuBarManager.AddMenuAction("STARTIRONPYTHON", new MenuBarManager.ActionCallback(RhinoCommands.StartIronPython));

                theUI.MenuBarManager.AddMenuAction("STARTGRASSHOPPER", new MenuBarManager.ActionCallback(GrasshopperCommands.StartGrasshopper));
                theUI.MenuBarManager.AddMenuAction("STARTGRASSHOPPERPLAYER", new MenuBarManager.ActionCallback(CommandGrasshopperPlayer.StartGrasshopperPlayer));
                theUI.MenuBarManager.AddMenuAction("GRASSHOPPERSOLVER", new MenuBarManager.ActionCallback(GrasshopperCommands.StartGrasshopper));
                theUI.MenuBarManager.AddMenuAction("GRASSHOPPERRECOMPUTE", new MenuBarManager.ActionCallback(GrasshopperCommands.GrasshopperRecompute));
                theUI.MenuBarManager.AddMenuAction("GRASSHOPPERBAKE", new MenuBarManager.ActionCallback(GrasshopperCommands.GrasshopperBake));

                theUI.MenuBarManager.AddMenuAction("CLOSERHINO", new MenuBarManager.ActionCallback(CloseRhino));

                MainWindow = new WindowHandle(NXOpenUI.FormUtilities.GetDefaultParentWindowHandle());

                Environment.SetEnvironmentVariable("RhinoInside_Program_Directory", RootPath);

                Logger.Info("Start RhinoInside Succesfully.");
            }
            catch (NXOpen.NXException ex)
            {
                ex.ToString().ShowNXMessageBox(NXMessageBox.DialogType.Error);
                Logger.Error($"Start RhinoInside Failed. {ex}");
                retValue = 1;
            }
            return retValue;
        }

        private static MenuBarManager.CallbackStatus CloseRhino(MenuButtonEvent buttonEvent)
        {
            Rhinoceros.Core.Dispose();
            Rhinoceros.Core = null;

            return MenuBarManager.CallbackStatus.Continue;
        }

        public static int GetUnloadOption(string arg)
        {
            //Unloads the image explicitly, via an unload dialog
            return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);

            //Unloads the image immediately after execution within NX
            // return System.Convert.ToInt32(Session.LibraryUnloadOption.Immediately);

            //Unloads the image when the NX session terminates
            //return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
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