using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen.Assemblies;
using static RhinoInside.NX.Extensions.Globals;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
    public static partial class ComponentEx
    {
        /// <summary>
        /// 获取组件内某一类型的所有对象，如果无法获取，可能需要先更改引用集
        /// </summary>
        /// <typeparam name="T">类型名</typeparam>
        /// <param name="component">要获取对象的组件</param>
        /// <param name="GetObjectsInPart">获取对象集合的方法</param>
        /// <returns></returns>
        public static List<T> GetObjects<T>(this NXOpen.Assemblies.Component component, Func<Part, TaggedObjectCollection> GetObjectsInPart) where T : TaggedObject
        {
            if (!(component.Prototype is Part))
                DisplayPart.ComponentAssembly.OpenComponents(ComponentAssembly.OpenOption.ComponentOnly, new Component[] { component }, out ComponentAssembly.OpenComponentStatus[] openCompStatus);

            var originalRefSet = component.ReferenceSet;

            if (originalRefSet != component.EntirePartRefsetName)
                WorkPart.ComponentAssembly.ReplaceReferenceSetInOwners(component.EntirePartRefsetName, new Component[] { component });  // 没有设置引用集时，可能会查找不到某些对象

            Part prototypePart = component.Prototype as Part;
            TaggedObjectCollection prototyObjects = GetObjectsInPart(prototypePart);

            List<T> prototyObjectList = prototyObjects.Cast<TaggedObject>().Where(obj => obj is T).Select(obj => obj as T).ToList();

            if (originalRefSet != component.EntirePartRefsetName)
                WorkPart.ComponentAssembly.ReplaceReferenceSetInOwners(originalRefSet, new Component[] { component });

            if (component == DisplayPart.ComponentAssembly.RootComponent)
                return prototyObjectList;
            else
            {
                List<T> resultObjects = new List<T>();
                foreach (var obj in prototyObjectList)
                {
                    Tag tagObj = TheUfSession.Assem.FindOccurrence(component.Tag, obj.Tag);
                    if (tagObj != Tag.Null)
                        resultObjects.Add(tagObj.GetTaggedObject() as T);
                }

                return resultObjects;
            }
        }

        /// <summary>
        /// 获取组件内某一类型的所有对象
        /// </summary>
        /// <typeparam name="T">类型名</typeparam>
        /// <param name="components">要获取对象的组件</param>
        /// <param name="GetObjectsInPart">获取对象集合的方法</param>
        /// <returns></returns>
        public static List<T> GetObjects<T>(this NXOpen.Assemblies.Component[] components, Func<Part, TaggedObjectCollection> GetObjectsInPart) where T : TaggedObject
        {
            List<T> resultObjects = new List<T>();
            foreach (var component in components)
                resultObjects.AddRange(component.GetObjects<T>(GetObjectsInPart));

            return resultObjects;
        }

        /// <summary>
        /// 判断组件内是否含有体
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static bool ContainsBody(this NXOpen.Assemblies.Component component)
        {
            var prototyBodies = (component.Prototype as Part).Bodies.ToArray();

            if (prototyBodies.Length != 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 查询组件原型部件的ID
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static string GetPrototypeID(this NXOpen.Assemblies.Component component)
        {
            try
            {
                return component.GetStringUserAttribute("DB_PART_NO", -1);
            }
            catch (Exception)
            {
                return component.DisplayName;
            }
        }

        /// <summary>
        /// 查询组件的名称（除非添加组件时设置，否则与部件名称通常相同）
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static string GetName(this NXOpen.Assemblies.Component component)
        {
            return component.Name;
        }

        /// <summary>
        /// 查询组件原型部件的名称
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static string GetPrototypeName(this NXOpen.Assemblies.Component component)
        {
            try
            {
                return component.GetStringUserAttribute("DB_PART_NAME", -1);
            }
            catch (Exception)
            {
                return component.DisplayName;
            }
        }

        /// <summary>
        /// 通过名称获取组件中的对象
        /// </summary>
        /// <param name="component"></param>
        /// <param name="objName"></param>
        /// <returns></returns>
        public static List<TaggedObject> GetObjectsByName(this NXOpen.Assemblies.Component component, string objName)
        {
            List<TaggedObject> namedObjects = new List<TaggedObject>();

            var originalRefSet = component.ReferenceSet;

            if (originalRefSet != component.EntirePartRefsetName)
                WorkPart.ComponentAssembly.ReplaceReferenceSet(component, component.EntirePartRefsetName);  // 没有设置引用集时，可能会查找不到某些对象

            Tag startTag = Tag.Null;
            do
            {
                TheUfSession.Obj.CycleByName(objName, ref startTag);

                if (startTag != Tag.Null)
                {
                    var partOcc = TheUfSession.Assem.AskPartOccurrence(startTag);

                    var taggedObj = startTag.GetTaggedObject();

                    if (partOcc == component.Tag)
                        namedObjects.Add(startTag.GetTaggedObject());
                }
                else
                    break;
            } while (true);

            if (originalRefSet != component.EntirePartRefsetName)
                WorkPart.ComponentAssembly.ReplaceReferenceSet(component, originalRefSet);

            return namedObjects;
        }

        /// <summary>
        /// 绕旋转轴旋转组件
        /// </summary>
        /// <param name="componentToRotate"></param>
        /// <param name="rotateOrigin"></param>
        /// <param name="rotateVector"></param>
        /// <param name="rotateAngle"></param>
        /// <param name="copyOriginal"></param>
        public static Component RotateComponent(this NXOpen.Assemblies.Component componentToRotate, Point3d rotateOrigin, Vector3d rotateVector, double rotateAngle, bool copyOriginal = false)
        {
            componentToRotate.GetPosition(out Point3d componentOrigin, out Matrix3x3 orientation);
            if (copyOriginal)
                componentToRotate = WorkPart.ComponentAssembly.AddComponent(componentToRotate.Prototype as Part, componentOrigin, orientation);

            double[] rotateMtx = new double[9];
            TheUfSession.Mtx3.RotateAboutAxis(rotateVector.ToArray(), (rotateAngle * Math.PI) / 180, rotateMtx);

            var orientationInDouble = orientation.ToArray();

            double[] xVec = new double[3];
            double[] yVec = new double[3];
            double[] zVec = new double[3];
            TheUfSession.Mtx3.XVec(orientationInDouble, xVec);
            TheUfSession.Mtx3.YVec(orientationInDouble, yVec);
            TheUfSession.Mtx3.ZVec(orientationInDouble, zVec);

            TheUfSession.Mtx3.VecMultiply(xVec, rotateMtx, xVec);
            TheUfSession.Mtx3.VecMultiply(yVec, rotateMtx, yVec);
            TheUfSession.Mtx3.VecMultiply(zVec, rotateMtx, zVec);

            double[] rotatedOrientation = new double[9];
            TheUfSession.Mtx3.Initialize(xVec, yVec, rotatedOrientation);

            var rotatedOrigin = componentOrigin.Rotate(rotateOrigin, rotateVector, rotateAngle);

            TheUfSession.Assem.RepositionInstance(TheUfSession.Assem.AskInstOfPartOcc(componentToRotate.Tag), rotatedOrigin.ToArray(), rotatedOrientation);

            return componentToRotate;
        }

        /// <summary>
        /// 移动组件
        /// </summary>
        /// <param name="componentToMove">要移动的组件</param>
        /// <param name="deltaX">X方向上的距离</param>
        /// <param name="deltaY">Y方向上的距离</param>
        /// <param name="deltaZ">Z方向上的距离</param>
        /// <param name="copyOriginal"></param>
        public static Component MoveComponent(this Component componentToMove, double deltaX, double deltaY, double deltaZ, bool copyOriginal = false)
        {
            componentToMove.GetPosition(out Point3d componentOrigin, out Matrix3x3 orientation);

            if (copyOriginal)
                componentToMove = WorkPart.ComponentAssembly.AddComponent(componentToMove.Prototype as Part, componentOrigin, orientation);

            var orientationInDouble = orientation.ToArray();

            componentOrigin = componentOrigin.Move(deltaX, deltaY, deltaZ);

            TheUfSession.Assem.RepositionInstance(TheUfSession.Assem.AskInstOfPartOcc(componentToMove.Tag), componentOrigin.ToArray(), orientation.ToArray());

            return componentToMove;
        }

        /// <summary>
        /// 移动组件，距离为矢量的长度
        /// </summary>
        /// <param name="componentToMove">要移动的组件</param>
        /// <param name="vector"></param>
        /// <param name="copyOriginal"></param>
        public static Component MoveComponent(this NXOpen.Assemblies.Component componentToMove, Vector3d vector, bool copyOriginal = false) => MoveComponent(componentToMove, vector.X, vector.Y, vector.Z, copyOriginal);

        /// <summary>
        /// 移动组件
        /// </summary>
        /// <param name="componentToMove">要移动的组件</param>
        /// <param name="vector"></param>
        /// <param name="distance"></param>
        /// <param name="copyOriginal"></param>
        public static Component MoveComponent(this NXOpen.Assemblies.Component componentToMove, Vector3d vector, double distance, bool copyOriginal = false)
        {
            componentToMove.GetPosition(out Point3d componentOrigin, out Matrix3x3 orientation);

            if (copyOriginal)
                componentToMove = WorkPart.ComponentAssembly.AddComponent(componentToMove.Prototype as Part, componentOrigin, orientation);

            var orientationInDouble = orientation.ToArray();

            componentOrigin = componentOrigin.Move(vector, distance);

            TheUfSession.Assem.RepositionInstance(TheUfSession.Assem.AskInstOfPartOcc(componentToMove.Tag), componentOrigin.ToArray(), orientation.ToArray());

            return componentToMove;
        }

        /// <summary>
        /// 获取组件的位置矩阵，该矩阵中包含方位与原点
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static Matrix4x4 GetPosition(this NXOpen.Assemblies.Component component)
        {
            Point3d point;
            Matrix3x3 matrix;
            component.GetPosition(out point, out matrix);
            Matrix4x4 result = Matrix4x4Ex.Create(point, matrix);
            return result;
        }

        /// <summary>
        /// 读取装配结构树
        /// </summary>
        /// <param name="component">要读取子零件的组件</param>
        /// <returns></returns>
        public static List<Component> GetAllChildren(this Component component)
        {
            var componentsIncludeSelf = ReadAssembly(component, null);
            componentsIncludeSelf.RemoveAt(0);
            return componentsIncludeSelf;
        }

        /// <summary>
        /// 读取装配结构树
        /// </summary>
        /// <param name="component">要读取子零件的组件，若为null，则读取整个装配树</param>
        /// <param name="componentList">零件列表，为递归时使用，用户可以不用设置</param>
        /// <returns></returns>
        public static List<Component> ReadAssembly(Component component = null, List<Component> componentList = null)
        {
            try
            {
                if (component == null)
                    component = DisplayPart.ComponentAssembly.RootComponent;

                if (componentList == null)
                    componentList = new List<Component>();

                if (component == null)
                    return componentList;
                else
                    componentList.Add(component);

                var children = component.GetChildren();

                if (children.Length != 0)
                {
                    foreach (var child in children)
                    {
                        ReadAssembly(child, componentList);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ListingWindowWriteLine();
            }

            return componentList;
        }

        /// <summary>
        /// 将组件设为工作部件
        /// </summary>
        /// <param name="component"></param>
        /// <param name="refsetOption">要使用的引用集</param>
        public static void MakeWork(this Component component, PartCollection.RefsetOption refsetOption = PartCollection.RefsetOption.Entire)
        {
            TheSession.Parts.SetWorkComponent(component, refsetOption, NXOpen.PartCollection.WorkComponentOption.Visible, out _);
        }

        /// <summary>
        /// 替换引用集
        /// </summary>
        /// <param name="component">组件</param>
        /// <param name="referenceSetName">引用集名称</param>
        public static void ReplaceReferenceSet(this Component component, string referenceSetName)
        {
            WorkPart.ComponentAssembly.ReplaceReferenceSetInOwners(referenceSetName.ToUpper(), new Component[] { component });
        }

        /// <summary>
        /// 计算组件内所有体的包围盒，作为组件的最小包围盒
        /// </summary>
        /// <param name="comp"></param>
        /// <param name="refFrame"></param>
        /// <returns></returns>
        public static BoundingBox3D GetBoundingBox(this Component comp, Matrix4x4 refFrame)
        {
            List<Body> bodies = comp.GetObjects<Body>(obj => obj.Bodies);
            return bodies.GetBoundingBox(refFrame);
        }
    }
}
