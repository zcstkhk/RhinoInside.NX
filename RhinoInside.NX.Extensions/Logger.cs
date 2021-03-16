using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen.UF;

namespace RhinoInside.NX.Extensions
{
    public static class Logger
    {
        static UFSession _theUfSession;
        static Logger()
        {
            _theUfSession = UFSession.GetUFSession();
        }

        public static void Info(string info)
        {
            _theUfSession.UF.PrintSyslog($"【信息】 {DateTime.Now.ToString("yyyy-MM-dd-HH-mm")} {info}", false);
        }

        public static void Error(string error)
        {
            _theUfSession.UF.PrintSyslog($"【错误】 {DateTime.Now.ToString("yyyy-MM-dd-HH-mm")} {error}", false);
        }

        public static void Fatal(string info)
        {
            _theUfSession.UF.PrintSyslog($"【严重】 {DateTime.Now.ToString("yyyy-MM-dd-HH-mm")} {info}", false);
        }

        public static void Warn(string warn)
        {
            _theUfSession.UF.PrintSyslog($"【警告】 {DateTime.Now.ToString("yyyy-MM-dd-HH-mm")} {warn}", false);
        }
    }
}
