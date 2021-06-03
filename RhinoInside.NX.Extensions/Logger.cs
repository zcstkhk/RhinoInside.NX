using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen.UF;
using static RhinoInside.NX.Extensions.Globals;

namespace RhinoInside.NX.Extensions
{
    public static class Logger
    {
        public static void Info(string info)
        {
            TheUfSession.UF.PrintSyslog($"【信息】 {DateTime.Now.ToString("yyyy-MM-dd-HH-mm")} {info}", false);
        }

        public static void Error(string error)
        {
            TheUfSession.UF.PrintSyslog($"【错误】 {DateTime.Now.ToString("yyyy-MM-dd-HH-mm")} {error}", false);
        }

        public static void Fatal(string info)
        {
            TheUfSession.UF.PrintSyslog($"【严重】 {DateTime.Now.ToString("yyyy-MM-dd-HH-mm")} {info}", false);
        }

        public static void Warn(string warn)
        {
            TheUfSession.UF.PrintSyslog($"【警告】 {DateTime.Now.ToString("yyyy-MM-dd-HH-mm")} {warn}", false);
        }
    }
}
