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

            langData["en-US"]["_missing"] = "[DATA MISSING]";
            langData["zh-CN"]["_missing"] = "【数据丢失】";
            langData["zh-TW"]["_missing"] = "【數據丟失】";

            langData["en-US"]["appName"] = "Niv";
            langData["zh-CN"]["appName"] = "小牛看图";
            langData["zh-TW"]["appName"] = "小牛看圖";

            langData["en-US"]["imageInfo"] = "Image Info";
            langData["zh-CN"]["imageInfo"] = "图片信息";
            langData["zh-TW"]["imageInfo"] = "圖片信息";

            langData["en-US"]["filename"] = "Filename";
            langData["zh-CN"]["filename"] = "文件名";
            langData["zh-TW"]["filename"] = "文件名";

            langData["en-US"]["size"] = "Size";
            langData["zh-CN"]["size"] = "大小";
            langData["zh-TW"]["size"] = "大小";

            langData["en-US"]["resolution"] = "Resolution";
            langData["zh-CN"]["resolution"] = "分辨率";
            langData["zh-TW"]["resolution"] = "解析度";

            langData["en-US"]["date"] = "Date";
            langData["zh-CN"]["date"] = "日期";
            langData["zh-TW"]["date"] = "日期";

            langData["en-US"]["about"] = "About";
            langData["zh-CN"]["about"] = "关于";
            langData["zh-TW"]["about"] = "關於";

            langData["en-US"]["help"] = "Help";
            langData["zh-CN"]["help"] = "帮助";
            langData["zh-TW"]["help"] = "説明";
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