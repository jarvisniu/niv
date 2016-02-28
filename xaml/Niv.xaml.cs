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

using com.jarvisniu;

namespace Niv
{
    public partial class NivWindow : Window
    {
        // components
        private About aboutWindow = new About();
        private AnimatorJar animationjar = new AnimatorJar();
        private ButtonAnimator buttonAnimator = new ButtonAnimator();

        // layout config
        static double WINDOW_MIN_WIDTH = 480;
        static double WINDOW_MIN_HEIGHT = 320;
        public static int SEPARATOR_HEIGHT = 2;
        //public static int MESSAGE_BOX_HEIGHT = 48;
        public static int MARGIN_SIZE = 50;
        //static int DB_CLICK_THRESH = 300;
        //static int AA_SIZE_THRESHHOLD = 257;
        //static int AA_SCALE_THRESHHOLD = 2;
        //static double PROGRESS_CAP = 2;
        static double INFO_WIDTH = 260;

        private string theme = "light";

        #region init and exit

        public NivWindow()
        {
            InitializeComponent();
            loadLanguage();
            setTheme(theme);
            initLayout();
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            exit();
        }

        private void loadLanguage()
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

        private void initLayout()
        {
            window.MinWidth = WINDOW_MIN_WIDTH;
            window.MinHeight = WINDOW_MIN_HEIGHT;
            exitButton.Visibility = System.Windows.Visibility.Hidden;
            container.Margin = new Thickness(-MARGIN_SIZE, -MARGIN_SIZE, -MARGIN_SIZE, 0);
            separator.Margin = new Thickness(0, 0, 0, MARGIN_SIZE - SEPARATOR_HEIGHT);
            progress.Margin = new Thickness(0, 0, 0, separator.Margin.Bottom - 1);
            toolbar.Margin = new Thickness(0, 0, 0, 0);
            info.Margin = new Thickness(-INFO_WIDTH, 0, 0, 0);
            info.Width = INFO_WIDTH;
            page.Margin = new Thickness(-1, -1, MARGIN_SIZE + 8, MARGIN_SIZE + 8);
            page.Opacity = 0;
            menu.Margin = new Thickness(0, 0, 0, MARGIN_SIZE);
            menu.Height = 0;

            // add animation effects to buttons
            buttonAnimator.apply(btnZoom).apply(btnPrev).apply(btnNext).apply(btnAA).apply(btnMenu).apply(exitButton)
                .apply(btnDelete).apply(btnRotateLeft).apply(btnRotateRight).apply(menuAbout)
                .apply(menuHelp).apply(menuInfo).apply(btnCloseInfo);
        }

        private void exit()
        {
            //restoreRotation();
            //recycleBin.clean();
            aboutWindow.exit();
            Application.Current.Shutdown();
        }

        #endregion

        #region event handlers

        #region toolbar buttons

        private void btnRotateLeft_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnRotateRight_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnDelete_MouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void btnPrev_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnNext_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnAA_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnZoom_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnMenu_MouseUp(object sender, MouseButtonEventArgs e)
        {
            theme = theme == "dark" ? "light" : "dark";
            setTheme(theme);
        }

        #endregion

        #region menu
        private void menuAbout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            showWindowAbout();
            //hideMainMenu();
        }

        #endregion

        #endregion

        #region life cycle
        
        private void setTheme(string theme)
        {
            buttonAnimator.setTheme(theme);

            if (theme == "light")
            {
                container.Background = new SolidColorBrush(Color.FromArgb(255, 250, 250, 250));
                toolbar.Background = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
                info.Background = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
                separator.Background = progress.Stroke = new SolidColorBrush(Color.FromArgb(255, 170, 170, 170));
                progress.Fill = new SolidColorBrush(Color.FromArgb(255, 222, 222, 222));
            }
            else
            {
                container.Background = new SolidColorBrush(Color.FromArgb(255, 64, 64, 64));
                toolbar.Background = new SolidColorBrush(Color.FromArgb(192, 32, 32, 32));
                info.Background = new SolidColorBrush(Color.FromArgb(255, 64, 64, 64));
                separator.Background = progress.Stroke = new SolidColorBrush(Color.FromArgb(192, 64, 64, 64));
                progress.Fill = new SolidColorBrush(Color.FromArgb(75, 255, 255, 255));
            }

            imageRotateLeft.Source = loadResourceBitmap("icon-rotate-left.png", theme);
            imageRotateRight.Source = loadResourceBitmap("icon-rotate-right.png", theme);
            imageDelete.Source = loadResourceBitmap("icon-delete.png", theme);
            imagePrev.Source = loadResourceBitmap("icon-prev.png", theme);
            imageNext.Source = loadResourceBitmap("icon-next.png", theme);
            imageSmooth.Source = loadResourceBitmap("icon-smooth-off.png", theme);
            imageZoom.Source = loadResourceBitmap("icon-zoom-fit.png", theme);
            imageMenu.Source = loadResourceBitmap("icon-menu.png", theme);
        }

        #endregion

        #region utils
        private BitmapImage loadResourceBitmap(string filename, string theme)
        {
            string namespaceName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            return new BitmapImage(new Uri("pack://application:,,,/" + namespaceName + ";component/res/theme-" + theme + "/" + filename));
        }

        #endregion

        #region ui toogle

        // window About
        private void showWindowAbout()
        {
            aboutWindow.Show();
        }

        #endregion

        // end of class
    }
}
