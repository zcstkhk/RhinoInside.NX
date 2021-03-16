using NXOpen.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
	public static partial class FeatureCollectionEx
	{
		/// <summary>
		/// 移除对象参数
		/// </summary>
		/// <param name="features"></param>
		/// <param name="objectsToRemoveParameter"></param>
		public static void RemoveParameters(this FeatureCollection features, NXObject[] objectsToRemoveParameter)
		{
			NXOpen.Features.RemoveParametersBuilder removeParametersBuilder = features.CreateRemoveParametersBuilder();

			bool added1 = removeParametersBuilder.Objects.Add(objectsToRemoveParameter);

			removeParametersBuilder.Commit();

			removeParametersBuilder.Destroy();
		}

		/// <summary>
		/// 移除对象参数
		/// </summary>
		/// <param name="features"></param>
		/// <param name="objectToRemoveParameter"></param>
		public static void RemoveParameters(this FeatureCollection features, NXObject objectToRemoveParameter) => RemoveParameters(features, new NXObject[] { objectToRemoveParameter });
	}
}
