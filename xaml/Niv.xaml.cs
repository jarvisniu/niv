using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;

namespace Niv
{
    public partial class NivWindow : Window
    {
        private About aboutWindow = new About();

        public NivWindow()
        {
            InitializeComponent();
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            loadLanguage();
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            exit();
        }

        #region event hadnler


        private void menuAbout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            showWindowAbout();
            //hideMainMenu();
        }

        #endregion

        #region life cycle

        public void loadLanguage()
        {
            window.Title = I18N._("appName");
            iImageInfo.Content = I18N._("imageInfo");
            iFilename.Content = I18N._("filename");
            iSize.Content = I18N._("size");
            iResolution.Content = I18N._("resolution");
            iDate.Content = I18N._("date");
            iHelp.Content = I18N._("help");
            iAbout.Content = I18N._("about");
            iImageInfoMenu.Content = I18N._("imageInfo");
            // aboutWindow
            aboutWindow.Title = I18N._("about");
            aboutWindow.iAppName.Content = I18N._("appName");
            aboutWindow.iDescription.Content = I18N._("description");
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            string versionString = version.Major + "." + version.Minor + "." + version.Build;
            aboutWindow.iVersion.Content = I18N._("version") + ": " + versionString;
            aboutWindow.iAuthor.Content = I18N._("author") + ": " + I18N._("jarvisNiu");
            aboutWindow.iOfficialWebsite.Text = I18N._("officialWebsite") + ": ";
        }

        private void exit()
        {
            //restoreRotation();
            //recycleBin.clean();
            aboutWindow.exit();
            Application.Current.Shutdown();
        }

        #endregion

        #region toogle
        
        // window About
        private void showWindowAbout()
        {
            aboutWindow.Show();
        }

        #endregion

        // end of class
    }
}
