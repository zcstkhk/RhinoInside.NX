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
using static NXOpen.Extensions.Globals;
using NXOpen.Extensions;

namespace RhinoInside.NX.Core
{
    /// <summary>
    /// ����Ϊ NX �������Զ����ص��࣬����ע�� Rhino ��ť����������������
    /// </summary>
    public partial class NX
    {
        #region �ֶ�
        private static UFSession theUfSession = UFSession.GetUFSession();
        private static UI theUI = UI.GetUI();
        private static Session theSession = Session.GetSession();
        private static Part workPart = theSession.Parts.Work;
        private static Part dispPart = theSession.Parts.Display;

        public static NX theProgram;
        public static bool isDisposeCalled;

        public static WindowHandle MainWindow { get; private set; } = WindowHandle.Zero;
        public static IntPtr MainWindowHandle => MainWindow.Handle;
        #endregion

        #region ���캯��

        #endregion

        public static int Register()
        {
            int retValue = 0;
            try
            {
                isDisposeCalled = false;

                theProgram = new NX();

                "����ע�� RhinoInside".ConsoleWriteLine();

                var result = Rhinoceros.RhinoStartup();
                if (result != MenuBarManager.CallbackStatus.Continue)
                {
                    "�޷����� Rhino ����".ConsoleWriteLine();
                    return 0;
                }

                theUI.MenuBarManager.AddMenuAction("STARTRHINOINSIDE", new MenuBarManager.ActionCallback(RhinoCommands.StartRhinoInside));
                theUI.MenuBarManager.AddMenuAction("STARTRHINO", new MenuBarManager.ActionCallback(RhinoCommands.StartRhino));
                theUI.MenuBarManager.AddMenuAction("IMPORTRHINO", new MenuBarManager.ActionCallback(ImportCommand.ImportRhino));
                theUI.MenuBarManager.AddMenuAction("STARTIRONPYTHON", new MenuBarManager.ActionCallback(RhinoCommands.StartIronPython));

                theUI.MenuBarManager.AddMenuAction("STARTGRASSHOPPER", new MenuBarManager.ActionCallback(GrasshopperCommands. StartGrasshopper));
                theUI.MenuBarManager.AddMenuAction("STARTGRASSHOPPERPLAYER", new MenuBarManager.ActionCallback(CommandGrasshopperPlayer.StartGrasshopperPlayer));
                theUI.MenuBarManager.AddMenuAction("GRASSHOPPERSOLVER", new MenuBarManager.ActionCallback(GrasshopperCommands.StartGrasshopper));
                theUI.MenuBarManager.AddMenuAction("GRASSHOPPERRECOMPUTE", new MenuBarManager.ActionCallback(GrasshopperCommands.GrasshopperRecompute));
                theUI.MenuBarManager.AddMenuAction("GRASSHOPPERBAKE", new MenuBarManager.ActionCallback(GrasshopperCommands.GrasshopperBake));

                MainWindow = new WindowHandle(Process.GetCurrentProcess().MainWindowHandle);

                "RhinoInside ע��ɹ�".ConsoleWriteLine();
            }
            catch (NXOpen.NXException ex)
            {
                ex.ToString().ConsoleWriteLine();
                "RhinoInside ע��ʧ��".ConsoleWriteLine();
                retValue = 1;
            }
            return retValue;
        }

        public static int GetUnloadOption(string arg)
        {
            //Unloads the image explicitly, via an unload dialog
            //return System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);

            //Unloads the image immediately after execution within NX
            // return System.Convert.ToInt32(Session.LibraryUnloadOption.Immediately);

            //Unloads the image when the NX session terminates
            return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
        }
    }
}