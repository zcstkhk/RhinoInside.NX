using NXOpen.Features;
using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace RhinoInside.NX.Extensions
{
	/// <summary>
	/// 用于创建特征
	/// </summary>
	public static partial class FeatureCollectionEx
	{
		public static Session theSession;
		public static UFSession theUfSession;
		private static Part _workPart;
		private static bool TCIntegrated;

		static FeatureCollectionEx()
		{
			theSession = Session.GetSession();
			theUfSession = UFSession.GetUFSession();
			_workPart = theSession.Parts.Work;
			theUfSession.UF.IsUgmanagerActive(out TCIntegrated);
		}
	}
}
