using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Extensions
{
    /// <summary>Contains enumerations representing NX object types and subtypes</summary>
    public enum ObjectType
    {
        /// <summary>Arc/circle type (NXOpen.UF.UFConstants.UF_circle_type)</summary>
        Arc = 5,
        /// <summary>Body type. Note: *any* body, solid or sheet (NXOpen.UF.UFConstants.UF_solid_type)</summary>
        Body = 70,
        /// <summary>Camera type</summary>
        Camera = 59,
        /// <summary>Arc/circle type (NXOpen.UF.UFConstants.UF_circle_type)</summary>
        // Token: 0x04000261 RID: 609
        Circle = 5,
        /// <summary>Component type (NXOpen.UF.UFConstants.UF_component_type)</summary>
        // Token: 0x04000262 RID: 610
        Component = 63,
        /// <summary>Conic type (NXOpen.UF.UFConstants.UF_conic_type)</summary>
        // Token: 0x04000263 RID: 611
        Conic = 6,
        /// <summary>Constraint type (NXOpen.UF.UFConstants.UF_constraint_type)</summary>
        // Token: 0x04000264 RID: 612
        Constraint = 160,
        /// <summary>CoordinateSystem type (NXOpen.UF.UFConstants.UF_coordinate_system_type)</summary>
        // Token: 0x04000265 RID: 613
        CoordinateSystem = 45,
        /// <summary>    DatumAxis type (not equal to NXOpen.UF.UFConstants.UF_datum_axis_type)</summary>
        /// <remarks>
        /// <para>
        /// Note that this corresponds to an NXOpen.Features.DatumAxisFeature,
        /// not to an NXOpen.DatumAxis. So, the value is not equal to
        /// NXOpen.UF.UFConstants.UF_datum_axis_type.
        /// </para>
        /// </remarks>
        // Token: 0x04000266 RID: 614
        DatumAxis = 1196,
        /// <summary>    DatumPlane type (not equal to NXOpen.UF.UFConstants.UF_datum_plane_type)</summary>
        /// <remarks>
        /// <para>
        /// Note that this corresponds to an NXOpen.Features.DatumPlaneFeature,
        /// not to an NXOpen.DatumPlane. So, the value is not equal to
        /// NXOpen.UF.UFConstants.UF_datum_plane_type.
        /// </para>
        /// </remarks>
        // Token: 0x04000267 RID: 615
        DatumPlane,
        /// <summary>Dimension type (NXOpen.UF.UFConstants.UF_dimension_type)</summary>
        // Token: 0x04000268 RID: 616
        Dimension = 26,
        /// <summary>DraftingEntity type (NXOpen.UF.UFConstants.UF_drafting_entity_type)</summary>
        // Token: 0x04000269 RID: 617
        DraftingEntity = 25,
        /// <summary>Drawing type (NXOpen.UF.UFConstants.UF_drawing_type)</summary>
        // Token: 0x0400026A RID: 618
        Drawing = 62,
        /// <summary>Edge type (no corresponding code in NXOpen.UF.UFConstants)</summary>
        // Token: 0x0400026B RID: 619
        Edge = 1002,
        /// <summary>Face type (no corresponding code in NXOpen.UF.UFConstants)</summary>
        // Token: 0x0400026C RID: 620
        Face = 1001,
        /// <summary>FacettedModel type (NXOpen.UF.UFConstants.UF_faceted_model_type)</summary>
        // Token: 0x0400026D RID: 621
        FacettedModel = 139,
        /// <summary>Feature type. (NXOpen.UF.UFConstants.UF_feature_type)</summary>
        // Token: 0x0400026E RID: 622
        Feature = 205,
        /// <summary>Group type (NXOpen.UF.UFConstants.UF_group_type)</summary>
        // Token: 0x0400026F RID: 623
        Group = 15,
        /// <summary>LayerCategory type (NXOpen.UF.UFConstants.UF_layer_category_type)</summary>
        // Token: 0x04000270 RID: 624
        LayerCategory = 12,
        /// <summary>Layout type (NXOpen.UF.UFConstants.UF_layout_type)</summary>
        // Token: 0x04000271 RID: 625
        Layout = 61,
        /// <summary>Line type (NXOpen.UF.UFConstants.UF_line_type)</summary>
        // Token: 0x04000272 RID: 626
        Line = 3,
        /// <summary>Light source type (NXOpen.UF.UFConstants.UF_line_type)</summary>
        // Token: 0x04000273 RID: 627
        LightSource = 182,
        /// <summary>Matrix type (NXOpen.UF.UFConstants.UF_matrix_type)</summary>
        // Token: 0x04000274 RID: 628
        Matrix = 55,
        /// <summary>Parameter object, used for global data (NXOpen.UF.UFConstants.UF_parameter_type)</summary>
        // Token: 0x04000275 RID: 629
        ParameterObject = 53,
        /// <summary>PartAttribute type (NXOpen.UF.UFConstants.UF_part_attribute_type)</summary>
        // Token: 0x04000276 RID: 630
        PartAttribute = 11,
        /// <summary>Pattern type (NXOpen.UF.UFConstants.UF_pattern_type)</summary>
        // Token: 0x04000277 RID: 631
        Pattern = 10,
        /// <summary>Plane type (NXOpen.UF.UFConstants.UF_plane_type)</summary>
        // Token: 0x04000278 RID: 632
        Plane = 46,
        /// <summary>Point type (NXOpen.UF.UFConstants.UF_point_type)</summary>
        // Token: 0x04000279 RID: 633
        Point = 2,
        /// <summary>ReferenceSet type (NXOpen.UF.UFConstants.UF_reference_set_type)</summary>
        // Token: 0x0400027A RID: 634
        ReferenceSet = 64,
        /// <summary>Skeleton object, used for grids, borders, etc. (NXOpen.UF.UFConstants.UF_skeleton_ent_type)</summary>
        // Token: 0x0400027B RID: 635
        Skeleton = 52,
        /// <summary>Spline type (NXOpen.UF.UFConstants.UF_spline_type)</summary>
        // Token: 0x0400027C RID: 636
        Spline = 9,
        /// <summary>TraceLine type (NXOpen.UF.UFConstants.UF_traceline_type)</summary>
        // Token: 0x0400027D RID: 637
        TraceLine = 164,
        /// <summary>View type (NXOpen.UF.UFConstants.UF_view_type)</summary>
        // Token: 0x0400027E RID: 638
        View = 60,
        /// <summary>Scalar type (NXOpen.UF.UFConstants.UF_scalar_type)</summary>
        // Token: 0x0400027F RID: 639
        Scalar = 215,
        /// <summary>Direction type (NXOpen.UF.UFConstants.UF_direction_type)</summary>
        // Token: 0x04000280 RID: 640
        Direction = 217
    }

    /// <summary>Subtypes of NX objects</summary>
    public enum ObjectSubType
    {
        /// <summary>Body subtype: General (either a sheet body or a solid body)</summary>
        BodyGeneral = 7000,

        /// <summary>Body subtype: Solid</summary>
        BodySolid = 7036,

        /// <summary>Body subtype: Sheet</summary>            
        BodySheet = 7035,

        /// <summary>Component subtype: General</summary>
        ComponentGeneral = 6300,

        /// <summary>Component subtype: Occurrence General</summary>
        PartOccurrenceGeneral,

        /// <summary>Component subtype: Occurrence Shadow</summary>
        PartOccurrenceShadow,

        /// <summary>Conic subtype: Ellipse (NXOpen.UF.UFConstants.UF_conic_ellipse_subtype)</summary>
        ConicEllipse = 602,

        /// <summary>Conic subtype: Parabola (NXOpen.UF.UFConstants.UF_conic_parabola_subtype)</summary>
        ConicParabola,

        /// <summary>Conic subtype: Hyperbola (NXOpen.UF.UFConstants.UF_conic_hyperbola_subtype)</summary>
        ConicHyperbola,

        /// <summary>CoordinateSystem subtype: General</summary>
        CsysGeneral = 4500,

        /// <summary>CoordinateSystem subtype: Wcs</summary>
        CsysWcs,

        /// <summary>CoordinateSystem subtype: Cylindrical</summary>
        CsysCylindrical,

        /// <summary>CoordinateSystem subtype: Spherical</summary>
        CsysSpherical,

        /// <summary>Dimension subtype: Horizontal</summary>
        DimensionHorizontal = 2601,

        /// <summary>Dimension subtype: Vertical</summary>
        DimensionVertical,

        /// <summary>Dimension subtype: Parallel</summary>
        DimensionParallel,

        /// <summary>Dimension subtype: Cylindrical</summary>
        DimensionCylindrical,

        /// <summary>Dimension subtype: Perpendicular</summary>
        DimensionPerpendicular,

        /// <summary>Dimension subtype: Angular Minor</summary>
        DimensionAngularMinor,

        /// <summary>Dimension subtype: Angular Major</summary>
        DimensionAngularMajor,

        /// <summary>Dimension subtype: Arc Length</summary>
        DimensionArclength,

        /// <summary>Dimension subtype: Radius</summary>
        DimensionRadius,

        /// <summary>Dimension subtype: Diameter</summary>
        // Token: 0x04000298 RID: 664
        DimensionDiameter,
        /// <summary>Dimension subtype: Hole</summary>
        // Token: 0x04000299 RID: 665
        DimensionHole,
        /// <summary>Dimension subtype: Concentric Circles</summary>
        // Token: 0x0400029A RID: 666
        DimensionConcCircle,
        /// <summary>Dimension subtype: Ordinate Horizontal</summary>
        // Token: 0x0400029B RID: 667
        DimensionOrdinateHoriz,
        /// <summary>Dimension subtype: Ordinate Vertical</summary>
        // Token: 0x0400029C RID: 668
        DimensionOrdinateVert,
        /// <summary>Dimension subtype: Assorted Parts</summary>
        // Token: 0x0400029D RID: 669
        DimensionAssortedParts,
        /// <summary>Drafting Entity subtype: Note</summary>
        // Token: 0x0400029E RID: 670
        DraftingEntityNote = 2501,
        /// <summary>Drafting Entity subtype: Label</summary>
        // Token: 0x0400029F RID: 671
        DraftingEntityLabel,
        /// <summary>Drafting Entity subtype: Id Symbol</summary>
        // Token: 0x040002A0 RID: 672
        DraftingEntityIdSymbol,
        /// <summary>Drafting Entity subtype: Fpt</summary>
        // Token: 0x040002A1 RID: 673
        DraftingEntityFpt,
        /// <summary>Drafting Entity subtype: Centerline</summary>
        // Token: 0x040002A2 RID: 674
        DraftingEntityCenterline,
        /// <summary>Drafting Entity subtype: Crosshatch</summary>
        // Token: 0x040002A3 RID: 675
        DraftingEntityCrosshatch,
        /// <summary>Drafting Entity subtype: Assorted Parts</summary>
        // Token: 0x040002A4 RID: 676
        DraftingEntityAssortedParts,
        /// <summary>Edge subtype: Line</summary>
        // Token: 0x040002A5 RID: 677
        EdgeLine = 100202,
        /// <summary>Edge subtype: Arc (or circle)</summary>
        // Token: 0x040002A6 RID: 678
        EdgeArc,
        /// <summary>Edge subtype: Arc (or circle)</summary>
        // Token: 0x040002A7 RID: 679
        EdgeCircle = 100203,
        /// <summary>Edge subtype: Ellipse</summary>
        // Token: 0x040002A8 RID: 680
        EdgeEllipse,
        /// <summary>Edge subtype: Intersection curve</summary>
        // Token: 0x040002A9 RID: 681
        EdgeIntersection,
        /// <summary>Edge subtype: Spline</summary>
        // Token: 0x040002AA RID: 682
        EdgeSpline,
        /// <summary>Edge subtype: SpCurve (parameter space curve)</summary>
        // Token: 0x040002AB RID: 683
        EdgeSpCurve,
        /// <summary>Edge subtype: IsoCurve (isoparametric curve)</summary>
        // Token: 0x040002AC RID: 684
        EdgeIsoCurve = 100209,
        /// <summary>Edge subtype: Unknown</summary>
        /// <remarks>
        /// <para>
        /// This is used to denote the type of an edge that is suppressed (because it belongs
        /// to a suppressed feature). Edges of suppressed features still exist, but they
        /// have no geometric data.
        /// </para>
        /// </remarks>
        // Token: 0x040002AD RID: 685
        EdgeUnknown,
        /// <summary>Face subtype: Plane</summary>
        // Token: 0x040002AE RID: 686
        FacePlane = 100121,
        /// <summary>Face subtype: Cylinder</summary>
        // Token: 0x040002AF RID: 687
        FaceCylinder,
        /// <summary>Face subtype: Cone</summary>
        // Token: 0x040002B0 RID: 688
        FaceCone,
        /// <summary>Face subtype: Sphere</summary>
        // Token: 0x040002B1 RID: 689
        FaceSphere,
        /// <summary>Face subtype: Torus</summary>
        // Token: 0x040002B2 RID: 690
        FaceTorus,
        /// <summary>Face subtype: Bsurface</summary>
        // Token: 0x040002B3 RID: 691
        FaceBsurface,
        /// <summary>Face subtype: Blend</summary>
        // Token: 0x040002B4 RID: 692
        FaceBlend,
        /// <summary>Face subtype: Offset</summary>
        // Token: 0x040002B5 RID: 693
        FaceOffset,
        /// <summary>Face subtype: Extruded</summary>
        // Token: 0x040002B6 RID: 694
        FaceExtruded,
        /// <summary>Face subtype: Revolved</summary>
        // Token: 0x040002B7 RID: 695
        FaceRevolved,
        /// <summary>Face subtype: Unknown</summary>
        /// <remarks>
        /// <para>
        /// This is used to denote the type of a face that is suppressed (because it belongs
        /// to a suppressed feature). Faces of suppressed features still exist, but they
        /// have no geometric data.
        /// </para>
        /// </remarks>
        FaceUnknown,
        /// <summary>Layout subtype: General</summary>
        // Token: 0x040002B9 RID: 697
        LayoutGeneral = 6100,
        /// <summary>Layout subtype: Canned</summary>
        // Token: 0x040002BA RID: 698
        LayoutCanned,
        /// <summary>Line subtype: General</summary>
        // Token: 0x040002BB RID: 699
        LineGeneral = 300,
        /// <summary>Line subtype: Infinite</summary>
        // Token: 0x040002BC RID: 700
        LineInfinite = 302,
        /// <summary>Pattern subtype: General</summary>
        // Token: 0x040002BD RID: 701
        PatternGeneral = 1000,
        /// <summary>Pattern subtype: Point</summary>
        // Token: 0x040002BE RID: 702
        PatternPoint,
        /// <summary>Part Attribute subtype: General</summary>
        // Token: 0x040002BF RID: 703
        PartAttributeGeneral = 1100,
        /// <summary>Part Attribute subtype: Cache</summary>
        // Token: 0x040002C0 RID: 704
        PartAttributeCache,
        /// <summary>ReferenceSet subtype: Design</summary>
        // Token: 0x040002C1 RID: 705
        ReferenceDesign = 6400,
        /// <summary>View Subtype : Section</summary>
        // Token: 0x040002C2 RID: 706
        ViewSection = 6000,
        /// <summary>View Subtype : Instance</summary>
        // Token: 0x040002C3 RID: 707
        ViewInstance,
        /// <summary>View Subtype : Imported</summary>
        // Token: 0x040002C4 RID: 708
        ViewImported,
        /// <summary>View Subtype : Base Member</summary>
        // Token: 0x040002C5 RID: 709
        ViewBaseMember,
        /// <summary>View Subtype : Orthographic</summary>
        // Token: 0x040002C6 RID: 710
        ViewOrthographic,
        /// <summary>View Subtype : Auxiliary</summary>
        // Token: 0x040002C7 RID: 711
        ViewAuxiliary,
        /// <summary>View Subtype : Detail</summary>
        // Token: 0x040002C8 RID: 712
        ViewDetail,
        /// <summary>View Subtype : Modeling</summary>
        // Token: 0x040002C9 RID: 713
        ViewModeling
    }

    /// <summary>A combination of a type and a subtype</summary>
    /// <remarks>
    /// <para>
    /// TypeCombo objects are used to control filtering in Selection. 
    /// Specifically, you can use TypeCombo objects as input to the
    /// <see cref="M:Snap.UI.Block.SelectObject.SetFilter(Snap.NX.ObjectTypes.TypeCombo[])">SetFilter</see> functions.
    /// </para>
    /// </remarks>
    /// <seealso cref="M:Snap.UI.Block.SelectObject.SetFilter(Snap.NX.ObjectTypes.TypeCombo[])">Snap.UI.Block.SelectObject.SetFilter</seealso>
    public struct ObjectTypes
    {
        /// <summary>Constructs a new TypeCombo from a given type and subtype</summary>
        /// <param name="type">The object type</param>
        /// <param name="subtype">The object subtype</param>
        public ObjectTypes(ObjectType type, ObjectSubType subtype)
        {
            Type = type;
            SubType = subtype;
        }

        /// <summary>Constructs a new TypeCombo from a given type</summary>
        /// <param name="type">The object type (subtype is set to zero)</param>
        public ObjectTypes(ObjectType type)
        {
            Type = type;
            SubType = 0;
        }

        /// <summary>Object type (e.g. "conic")</summary>
        public ObjectType Type;

        /// <summary>Object subtype (e.g. "ellipse")</summary>
        public ObjectSubType SubType;
    }
}