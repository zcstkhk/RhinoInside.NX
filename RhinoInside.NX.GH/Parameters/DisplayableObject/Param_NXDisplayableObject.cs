using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using System.Windows.Forms;
using Grasshopper.Kernel.Data;
using RhinoInside.NX.GH.Types;
using NXOpen.Extensions;
using NXOpen;
using GH_IO.Serialization;
using Grasshopper.Kernel.Attributes;
using Timer = System.Timers.Timer;
using static RhinoInside.NX.GH.Properties.Languages;

namespace RhinoInside.NX.GH.Parameters
{
    class NXDiaplayableParamAttributes : GH_FloatingParamAttributes
    {
        public NXDiaplayableParamAttributes(IGH_Param param) : base(param)
        {
            PerformLayout();
        }

        public override bool Selected
        {
            get
            {
                return base.Selected;
            }
            set
            {
                if (!Owner.VolatileData.IsEmpty)
                {
                    var validDatas = Owner.VolatileData.AllData(true).Where(obj => obj.IsValid).Select(obj => (obj as INXDisplayableObject).Tag).ToArray();

                    if (validDatas.Length > 0)
                        Globals.TheUfSession.Disp.SetHighlights(validDatas.Length, validDatas, value ? 1 : 0);
                }

                base.Selected = value;
            }
        }
    }

    public abstract class Param_NXDisplayableObject<T> : GH_PersistentParam<T> where T : class, IGH_Goo, INXDisplayableObject
    {
        private Timer _timer;

        protected bool Visible;

        public Param_NXDisplayableObject(string name, string nickName, string desc, string category, string subCategory) : base(name, nickName, desc, category, subCategory)
        {
            Visible = true;
        }

        public override void AddedToDocument(GH_Document document)
        {
            _timer = new Timer(1000);
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

            base.AddedToDocument(document);
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (PersistentData.Any(obj => !obj.IsValid))
            {
                if (!RuntimeMessages(GH_RuntimeMessageLevel.Error).Contains("所选对象已不存在，请重新选择！"))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "所选对象已不存在，请重新选择！");
                }
            }
            else
            {
                if (RuntimeMessages(GH_RuntimeMessageLevel.Error).Contains("所选对象已不存在，请重新选择！"))
                {
                    ClearRuntimeMessages();
                }
            }

            OnVolatileDataCollected();
        }

        /// <summary>
        /// 单选时的操作
        /// </summary>
        /// <param name="type"></param>
        /// <param name="selectedObjectTag"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        protected TaggedObject Select_Singular(params Selection.MaskTriple[] type)
        {
            using (Core.Externel.EditScope scope = new Core.Externel.EditScope())
            {
                var selectionResponse = UI.GetUI().SelectionManager.SelectTaggedObject($"{GetString("DisplayableParamSelectMessage")} {NickName}", $"{GetString("DisplayableParamSelectMessage")} {NickName}", Selection.SelectionScope.WorkPart, Selection.SelectionAction.ClearAndEnableSpecific, false, false, type, out var selectedObject, out var cursor);

                if (selectionResponse == Selection.Response.ObjectSelected)
                    return selectedObject;

                return null;
            }
        }

        protected TaggedObject[] Select_Plural(params Selection.MaskTriple[] type)
        {
            using (Core.Externel.EditScope scope = new Core.Externel.EditScope())
            {
                var selectionResponse = Globals.TheUI.SelectionManager.SelectTaggedObjects($"{GetString("DisplayableParamSelectMessage")} {NickName}", $"{GetString("DisplayableParamSelectMessage")} {NickName}", Selection.SelectionScope.WorkPart, Selection.SelectionAction.ClearAndEnableSpecific, false, false, type, out var selectedObjects);

                if (selectionResponse == Selection.Response.Ok)
                    return selectedObjects;

                return null;
            }
        }

        protected void SelectionPreparation()
        {
            if (PersistentData.DataCount != 0)
            {
                foreach (INXDisplayableObject data in PersistentData)
                {
                    if (data.Highlight)
                        data.Highlight = false;
                }
            }
        }

        public override void CreateAttributes()
        {
            m_attributes = new NXDiaplayableParamAttributes(this);
        }

        private void Menu_ViewInNX(object sender, EventArgs e)
        {
            // TODO
            System.Windows.Forms.MessageBox.Show("View in NX, Not Implement yet!");
        }

        public override bool AppendMenuItems(ToolStripDropDown menu)
        {
            if (Attributes.IsTopLevel)
            {
                Menu_AppendObjectNameEx(menu);
            }
            else
            {
                Menu_AppendObjectName(menu);
            }

            Menu_AppendItem(menu, "Visible", Menu_VisibleInNXClicked, Properties.Icons.NX_Show, true, Visible);
            Menu_AppendEnableItem(menu);
            Menu_AppendItem(menu, "View in NX", Menu_ViewInNX);
            Menu_AppendRuntimeMessages(menu);

            Menu_AppendReverseParameter(menu);
            Menu_AppendFlattenParameter(menu);
            Menu_AppendGraftParameter(menu);
            Menu_AppendSimplifyParameter(menu);
            Menu_AppendSeparator(menu);

            if (Kind != GH_ParamKind.output)
            {
                ToolStripMenuItem singleValueMenuItem = Menu_CustomSingleValueItem();
                if (singleValueMenuItem == null)
                {
                    Menu_AppendPromptOne(menu);
                }
                else
                {
                    singleValueMenuItem.Enabled &= (SourceCount == 0);
                    menu.Items.Add(singleValueMenuItem);
                }
                ToolStripMenuItem multiValueMenuItem = Menu_CustomMultiValueItem();
                if (multiValueMenuItem == null)
                {
                    Menu_AppendPromptMore(menu);
                }
                else
                {
                    multiValueMenuItem.Enabled &= (SourceCount == 0);
                    menu.Items.Add(multiValueMenuItem);
                }

                Menu_AppendSeparator(menu);
                Menu_AppendItem(menu, "Clear values", Menu_ClearPersistentData, PersistentData.DataCount > 0);
                Menu_AppendInternaliseData(menu);
                Menu_AppendExtractParameter(menu);
            }

            Menu_AppendSeparator(menu);
            Menu_AppendObjectHelp(menu);

            return true;
        }

        protected virtual void Menu_VisibleInNXClicked(object sender, EventArgs e)
        {
            Visible = !Visible;

            for (int i = 0; i < PersistentDataCount; i++)
            {
                PersistentData.ElementAt(i).Visible = Visible;
            }
        }

        private void Menu_ClearPersistentData(object sender, EventArgs e)
        {
            if (PersistentDataCount > 0)
            {
                foreach (INXDisplayableObject item in PersistentData)
                {
                    if (item.Highlight)
                        item.Highlight = false;
                }

                PersistentData.Clear();

                OnPingDocument()?.ClearReferenceTable();

                ExpireSolution(true);
            }
        }

        protected override void OnVolatileDataCollected()
        {

        }

        public override void RemovedFromDocument(GH_Document document)
        {
            _timer.Dispose();
            base.RemovedFromDocument(document);
        }
    }
}
