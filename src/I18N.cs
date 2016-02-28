using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Resources;
using System.Reflection;
using System.Windows;

namespace Niv
{
    public class I18N
    {
        private static string cultureCode;
        private static Dictionary<string, Dictionary<string, string>> langData = new Dictionary<string, Dictionary<string, string>>();

        static I18N()
        {
            loadLangData();

            cultureCode = Thread.CurrentThread.CurrentCulture.Name;
            if (!langData.ContainsKey(cultureCode)) cultureCode = "en-US";

            // cultureCode = "zh-TW";  // test none-english
            // cultureCode = "zh-TW2";  // test not exist

            try
            {
                new CultureInfo(cultureCode, true);
            }
            catch (Exception ex)
            {
                cultureCode = "en-US";
            }
        }

        private static void loadLangData()
        {
            // Add your language here
            langData.Add("en-US", new Dictionary<string, string>());
            langData.Add("zh-CN", new Dictionary<string, string>());
            langData.Add("zh-TW", new Dictionary<string, string>());

            langData["en-US"]["appName"] = "Niv";
            langData["zh-CN"]["appName"] = "小牛看图";
            langData["zh-TW"]["appName"] = "小牛看圖";

            langData["en-US"]["_missing"] = "[DATA MISSING]";
            langData["zh-CN"]["_missing"] = "【数据丢失】";
            langData["zh-TW"]["_missing"] = "【數據丟失】";
        }

        public static string _(string key)
        {
            try
            {
                return langData[cultureCode][key];
            }
            catch (MissingManifestResourceException ex)
            {
                return _("_missing");
            }
        }
        // end of class
    }
}