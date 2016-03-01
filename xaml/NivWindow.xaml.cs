using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
using System.Reflection;

using com.jarvisniu;

namespace Niv
{
    public partial class NivWindow : Window
    {
        // components
        private AboutWindow aboutWindow = new AboutWindow();
        private AnimatorJar animatorJar = new AnimatorJar();
        private ButtonAnimator buttonAnimator = new ButtonAnimator();
        Transformer marginManager;
        Controller inputController;
        FolderWalker folderWalker = new FolderWalker();
        // RecycleBin recycleBin = new RecycleBin(AppDomain.CurrentDomain.BaseDirectory);

        // delayed closing timers
        Timer timerClosePage;

        // layout config
        static double WINDOW_MIN_WIDTH = 680;
        static double WINDOW_MIN_HEIGHT = 470;
        public static int SEPARATOR_HEIGHT = 2;
        //public static int MESSAGE_BOX_HEIGHT = 48;
        public static int MARGIN_SIZE = 50;
        // static int DB_CLICK_THRESH = 300; // TODO double click detect give to InputHandler
        static int AA_SCALE_THRESHHOLD = 2;
        static double PROGRESS_CAP = 2;
        static double INFO_WIDTH = 260;
        static double MENU_HEIGHT =
            40 * 3 + // menu item
            2 * 1 +  // top border
            1 * 1 +  // seprate line
            6 * 4;   // gap
        private BitmapImage ERROR_IMAGE = loadResourceBitmap("res/Niv.ico");

        private string theme = "light";
        private Dictionary<FrameworkElement, bool> visibleStates = new Dictionary<FrameworkElement, bool>();
        private bool isMarginBottomExist = true;
        private bool isFullscreen = false;
        private bool isAutoHideToolbar = false;
        private WindowState lastWindowState = WindowState.Maximized;

        private double gridHeight = 0;
        private Point mousePos;
        BitmapImage bitmapImage; //TODO can remove this?
        private bool isSmoothOn = true; //TODO can remove this?
        private bool zoomButtonFit = true; //TODO can remove this?

        #region Initialize

        public NivWindow()
        {
            this.Opacity = 0;
            InitializeComponent();
            loadLanguage();
            setTheme(theme);
            initLayout();
            initComponents();
            loadCommandLineFile();
            this.Opacity = 1;

            bindContainerEvents();
            bindMenuEvents();
            bindOtherEvents();
            bindToolbarButtonsEvents();
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
            btnExit.Visibility = System.Windows.Visibility.Hidden;
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

            visibleStates[btnExit] = false;
            visibleStates[container] = false;
            visibleStates[toolbar] = true;
            visibleStates[info] = false;
            visibleStates[page] = false;
            visibleStates[menu] = false;

            // add animation effects to buttons
            buttonAnimator.apply(btnZoom).apply(btnPrevImage).apply(btnNextImage).apply(btnSmooth).apply(btnMenu).apply(btnExit)
                .apply(btnDeleteImage).apply(btnRotateLeft).apply(btnRotateRight).apply(menuAbout)
                .apply(menuHelp).apply(menuImageInfo).apply(btnCloseInfo);

            // Hide the toolbar buttons
            onWalkerCountChanged();
        }

        private void initComponents()
        {
            // Margin manager
            marginManager = new Transformer(container, image, null, folderWalker);
            marginManager.onScaleChanged = setButtonSmoothVisibility;

            // Input-controller
            inputController = new Controller(marginManager);

            // Timer
            timerClosePage = new Timer(3000);
            timerClosePage.Elapsed += timerClosePage_Elapsed;
        }

        private void setButtonSmoothVisibility()
        {
            double s = folderWalker.currentImageInfo == null ? 1 : folderWalker.currentImageInfo.scale;
            if (s > AA_SCALE_THRESHHOLD && folderWalker.count > 0)
                btnSmooth.Visibility = Visibility.Visible;
            else if (s <= AA_SCALE_THRESHHOLD)
                btnSmooth.Visibility = Visibility.Collapsed;
        }

        private void setTheme(string theme)
        {
            buttonAnimator.setTheme(theme);

            if (theme == "light")
            {
                container.Background = grayBrush(250);
                toolbar.Background = menu.Background = iImageInfoTitle.Background = grayBrush(220);
                info.Background = grayBrush(235);
                separator.Background = progress.Stroke = menu.BorderBrush = menuLine.Stroke = grayBrush(170);
                progress.Fill = grayBrush(255);
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
                toolbar.Background = grayBrush(32, 0.75);
                menu.Background = grayBrush(40);
                menu.BorderBrush = menuLine.Stroke = grayBrush(80, 0.75);
                info.Background = iImageInfoTitle.Background = grayBrush(64);
                separator.Background = progress.Stroke = iFilename.Foreground = iSize.Foreground =
                    iResolution.Foreground = iDate.Foreground = grayBrush(64, 0.75);
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
            imageRotateLeft.Source = loadThemeBitmap("icon-rotate-left.png", theme);
            imageRotateRight.Source = loadThemeBitmap("icon-rotate-right.png", theme);
            imageDelete.Source = loadThemeBitmap("icon-delete.png", theme);
            imagePrev.Source = loadThemeBitmap("icon-prev.png", theme);
            imageNext.Source = loadThemeBitmap("icon-next.png", theme);
            imageSmooth.Source = loadThemeBitmap(isSmoothOn ? "icon-smooth-off.png" : "icon-smooth-on.png", theme);
            imageZoom.Source = loadThemeBitmap("icon-zoom-fit.png", theme);
            imageMenu.Source = loadThemeBitmap("icon-menu.png", theme);
            imageCloseInfo.Source = loadThemeBitmap("icon-close.png", theme);
            // menu images
            imageHelp.Source = loadThemeBitmap("icon-about.png", theme);
            imageAbout.Source = loadThemeBitmap("icon-about.png", theme);
            imageInfo.Source = loadThemeBitmap("icon-list.png", theme);
        }

        #endregion

        #region Events

        private void bindToolbarButtonsEvents()
        {
            // Rotate-left button click
            btnRotateLeft.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                folderWalker.currentImageInfo.rotationAngle -= 90;
                folderWalker.currentImageInfo.rotationAngle = simplifyAngle(folderWalker.currentImageInfo.rotationAngle);
                animatorJar.rotateTo(image, folderWalker.currentImageInfo.rotationAngle);
            };

            // Rotate-right button click
            btnRotateRight.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                folderWalker.currentImageInfo.rotationAngle += 90;
                folderWalker.currentImageInfo.rotationAngle = simplifyAngle(folderWalker.currentImageInfo.rotationAngle);
                animatorJar.rotateTo(image, folderWalker.currentImageInfo.rotationAngle);
            };

            // Delete-image button click
            btnDeleteImage.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {

            };

            // Prev-image button click
            btnPrevImage.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                prevImage();
            };

            // Rotate-left button click
            btnNextImage.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                nextImage();
            };

            // Smooth switch button click
            btnSmooth.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                setSmoothTo(!isSmoothOn);
            };

            // Zoom button click
            btnZoom.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                toggleZoomIn121AndFit();
            };

            // Menu button click
            btnMenu.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                toggleMainMenu();
            };
        }

        private void bindImageEvents()
        {
            image.MouseMove += (object sender, MouseEventArgs e) =>
            {
                // 处理拖动还是点击的逻辑
            };

            image.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) =>
            {
                //if (e.Timestamp - lastMouseUpOnImage.Timestamp < DB_CLICK_THRESH)
                //{
                //    inputController.onMouseDoubleClick();
                //    refreshZoomButton();
                //    lastMouseUpOnImage.Timestamp = e.Timestamp - DB_CLICK_THRESH - 1;
                //}
                //else
                //{
                //    if (lastMouseUpOnImage.NextUpInvalid)
                //    {
                //        lastMouseUpOnImage.Timestamp = e.Timestamp - DB_CLICK_THRESH - 1;
                //        lastMouseUpOnImage.NextUpInvalid = false;
                //    }
                //    else
                //    {
                //        lastMouseUpOnImage.Timestamp = e.Timestamp;
                //        lastMouseUpOnImage.Position = e.GetPosition(grid);
                //    }
                //}
            };
        }

        private void bindContainerEvents()
        {
            // Drag-enter event
            container.DragEnter += (object sender, DragEventArgs e) =>
            {
                e.Effects = DragDropEffects.Move;
            };

            // Drop event
            container.Drop += (object sender, DragEventArgs e) =>
            {
                string[] urls = (string[])e.Data.GetData(DataFormats.FileDrop);
                onReceiveImageFile(urls[0]);
            };

            // Size-changed envent
            container.SizeChanged += (object sender, SizeChangedEventArgs e) =>
            {
                if (folderWalker.count > 0 && folderWalker.currentImageInfo.isFullwindow)
                    marginManager.fullwindow().setMarginDesByKeys().apply();
                else
                    marginManager.setMarginDesByKeys().apply();
                gridHeight = e.NewSize.Height;
                Console.WriteLine(window.Width + ", " + window.Height);
            };

            // Size-changed envent
            separator.SizeChanged += (object sender, SizeChangedEventArgs e) =>
            {
                refreshProgressI();
            };

            container.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                inputController.onMouseLeftDown();
            };

            // Mouse-move envet
            container.MouseMove += (object sender, MouseEventArgs e) =>
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


                inputController.onMouseMove(e.GetPosition(container));

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (!inputController.isLeftButtonDown)
                        inputController.onMouseLeftDown();
                    inputController.onDragMove();
                }
                refreshZoomButton();
            };

            // Mouse-wheel envet
            container.MouseWheel += (object sender, MouseWheelEventArgs e) =>
            {
                bool up = e.Delta > 0;
                inputController.onMouseWheel(up);
                refreshZoomButton();
            };
        }

        private void bindMenuEvents()
        {
            // Help menu
            menuHelp.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                theme = theme == "dark" ? "light" : "dark";
                setTheme(theme);
                hideMainMenu();
            };

            // About menu
            menuAbout.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                aboutWindow.Show();
                hideMainMenu();
            };

            // Image info menu
            menuImageInfo.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                toggleInfo();
                hideMainMenu();
            };
        }

        private void bindOtherEvents()
        {
            // Click to close menu events
            container.MouseDown += closeMenu;
            toolbar.MouseDown += closeMenu;
            info.MouseDown += closeMenu;
            window.StateChanged += (object sender, EventArgs e) => hideMainMenu();

            // Keyboard shortcut
            window.KeyDown += (object sender, KeyEventArgs e) =>
            {
                string keyString = e.Key.ToString();

                if (keyString == "Left") prevImage();
                else if (keyString == "Right") nextImage();
                else if (keyString == "Up") marginManager.zoomAtStand(2).animate();
                else if (keyString == "Down") marginManager.zoomAtStand(0.5).animate();
                else if (keyString == "A") setSmoothTo(!isSmoothOn);
                else if (keyString == "D") debug();
                else if (keyString == "I") toggleInfo();
                else if (keyString == "Delete") delete();
                else if (keyString == "Escape") exit();
                else if (keyString == "Space") toggleZoomIn121AndFit();
                else if (keyString == "Return") toggleFullscreen();
            };

            // Window-closing
            window.Closing += (object sender, System.ComponentModel.CancelEventArgs e) =>
            {
                exit();
            };

            // Close-info button click
            btnCloseInfo.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                hideInfo();
            };

            // Exit button
            btnExit.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                hideInfo();
            };
        }

        #endregion

        #region load

        private void loadCommandLineFile()
        {
            string cmdFilename = getFirstCommandLineFilename();
            if (cmdFilename.Length > 0)
                onReceiveImageFile(cmdFilename);
        }

        private void loadFromWalker()
        {
            ImageInfo info = folderWalker.currentImageInfo;
            // The image may be broken
            bitmapImage = tryLoadBitmap(info.filename);
            marginManager.setImage(bitmapImage);

            // infos
            labelInfoFilename.Content = Path.GetFileName(info.filename);
            FileInfo fi = new FileInfo(info.filename);
            labelInfoSize.Content = humanifyNumber(fi.Length) + "B";
            labelInfoResolution.Content = bitmapImage.PixelWidth + " x " + bitmapImage.PixelHeight;
            labelInfoDate.Content = dateTimeToString(fi.LastWriteTime);
            this.Title = Path.GetFileName(info.filename) + " - " + I18n._("appName");
            refreshProgress();
            showPage();

            if (!isFileGif(info.filename))
            {
                // ImageBehavior.SetAnimatedSource(image, null);
                image.Source = bitmapImage;
            }
            else
            {
                // ImageBehavior.SetAnimatedSource(image, bitmapImage);
            }

            if (info.virgin)
            {
                marginManager.fullsize().setMarginDesByKeys().apply();
                setSmoothByImageResolution();

                marginManager.screenCenter().setMarginDesByKeys().animate();
                info.virgin = false;
            }
            else
            {
                if (info.isFullwindow)
                {
                    marginManager.fullsize().setMarginDesByKeys().apply();
                    marginManager.screenCenter().setMarginDesByKeys().animate();
                }
                else
                {
                    marginManager.exitFullwindowMode().setScale(info.scale).setMarginDesByKeys().apply();
                    marginManager.setCenter(info.center).setMarginDesByKeys().animate();
                }
                animatorJar.rotateToI(image, info.rotationAngle);
            }

            refreshZoomButton();
        }

        private void onReceiveImageFile(string url)
        {
            if (!FolderWalker.isFormatSupported(url))
            {
                showMessage("不支持此文件格式： " + Path.GetExtension(url)); // TODO use other solution
                exit();
            }
            else if (!isFileExist(url))
            {
                MessageBox.Show("文件不存在： " + url);
                exit();
            }
            else
            {
                folderWalker.loadFolder(url);
                onWalkerCountChanged();
                //marginManager.fullwindow().apply();
                page.Width = 32 + folderWalker.count.ToString().Length * 16;
                loadFromWalker();
            }
        }

        // Try to read and decode the image file to the BitmapImage
        public BitmapImage tryLoadBitmap(string url)
        {
            BitmapImage img;

            // TODO 打开之后再删文件会打不开文件
            FileInfo fi = new FileInfo(url);
            if (fi.Length == 0)
            {
                img = ERROR_IMAGE;
            }
            else
            {
                try
                {
                    img = loadBitmap(url);

                }
                catch (NotSupportedException)
                {
                    img = ERROR_IMAGE;
                }
            }
            return img;
        }

        public static BitmapImage loadBitmap(string url)
        {
            // load bitmap without handling on the file (i.e. you can delete it while viewing it)
            // for release handle on file
            // SEE: http://stackoverflow.com/questions/10319447/release-handle-on-file-imagesource-from-bitmapimage
            Uri uri = new Uri(url, UriKind.Absolute);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = uri;
            bitmap.EndInit();
            return bitmap;
        }

        private void onWalkerCountChanged()
        {
            int count = folderWalker.count;

            // Normal buttons
            if (count == 0)
            {
                btnDeleteImage.Visibility = Visibility.Hidden;
                btnRotateLeft.Visibility = Visibility.Hidden;
                btnRotateRight.Visibility = Visibility.Hidden;
                btnZoom.Visibility = Visibility.Hidden;
            }
            else
            {
                btnDeleteImage.Visibility = Visibility.Visible;
                btnRotateLeft.Visibility = Visibility.Visible;
                btnRotateRight.Visibility = Visibility.Visible;
                btnZoom.Visibility = Visibility.Visible;
            }

            // Prev-image and next-image button
            if (count > 1)
            {
                btnPrevImage.Visibility = Visibility.Visible;
                btnNextImage.Visibility = Visibility.Visible;
            }
            else
            {
                btnPrevImage.Visibility = Visibility.Hidden;
                btnNextImage.Visibility = Visibility.Hidden;
            }

            // Smooth button
            setButtonSmoothVisibility();
        }

        #endregion

        #region utils

        private string getFirstCommandLineFilename()
        {
            string commandLine = Environment.CommandLine;
            int secondIndexOfComma = commandLine.IndexOf("\"", 1);
            string trimedCommandLine = commandLine.Substring(secondIndexOfComma + 2);

            if (trimedCommandLine.Length > 0)
            {
                if (trimedCommandLine.StartsWith("\""))
                {
                    int secondCommaIndex = trimedCommandLine.IndexOf("\"", 1);
                    return trimedCommandLine.Substring(1, secondCommaIndex - 1);
                }
                else
                {
                    int indexFirstSpace = trimedCommandLine.IndexOf(" ");
                    if (indexFirstSpace == -1)
                        return trimedCommandLine;
                    else
                        return trimedCommandLine.Substring(0, indexFirstSpace);
                }
            }
            return "";
        }
        private void showMessage(string message)
        {
            MessageBox.Show(message);
        }

        private static BitmapImage loadThemeBitmap(string filename, string theme)
        {
            string namespaceName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            return new BitmapImage(new Uri("pack://application:,,,/" + namespaceName + ";component/res/theme-" + theme + "/" + filename));
        }

        private static BitmapImage loadResourceBitmap(string filepath)
        {
            string namespaceName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            return new BitmapImage(new Uri("pack://application:,,,/" + namespaceName + ";component/" + filepath));
        }

        private SolidColorBrush grayBrush(byte brightness)
        {
            return new SolidColorBrush(Color.FromArgb(255, brightness, brightness, brightness));
        }

        private SolidColorBrush grayBrush(byte brightness, double alpha)
        {
            return new SolidColorBrush(Color.FromArgb(Convert.ToByte(255 * alpha), brightness, brightness, brightness));
        }

        private void setImageRotationBackToZero()
        {
            if (folderWalker.count == 0) return;
            // 善后
            saveRotationToFile();             // 将旋转保存到文件
            animatorJar.rotateToI(image, 0);    // 恢复旋转变化
        }

        public void saveRotationToFile()
        {
            string filename = folderWalker.currentImageInfo.filename;
            double rotationAngle = folderWalker.currentImageInfo.rotationAngle;
            double savedAngle = folderWalker.currentImageInfo.savedRotationAngle;
            double angle = simplifyAngle(rotationAngle - savedAngle);
            if (angle == 0) return;

            System.Drawing.Image imgSrc = System.Drawing.Image.FromFile(filename);
            if (angle == 90)
            {
                imgSrc.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
            }
            else if (angle == 180)
            {
                imgSrc.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            }
            else if (angle == 270)
            {
                imgSrc.RotateFlip(System.Drawing.RotateFlipType.Rotate270FlipNone);
            }

            try
            {
                imgSrc.Save(filename);
            }
            catch (Exception ex)
            {
                showMessage("保存旋转失败，文件被占用或者权限不够。\nMessage: " + ex.Message);
            }

            folderWalker.currentImageInfo.savedRotationAngle = folderWalker.currentImageInfo.rotationAngle;
        }

        private void refreshProgress()
        {
            setProgressWidth();

            double left = getProgressLeft();
            if (folderWalker.isJumpBetweenEnds())
                animatorJar.translateLeftToI(progress, left);
            else
                animatorJar.translateLeftTo(progress, left);

            iPage.Content = (folderWalker.currentIndex + 1) + "/" + folderWalker.count;
        }

        private void refreshProgressI()
        {
            setProgressWidth();
            animatorJar.translateLeftToI(progress, getProgressLeft());
        }

        private void setProgressWidth()
        {
            int count = folderWalker.count;
            if (count == 0)
            {
                progress.Width = 0;
            }
            else
            {
                progress.Width = separator.RenderSize.Width / folderWalker.count + PROGRESS_CAP * 2;
            }
        }

        private double getProgressLeft()
        {
            double percent = (double)folderWalker.currentIndex / folderWalker.count;
            return separator.RenderSize.Width * percent + separator.Margin.Left - PROGRESS_CAP;
        }

        private void toggleZoomIn121AndFit()
        {

            if (marginManager.isFullwindow)
                marginManager.initOne().animate();
            else
                marginManager.fullwindow().animate();

            refreshZoomButton();
        }

        private void setSmoothTo(bool on)
        {
            isSmoothOn = on;
            if (isSmoothOn)
            {
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            }
            else
            {
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            }
            folderWalker.currentImageInfo.smooth = isSmoothOn;
        }

        private void setSmoothByImageResolution()
        {
            if (bitmapImage == null) return;

            bool isImageSmall = bitmapImage.PixelWidth < 257 && bitmapImage.PixelHeight < 257;
            setSmoothTo(!isImageSmall);
        }


        private void refreshZoomButton()
        {
            if (folderWalker.count == 0) return;

            if (marginManager.isFullwindow)
            {
                if (zoomButtonFit)
                {
                    imageZoom.Source = loadThemeBitmap("icon-zoom-121.png", theme);
                    zoomButtonFit = false;
                }
            }
            else
            {
                if (!zoomButtonFit)
                {
                    imageZoom.Source = loadThemeBitmap("icon-zoom-fit.png", theme);
                    zoomButtonFit = true;
                }
            }
        }

        private bool isRotated()
        {
            double angleDelta = folderWalker.currentImageInfo.rotationAngle - folderWalker.currentImageInfo.savedRotationAngle;
            while (angleDelta < 0) angleDelta += 360;
            while (angleDelta > 360) angleDelta -= 360;
            return angleDelta % 360 != 0;
        }

        private bool isFileGif(string name)
        {
            return name.ToLower().EndsWith(".gif");
        }

        private bool isFileExist(string url)
        {

            FileInfo fi = new FileInfo(url);
            return fi.Exists;
        }

        private double simplifyAngle(double angle)
        {
            angle = angle % 360;
            while (angle < 0) angle += 360;
            return angle;
        }

        private string humanifyNumber(double num)
        {
            // 科学计数法
            string[] units = { "", "k", "M", "G", "T" };
            int level = 0;

            while (num > 1024)
            {
                num /= 1024;
                level++;
            }

            if (num >= 10)
                num = (int)(num);
            else
                num = Math.Round(num, 1);

            return num + " " + units[level];
        }

        private string dateTimeToString(DateTime dt)
        {
            string str = dt.Year + "年" + dt.Month + "月" + dt.Day + "日  ";

            if (dt.DayOfWeek == DayOfWeek.Monday)
                str += "星期一";
            else if (dt.DayOfWeek == DayOfWeek.Tuesday)
                str += "星期二";
            else if (dt.DayOfWeek == DayOfWeek.Wednesday)
                str += "星期三";
            else if (dt.DayOfWeek == DayOfWeek.Thursday)
                str += "星期四";
            else if (dt.DayOfWeek == DayOfWeek.Friday)
                str += "星期五";
            else if (dt.DayOfWeek == DayOfWeek.Saturday)
                str += "星期六";
            else if (dt.DayOfWeek == DayOfWeek.Sunday)
                str += "星期日";

            str += "  " + dt.ToShortTimeString();
            return str;
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
            this.Dispatcher.Invoke(new Action(delegate
            {
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
                animatorJar.translateLeftTo(container, INFO_WIDTH - MARGIN_SIZE);
                animatorJar.translateLeftTo(toolbar, INFO_WIDTH);
                animatorJar.translateLeftTo(separator, INFO_WIDTH);
                animatorJar.translateLeftTo(info, 0);
            }
            else
            {
                animatorJar.translateLeftTo(container, -MARGIN_SIZE);
                animatorJar.translateLeftTo(toolbar, 0);
                animatorJar.translateLeftTo(separator, 0);
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

            btnExit.Visibility = System.Windows.Visibility.Visible;

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

            btnExit.Visibility = System.Windows.Visibility.Hidden;

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




        #endregion

        private void closeMenu(object sender, MouseButtonEventArgs e)
        {
            if (visibleStates[menu] && e.OriginalSource != imageMenu && e.OriginalSource != btnMenu)
                hideMainMenu();
        }

        private void prevImage()
        {
            setImageRotationBackToZero();
            folderWalker.switchBackward();
            loadFromWalker();
        }

        private void nextImage()
        {
            setImageRotationBackToZero();
            folderWalker.switchForward();
            loadFromWalker();
        }

        private void delete()
        {
            if (folderWalker.count == 0) return;

            FileInfo fi = new FileInfo(folderWalker.currentImageInfo.filename);
            if (fi.Exists)
            {
                //recycleBin.receive(filename);
                showMessage("图片已删除至图片回收站。");
                showPage();
            }

            folderWalker.removeCurrentImageInfo();

            if (folderWalker.count > 0)
                loadFromWalker();
            else
                exit(); // TODO不能退出，因为可能只有一张而删错，应该显示一个打开文件的按钮
        }

        private void debug()  // press key "D"
        {
        }

        private void exit()
        {
            //restoreRotation();
            //recycleBin.clean();
            aboutWindow.exit();
            Application.Current.Shutdown();
        }

        // EOC
    }
}
