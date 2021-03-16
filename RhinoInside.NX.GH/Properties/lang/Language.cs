using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.GH.Properties
{
    public static class Languages
    {
        static System.Resources.ResourceManager _resourceManager;
        static System.Resources.ResourceManager _defaultResourceMan;
        static Languages()
        {
            string resourceName = "RhinoInside.NX.GH.Properties.lang.";
            switch (Core.NX.CurrentLanguage)
            {
                case Core.Language.Simple_Chinese:
                    resourceName += "zh-CN";
                    break;
                case Core.Language.English:
                    resourceName += "en-US";
                    break;
                case Core.Language.French:
                    resourceName += "fr-FR";
                    break;
                case Core.Language.German:
                    resourceName += "de-DE";
                    break;
                case Core.Language.Japanese:
                    resourceName += "ja-JP";
                    break;
                case Core.Language.Italian:
                    resourceName += "it-IT";
                    break;
                case Core.Language.Russian:
                    resourceName += "ru-RU";
                    break;
                case Core.Language.Korean:
                    resourceName += "ko-KR";
                    break;
                case Core.Language.Trad_Chinese:
                    resourceName += "zh-HK";
                    break;
                default:
                    break;
            }

            _defaultResourceMan = new System.Resources.ResourceManager("RhinoInside.NX.GH.Properties.lang.zh-CN", Assembly.GetExecutingAssembly());

            if (Assembly.GetExecutingAssembly().GetManifestResourceNames().Contains(resourceName + ".resources"))
                _resourceManager = new System.Resources.ResourceManager(resourceName, Assembly.GetExecutingAssembly());
            else
                _resourceManager = _defaultResourceMan;
        }

        public static string GetString(string name)
        {
            var result = _resourceManager.GetString(name);
            if (result == null)
                return _defaultResourceMan.GetString(name);
            else
                return result;
        }
    }
}
