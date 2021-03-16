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
using RhinoInside.NX.Extensions;
using GH_IO.Serialization;
using System.Diagnostics;
using System.Timers;

namespace RhinoInside.NX.GH.Types
{
    public interface INXDisplayableObject
    {
        Tag Tag { get; }

        bool Highlight { get; set; }

        bool Hidden { get; set; }

        int Layer { get; set; }

        void RedisplayObject();
    }

    public abstract class NX_DisplayableObject<T> : GH_Goo<T>, INXDisplayableObject where T : class
    {
        private string Handle;

        public NX_DisplayableObject()
        {
        }

        public NX_DisplayableObject(Tag tag) : base()
        {
            _tag = tag;

            UFSession.GetUFSession().Tag.AskHandleFromTag(tag, out Handle);
        }

        /// <summary>
        /// 是否高亮显示
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="highlight"></param>
        public NX_DisplayableObject(Tag tag, bool highlight) : this(tag)
        {
            Highlight = highlight;
        }

        public override T Value
        {
            get
            {
                if (Tag == Tag.Null)
                    return null;
                else
                {
                    Tag.TryGetTaggedObject(out var obj);
                    return obj as T;
                }
            }
            set => base.Value = value;
        }

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

        public bool Hidden
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public int Layer
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        protected Tag _tag;

        public Tag Tag => _tag;

        public void RedisplayObject()
        {
            throw new NotImplementedException();
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
                _tag = objectTag;
            else
                _tag = Tag.Null;

            return base.Read(reader);
        }

        public override bool IsValid
        {
            get
            {
                if (_tag == Tag.Null || Value == null)
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
