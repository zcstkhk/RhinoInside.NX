using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.InteropExtension;
using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using DB = NXOpen;

namespace RhinoInside.NX.GH.Parameters
{
    public abstract class ElementIdParam<T, R> :
    PersistentGeometryParam<T>,
    Kernel.IGH_ElementIdParam
    where T : class, Types.IGH_Tag
    {
        public override sealed string TypeName => "NX " + Name;
        protected ElementIdParam(string name, string nickname, string description, string category, string subcategory) :
          base(name, nickname, description, category, subcategory)
        { }
        protected override T PreferredCast(object data) => data is R ? Types.Element.FromValue(data) as T : null;

        protected T Current
        {
            get
            {
                var current = (SourceCount == 0 && PersistentDataCount == 1) ? PersistentData.get_FirstItem(true) : default;
                return (current?.LoadElement() == true ? current : default) as T;
            }
        }

        internal static IEnumerable<Types.IGH_Tag> ToElementIds(IGH_Structure data) =>
          data.AllData(true).
          OfType<Types.IGH_Tag>().
          Where(x => x.IsValid);

        [Flags]
        public enum DataGrouping
        {
            None = 0,
            Document = 1,
            Workset = 2,
            DesignOption = 4,
            Category = 8,
        };

        public DataGrouping Grouping { get; set; } = DataGrouping.None;

        public override sealed bool Read(GH_IReader reader)
        {
            if (!base.Read(reader))
                return false;

            int grouping = (int)DataGrouping.None;
            reader.TryGetInt32("Grouping", ref grouping);
            Grouping = (DataGrouping)grouping;

            return true;
        }
        public override sealed bool Write(GH_IWriter writer)
        {
            if (!base.Write(writer))
                return false;

            if (Grouping != DataGrouping.None)
                writer.SetInt32("Grouping", (int)Grouping);

            return true;
        }

        public override void ClearData()
        {
            base.ClearData();

            if (PersistentDataCount == 0)
                return;

            foreach (var goo in PersistentData.OfType<T>())
                goo?.UnloadElement();
        }

        protected override void LoadVolatileData()
        {
            if (SourceCount == 0)
            {
                foreach (var branch in m_data.Branches)
                {
                    for (int i = 0; i < branch.Count; i++)
                    {
                        var item = branch[i];
                        if (item?.IsReferencedElement ?? false)
                        {
                            if (!item.LoadElement())
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"A referenced {item.TypeName} could not be found in the Revit document.");
                                branch[i] = null;
                            }
                        }
                    }
                }
            }
        }

        #region UI
        protected override void Menu_AppendPreProcessParameter(ToolStripDropDown menu)
        {
            base.Menu_AppendPreProcessParameter(menu);

            var Group = Menu_AppendItem(menu, "Group by") as ToolStripMenuItem;

            Group.Checked = Grouping != DataGrouping.None;
            Menu_AppendItem(Group.DropDown, "Document", (s, a) => Menu_GroupBy(DataGrouping.Document), true, (Grouping & DataGrouping.Document) != 0);
            Menu_AppendItem(Group.DropDown, "Workset", (s, a) => Menu_GroupBy(DataGrouping.Workset), true, (Grouping & DataGrouping.Workset) != 0);
            Menu_AppendItem(Group.DropDown, "Design Option", (s, a) => Menu_GroupBy(DataGrouping.DesignOption), true, (Grouping & DataGrouping.DesignOption) != 0);
            Menu_AppendItem(Group.DropDown, "Category", (s, a) => Menu_GroupBy(DataGrouping.Category), true, (Grouping & DataGrouping.Category) != 0);
        }

        private void Menu_GroupBy(DataGrouping value)
        {
            RecordUndoEvent("Set: Grouping");

            if ((Grouping & value) != 0)
                Grouping &= ~value;
            else
                Grouping |= value;

            OnObjectChanged(GH_ObjectEventType.Options);

            if (Kind == GH_ParamKind.output)
                ExpireOwner();

            ExpireSolution(true);
        }

        protected override void PrepareForPrompt() { }
        protected override void RecoverFromPrompt() { }
        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);

            Menu_AppendSeparator(menu);
            //Menu_AppendActions(menu);
        }

        protected override bool Prompt_ManageCollection(GH_Structure<T> values)
        {
            foreach (var item in values.AllData(true))
            {
                if (item.IsValid)
                    continue;

                if (item is Types.IGH_Tag elementId)
                {
                    if (elementId.IsReferencedElement)
                        elementId.LoadElement();
                }
            }

            return base.Prompt_ManageCollection(values);
        }
        #endregion

        #region IGH_ElementIdParam
        bool Kernel.IGH_ElementIdParam.NeedsToBeExpired
        (
          NXOpen.Part doc,
          ICollection<DB.Tag> added,
          ICollection<DB.Tag> deleted,
          ICollection<DB.Tag> modified
        )
        {
            if (DataType == GH_ParamData.remote)
                return false;

            foreach (var data in VolatileData.AllData(true).OfType<Types.IGH_Tag>())
            {
                if (!data.IsElementLoaded)
                    continue;

                if (modified.Contains(data.Tag))
                    return true;

                if (deleted.Contains(data.Tag))
                    return true;
            }

            return false;
        }
        #endregion
    }

    public abstract class ElementIdWithoutPreviewParam<T, R> : ElementIdParam<T, R>
      where T : class, Types.IGH_Tag
    {
        protected ElementIdWithoutPreviewParam(string name, string nickname, string description, string category, string subcategory) :
          base(name, nickname, description, category, subcategory)
        { }

        protected override void Menu_AppendPromptOne(ToolStripDropDown menu) { }
        protected override void Menu_AppendPromptMore(ToolStripDropDown menu) { }
        protected override GH_GetterResult Prompt_Plural(ref List<T> values) => GH_GetterResult.cancel;
        protected override GH_GetterResult Prompt_Singular(ref T value) => GH_GetterResult.cancel;
    }

    public abstract class ElementIdWithPreviewParam<X, R> : ElementIdParam<X, R>, IGH_PreviewObject
    where X : class, Types.IGH_Tag
    {
        protected ElementIdWithPreviewParam(string name, string nickname, string description, string category, string subcategory) :
        base(name, nickname, description, category, subcategory)
        { }

        #region IGH_PreviewObject
        bool IGH_PreviewObject.Hidden { get; set; }
        bool IGH_PreviewObject.IsPreviewCapable => !VolatileData.IsEmpty;
        Rhino.Geometry.BoundingBox IGH_PreviewObject.ClippingBox => Preview_ComputeClippingBox();
        void IGH_PreviewObject.DrawViewportMeshes(IGH_PreviewArgs args) => Preview_DrawMeshes(args);
        void IGH_PreviewObject.DrawViewportWires(IGH_PreviewArgs args) => Preview_DrawWires(args);
        #endregion
    }
}
