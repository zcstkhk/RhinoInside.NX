using NXOpen.Assemblies;
using NXOpen.UF;
using System;
using NXOpen;
using System.IO;
using static RhinoInside.NX.Extensions.Globals;

namespace RhinoInside.NX.Extensions
{
    /// <summary>
    /// NXOpen.Part 类的扩展类
    /// </summary>
    public static class PartEx
    {
        static NXOpen.Session theSession;
        static NXOpen.Part workPart;
        static NXOpen.UI theUI;

        static PartEx()
        {
            theSession = NXOpen.Session.GetSession();
            workPart = theSession.Parts.Work;
            theUI = UI.GetUI();
        }

        /// <summary>
        /// 查询部件在TC中的ID号
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static string GetID(this Part part)
        {
            try
            {
                return part.GetStringUserAttribute("DB_PART_NO", -1);
            }
            catch (Exception)
            {
                return part.Name;
            }
        }

        /// <summary>
        /// 查询部件在TC中的版本号
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static string GetRevision(this Part part)
        {
            try
            {
                return part.GetStringUserAttribute("DB_PART_REV", -1);
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 判断零件内是否包含 Body
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static bool ContainsBody(this Part part)
        {
            var bodies = part.Bodies.ToArray();

            if (bodies.Length == 0)
                return false;
            else
                return true;
            //Tag start = Tag.Null;
            //do
            //{
            //	start = theUfSession.Obj.CycleAll(part.Tag, start);

            //	if (start != Tag.Null)
            //	{
            //		var taggedObj = start.GetTaggedObject();
            //		if (taggedObj is Body)
            //		{
            //			theUfSession.Disp.SetHighlight(start, 1);
            //			return true;
            //		}
            //	}
            //} while (start != Tag.Null);

            //return false;
        }


        ///// <summary>
        ///// 在当前工作部件中通过名称获取对象
        ///// </summary>
        ///// <param name="objectName"></param>
        ///// <returns></returns>
        //public static TaggedObject GetObject(string objectName)
        //{
        //	Tag objTag = Tag.Null;

        //	theUfSession.Obj.CycleByName(objectName, ref objTag);

        //	foreach (Curve curve in part.Curves)
        //	{
        //		if (curve.Name == objectName.ToUpper())
        //			resultCurve = curve;
        //	}

        //	if (resultCurve == null)
        //	{
        //		foreach (Point point in part.Points)
        //		{
        //			if (point.Name == objectName.ToUpper())
        //				resultCurve = point;
        //		}
        //	}
        //	return resultCurve;
        //}

        /// <summary>
        /// 读取 Teamcenter 中零组件的最新版本
        /// </summary>
        /// <param name="part">要获取的零组件编号</param>
        /// <returns>如果处于非集成环境，将返回"N/A"</returns>
        public static string GetLatestRevision(this Part part)
        {
            if (ManagedMode == false)
            {
                return "N/A";
            }
            else
            {
                Tag part_Tag;
                TheUfSession.Ugmgr.AskPartTag(part.GetID(), out part_Tag);
                int revisionCount;
                Tag[] partRevisionTags;
                TheUfSession.Ugmgr.ListPartRevisions(part_Tag, out revisionCount, out partRevisionTags);
                string partRevisionId;
                TheUfSession.Ugmgr.AskPartRevisionId(partRevisionTags[partRevisionTags.Length - 1], out partRevisionId);
                return partRevisionId;
            }
        }

        /// <summary>
        /// 读取 Teamcenter 中零组件的最新版本
        /// </summary>
        /// <param name="partNumber">要获取的零组件编号</param>
        /// <returns>如果处于非集成环境，将返回"N/A"</returns>
        public static string GetLatestRevision(string partNumber)
        {
            if (ManagedMode == false)
            {
                return "N/A";
            }
            else
            {
                Tag part_Tag;
                TheUfSession.Ugmgr.AskPartTag(partNumber, out part_Tag);
                int revisionCount;
                Tag[] partRevisionTags;
                TheUfSession.Ugmgr.ListPartRevisions(part_Tag, out revisionCount, out partRevisionTags);
                string partRevisionId;
                TheUfSession.Ugmgr.AskPartRevisionId(partRevisionTags[partRevisionTags.Length - 1], out partRevisionId);
                return partRevisionId;
            }
        }

        /// <summary>
        /// 打开部件，并选择是否更改为当前显示部件
        /// </summary>
        /// <param name="partNumber">要打开的部件编号,若为本地零件，则为部件的完整路径</param>
        /// <param name="partRevision">要打开的部件版本,若为null,则打开最新版本</param>
        /// <param name="displayPart">是否设为显示部件</param>
        /// <returns></returns>
        public static Part OpenPart(string partNumber, string partRevision, bool displayPart)
        {
            Tag partTag = default;
            UFPart.LoadStatus errorStatus;
            string partName = null;

            if (ManagedMode == true)
            {
                if (partRevision == null)
                    partRevision = GetLatestRevision(partNumber);

                partName = "@DB/" + partNumber + "/" + partRevision;
            }
            else
            {
                partName = partNumber;
            }

            TheUfSession.Part.OpenQuiet(partName, out partTag, out errorStatus);

            if (displayPart)
                TheUfSession.Part.SetDisplayPart(partTag);

            Part part = (Part)NXOpen.Utilities.NXObjectManager.Get(partTag);

            return part;
        }

        /// <summary>
        /// 打开 Teamcenter 中的部件，并选择是否更改为当前显示部件
        /// </summary>
        /// <param name="partNumber">要打开的部件编号</param>
        /// <param name="partRevision">要打开的部件版本,若为null,则打开最新版本</param>
        /// <param name="displayPart">是否设为显示部件</param>
        /// <returns></returns>
        public static Part OpenPartInDB(string partNumber, string partRevision, bool displayPart)
        {
            Tag partTag = default;
            UFPart.LoadStatus errorStatus;

            if (partRevision == null)
                partRevision = GetLatestRevision(partNumber);

            string partName = "@DB/" + partNumber + "/" + partRevision;

            TheUfSession.Part.OpenQuiet(partName, out partTag, out errorStatus);

            if (displayPart)
                TheUfSession.Part.SetDisplayPart(partTag);

            Part part = (Part)NXOpen.Utilities.NXObjectManager.Get(partTag);

            return part;
        }
    }
}
