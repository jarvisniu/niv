using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
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
        private AnimatorJar animatorJar = new AnimatorJar();
        private ButtonAnimator buttonAnimator = new ButtonAnimator();

        // delayed closing timers
        Timer timerClosePage;

        // layout config
        static double WINDOW_MIN_WIDTH = 480;
        static double WINDOW_MIN_HEIGHT = 360;
        public static int SEPARATOR_HEIGHT = 2;
        //public static int MESSAGE_BOX_HEIGHT = 48;
        public static int MARGIN_SIZE = 50;
        //static int DB_CLICK_THRESH = 300;
        //static int AA_SIZE_THRESHHOLD = 257;
        //static int AA_SCALE_THRESHHOLD = 2;
        //static double PROGRESS_CAP = 2;
        static double INFO_WIDTH = 260;
        static double MENU_HEIGHT =
            40 * 3 + // menu_item
            2 * 1 +  // top_border
            17 * 1 + // seprate_line
            8 * 2;   // vertical_margin

        private string theme = "light";
        private Dictionary<FrameworkElement, bool> visibleStates = new Dictionary<FrameworkElement, bool>();
        private bool isMarginBottomExist = true;
        private bool isFullscreen = false;
        private bool isAutoHideToolbar = false;
        private WindowState lastWindowState = WindowState.Maximized;

        private double gridHeight = 0;
        private Point mousePos;

        #region init and exit

        public NivWindow()
        {
            InitializeComponent();
            loadLanguage();
            setTheme(theme);
            initLayout();
            initComponents();
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
            window.Title = I18n._("appName");
            iImageInfoTitle.Content = I18n._("imageInfo");
            iFilename.Content = I18n._("filename");
            iSize.Content = I18n._("size");
            iResolution.Content = I18n._("resolution");
            iDate.Content = I18n._("date");
            iHelp.Content = I18n._("help");
            iAbout.Content = I18n._("about");
            iImageInfo.Content = I18n._("imageInfo");
            // aboutWindow
            aboutWindow.Title = I18n._("about");
            aboutWindow.iAppName.Content = I18n._("appName");
            aboutWindow.iDescription.Content = I18n._("description");
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            string versionString = version.Major + "." + version.Minor + "." + version.Build;
            aboutWindow.iVersion.Content = I18n._("version") + ": " + versionString;
            aboutWindow.iAuthor.Content = I18n._("author") + ": " + I18n._("jarvisNiu");
            aboutWindow.iOfficialWebsite.Text = I18n._("officialWebsite") + ": ";
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

            visibleStates[exitButton] = false;
            visibleStates[container] = false;
            visibleStates[toolbar] = true;
            visibleStates[info] = false;
            visibleStates[page] = false;
            visibleStates[menu] = false;

            // add animation effects to buttons
            buttonAnimator.apply(btnZoom).apply(btnPrev).apply(btnNext).apply(btnSmooth).apply(btnMenu).apply(exitButton)
                .apply(btnDelete).apply(btnRotateLeft).apply(btnRotateRight).apply(menuAbout)
                .apply(menuHelp).apply(menuInfo).apply(btnCloseInfo);
        }

        private void initComponents()
        {
            timerClosePage = new Timer(3000);
            timerClosePage.Elapsed += timerClosePage_Elapsed;
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
            showPage();
        }

        private void btnPrev_MouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void btnNext_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnSmooth_MouseUp(object sender, MouseButtonEventArgs e)
        {
            theme = theme == "dark" ? "light" : "dark";
            setTheme(theme);
        }

        private void btnZoom_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnMenu_MouseUp(object sender, MouseButtonEventArgs e)
        {
            toggleMainMenu();
        }

        #endregion

        #region menu

        private void menuAbout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            aboutWindow.Show();
            hideMainMenu();
        }

        private void menuInfo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            toggleInfo();
            hideMainMenu();
        }

        #endregion

        #region other

        private void btnCloseInfo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            hideInfo();
        }

        #endregion

        #endregion

        #region utils

        private void setTheme(string theme)
        {
            buttonAnimator.setTheme(theme);

            if (theme == "light")
            {
                container.Background = grayBrush(250);
                toolbar.Background = menu.Background = iImageInfoTitle.Background = grayBrush(220);
                info.Background = grayBrush(235);
                separator.Background = progress.Stroke = menu.BorderBrush = menuLine.Stroke = grayBrush(170);
                progress.Fill = grayBrush(222);
                iHelp.Foreground = iAbout.Foreground = iImageInfo.Foreground = iImageInfoTitle.Foreground = iPage.Foreground
                    = iFilename.Foreground = iSize.Foreground = iResolution.Foreground = iDate.Foreground = grayBrush(48);
                labelInfoFilename.Foreground = labelInfoSize.Foreground = labelInfoResolution.Foreground
                    = labelInfoDate.Foreground = grayBrush(0);
                page.Background = grayBrush(255, 0.75);
                page.BorderBrush = grayBrush(128, 0.75);
                
            }
            else if (theme == "dark")
            {
                container.Background = grayBrush(32);
                toolbar.Background = menu.Background = grayBrush(32, 0.75);
                info.Background = iImageInfoTitle.Background = grayBrush(64);
                separator.Background = progress.Stroke = menu.BorderBrush = menuLine.Stroke = iFilename.Foreground
                    = iSize.Foreground = iResolution.Foreground = iDate.Foreground = grayBrush(64, 0.75);
                progress.Fill = grayBrush(255, 0.3);
                iHelp.Foreground = iAbout.Foreground = iImageInfo.Foreground = iImageInfoTitle.Foreground = iPage.Foreground
                    = iFilename.Foreground = iSize.Foreground = iResolution.Foreground = iDate.Foreground = grayBrush(192);
                labelInfoFilename.Foreground = labelInfoSize.Foreground = labelInfoResolution.Foreground
                    = labelInfoDate.Foreground = grayBrush(255);
                page.Background = grayBrush(64, 0.75);
                page.BorderBrush = grayBrush(128, 0.75);
            }
            else MessageBox.Show("Not supported theme: " + theme);

            // button images
            imageRotateLeft.Source = loadResourceBitmap("icon-rotate-left.png", theme);
            imageRotateRight.Source = loadResourceBitmap("icon-rotate-right.png", theme);
            imageDelete.Source = loadResourceBitmap("icon-delete.png", theme);
            imagePrev.Source = loadResourceBitmap("icon-prev.png", theme);
            imageNext.Source = loadResourceBitmap("icon-next.png", theme);
            imageSmooth.Source = loadResourceBitmap("icon-smooth-off.png", theme);
            imageZoom.Source = loadResourceBitmap("icon-zoom-fit.png", theme);
            imageMenu.Source = loadResourceBitmap("icon-menu.png", theme);
            imageCloseInfo.Source = loadResourceBitmap("icon-close.png", theme);
            // menu images
            imageHelp.Source = loadResourceBitmap("icon-about.png", theme);
            imageAbout.Source = loadResourceBitmap("icon-about.png", theme);
            imageInfo.Source = loadResourceBitmap("icon-list.png", theme);
        }

        private BitmapImage loadResourceBitmap(string filename, string theme)
        {
            string namespaceName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            return new BitmapImage(new Uri("pack://application:,,,/" + namespaceName + ";component/res/theme-" + theme + "/" + filename));
        }

        private SolidColorBrush grayBrush(byte brightness)
        {
            return new SolidColorBrush(Color.FromArgb(255, brightness, brightness, brightness));
        }

        private SolidColorBrush grayBrush(byte brightness, double alpha)
        {
            return new SolidColorBrush(Color.FromArgb(Convert.ToByte(255 * alpha), brightness, brightness, brightness));
        }

        #endregion

        #region ui toogle

        // page
        private void showPage()
        {
            visibleStates[page] = true;
            animatorJar.fadeIn(page);
            timerClosePage.Stop();
            timerClosePage.Start();
        }
        private void hidePage()
        {
            visibleStates[page] = false;
            animatorJar.fadeOut(page);
        }
        void timerClosePage_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(delegate {
                if (visibleStates[page]) hidePage();
            }));
            timerClosePage.Stop();
        }


        // info panel
        private void hideInfo()
        {
            if (visibleStates[info]) toggleInfo();
        }
        private void toggleInfo()
        {
            visibleStates[info] = !visibleStates[info];
            if (visibleStates[info])
            {
                this.MinWidth = WINDOW_MIN_WIDTH + INFO_WIDTH;
                animatorJar.translateLeftTo(container, INFO_WIDTH - MARGIN_SIZE);
                animatorJar.translateLeftTo(info, 0);
            }
            else
            {
                this.MinWidth = WINDOW_MIN_WIDTH;
                animatorJar.translateLeftTo(container, -MARGIN_SIZE);
                animatorJar.translateLeftTo(info, -INFO_WIDTH);
            }
        }

        // toolbar
        private void toggleToolbar()
        {
            if (visibleStates[toolbar])
                hideToolbar();
            else
                showToolbar();
        }
        private void showToolbar()
        {
            visibleStates[toolbar] = true;
            animatorJar.marginBottomTo(toolbar, MARGIN_SIZE)
                .marginBottomTo(separator, 48 + MARGIN_SIZE)
                .marginBottomTo(progress, 48 + MARGIN_SIZE - 1)
                .fadeIn(separator).fadeIn(progress);
        }
        private void hideToolbar()
        {
            visibleStates[toolbar] = false;

            animatorJar.marginBottomTo(toolbar, MARGIN_SIZE - 48)
                .marginBottomTo(separator, 0 + MARGIN_SIZE)
                .marginBottomTo(progress, 0 + MARGIN_SIZE - 1)
                .fadeOut(separator).fadeOut(progress);
        }

        // margion bottom
        public void toggleMarginBottom()
        {
            if (isMarginBottomExist)
            {
                hideMarginBottom();
            }
            else
            {
                showMarginBottom();
            }
        }
        public void showMarginBottom()
        {
            isMarginBottomExist = true;

            animatorJar.marginTo(container, new Thickness(-MARGIN_SIZE + (visibleStates[info] ? INFO_WIDTH : 0), -MARGIN_SIZE, -MARGIN_SIZE, 0));
            animatorJar.marginBottomTo(separator, MARGIN_SIZE - SEPARATOR_HEIGHT);
            animatorJar.marginBottomTo(progress, MARGIN_SIZE - SEPARATOR_HEIGHT - 1);
            animatorJar.marginBottomTo(toolbar, 0);

            page.Margin = new Thickness(-1, -1, MARGIN_SIZE + 8, MARGIN_SIZE + 8);
        }
        public void hideMarginBottom()
        {
            isMarginBottomExist = false;

            animatorJar.marginTo(container, new Thickness(-MARGIN_SIZE + (visibleStates[info] ? INFO_WIDTH : 0), -MARGIN_SIZE, -MARGIN_SIZE, -MARGIN_SIZE));
            animatorJar.marginBottomTo(separator, MARGIN_SIZE * 2 - SEPARATOR_HEIGHT);
            //animatorJar.marginBottomTo(progress, MARGIN_SIZE * 2 - SEPARATOR_HEIGHT - 10);
            animatorJar.marginBottomTo(toolbar, MARGIN_SIZE);

            page.Margin = new Thickness(-1, -1, MARGIN_SIZE + 8, MARGIN_SIZE * 2 + 8);
        }

        // fullscreen
        private void toggleFullscreen()
        {
            if (isFullscreen)
                exitFullscreen();
            else
                enterFullscreen();
        }
        private void enterFullscreen()
        {
            lastWindowState = this.WindowState;

            this.Opacity = 0;
            this.WindowStyle = System.Windows.WindowStyle.None;
            if (this.WindowState != System.Windows.WindowState.Normal)
                this.WindowState = System.Windows.WindowState.Normal;
            this.WindowState = System.Windows.WindowState.Maximized;
            this.Opacity = 1;

            exitButton.Visibility = System.Windows.Visibility.Visible;

            isFullscreen = true;
            isAutoHideToolbar = true;

            hideMarginBottom();
            if (mousePos.Y < gridHeight - MARGIN_SIZE && visibleStates[toolbar])
                hideToolbar();
        }
        private void exitFullscreen()
        {
            if (!visibleStates[toolbar]) showToolbar();
            showMarginBottom();

            this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
            this.WindowState = lastWindowState;

            exitButton.Visibility = System.Windows.Visibility.Hidden;

            isFullscreen = false;
            isAutoHideToolbar = false;
        }

        // menu
        private void toggleMainMenu()
        {
            visibleStates[menu] = !visibleStates[menu];
            if (visibleStates[menu])
                showMainMenu();
            else
                hideMainMenu();
        }
        private void showMainMenu()
        {
            visibleStates[menu] = true;
            animatorJar.heightTo(menu, MENU_HEIGHT);
            animatorJar.fadeIn(menu);
        }
        private void hideMainMenu()
        {
            visibleStates[menu] = false;
            animatorJar.heightTo(menu, 0);
            animatorJar.fadeOut(menu);
        }

        private void container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            gridHeight = e.NewSize.Height;
        }

        private void container_MouseMove(object sender, MouseEventArgs e)
        {
            mousePos = e.GetPosition(niv);
            // auto show&hide toolbar
            if (isAutoHideToolbar && e.LeftButton == MouseButtonState.Released)
            {
                if (mousePos.Y > gridHeight - MARGIN_SIZE && !visibleStates[toolbar])
                    showToolbar();
                else if (mousePos.Y < gridHeight - MARGIN_SIZE && visibleStates[toolbar] && visibleStates[menu])
                    hideToolbar();
            }
        }


        #endregion

        // end of class
    }
}
