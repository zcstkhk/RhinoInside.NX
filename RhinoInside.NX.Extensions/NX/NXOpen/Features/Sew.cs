using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Features;
using static RhinoInside.NX.Extensions.Globals;

namespace RhinoInside.NX.Extensions
{
    public static partial class FeatureCollectionEx
    {
        /// <summary>
        /// 缝合体
        /// </summary>
        /// <param name="features"></param>
        /// <param name="bodiesToSew">要缝合的体，第一个体将被作为目标体，剩余体用作工具体</param>
        public static Sew CreateSew(this FeatureCollection features, Body[] bodiesToSew)
        {
            NXOpen.Features.SewBuilder sewBuilder = WorkPart.Features.CreateSewBuilder(null);

            sewBuilder.Tolerance = 0.01;

            bool added1 = sewBuilder.TargetBodies.Add(bodiesToSew[0]);

            List<Body> toolBodies = bodiesToSew.ToList();

            toolBodies.RemoveAt(0);

            bool added2 = sewBuilder.ToolBodies.Add(toolBodies.ToArray());

            Sew nXObject1 = (Sew)sewBuilder.Commit();

            sewBuilder.Destroy();

            return nXObject1;
        }

    }
}