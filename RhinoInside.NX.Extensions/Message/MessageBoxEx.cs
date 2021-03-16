using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Extensions
{
    public static class MessageBox
    {
        static UI _theUI;

        static MessageBox()
        {
            _theUI = UI.GetUI();
        }

        public static void ShowNXMessageBox(this string str, NXMessageBox.DialogType type)
        {
            _theUI.NXMessageBox.Show(type.ToString(), type, str);
        }
    }
}