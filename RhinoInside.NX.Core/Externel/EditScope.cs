using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Core.Externel
{
    public sealed class EditScope : IDisposable
    {
        readonly WindowHandle activeWindow = WindowHandle.ActiveWindow;
        readonly bool WasExposed = Rhinoceros.MainWindow.Visible;
        readonly bool WasEnabled = NX.MainWindow.Enabled;

        public EditScope()
        {
            Rhinoceros.MainWindow.HideOwnedPopups();
            if (WasExposed) 
                Rhinoceros.MainWindow.Visible = false;

            NX.MainWindow.Enabled = true;
            WindowHandle.ActiveWindow = NX.MainWindow;
        }

        void IDisposable.Dispose()
        {
            if (WasExposed) 
                Rhinoceros.MainWindow.Visible = WasExposed;
            Rhinoceros.MainWindow.ShowOwnedPopups();

            NX.MainWindow.Enabled = WasEnabled;
            WindowHandle.ActiveWindow = activeWindow;
        }
    }
}
