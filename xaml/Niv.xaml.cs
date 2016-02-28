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

namespace Niv
{
    public partial class NivWindow : Window
    {
        
        public NivWindow()
        {
            InitializeComponent();
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            loadLanguage();
        }

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
        }
        // end of class
    }
}
