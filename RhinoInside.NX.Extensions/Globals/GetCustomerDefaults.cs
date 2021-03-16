using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NXOpen.Extensions
{
	/// <summary>
	/// 用于创建特征
	/// </summary>
	public static partial class Globals
	{
		/// <summary>
		/// 获取用户默认设置中的选项值
		/// </summary>
		/// <param name="optionName">模块 ID 以及选项名称，此参数可以通过先修改用户默认设置中的对应值，然后在用户目录中查看修改后的用户默认设置文件，其中的 name 项即为所需的参数。</param>
		/// <returns></returns>
		public static string GetCustomerDefaults(string optionName)
		{
			TheUfSession.UF.GetCustomerDefault(optionName, 0, out string defaultValue);

			return defaultValue;
		}
	}
}
