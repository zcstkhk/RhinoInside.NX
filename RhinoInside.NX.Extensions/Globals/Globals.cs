using System;
using System.Drawing;
using NXOpen;
using NXOpen.Layer;
using NXOpen.Preferences;
using NXOpen.UF;
using System.Reflection;
using Units = NXOpen.BasePart.Units;

namespace RhinoInside.NX.Extensions
{
    /// <summary>Provides access to various global settings that affect the user's working environment.</summary>
    public static partial class Globals
    {
        public static string GrassHopperDefaultAssemblyFolder;
    }
}
