using NXOpen.MenuBar;
using Rhino;
using Rhino.DocObjects;
using Rhino.FileIO;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RhinoInside.NX.Translator;
using System.Windows.Forms;
using static NXOpen.Extensions.Globals;

namespace RhinoInside.NX.Core
{
    public static class ImportCommand
    {
       public static MenuBarManager.CallbackStatus Import3DMFile(string filePath)
        {
            using (File3dm model = File3dm.Read(filePath))
            {
                var scaleFactor = RhinoMath.UnitScale(model.Settings.ModelUnitSystem, PartUnit.ToRhinoUnits());

                var elements = new List<NXOpen.NXObject>();

                //model.Objects.Select(obj => obj.Geometry.GetType().ToString()).ToList().ForEach(obj => obj.ListingWindowWriteLine());

                foreach (var obj in model.Objects.Where(x => !x.Attributes.IsInstanceDefinitionObject && x.Attributes.Space == ActiveSpace.ModelSpace))
                {
                    if (!obj.Attributes.Visible)
                        continue;

                    var layer = model.AllLayers.FindIndex(obj.Attributes.LayerIndex);
                    if (layer?.IsVisible != true)
                        continue;

                    var geometry = obj.Geometry;

                    if (geometry is Extrusion extrusion) geometry = extrusion.ToBrep();
                    else if (geometry is SubD subD) geometry = subD.ToBrep(new SubDToBrepOptions());

                    try
                    {
                        switch (geometry)
                        {
                            case Point point:
                                var referncePoint = WorkPart.Points.CreatePoint(point.Location.ToXYZ(scaleFactor));
                                referncePoint.SetVisibility(NXOpen.SmartObject.VisibilityOption.Visible);
                                elements.Add(referncePoint);
                                break;
                            case Curve curve:
                                if (curve.TryGetPlane(out var plane, DistanceTolerance))        // 平面曲线转换
                                {
                                    if (CurveEncoder.ToNXCurve(curve, scaleFactor) is NXOpen.Curve crv)
                                        elements.Add(crv);
                                }
                                else
                                {
                                    var shape = curve.ToShape(scaleFactor);
                                    elements.AddRange(shape);
                                }
                                break;
                            case Brep brep:
                                BrepEncoder.ToSolid(brep);

                                //if (brep.ToSolid(scaleFactor) is DB.Solid solid)
                                //{
                                //    if (DB.FreeFormElement.Create(doc, solid) is DB.FreeFormElement freeForm)
                                //    {
                                //        elements.Add(freeForm.Id);

                                //        {
                                //            var categoryId = ImportLayer(doc, model, layer, categories, materials);
                                //            if (categoryId != DB.ElementId.InvalidElementId)
                                //                freeForm.get_Parameter(DB.BuiltInParameter.FAMILY_ELEM_SUBCATEGORY).Set(categoryId);
                                //        }

                                //        if (obj.Attributes.MaterialSource == ObjectMaterialSource.MaterialFromObject)
                                //        {
                                //            if (model.AllMaterials.FindIndex(obj.Attributes.MaterialIndex) is Material material)
                                //            {
                                //                var categoryId = ImportMaterial(doc, material, materials);
                                //                if (categoryId != DB.ElementId.InvalidElementId)
                                //                    freeForm.get_Parameter(DB.BuiltInParameter.MATERIAL_ID_PARAM).Set(categoryId);
                                //            }
                                //        }
                                //    }
                                //}
                                break;
                        }
                    }
                    catch (NXOpen.NXException ex) { }
                }
            }
            return MenuBarManager.CallbackStatus.Continue;
        }

        /// <summary>
        /// 导入 Rhino 3dm 文件
        /// </summary>
        /// <param name="buttonEvent"></param>
        /// <returns></returns>
        public static MenuBarManager.CallbackStatus ImportRhino(MenuButtonEvent buttonEvent)
        {
            try
            {
                using (var openFileDialog = new OpenFileDialog()
                {
                    Filter = "Rhino 3D models (*.3dm)|*.3dm",
                    //FilterIndex = 1,
                    RestoreDirectory = true
                })
                {
                    switch (openFileDialog.ShowDialog())
                    {
                        case DialogResult.None:
                            break;
                        case DialogResult.OK:
                            ImportCommand.Import3DMFile(openFileDialog.FileName);
                            break;
                        case DialogResult.Cancel:
                            break;
                        case DialogResult.Abort:
                            break;
                        case DialogResult.Retry:
                            break;
                        case DialogResult.Ignore:
                            break;
                        case DialogResult.Yes:
                            break;
                        case DialogResult.No:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                TheUI.NXMessageBox.Show("Block Styler", 0, ex.ToString());
            }
            finally
            {

            }
            return 0;
        }
    }
}
