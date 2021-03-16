using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RhinoInside.NX.Extensions.Globals;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
    public static partial class DisplayableObjectEx
    {
        /// <summary>
        /// 更改对象颜色
        /// </summary>
        /// <param name="displayableObject"></param>
        /// <param name="color"></param>
        public static void SetColor(this DisplayableObject displayableObject, int color)
        {
            NXOpen.Part displayPart = TheSession.Parts.Display;

            NXOpen.DisplayModification displayModification = TheSession.DisplayManager.NewDisplayModification();

            displayModification.ApplyToAllFaces = true;

            displayModification.ApplyToOwningParts = false;

            displayModification.NewColor = color;

            displayModification.Apply(new DisplayableObject[] { displayableObject });

            displayModification.Dispose();
        }

        /// <summary>
        /// 更改对象颜色
        /// </summary>
        /// <param name="displayableObjects"></param>
        /// <param name="color"></param>
        public static void SetColor(this DisplayableObject[] displayableObjects, int color)
        {
            NXOpen.Part displayPart = TheSession.Parts.Display;

            NXOpen.DisplayModification displayModification = TheSession.DisplayManager.NewDisplayModification();

            displayModification.ApplyToAllFaces = true;

            displayModification.ApplyToOwningParts = false;

            displayModification.NewColor = color;

            displayModification.Apply(displayableObjects);

            displayModification.Dispose();
        }

        /// <summary>
        /// 设置对象的透明度
        /// </summary>
        /// <param name="displayableObject">要设置透明度的对象</param>
        /// <param name="translucency">透明度值</param>
        /// <param name="applyToAllFaces">是否应用于所有面</param>
        /// <param name="applyToOwningParts">是否应用于所属部件</param>
        public static void SetTranslucency(this DisplayableObject displayableObject, int translucency, bool applyToAllFaces = true, bool applyToOwningParts = false) => SetTranslucency(new DisplayableObject[] { displayableObject }, translucency, applyToAllFaces, applyToOwningParts);

        /// <summary>
        /// 设置对象的透明度
        /// </summary>
        /// <param name="displayableObjects">要设置透明度的对象</param>
        /// <param name="translucency">透明度值</param>
        /// <param name="applyToAllFaces">是否应用于所有面</param>
        /// <param name="applyToOwningParts">是否应用于所属部件</param>
        public static void SetTranslucency(this DisplayableObject[] displayableObjects, int translucency, bool applyToAllFaces = true, bool applyToOwningParts = false)
        {
            NXOpen.DisplayModification displayModification = TheSession.DisplayManager.NewDisplayModification();

            displayModification.ApplyToAllFaces = applyToAllFaces;

            displayModification.ApplyToOwningParts = applyToOwningParts;

            displayModification.NewTranslucency = translucency;

            displayModification.Apply(displayableObjects);

            displayModification.Dispose();
        }

        /// <summary>
        /// 显示对象，将对象所在图层设为可选图层
        /// </summary>
        /// <param name="objectToShow">要显示的对象</param>
        public static void Show(this DisplayableObject objectToShow)
        {
            TheSession.DisplayManager.ShowObjects(new DisplayableObject[] { objectToShow }, DisplayManager.LayerSetting.ChangeLayerToSelectable);

            WorkPart.ModelingViews.WorkView.FitAfterShowOrHide(View.ShowOrHideType.ShowOnly);
        }

        /// <summary>
        /// 显示对象，将对象所在图层设为可选图层
        /// </summary>
        /// <param name="objectsToShow">要显示对象</param>
        public static void Show(this IEnumerable<DisplayableObject> objectsToShow)
        {
            TheSession.DisplayManager.ShowObjects(objectsToShow.ToArray(), DisplayManager.LayerSetting.ChangeLayerToSelectable);

            WorkPart.ModelingViews.WorkView.FitAfterShowOrHide(View.ShowOrHideType.ShowOnly);
        }

        /// <summary>
        /// 隐藏对象
        /// </summary>
        /// <param name="objectsToBlank">要隐藏的对象</param>
        public static void Blank(this IEnumerable<DisplayableObject> objectsToBlank)
        {
            TheSession.DisplayManager.BlankObjects(objectsToBlank.ToArray());

            WorkPart.ModelingViews.WorkView.FitAfterShowOrHide(View.ShowOrHideType.HideOnly);
        }
    }
}
