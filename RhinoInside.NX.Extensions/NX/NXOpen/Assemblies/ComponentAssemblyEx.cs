using NXOpen.Assemblies;
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
    public static partial class ComponentAssemblyEx
    {
        #region AddComponent
        /// <summary>
        /// 往指定部件中添加组件并移动
        /// </summary>
        /// <param name="componentAssembly"></param>
        /// <param name="partToAdd">要添加的数模</param>
        /// <param name="locateOrigin">要添加到的原点</param>
        /// <param name="layer">要添加到的图层，默认为-1，即原始图层，0表示工作图层，其余表示特定图层</param>
        /// <returns></returns>
        public static Component AddComponent(this ComponentAssembly componentAssembly, Part partToAdd, Point3d locateOrigin, int layer = -1) => AddComponent(componentAssembly, partToAdd, locateOrigin, Matrix3x3Ex.Identity, layer);

        /// <summary>
        /// 往工作部件中添加组件并移动
        /// </summary>
        /// <param name="componentAssembly"></param>
        /// <param name="partToAdd">要添加的数模</param>
        /// <param name="locateOrigin">要添加到的原点</param>
        /// <param name="orientation">方位</param>
        /// <param name="layer">要添加到的图层，默认为-1，即原始图层，0表示工作图层，其余表示特定图层</param>
        /// <returns></returns>
        public static Component AddComponent(this ComponentAssembly componentAssembly, Part partToAdd, Point3d locateOrigin, Matrix3x3 orientation, int layer = -1)
        {
            PartLoadStatus loadStatus;

            Component component;

            if (ManagedMode == true)
            {
                string partNumber = partToAdd.GetID();

                string partRevision = partToAdd.GetRevision();

                if (partRevision == "N/A")
                    partRevision = partToAdd.GetLatestRevision();

                component = componentAssembly.AddComponent("@DB/" + partNumber + @"/" + partRevision, "Entire Part", partNumber, locateOrigin, orientation, layer, out loadStatus);
            }
            else
            {
                // 添加本地组件
                component = componentAssembly.AddComponent(partToAdd.FullPath, "Entire Part", partToAdd.GetID(), locateOrigin, orientation, layer, out loadStatus);
            }
            return component;
        }

        /// <summary>
        /// 往指定部件中添加组件并移动到指定位置，方向为默认
        /// </summary>
        /// <param name="componentAssembly"></param>
        /// <param name="partNumber">要添加的数模号，若为本地，则需要指定完整路径</param>
        /// <param name="partRevision">要添加的数模版本，仅用于 Teamcenter 集成环境，本地模式忽略此参数，若版本为空或null，则添加最新版本</param>
        /// <param name="locateOrigin">要添加到的原点</param>
        /// <param name="layer">要添加到的图层，默认为-1，即原始图层，0表示工作图层，其余表示特定图层</param>
        /// <returns></returns>
        public static Component AddComponent(this ComponentAssembly componentAssembly, string partNumber, string partRevision, Point3d locateOrigin, int layer = -1) => AddComponent(componentAssembly, partNumber, partRevision, locateOrigin, Matrix3x3Ex.Identity, layer);

        /// <summary>
        /// 往指定部件中添加组件并移动到指定位置，方向为默认
        /// </summary>
        /// <param name="componentAssembly"></param>
        /// <param name="partNumber">要添加的数模号，若为本地，则需要指定完整路径</param>
        /// <param name="partRevision">要添加的数模版本，仅用于 Teamcenter 集成环境，本地模式忽略此参数，若版本为空或null，则添加最新版本</param>
        /// <param name="locateOrigin">要添加到的原点</param>
        /// <param name="orientation">组件方位</param>
        /// <param name="layer">要添加到的图层，默认为-1，即原始图层，0表示工作图层，其余表示特定图层</param>
        /// <returns></returns>
        public static Component AddComponent(this ComponentAssembly componentAssembly, string partNumber, string partRevision, Point3d locateOrigin, Matrix3x3 orientation, int layer = -1)
        {
            PartLoadStatus loadStatus;

            NXOpen.Assemblies.Component component;

            if (ManagedMode == true)
            {
                if (string.IsNullOrEmpty(partRevision) || partRevision == "N/A")
                    partRevision = PartEx.GetLatestRevision(partNumber);

                component = componentAssembly.AddComponent("@DB/" + partNumber + @"/" + partRevision, "Entire Part", partNumber, locateOrigin, orientation, layer, out loadStatus);
            }
            else
            {
                // 添加本地组件
                component = componentAssembly.AddComponent(partNumber, "Entire Part", partNumber, locateOrigin, orientation, layer, out loadStatus);
            }
            return component;
        }
        #endregion
    }
}
