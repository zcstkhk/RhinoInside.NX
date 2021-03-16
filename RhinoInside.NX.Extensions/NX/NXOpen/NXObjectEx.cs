using NXOpen;
using System;
using NXOpen.UF;
using NXOpen.Features;

namespace RhinoInside.NX.Extensions
{
    public static class NXObjectEx
    {
        private static Session theSession;
        private static UFSession theUfSession;

        static NXObjectEx()
        {
            theSession = Session.GetSession();
            theUfSession = UFSession.GetUFSession();
        }

        /// <summary>
        /// 复制粘贴对象
        /// </summary>
        /// <param name="nxObject">要粘贴的对象</param>
        /// <returns></returns>
        public static NXObject[] CopyAndPaste(this NXObject nxObject) => CopyAndPaste(new NXObject[] { nxObject });

        /// <summary>
        /// 复制粘贴对象
        /// </summary>
        /// <param name="nxObjects">要粘贴的对象</param>
        /// <returns></returns>
        public static NXObject[] CopyAndPaste(this NXObject[] nxObjects)
        {
            Part workPart = theSession.Parts.Work;

            workPart.PmiManager.RestoreUnpastedObjects();
            NXOpen.Gateway.CopyCutBuilder copyCutBuilder = workPart.ClipboardOperationsManager.CreateCopyCutBuilder();
            copyCutBuilder.CanCopyAsSketch = true;
            copyCutBuilder.IsCut = false;
            copyCutBuilder.ToClipboard = true;
            copyCutBuilder.DestinationFilename = null;
            copyCutBuilder.SetObjects(nxObjects);
            NXObject nXObject1 = copyCutBuilder.Commit();
            copyCutBuilder.Destroy();

            NXOpen.Gateway.PasteBuilder pasteBuilder = workPart.ClipboardOperationsManager.CreatePasteBuilder();
            pasteBuilder.Commit();
            NXObject[] finalObjects = pasteBuilder.GetCommittedObjects();
            pasteBuilder.Destroy();
            return finalObjects;
        }

        /// <summary>
        /// 剪切粘贴对象
        /// </summary>
        /// <param name="nxObject">要粘贴的对象</param>
        /// <returns></returns>
        public static NXObject[] CutAndPaste(this NXObject nxObject) => CutAndPaste(new NXObject[] { nxObject });

        /// <summary>
        /// 剪切粘贴对象
        /// </summary>
        /// <param name="nxObjects">要粘贴的对象</param>
        /// <returns></returns>
        public static NXObject[] CutAndPaste(this NXObject[] nxObjects)
        {
            Part workPart = theSession.Parts.Work;

            workPart.PmiManager.RestoreUnpastedObjects();

            NXOpen.Gateway.CopyCutBuilder copyCutBuilder = workPart.ClipboardOperationsManager.CreateCopyCutBuilder();

            copyCutBuilder.CanCopyAsSketch = false;

            copyCutBuilder.IsCut = true;

            copyCutBuilder.ToClipboard = true;

            copyCutBuilder.DestinationFilename = null;

            copyCutBuilder.SetObjects(nxObjects);

            copyCutBuilder.Commit();

            copyCutBuilder.Destroy();

            NXOpen.Gateway.PasteBuilder pasteBuilder = workPart.ClipboardOperationsManager.CreatePasteBuilder();

            pasteBuilder.Commit();
            NXObject[] finalObjects = pasteBuilder.GetCommittedObjects();
            pasteBuilder.Destroy();

            return finalObjects;
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="objectTodelete"></param>
        public static void Delete(this NXObject objectTodelete) => Delete(new NXObject[] { objectTodelete });

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="objectsTodelete"></param>
        public static void Delete(this NXObject[] objectsTodelete)
        {
            NXOpen.Session.UndoMarkId markId;
            markId = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Delete");
            try
            {
                theSession.UpdateManager.AddObjectsToDeleteList(objectsTodelete);
            }
            catch (Exception)
            {

            }
            finally
            {
                theSession.UpdateManager.DoUpdate(markId);
            }
        }

        /// <summary>
        /// 移除对象参数
        /// </summary>
        /// <param name="objectsToRemoveParameter"></param>
        public static void RemoveParameters(this NXObject[] objectsToRemoveParameter)
        {
            Part workPart = theSession.Parts.Work;
            workPart.Features.RemoveParameters(objectsToRemoveParameter);
        }

        /// <summary>
        /// 移除对象参数
        /// </summary>
        /// <param name="objectToRemoveParameter"></param>
        public static void RemoveParameters(this NXObject objectToRemoveParameter)
        {
            Part workPart = theSession.Parts.Work;
            workPart.Features.RemoveParameters(objectToRemoveParameter);
        }

        public static ObjectTypes GetObjectType(this NXObject obj)
        {
            theUfSession.Obj.AskTypeAndSubtype(obj.Tag, out int type, out int subType);
            ObjectType objType = (ObjectType)type;
            if (type == 70)
            {
                if (subType == 0)
                    objType = ObjectType.Body;
                else if (subType == 2)
                    objType = ObjectType.Face;
                else if (subType == 3)
                    objType = ObjectType.Edge;
            }
            else if (type == 205)
            {
                Feature feature = (Feature)obj;
                if (feature is DatumAxisFeature)
                    objType = ObjectType.DatumAxis;
                else if (feature is DatumPlaneFeature)
                    objType = ObjectType.DatumPlane;
            }


            ObjectSubType objSubtype = (ObjectSubType)(100 * type + subType);

            return new ObjectTypes(objType, objSubtype);
        }
    }
}
