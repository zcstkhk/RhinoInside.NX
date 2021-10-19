using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen.UF;
using static NXOpen.Extensions.Globals;

namespace RhinoInside.NX.Extensions
{
    public static class Logger
    {
        public static void Info(string info)
        {
            Console.WriteLine($"【信息】 {DateTime.Now.ToString("yyyy-MM-dd-HH-mm")} {info}");
            TheUfSession.UF.PrintSyslog($"【信息】 {DateTime.Now.ToString("yyyy-MM-dd-HH-mm")} {info}\n", false);
        }

        public static void Error(string error)
        {
            Console.WriteLine($"【错误】 {DateTime.Now.ToString("yyyy-MM-dd-HH-mm")} {error}");
            TheUfSession.UF.PrintSyslog($"【错误】 {DateTime.Now.ToString("yyyy-MM-dd-HH-mm")} {error}\n", false);
        }

        public static void Fatal(string info)
        {
            Console.WriteLine($"【严重】 {DateTime.Now.ToString("yyyy-MM-dd-HH-mm")} {info}");
            TheUfSession.UF.PrintSyslog($"【严重】 {DateTime.Now.ToString("yyyy-MM-dd-HH-mm")} {info}\n", false);
        }

        public static void Warn(string warn)
        {
            Console.WriteLine($"【警告】 {DateTime.Now.ToString("yyyy-MM-dd-HH-mm")} {warn}");
            TheUfSession.UF.PrintSyslog($"【警告】 {DateTime.Now.ToString("yyyy-MM-dd-HH-mm")} {warn}\n", false);
        }
    }
}
