/**
 * I18n - Multi-language support component
 * Jarvis Niu(牛俊为) - http://jarvisniu.com/
 * MIT Licence
 * 
 * ## Usage
 *    1. Set the language data in `loadLanguageData()`.
 *    2. Get the translated string of string key:
 *        window.Title = I18n._("key");
 *    
 *    This library will automaticly detect the user's language
 *    and fall back to the default if the user's is not supported.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Resources;
using System.Reflection;
using System.Windows;

namespace com.jarvisniu
{
    public class I18n
    {
        // The default code of this program
        private static string DEFAULT_CULTURE_CODE = "en-US";

        // The culture-code of current language
        private static string cultureCode;

        // Language data: keys and each translations
        private static Dictionary<string, Dictionary<string, string>> langData = new Dictionary<string, Dictionary<string, string>>();

        // Constructor
        static I18n()
        {
            // Load the language data
            loadLanguageData();

            // Detect the user's default culture code
            cultureCode = Thread.CurrentThread.CurrentCulture.Name;

            // cultureCode = "en-US";  // test default code
            // cultureCode = "zh-TW";  // test avialable non-default code
            // cultureCode = "zh-HK";  // test not avialable code

            // If the user's culture code is not supported, use the default.
            if (!langData.ContainsKey(cultureCode)) cultureCode = DEFAULT_CULTURE_CODE;
        }

        // Get the string to the key
        public static string _(string key)
        {
            try
            {
                return langData[cultureCode][key];
            }
            catch (Exception ex)
            {
                MessageBox.Show("I18N: langData[" + key + "] not exists");
                return _("_missing");
            }
        }

        // Load the language data
        private static void loadLanguageData()
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

            langData["en-US"]["setting"] = "Switch Themes";  // Setting
            langData["zh-CN"]["setting"] = "切换主题";  // 设置
            langData["zh-TW"]["setting"] = "切換主題";  // 設置

            langData["en-US"]["version"] = "Version";
            langData["zh-CN"]["version"] = "版本";
            langData["zh-TW"]["version"] = "版本";

            langData["en-US"]["author"] = "Author";
            langData["zh-CN"]["author"] = "作者";
            langData["zh-TW"]["author"] = "作者";

            langData["en-US"]["jarvisNiu"] = "Jarvis Niu";
            langData["zh-CN"]["jarvisNiu"] = "牛俊为";
            langData["zh-TW"]["jarvisNiu"] = "牛俊為";

            langData["en-US"]["description"] = "A compact fast image viewer";
            langData["zh-CN"]["description"] = "一个简约快速的看图软件";
            langData["zh-TW"]["description"] = "一個簡約快速的看圖軟體";

            langData["en-US"]["officialWebsite"] = "Official Website";
            langData["zh-CN"]["officialWebsite"] = "官方网站";
            langData["zh-TW"]["officialWebsite"] = "官方網站";
        }
        
        // EOC
    }
}
