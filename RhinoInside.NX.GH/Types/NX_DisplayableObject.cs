using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using NXOpen;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen.UF;
using NXOpen.Extensions;
using GH_IO.Serialization;
using System.Diagnostics;
using System.Timers;

namespace RhinoInside.NX.GH.Types
{
    /// <summary>
    /// 用于关联到 NXOpen.DisplayableObject
    /// </summary>
    public interface INXDisplayableObject
    {
        Tag Tag { get; }

        bool Highlight { get; set; }

        bool Visible { get; set; }

        int Layer { get; set; }

        void RedisplayObject();

        void Delete();
    }

    public abstract class NX_DisplayableObject<T> : GH_Goo<T>, INXDisplayableObject where T : DisplayableObject
    {
        private string Handle;

        public NX_DisplayableObject()
        {

        }

        public NX_DisplayableObject(T obj)
        {
            m_value = obj;
        }

        public NX_DisplayableObject(Tag tag) : base()
        {
            m_value = tag.GetTaggedObject() as T;

            UFSession.GetUFSession().Tag.AskHandleFromTag(tag, out Handle);
        }

        /// <summary>
        /// 是否高亮显示
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="highlight"></param>
        public NX_DisplayableObject(Tag tag, bool highlight) : this(tag)
        {
            m_value = tag.GetTaggedObject() as T;
            Highlight = highlight;
        }

        //public override T Value
        //{
        //    get
        //    {
        //        if (Tag == Tag.Null)
        //            return null;
        //        else
        //        {
        //            Tag.TryGetTaggedObject(out var obj);
        //            return obj as T;
        //        }
        //    }
        //    set => m_value = value;
        //}

        bool _highlight;
        public bool Highlight
        {
            get => _highlight;
            set
            {
                if (IsValid)
                {
                    if (_highlight && !value)
                        Globals.TheUfSession.Disp.SetHighlight(Tag, 0);
                    else if (!_highlight && value)
                        Globals.TheUfSession.Disp.SetHighlight(Tag, 1);
                }

                _highlight = value;
            }
        }

        protected bool _visible;

        public virtual bool Visible
        {
            get => _visible;
            set
            {
                if (value)
                    Value.Unblank();
                else
                    Value.Blank();

                _visible = value;
            }
        }

        public int Layer
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public Tag Tag => m_value.Tag;

        public void RedisplayObject()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            m_value.Delete();
            m_value = null;
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetString("Object Handle", Handle);

            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            var handle = reader.GetString("Object Handle");

            var objectTag = Globals.TheUfSession.Tag.AskTagOfHandle(handle);

            if (objectTag != Tag.Null)
                m_value = objectTag.GetTaggedObject() as T;
            else
                m_value = null;

            return base.Read(reader);
        }

        public override bool IsValid
        {
            get
            {
                if (Value == null)
                {
                    return false;
                }

                return true;
            }
        }

        public override string IsValidWhyNot
        {
            get
            {
                if (IsValid)
                    return string.Empty;
                else if (Tag == Tag.Null)
                    return $"引用空的对象！";
                else if (Value == null)
                    return $"引用非法的对象！";
                else
                    return base.IsValidWhyNot;
            }
        }

        public override IGH_Goo Duplicate()
        {
            return (IGH_Goo)MemberwiseClone();
        }

        public override string ToString()
        {
            if (Value != null)
            {
                StringBuilder result = new StringBuilder();
                result.AppendLine(TypeName);
                result.AppendLine($"Tag {Tag}");
                result.AppendLine($"Handle: {Handle}");
                return result.ToString();
            }
            return string.Empty;
        }
    }
}
