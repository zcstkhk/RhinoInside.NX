using NXOpen.Features;
using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Extensions
{
    public static partial class Globals
    {
        /// <summary>
        /// 软件根目录
        /// </summary>
        public static string RootPath
        {
            get => (new System.IO.DirectoryInfo(System.Reflection.Assembly.GetExecutingAssembly().Location)).Parent.Parent.FullName.ToString();
        }

        /// <summary>
        /// 软件应用程序目录
        /// </summary>
        public static string ApplicationPath
        {
            get => System.IO.Path.Combine(RootPath, "Application");
        }

        /// <summary>
        /// 图片文件目录
        /// </summary>
        public static string BitmapPath
        {
            get => System.IO.Path.Combine(RootPath, "Bitmap");
        }

        /// <summary>
        /// 配置文件目录
        /// </summary>
        public static string ConfigPath
        {
            get => System.IO.Path.Combine(RootPath, "Config");
        }

        /// <summary>
        /// 配置文件目录
        /// </summary>
        public static string StartupPath
        {
            get => System.IO.Path.Combine(RootPath, "Startup");
        }

        /// <summary>
        /// 模板文件目录
        /// </summary>
        public static string TemplatePath
        {
            get => System.IO.Path.Combine(RootPath, "Template");
        }
    }
}