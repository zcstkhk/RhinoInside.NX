using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

//using Eto.Forms;
using Rhino.PlugIns;
using System.Diagnostics;
using NXOpen.MenuBar;
using System.Windows.Forms;
using NXOpen.UF;
using NXOpen;
using NXOpen.Extensions;

namespace RhinoInside.NX.Core
{
    public class RhinoCommands
    {
        private static bool RibbonCreated = false;     // �Ƿ��Ѿ������� Ribbon ������
        static IntPtr RhinoInsideRibbon;

        private static UFSession theUfSession = UFSession.GetUFSession();
        private static UI theUI = UI.GetUI();
        private static Session theSession = Session.GetSession();
        private static Part workPart = theSession.Parts.Work;
        private static Part dispPart = theSession.Parts.Display;

        /// <summary>
        /// ��ʾ������
        /// </summary>
        /// <param name="buttonEvent"></param>
        /// <returns></returns>
        public static MenuBarManager.CallbackStatus StartRhinoInside(MenuButtonEvent buttonEvent)
        {
            try
            {
                if (!RibbonCreated)
                {
                    theUfSession.Ui.CreateRibbon("RhinoInside.rtb", 0, out RhinoInsideRibbon);

                    if (RhinoInsideRibbon == IntPtr.Zero)
                    {
                        "����������ʧ��".ListingWindowWriteLine();
                        return MenuBarManager.CallbackStatus.Error;
                    }

                    RibbonCreated = true;
                }

                if (buttonEvent.ActiveButton.ToggleStatus == MenuButton.Toggle.On)
                {
                    theUfSession.Ui.SetRibbonVis(RhinoInsideRibbon, 1);
                }
                else
                {
                    "�ر� RhinoInside ����".ConsoleWriteLine();

                    Rhinoceros.Exposed = false;

                    theUfSession.Ui.SetRibbonVis(RhinoInsideRibbon, 0);
                }

            }
            catch (Exception ex)
            {
                theUI.NXMessageBox.Show("Block Styler", 0, ex.ToString());
            }
            finally
            {

            }
            return 0;
        }

        /// <summary>
        /// �û��ر� Rhino �����ͨ���˰�ť���´�
        /// </summary>
        /// <param name="buttonEvent"></param>
        /// <returns></returns>
        public static MenuBarManager.CallbackStatus StartRhino(MenuButtonEvent buttonEvent)
        {
            if (Rhinoceros.Core == null)
            {
                Rhinoceros.RhinoStartup();
            }

            try
            {
                Rhinoceros.Exposed = true;
            }
            catch (Exception ex)
            {
                theUI.NXMessageBox.Show("Block Styler", 0, ex.ToString());
            }
            finally
            {

            }
            return 0;
        }

        /// <summary>
        /// ���� Python
        /// </summary>
        /// <param name="buttonEvent"></param>
        /// <returns></returns>
        public static MenuBarManager.CallbackStatus StartIronPython(MenuButtonEvent buttonEvent)
        {
            Guid PluginId = new Guid("814d908a-e25c-493d-97e9-ee3861957f49");

            if (!PlugIn.LoadPlugIn(PluginId, true, true))
                throw new Exception("Failed to startup IronPyhton");

            Rhinoceros.RunScript("_EditPythonScript", activate: true);

            return MenuBarManager.CallbackStatus.Continue;
        }
    }
}
