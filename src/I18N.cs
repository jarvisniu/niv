using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Resources;
using System.Reflection;

namespace Niv
{
    public class I18N
    {
        private static ResourceManager resManager;

        // Supported languages: "zh-CN", "zh-TW" and "en-US", default is "en-US"
        private static string currLang;
        private static CultureInfo cultureInfo;

        static I18N()
        {
            currLang = Thread.CurrentThread.CurrentCulture.Name;
            if (currLang != "zh-CN" && currLang != "zh-TW") currLang = "en-US";
            // currLang = "zh-TW";

            cultureInfo = new CultureInfo(currLang, true);
            resManager = new ResourceManager("Niv.resx.Languages", Assembly.GetExecutingAssembly());
        }

        public static string _(string key)
        {
            try
            {
                return resManager.GetString(key, cultureInfo);
            }
            catch (MissingManifestResourceException ex)
            {
                return _("_missing");
            }
        }
        // end of class
    }
}