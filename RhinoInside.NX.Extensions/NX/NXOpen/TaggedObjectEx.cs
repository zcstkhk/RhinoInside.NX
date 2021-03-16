using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using static RhinoInside.NX.Extensions.Globals;

namespace RhinoInside.NX.Extensions
{
    /// <summary>
    /// TaggedObjec 的扩展方法类
    /// </summary>
    public static class TaggedObjectEx
    {
        #region 常规方法
        /// <summary>
        /// 获取 Tag 指向的对象
        /// </summary>
        /// <param name="objectTag"></param>
        /// <returns></returns>
        public static TaggedObject GetTaggedObject(this Tag objectTag)
        {
            return NXOpen.Utilities.NXObjectManager.Get(objectTag);
        }

        public static bool TryGetTaggedObject(this Tag objectTag, out TaggedObject taggedObject)
        {
            try
            {
                taggedObject = NXOpen.Utilities.NXObjectManager.Get(objectTag);

                return true;
            }
            catch (Exception)
            {
                taggedObject = null;
                return false;
            }
        }

        /// <summary>
        /// Return the persistent HANDLE for an object tag.
        /// </summary>
        /// <param name="taggedObject"></param>
        /// <returns></returns>
        public static string GetHandle(this TaggedObject taggedObject)
        {
            TheUfSession.Tag.AskHandleFromTag(taggedObject.Tag, out string handle);

            return handle;
        }

        #endregion

    }
}
