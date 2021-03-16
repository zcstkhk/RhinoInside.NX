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
        /// <summary>
        /// PreselectionColor
        /// </summary>
        public static int PreselectionColor
        {
            get => WorkPart.Preferences.ColorSettingVisualization.PreselectionColor;
            set => WorkPart.Preferences.ColorSettingVisualization.PreselectionColor = value;
        }

        /// <summary>The line width (thin, medium, or thick) to be assigned to newly-created objects</summary>
        public static Width LineWidth
        {
            get
            {
                PartObject.WidthType width = WorkPart.Preferences.ObjectPreferences.GetWidth(PartObject.ObjectType.General);
                return (Width)ConvertLineWidthType(width);
            }
            set
            {
                PartObject.WidthType width = ConvertLineWidthType((DisplayableObject.ObjectWidth)value);
                WorkPart.Preferences.ObjectPreferences.SetWidth(PartObject.ObjectType.General, width);
            }
        }

        /// <summary>
        /// 返回 NX 的主版本号，如 11，1872
        /// </summary>
        public static int MajorRelease
        {
            get
            {
                TheUfSession.UF.GetRelease(out string release);
                return System.Convert.ToInt32(release.Split(new char[] { '.' })[0].Replace("NX V", ""));
            }
        }

        /// <summary>The line font (solid, dashed, etc.) to be assigned to newly-created objects</summary>
        public static Globals.Font LineFont
        {
            get
            {
                PartObject.LineFontType lineFont = WorkPart.Preferences.ObjectPreferences.GetLineFont(PartObject.ObjectType.General);
                return (Globals.Font)Globals.ConvertLineFontType(lineFont);
            }
            set
            {
                PartObject.LineFontType lineFont = Globals.ConvertLineFontType((DisplayableObject.ObjectFont)value);
                WorkPart.Preferences.ObjectPreferences.SetLineFont(PartObject.ObjectType.General, lineFont);
            }
        }

        /// <summary>Gets the default line font that will be assigned to newly-created objects of the specified type</summary>
        /// <param name="type">Object type</param>
        /// <returns>The default line font that will be assigned</returns>
        public static Globals.Font GetLineFont(Globals.DisplayType type)
        {
            PartObject.LineFontType lineFont = WorkPart.Preferences.ObjectPreferences.GetLineFont((PartObject.ObjectType)type);
            return (Globals.Font)Globals.ConvertLineFontType(lineFont);
        }

        /// <summary>Gets the default line font that will be assigned to newly-created objects of the specified type</summary>
        /// <param name="type">Object type</param>
        /// <param name="lineFontType">The default line font that will be assigned</param>
        public static void SetLineFont(Globals.DisplayType type, Globals.Font lineFontType)
        {
            PartObject.LineFontType lineFont = Globals.ConvertLineFontType((DisplayableObject.ObjectFont)lineFontType);
            WorkPart.Preferences.ObjectPreferences.SetLineFont((PartObject.ObjectType)type, lineFont);
        }

        private static DisplayableObject.ObjectFont ConvertLineFontType(PartObject.LineFontType lineFontType)
        {
            if (lineFontType == PartObject.LineFontType.Centerline)
            {
                return DisplayableObject.ObjectFont.Centerline;
            }
            if (lineFontType == PartObject.LineFontType.Dashed)
            {
                return DisplayableObject.ObjectFont.Dashed;
            }
            if (lineFontType == PartObject.LineFontType.Dotted)
            {
                return DisplayableObject.ObjectFont.Dotted;
            }
            if (lineFontType == PartObject.LineFontType.DottedDashed)
            {
                return DisplayableObject.ObjectFont.DottedDashed;
            }
            if (lineFontType == PartObject.LineFontType.LongDashed)
            {
                return DisplayableObject.ObjectFont.LongDashed;
            }
            if (lineFontType == PartObject.LineFontType.Phantom)
            {
                return DisplayableObject.ObjectFont.Phantom;
            }
            return DisplayableObject.ObjectFont.Solid;
        }

        // Token: 0x06000022 RID: 34 RVA: 0x000028E6 File Offset: 0x00000AE6
        private static PartObject.LineFontType ConvertLineFontType(DisplayableObject.ObjectFont lineFontType)
        {
            if (lineFontType == DisplayableObject.ObjectFont.Centerline)
            {
                return PartObject.LineFontType.Centerline;
            }
            if (lineFontType == DisplayableObject.ObjectFont.Dashed)
            {
                return PartObject.LineFontType.Dashed;
            }
            if (lineFontType == DisplayableObject.ObjectFont.Dotted)
            {
                return PartObject.LineFontType.Dotted;
            }
            if (lineFontType == DisplayableObject.ObjectFont.DottedDashed)
            {
                return PartObject.LineFontType.DottedDashed;
            }
            if (lineFontType == DisplayableObject.ObjectFont.LongDashed)
            {
                return PartObject.LineFontType.LongDashed;
            }
            if (lineFontType == DisplayableObject.ObjectFont.Phantom)
            {
                return PartObject.LineFontType.Phantom;
            }
            return PartObject.LineFontType.Solid;
        }


        private static DisplayableObject.ObjectWidth ConvertLineWidthType(PartObject.WidthType widthType)
        {
            if (widthType == PartObject.WidthType.NormalWidth)
            {
                return DisplayableObject.ObjectWidth.Normal;
            }
            if (widthType == PartObject.WidthType.ThickWidth)
            {
                return DisplayableObject.ObjectWidth.Thick;
            }
            if (widthType == PartObject.WidthType.ThinWidth)
            {
                return DisplayableObject.ObjectWidth.Thin;
            }
            if (widthType == PartObject.WidthType.WidthOne)
            {
                return DisplayableObject.ObjectWidth.One;
            }
            if (widthType == PartObject.WidthType.WidthTwo)
            {
                return DisplayableObject.ObjectWidth.Two;
            }
            if (widthType == PartObject.WidthType.WidthThree)
            {
                return DisplayableObject.ObjectWidth.Three;
            }
            if (widthType == PartObject.WidthType.WidthFour)
            {
                return DisplayableObject.ObjectWidth.Four;
            }
            if (widthType == PartObject.WidthType.WidthFive)
            {
                return DisplayableObject.ObjectWidth.Five;
            }
            if (widthType == PartObject.WidthType.WidthSix)
            {
                return DisplayableObject.ObjectWidth.Six;
            }
            if (widthType == PartObject.WidthType.WidthSeven)
            {
                return DisplayableObject.ObjectWidth.Seven;
            }
            if (widthType == PartObject.WidthType.WidthEight)
            {
                return DisplayableObject.ObjectWidth.Eight;
            }
            return DisplayableObject.ObjectWidth.Nine;
        }

        // Token: 0x06000024 RID: 36 RVA: 0x0000296C File Offset: 0x00000B6C
        private static PartObject.WidthType ConvertLineWidthType(DisplayableObject.ObjectWidth widthType)
        {
            if (widthType == DisplayableObject.ObjectWidth.Normal)
            {
                return PartObject.WidthType.NormalWidth;
            }
            if (widthType == DisplayableObject.ObjectWidth.Thick)
            {
                return PartObject.WidthType.ThickWidth;
            }
            if (widthType == DisplayableObject.ObjectWidth.Thin)
            {
                return PartObject.WidthType.ThinWidth;
            }
            if (widthType == DisplayableObject.ObjectWidth.One)
            {
                return PartObject.WidthType.WidthOne;
            }
            if (widthType == DisplayableObject.ObjectWidth.Two)
            {
                return PartObject.WidthType.WidthTwo;
            }
            if (widthType == DisplayableObject.ObjectWidth.Three)
            {
                return PartObject.WidthType.WidthThree;
            }
            if (widthType == DisplayableObject.ObjectWidth.Four)
            {
                return PartObject.WidthType.WidthFour;
            }
            if (widthType == DisplayableObject.ObjectWidth.Five)
            {
                return PartObject.WidthType.WidthFive;
            }
            if (widthType == DisplayableObject.ObjectWidth.Six)
            {
                return PartObject.WidthType.WidthSix;
            }
            if (widthType == DisplayableObject.ObjectWidth.Seven)
            {
                return PartObject.WidthType.WidthSeven;
            }
            if (widthType == DisplayableObject.ObjectWidth.Eight)
            {
                return PartObject.WidthType.WidthEight;
            }
            return PartObject.WidthType.WidthNine;
        }

        /// <summary>
        /// 新建对象的透明度
        /// </summary>
        /// <remarks>
        /// The translucency value is an integer between 0 and 100. A value of 0 indicates that the
        /// object is completely opaque (no translucency at all). A value of 100 indicates that
        /// the object is completely translucent, and therefore invisible.
        /// <para>
        /// The translucency setting affects any bodies (solids or sheets) or facetted bodies that are created subsequently.
        /// </para>
        /// </remarks>
        public static int Translucency
        {
            get => WorkPart.Preferences.ObjectPreferences.Translucency;

            set => WorkPart.Preferences.ObjectPreferences.Translucency = value;
        }

        /// <summary>The work layer (the layer on which newly-created objects should be placed)</summary>
        /// <remarks>
        /// When you change the work layer, the previous work layer is given the status "Selectable".
        /// </remarks>
        public static int WorkLayer
        {
            get => WorkPart.Layers.WorkLayer;

            set => WorkPart.Layers.WorkLayer = value;
        }

        /// <summary>The "array" of layer states. LayerStates[n] gives the state of layer n.</summary>
        public static LayerStatesArray LayerStates
        {
            get => new LayerStatesArray();
        }

        /// <summary>获取指定图层上的对象数量</summary>
        /// <param name="layer">图层编号</param>
        /// <returns>The number of objects on the specified layer</returns>
        public static int LayerObjectCount(int layer)
        {
            NXOpen.NXObject[] allObjectsOnLayer = Globals.WorkPart.Layers.GetAllObjectsOnLayer(layer);
            return allObjectsOnLayer.Length;
        }

        /// <summary>Gets and sets the work part of the session</summary>
        /// <remarks>
        /// NX Open functions always create objects in the work part. So, if you want to create objects 
        /// in several different parts, you use this property to change the work part.
        /// </remarks>
        public static Part WorkPart
        {
            get => TheSession.Parts.Work;

            set
            {
                if (WorkPart == null)
                {
                    PartLoadStatus partLoadStatus;
                    Globals.TheSession.Parts.SetDisplay(value, true, true, out partLoadStatus);
                    partLoadStatus.Dispose();
                    Globals.TheSession.Parts.SetWork(value);
                    return;
                }
                if (WorkPart.Tag != value.Tag)
                {
                    if (DisplayPart.ComponentAssembly.RootComponent == null)
                    {
                        PartLoadStatus partLoadStatus2;
                        Globals.TheSession.Parts.SetDisplay(value, false, false, out partLoadStatus2);
                        partLoadStatus2.Dispose();
                        Globals.TheSession.Parts.SetWork(value);
                        return;
                    }
                    Tag[] array;
                    Globals.TheUfSession.Assem.AskOccsOfPart(DisplayPart.Tag, value.Tag, out array);
                    if (array.Length == 0)
                    {
                        PartLoadStatus partLoadStatus3;
                        Globals.TheSession.Parts.SetDisplay(value, false, false, out partLoadStatus3);
                        partLoadStatus3.Dispose();
                        Globals.TheSession.Parts.SetWork(value);
                        return;
                    }
                    Globals.TheSession.Parts.SetWork(value);
                }
            }
        }

        /// <summary>Gets and sets the display part</summary>
        public static Part DisplayPart
        {
            get
            {
                return TheSession.Parts.Display;
            }
            set
            {
                PartLoadStatus partLoadStatus;
                Globals.TheSession.Parts.SetDisplay(value, true, true, out partLoadStatus);
                partLoadStatus.Dispose();
            }
        }

        /// <summary>The work view of the work part</summary>
        public static View WorkView
        {
            get
            {
                UFSession ufsession = Globals.TheUfSession;
                Tag tag;
                ufsession.View.AskWorkView(out tag);
                NXOpen.NXObject objectFromTag = NXOpen.Utilities.NXObjectManager.Get(tag) as NXObject;
                NXOpen.View view = (NXOpen.View)objectFromTag;
                return view;
            }
            set
            {
                value.MakeWork();
            }
        }

        /// <summary>The work coordinate system (Wcs) of the work part</summary>
        public static CoordinateSystem Wcs
        {
            get
            {
                UFSession ufsession = TheUfSession;
                Tag tag;
                ufsession.Csys.AskWcs(out tag);
                NXOpen.NXObject objectFromTag = NXOpen.Utilities.NXObjectManager.Get(tag) as NXObject;
                NXOpen.CoordinateSystem csys = (NXOpen.CoordinateSystem)objectFromTag;
                return csys;
            }
            set
            {
                Tag nxopenTag = value.Tag;
                TheUfSession.Csys.SetWcs(nxopenTag);
            }
        }

        /// <summary>The orientation of the Wcs of the work part</summary>
        public static Matrix3x3 WcsOrientation
        {
            get => Wcs.Orientation.Element;
            set => WorkPart.WCS.SetOriginAndMatrix(Globals.Wcs.Origin, value);

        }

        /// <summary>Millimeters Per Unit (either 1 or 25.4)</summary>
        /// <remarks>
        /// A constant representing the number of millimeters in one part unit.
        /// <para>If UnitType == Millimeter, then MillimetersPerUnit = 1.</para>
        /// <para>If UnitType == Inch, then MillimetersPerUnit = 25.4</para>
        /// </remarks>
        public static double MillimetersPerUnit
        {
            get
            {
                if (UnitType != Units.Millimeters)
                {
                    return 25.4;
                }
                return 1.0;
            }
        }

        /// <summary>Inches per part unit (either 1 or roughly 0.04)</summary>
        /// <remarks>
        /// A constant representing the number of inches in one part unit.
        /// <para>If UnitType = Millimeter, then InchesPerUnit = 0.0393700787402</para>
        /// <para>If UnitType = Inch, then InchesPerUnit = 1.</para>
        /// </remarks>
        public static double InchesPerUnit
        {
            get
            {
                if (UnitType != Units.Millimeters)
                {
                    return 1.0;
                }
                return 0.0393700787402;
            }
        }

        /// <summary>Distance tolerance</summary>
        /// <remarks>
        /// This distance tolerance is the same one that you access via Preferences → Modeling Preferences in interactive NX.
        /// In many functions in NX, an approximation process is used to construct geometry (curves or bodies).
        /// The distance tolerance (together with the angle tolerance) controls the accuracy of this approximation, unless 
        /// you specify some over-riding tolerance within the function itself. For example, when you offset a curve, NX 
        /// will construct a spline curve that approximates the true offset to within the current distance tolerance.
        /// </remarks>
        public static double DistanceTolerance
        {
            get => WorkPart.Preferences.Modeling.DistanceToleranceData;

            set => WorkPart.Preferences.Modeling.DistanceToleranceData = value;
        }

        /// <summary>Angle tolerance, in degrees</summary>
        /// <remarks>
        /// This angle tolerance is the same one that you access via Preference → Modeling Preferences in interactive NX.
        /// In many functions in NX, an approximation process is used to construct geometry (curves or bodies).
        /// The angle tolerance (together with the distance tolerance) controls the accuracy of this approximation, unless 
        /// you specify some over-riding tolerance within the function itself. For example, when you create a Through Curve Mesh
        /// feature in NX, the resulting surface will match the input curves to within the current distance and angle tolerances.
        /// <para>
        /// The angle tolerance is expressed in degrees.
        /// </para>
        /// </remarks>
        public static double AngleTolerance
        {
            get => WorkPart.Preferences.Modeling.AngleToleranceData;

            set => WorkPart.Preferences.Modeling.AngleToleranceData = value;
        }

        /// <summary>The chaining tolerance used in building "section" objects</summary>
        /// <remarks>
        /// Most modeling features seem to set this internally to 0.95*DistanceTolerance,
        /// so that's what we use here.
        /// </remarks>
        public static double ChainingTolerance => 0.95 * Globals.DistanceTolerance;

        /// <summary>Get NX Open Session</summary>
        public static Session TheSession => Session.GetSession();

        /// <summary>Get NXOpen.UF.UFSession</summary>
        public static UFSession TheUfSession => UFSession.GetUFSession();

        public static UI TheUI => UI.GetUI();

        /// <summary>
        /// If true, indicates that NX is running in managed mode,
        /// with Teamcenter (as opposed to native mode).
        /// </summary>
        public static bool ManagedMode
        {
            get
            {
                TheUfSession.UF.IsUgmanagerActive(out bool TCIntegrated);
                return TCIntegrated;
            }
        }

        /// <summary>The unit type of the work part</summary>
        /// <remarks>
        /// This property only gives the type of the unit.
        /// To get a Snap.NX.Unit object, please use the 
        /// <see cref="P:Snap.Globals.PartUnit">Snap.Globals.PartUnit</see>
        /// property, instead. 
        /// </remarks>
        /// <seealso cref="P:Snap.Globals.PartUnit">Snap.Globals.PartUnit</seealso>
        public static Units UnitType
        {
            get
            {
                int num = 0;
                TheUfSession.Part.AskUnits(WorkPart.Tag, out num);
                if (num == 1)
                {
                    return Units.Millimeters;
                }
                return Units.Inches;
            }
        }

        /// <summary>The length unit of the work part</summary>
        /// <remarks>
        /// This will be either Snap.NX.Unit.Millimeter or Snap.NX.Unit.Inch
        /// </remarks>
        /// <seealso cref="T:Snap.NX.Unit">Snap.NX.Unit</seealso>
        /// <seealso cref="F:Snap.NX.Unit.Millimeter">Snap.NX.Unit.Millimeter</seealso>
        /// <seealso cref="F:Snap.NX.Unit.Inch">Snap.NX.Unit.Inch</seealso>
        public static Units PartUnit
        {
            get
            {
                Units result = Units.Millimeters;
                if (WorkPart == null)
                    return result;

                if (UnitType == Units.Inches)
                {
                    result = Units.Inches;
                }
                return result;
            }
        }

        // Note: this type is marked as 'beforefieldinit'.
        static Globals()
        {
        }

        /// <summary>Enumeration of display widths for use when drawing objects such as curves</summary>
        public enum Width
        {
            /// <summary>Normal width -- the same as Width018 (0.18 mm)</summary>
            Normal,
            /// <summary>Thick width -- the same as Width025 (0.25 mm)</summary>
            Thick,
            /// <summary>Thin width -- the same as Width013 (0.13 mm)</summary>
            Thin,
            /// <summary>Width is 0.13 mm</summary>
            Width013 = 2,
            /// <summary>Width is 0.18 mm</summary>
            Width018 = 0,
            /// <summary>Width is 0.25 mm</summary>
            Width025,
            /// <summary>Width is 0.35 mm</summary>
            Width035 = 8,
            /// <summary>Width is 0.50 mm</summary>
            Width050,
            /// <summary>Width is 0.70 mm</summary>
            Width070,
            /// <summary>Width is 1.00 mm</summary>
            Width100,
            /// <summary>Width is 1.40 mm</summary>
            Width140,
            /// <summary>Width is 2.00 mm</summary>
            Width200
        }

        /// <summary>Enumeration of line fonts to be used when drawing objects such as curves</summary>
        // Token: 0x02000008 RID: 8
        public enum Font
        {
            /// <summary>Centerline</summary>
            // Token: 0x0400002D RID: 45
            Centerline = 4,
            /// <summary>Dashed</summary>
            // Token: 0x0400002E RID: 46
            Dashed = 2,
            /// <summary>Dotted</summary>
            // Token: 0x0400002F RID: 47
            Dotted = 5,
            /// <summary>DottedDashed</summary>
            // Token: 0x04000030 RID: 48
            DottedDashed = 7,
            /// <summary>LongDashed</summary>
            // Token: 0x04000031 RID: 49
            LongDashed = 6,
            /// <summary>Phantom</summary>
            // Token: 0x04000032 RID: 50
            Phantom = 3,
            /// <summary>Solid</summary>
            // Token: 0x04000033 RID: 51
            Solid = 1
        }

        /// <summary>Enumeration of the object types whose default display properties can be set</summary>
        // Token: 0x02000009 RID: 9
        public enum DisplayType
        {
            /// <summary>Default Type Value</summary>
            // Token: 0x04000035 RID: 53
            General,
            /// <summary>Line</summary>
            // Token: 0x04000036 RID: 54
            Line,
            /// <summary>Arc</summary>
            // Token: 0x04000037 RID: 55
            Arc,
            /// <summary>Conic</summary>
            // Token: 0x04000038 RID: 56
            Conic,
            /// <summary>Spline</summary>
            // Token: 0x04000039 RID: 57
            Spline,
            /// <summary>Solid Body</summary>
            // Token: 0x0400003A RID: 58
            Solidbody,
            /// <summary>Sheet Body</summary>
            // Token: 0x0400003B RID: 59
            Sheetbody,
            /// <summary>Datum</summary>
            // Token: 0x0400003C RID: 60
            Datum,
            /// <summary>Point</summary>
            // Token: 0x0400003D RID: 61
            Point,
            /// <summary>Coordinate System</summary>
            // Token: 0x0400003E RID: 62
            CoordinateSystem,
            /// <summary>All But Default</summary>
            // Token: 0x0400003F RID: 63
            AllButDefault,
            /// <summary>Datum CSYS Feature</summary>
            // Token: 0x04000040 RID: 64
            DatumCsys,
            /// <summary>Traceline</summary>
            // Token: 0x04000041 RID: 65
            Traceline,
            /// <summary>Infinite Line</summary>
            // Token: 0x04000042 RID: 66
            InfiniteLine
        }

        /// <summary>A class to support indexed access to layer states.</summary>
        /// <remarks>
        /// The actual states of the layers in the Work Part are accessible via the 
        /// <see cref="P:Snap.Globals.LayerStates">Globals.LayerStates</see> array. 
        /// Specifically, the array entry Snap.Globals.LayerStates[n] gives the status
        /// of layer number n.
        /// </remarks>
        /// <seealso cref="P:Snap.Globals.LayerStates">Snap.Globals.LayerStates</seealso>
        // Token: 0x0200000A RID: 10
        public class LayerStatesArray
        {
            /// <summary>The indexer for the LayerStatesArray class</summary>
            /// <param name="n">The index</param>
            /// <returns>The n-th element of the "array"</returns>
            // Token: 0x1700001C RID: 28
            public Globals.LayerState this[int n]
            {
                get
                {
                    return (Globals.LayerState)WorkPart.Layers.GetState(n);
                }
                set
                {
                    StateInfo[] array = new StateInfo[1];
                    array[0].State = (State)value;
                    array[0].Layer = n;
                    WorkPart.Layers.ChangeStates(array, false);
                }
            }

            // Token: 0x0600004A RID: 74 RVA: 0x00002EB5 File Offset: 0x000010B5
            public LayerStatesArray()
            {
            }
        }

        /// <summary>The possible states of a layer</summary>
        /// <remarks>
        /// The actual states of the layers in the Work Part are accessible via the 
        /// <see cref="P:Snap.Globals.LayerStates">Snap.Globals.LayerStates</see> array. 
        /// Specifically, the array entry Snap.Globals.LayerStates[n] gives the status
        /// of layer number n.
        /// </remarks>
        /// <seealso cref="P:Snap.Globals.LayerStates">Snap.Globals.LayerStates</seealso>
        // Token: 0x0200000B RID: 11
        public enum LayerState
        {
            /// <summary>Work layer. The layer on which all newly created objects are placed.</summary>
            // Token: 0x04000044 RID: 68
            WorkLayer,
            /// <summary>Objects on the layer are selectable</summary>
            // Token: 0x04000045 RID: 69
            Selectable,
            /// <summary>Objects on the layer are visible, but not selectable</summary>
            // Token: 0x04000046 RID: 70
            Visible,
            /// <summary>Objects on the layer are not visible and not selectable</summary>
            // Token: 0x04000047 RID: 71
            Hidden
        }

        public static string[] FilesToCopyToLibrary = new string[] { "RhinoInside.NX.GH.Loader.gha", "RhinoInside.NX.GH.Loader.pdb" };

        public static string GrassHopperDefaultAssemblyFolder;
    }
}
