using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
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
using WpfAnimatedGif;

namespace Niv
{
    public partial class NivWindow : Window
    {
        // components
        private AboutWindow aboutWindow = new AboutWindow();
        private AnimatorJar animatorJar = new AnimatorJar();
        private ButtonAnimator buttonAnimator = new ButtonAnimator();
        Transformer transformer;
        Controller inputController;
        FolderWalker walker = new FolderWalker();
        Recycle recycle = new Recycle();

        // delayed closing timers
        Timer timerClosePage;

        // layout config
        static double WINDOW_MIN_WIDTH = 540;
        static double WINDOW_MIN_HEIGHT = 384;
        public static int SEPARATOR_HEIGHT = 2;
        public static int TOOLBAR_HEIGHT = 48;
        public static int MARGIN_SIZE = 50;
        static int AA_SCALE_THRESHHOLD = 2;
        static double PROGRESS_CAP = 2;
        static double INFO_WIDTH = 260;
        static double MENU_HEIGHT =
            40 * 4 + // menu item
            2 * 1 +  // top border
            1 * 1 +  // seprate line
            6 * 4;   // gap
        private BitmapImage ERROR_IMAGE = loadResourceBitmap("res/Niv.ico");
        private Dictionary<Border, string> BUTTON_TO_TIP_MAP = new Dictionary<Border, string>();

        private Dictionary<FrameworkElement, bool> visibleStates = new Dictionary<FrameworkElement, bool>();
        private bool isMarginBottomExist = true;
        private bool isFullscreen = false;
        private bool isAutoHideToolbar = false;
        private WindowState lastWindowState = WindowState.Maximized;

        private double gridHeight = 0;
        private Point mousePos;
        private bool isTooltipVisible = false;

        #region Initialize

        public NivWindow()
        {
            InitializeComponent();
            loadLanguage();
            setTheme();
            initLayout();
            initComponents();
            loadCommandLineFile();

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
            iSetting.Content = I18n._("setting");
            iImageInfo.Content = I18n._("imageInfo");

            // tooltip
            BUTTON_TO_TIP_MAP[btnRotateLeft] = I18n._("tooltip.rotate-left");
            BUTTON_TO_TIP_MAP[btnRotateRight] = I18n._("tooltip.rotate-right");
            BUTTON_TO_TIP_MAP[btnDelete] = I18n._("tooltip.delete");
            BUTTON_TO_TIP_MAP[btnUndelete] = I18n._("tooltip.undelete");  // dynamic
            BUTTON_TO_TIP_MAP[btnPrevImage] = I18n._("tooltip.prev-image");
            BUTTON_TO_TIP_MAP[btnNextImage] = I18n._("tooltip.next-image");
            BUTTON_TO_TIP_MAP[btnMenu] = I18n._("menu");
            //BUTTON_TO_TIP_MAP[btnCloseInfo] = I18n._("close");
            //BUTTON_TO_TIP_MAP[btnExit] = I18n._("tooltip.exit-program");
            BUTTON_TO_TIP_MAP[btnSmooth] = I18n._("tooltip.disable-smooth");  // dynamic
            BUTTON_TO_TIP_MAP[btnZoom] = I18n._("tooltip.one-to-one");  // dynamic

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
            page.Margin = new Thickness(-1, -1, 8, MARGIN_SIZE + 8);
            page.Opacity = 0;
            menu.Margin = new Thickness(0, 0, 0, MARGIN_SIZE);
            menu.Height = 0;

            visibleStates[btnExit] = false;
            visibleStates[btnSmooth] = false;
            visibleStates[container] = false;
            visibleStates[toolbar] = true;
            visibleStates[info] = false;
            visibleStates[page] = false;
            visibleStates[menu] = false;

            // add animation effects to buttons
            buttonAnimator.apply(btnZoom).apply(btnPrevImage).apply(btnNextImage).apply(btnSmooth).apply(btnMenu).apply(btnExit)
                .apply(btnDelete).apply(btnUndelete).apply(btnRotateLeft).apply(btnRotateRight).apply(menuAbout)
                .apply(menuHelp).apply(menuSetting).apply(menuImageInfo).apply(btnCloseInfo);

            // Hide the toolbar buttons
            onWalkerCountChanged();

            // Hide undelete button
            onRecycleCountChanged();

            // Set render to high quality of images
            Image[] images = { imageRotateLeft, imageRotateRight, imageDelete, imageUndelete, imagePrev, imageNext,
                imageSmooth, imageZoom, imageMenu, imageCloseInfo, imageHelp, imageAbout, imageSetting, imageInfo, imageExit };
            foreach (Image image in images)
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);

            if (Settings.Default.windowState == 2)
                enterFullscreen();
            else if (Settings.Default.windowState == 0)
            {
                window.WindowState = WindowState.Normal;
                window.Left = Settings.Default.left;
                window.Top = Settings.Default.top;
                window.Width = Settings.Default.width;
                window.Height = Settings.Default.height;
            }
        }

        private void initComponents()
        {
            // Margin manager
            transformer = new Transformer(container, image, null, walker);
            transformer.onScaleChanged = setButtonSmoothVisibility;

            // Input-controller
            inputController = new Controller(window, transformer);

            // Timer
            timerClosePage = new Timer(3000);
            timerClosePage.Elapsed += timerClosePage_Elapsed;
        }

        private void setButtonSmoothVisibility()
        {
            double s = walker.currentImageInfo == null ? 1 : walker.currentImageInfo.scale;
            if (s > AA_SCALE_THRESHHOLD && walker.count > 0)
            {
                animatorJar.fadeIn(btnSmooth);
                visibleStates[btnSmooth] = true;
            }
            else if (s <= AA_SCALE_THRESHHOLD)
            {
                animatorJar.fadeOut(btnSmooth);
                visibleStates[btnSmooth] = false;
            }
            refreshSmoothButton();
        }

        private void setTheme()
        {
            buttonAnimator.setTheme(Settings.Default.theme);

            if (Settings.Default.theme == "light")
            {
                container.Background = grayBrush(250);
                toolbar.Background = menu.Background = iImageInfoTitle.Background = grayBrush(220);
                info.Background = grayBrush(235);
                separator.Background = progress.Stroke = menu.BorderBrush = menuLine.Stroke
                    = infoTitleLine.Stroke = infoRightLine.Stroke = grayBrush(170);
                progress.Fill = grayBrush(255);
                iHelp.Foreground = iAbout.Foreground = iSetting.Foreground = iImageInfo.Foreground
                    = iImageInfoTitle.Foreground = iTooltip.Foreground = iPage.Foreground = grayBrush(48);
                iFilename.Foreground = iSize.Foreground = iResolution.Foreground = iDate.Foreground = grayBrush(84);
                labelInfoFilename.Foreground = labelInfoSize.Foreground = labelInfoResolution.Foreground
                    = labelInfoDate.Foreground = grayBrush(0);
                borderTooltip.Fill = page.Background = grayBrush(255, 0.75);
                borderTooltip.Stroke = page.BorderBrush = grayBrush(128, 0.75);
                btnExit.BorderBrush = grayBrush(128);
            }
            else if (Settings.Default.theme == "dark")
            {
                container.Background = grayBrush(32);
                toolbar.Background = grayBrush(32, 0.75);
                menu.Background = grayBrush(40);
                menu.BorderBrush = menuLine.Stroke = infoTitleLine.Stroke = infoRightLine.Stroke = grayBrush(80, 0.75);
                info.Background = iImageInfoTitle.Background = grayBrush(64);
                separator.Background = progress.Stroke = iFilename.Foreground = iSize.Foreground =
                    iResolution.Foreground = iDate.Foreground = grayBrush(64, 0.75);
                progress.Fill = grayBrush(255, 0.75);
                iHelp.Foreground = iAbout.Foreground = iSetting.Foreground = iImageInfo.Foreground = iImageInfoTitle.Foreground
                     = iTooltip.Foreground = iPage.Foreground = iFilename.Foreground = iSize.Foreground
                     = iResolution.Foreground = iDate.Foreground = grayBrush(192);
                labelInfoFilename.Foreground = labelInfoSize.Foreground = labelInfoResolution.Foreground
                    = labelInfoDate.Foreground = grayBrush(255);
                borderTooltip.Fill = page.Background = grayBrush(64, 0.75);
                borderTooltip.Stroke = page.BorderBrush = grayBrush(128, 0.75);
                btnExit.BorderBrush = grayBrush(0, 0);
            }
            else MessageBox.Show("Not supported theme: " + Settings.Default.theme);

            // button images
            imageRotateLeft.Source = loadThemeBitmap("icon-rotate-left.png");
            imageRotateRight.Source = loadThemeBitmap("icon-rotate-right.png");
            imageDelete.Source = loadThemeBitmap("icon-delete.png");
            imageUndelete.Source = loadThemeBitmap("icon-undelete.png");
            imagePrev.Source = loadThemeBitmap("icon-prev.png");
            imageNext.Source = loadThemeBitmap("icon-next.png");
            refreshSmoothButton();
            refreshZoomButton();
            imageMenu.Source = loadThemeBitmap("icon-menu.png");
            imageCloseInfo.Source = imageExit.Source = loadThemeBitmap("icon-close.png");
            // menu images
            imageHelp.Source = loadThemeBitmap("icon-help.png");
            imageAbout.Source = loadThemeBitmap("icon-info.png");
            imageSetting.Source = loadThemeBitmap("icon-setting.png");
            imageInfo.Source = loadThemeBitmap("icon-list.png");
        }

        #endregion

        #region Events

        private void bindToolbarButtonsEvents()
        {
            // Rotate-left button click
            btnRotateLeft.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                walker.currentImageInfo.rotationAngle -= 90;
                walker.currentImageInfo.rotationAngle = simplifyAngle(walker.currentImageInfo.rotationAngle);
                animatorJar.rotateTo(image, walker.currentImageInfo.rotationAngle);
            };

            // Rotate-right button click
            btnRotateRight.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                walker.currentImageInfo.rotationAngle += 90;
                walker.currentImageInfo.rotationAngle = simplifyAngle(walker.currentImageInfo.rotationAngle);
                animatorJar.rotateTo(image, walker.currentImageInfo.rotationAngle);
            };

            // Delete-image button click
            btnDelete.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                delete();
            };

            // Delete-image button click
            btnUndelete.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                undeleteLast();
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
                if (visibleStates[btnSmooth]) setSmoothTo(!walker.currentImageInfo.smooth);
                refreshSmoothButton();
                iTooltip.Content = BUTTON_TO_TIP_MAP[btnSmooth];
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

            // toggle tooltip
            Border[] buttons = { btnRotateLeft, btnRotateRight, btnDelete, btnUndelete,
                                 btnPrevImage, btnNextImage, btnSmooth, btnZoom, btnMenu };
            foreach (Border button in buttons)
            {
                button.MouseMove += onMouseMoveToolButton;
                button.MouseLeave += onMouseLeaveToolButton;
            }
        }

        private void onMouseMoveToolButton(object sender, MouseEventArgs e)
        {
            Border button = (Border)sender;
            if (isTooltipVisible || button.Opacity < 0.1) return;

            animatorJar.fadeIn(tooltip);
            isTooltipVisible = true;

            iTooltip.Content = BUTTON_TO_TIP_MAP[button];
            moveTooltipTo((FrameworkElement)sender);
        }

        private void onMouseLeaveToolButton(object sender, MouseEventArgs e)
        {
            if (!isTooltipVisible) return;

            animatorJar.fadeOut(tooltip);
            isTooltipVisible = false;
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
                if (walker.count > 0 && walker.currentImageInfo.fitWindow)
                    transformer.fitWindow().calcMarginDesByKeys().apply();
                else
                    transformer.calcMarginDesByKeys().apply();
                gridHeight = e.NewSize.Height;
            };

            // Size-changed envent. When info panel close/hide refresh the progress position.
            separator.SizeChanged += (object sender, SizeChangedEventArgs e) =>
            {
                refreshProgressI();
                gridSwitch.Visibility = separator.ActualWidth < 480 ? Visibility.Hidden : Visibility.Visible;
            };

            container.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                toolbar.IsHitTestVisible = false;
                inputController.onMouseLeftDown(e.Timestamp);
            };

            // Mouse-move envet
            container.MouseMove += (object sender, MouseEventArgs e) =>
            {
                mousePos = e.GetPosition(niv);

                // auto show&hide toolbar
                if (e.LeftButton == MouseButtonState.Released) tryHideToolbar();

                inputController.onMouseMove(e.GetPosition(container));

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (!inputController.isLeftButtonDown)
                        inputController.onMouseLeftDown(e.Timestamp);
                    inputController.onDragMove();
                }
                if (walker.count > 0) refreshZoomButton();
            };

            container.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) =>
            {
                toolbar.IsHitTestVisible = true;
                inputController.onMouseLeftUp(e.Timestamp);
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
                MessageBox.Show("TODO: 帮助功能正在开发");
                hideMainMenu();
            };

            // About menu
            menuAbout.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                aboutWindow.Show();
                hideMainMenu();
            };

            // Setting menu
            menuSetting.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                toggleTheme();
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
            window.StateChanged += (object sender, EventArgs e) =>
            {
                hideMainMenu();
                saveWindowSate();
            };

            // Keyboard shortcut
            window.KeyDown += (object sender, KeyEventArgs e) =>
            {
                string keyString = e.Key.ToString();

                if (keyString == "Left") prevImage();
                else if (keyString == "Right") nextImage();
                else if (keyString == "Up") transformer.zoomAtWindowCenter(2).animate();
                else if (keyString == "Down") transformer.zoomAtWindowCenter(0.5).animate();
                else if (keyString == "A") setSmoothTo(!walker.currentImageInfo.smooth);
                else if (keyString == "D") debug();
                else if (keyString == "I") toggleInfo();
                else if (keyString == "T") toggleTheme();
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
                exit();
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
            ImageInfo info = walker.currentImageInfo;
            // The image may be broken
            transformer.bitmap = tryLoadBitmap(info.filename);

            // infos
            labelInfoFilename.Content = Path.GetFileName(info.filename);
            FileInfo fi = new FileInfo(info.filename);
            labelInfoSize.Content = suffixifyNumber(fi.Length) + "B";
            labelInfoResolution.Content = transformer.bitmap.PixelWidth + " x " + transformer.bitmap.PixelHeight;
            labelInfoDate.Content = dateTimeToString(fi.LastWriteTime);
            this.Title = Path.GetFileName(info.filename) + " - " + I18n._("appName");
            refreshProgress();
            refreshPageText();
            showPage();

            if (!isFileGif(info.filename))
            {
                ImageBehavior.SetAnimatedSource(image, null);
                image.Source = transformer.bitmap;
            }
            else
            {
                ImageBehavior.SetAnimatedSource(image, transformer.bitmap);
            }

            if (info.virgin)
            {
                transformer.fullsize().calcMarginDesByKeys().apply();
                setSmoothByImageResolution();

                transformer.screenCenter().calcMarginDesByKeys().animate();
                info.virgin = false;
            }
            else
            {
                if (info.fitWindow)
                {
                    transformer.fullsize().calcMarginDesByKeys().apply();
                    transformer.screenCenter().calcMarginDesByKeys().animate();
                }
                else
                {
                    transformer.exitFitWindowMode().setScale(info.scale).calcMarginDesByKeys().apply();
                    transformer.setCenter(info.center).calcMarginDesByKeys().animate();
                }
                animatorJar.rotateToI(image, info.rotationAngle);
            }

            refreshSmoothButton();
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
                int indexInList = walker.getImageFileIndex(url);
                if (indexInList > -1)
                {
                    if (indexInList != walker.currentIndex)
                    {
                        walker.currentIndex = indexInList;
                        loadFromWalker();
                    }
                    else
                    {
                        toggleZoomIn121AndFit();
                    }
                }
                else
                {
                    walker.loadFolder(url);
                    onWalkerCountChanged();
                    page.Width = 32 + walker.count.ToString().Length * 16;
                    loadFromWalker();
                }
            }
        }

        // Try to read and decode the image file to the BitmapImage
        public BitmapImage tryLoadBitmap(string url)
        {
            BitmapImage img = ERROR_IMAGE;

            // TODO 打开之后再删文件会打不开文件
            FileInfo fi = new FileInfo(url);
            if (fi.Length > 0)
            {
                img = loadBitmap(url);
                walker.currentImageInfo.broken = false;
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
            int count = walker.count;

            // Normal buttons
            if (count == 0)
            {
                btnDelete.Visibility = Visibility.Hidden;
                btnRotateLeft.Visibility = Visibility.Hidden;
                btnRotateRight.Visibility = Visibility.Hidden;
                btnZoom.Visibility = Visibility.Hidden;

                image.Source = null;
                ImageBehavior.SetAnimatedSource(image, null);
                window.Title = I18n._("appName");
            }
            else
            {
                btnDelete.Visibility = Visibility.Visible;
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

        private void showMessage(object message)
        {
            MessageBox.Show(message.ToString());
        }

        private static BitmapImage loadThemeBitmap(string filename)
        {
            string namespaceName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            return new BitmapImage(new Uri("pack://application:,,,/" + namespaceName + ";component/res/theme-" + Settings.Default.theme + "/" + filename));
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
            if (walker.count == 0) return;
            // 善后
            saveRotationToFile();             // 将旋转保存到文件
            animatorJar.rotateToI(image, 0);    // 恢复旋转变化
        }

        public void saveRotationToFile()
        {
            string filename = walker.currentImageInfo.filename;
            string ext = Path.GetExtension(filename).ToLower();

            ImageInfo info = walker.currentImageInfo;
            double rotationAngle = info.rotationAngle;
            double savedAngle = info.savedRotationAngle;
            double angle = simplifyAngle(rotationAngle - savedAngle);

            if (angle == 0 || info.broken || ext == ".gif" || ext == ".ico") return;

            System.Drawing.Image imgSrc = System.Drawing.Image.FromFile(filename);
            if (angle == 90)
                imgSrc.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
            else if (angle == 180)
                imgSrc.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            else if (angle == 270)
                imgSrc.RotateFlip(System.Drawing.RotateFlipType.Rotate270FlipNone);

            try
            {
                switch (ext)
                {
                    case ".jpg":
                    case ".jpeg":
                        saveJpegFile(imgSrc, filename);
                        break;
                    case ".png":
                        imgSrc.Save(filename, ImageFormat.Png);
                        break;
                    case ".bmp":
                        imgSrc.Save(filename, ImageFormat.Bmp);
                        break;
                    case ".tif":
                    case ".tiff":
                        imgSrc.Save(filename, ImageFormat.Tiff);
                        break;
                }
            }
            catch (Exception ex)
            {
                showMessage("保存旋转失败，文件被占用或者权限不够。\nMessage: " + ex.Message);
            }

            walker.currentImageInfo.savedRotationAngle = walker.currentImageInfo.rotationAngle;
        }

        private void saveJpegFile(System.Drawing.Image img, string filename)
        {
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            ImageCodecInfo jpgEncoder = null;
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
                if (codec.MimeType == "image/jpeg") jpgEncoder = codec;
            EncoderParameter myEncoderParameter = new EncoderParameter(Encoder.Quality, 90L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            img.Save(filename, jpgEncoder, myEncoderParameters);
        }

        private void refreshPageText()
        {
            iPage.Content = (walker.currentIndex + 1) + "/" + walker.count;
        }

        private void refreshProgress()
        {
            setProgressWidth();

            double left = getProgressLeft();
            double bottom = TOOLBAR_HEIGHT - 1;
            if (isFullscreen && !visibleStates[toolbar]) bottom = -4;

            if (walker.isJumpBetweenEnds() || !visibleStates[toolbar])
                animatorJar.translateLeftBottomToI(progress, left, bottom);
            else
                animatorJar.translateLeftBottomTo(progress, left, bottom);
        }

        private void refreshProgressI()
        {
            setProgressWidth();

            double left = getProgressLeft();
            double bottom = isFullscreen && !visibleStates[toolbar] ? -4 : TOOLBAR_HEIGHT - 1;

            animatorJar.translateLeftBottomToI(progress, left, bottom);
        }

        private void setProgressWidth()
        {
            int count = walker.count;
            if (count == 0)
            {
                progress.Width = 0;
            }
            else
            {
                progress.Width = separator.RenderSize.Width / walker.count + PROGRESS_CAP * 2;
            }
        }

        private double getProgressLeft()
        {
            double percent = (double)walker.currentIndex / walker.count;
            return separator.RenderSize.Width * percent + separator.Margin.Left - PROGRESS_CAP;
        }

        private void toggleZoomIn121AndFit()
        {

            if (transformer.isFitWindow)
                transformer.initOne().animate();
            else
                transformer.fitWindow().animate();

            refreshZoomButton();
        }

        private void setSmoothTo(bool on)
        {
            walker.currentImageInfo.smooth = on;
            if (walker.currentImageInfo.smooth)
            {
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            }
            else
            {
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            }
        }

        private void setSmoothByImageResolution()
        {
            if (transformer.bitmap == null) return;

            bool isImageSmall = transformer.bitmap.PixelWidth < 257 && transformer.bitmap.PixelHeight < 257;
            setSmoothTo(!isImageSmall);
        }

        private void refreshSmoothButton()
        {
            if (walker.currentImageInfo != null)
            {
                imageSmooth.Source = loadThemeBitmap(walker.currentImageInfo.smooth ? "icon-smooth-off.png" : "icon-smooth-on.png");
                BUTTON_TO_TIP_MAP[btnSmooth] = visibleStates[btnSmooth] ?
                    I18n._(walker.currentImageInfo.smooth ? "tooltip.disable-smooth" : "tooltip.enable-smooth") : null;
            }
        }

        private void refreshZoomButton()
        {
            if (walker.currentImageInfo != null)
            {
                imageZoom.Source = loadThemeBitmap(walker.currentImageInfo.fitWindow ? "icon-one-to-one.png" : "icon-fit-window.png");
                BUTTON_TO_TIP_MAP[btnZoom] = I18n._(walker.currentImageInfo.fitWindow ? "tooltip.one-to-one" : "tooltip.fit-window");
            }
        }

        private bool isRotated()
        {
            double angleDelta = walker.currentImageInfo.rotationAngle - walker.currentImageInfo.savedRotationAngle;
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

        // Convert a number into the form that has a suffix k, M, G etc.
        private string suffixifyNumber(double num)
        {
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
            animatorJar.translateBottomTo(toolbar, 0)
                .translateBottomTo(page, MARGIN_SIZE + 8)
                .translateBottomTo(separator, TOOLBAR_HEIGHT)
                .fadeIn(separator);
            refreshProgress();
        }
        private void hideToolbar()
        {
            visibleStates[toolbar] = false;

            animatorJar.translateBottomTo(toolbar, -TOOLBAR_HEIGHT)
                .translateBottomTo(page, 8)
                .translateBottomTo(separator, 0)
                .fadeOut(separator);
            refreshProgress();
        }

        private void tryHideToolbar()
        {
            if (isAutoHideToolbar)
            {
                if (mousePos.Y > gridHeight - MARGIN_SIZE * 3 && !visibleStates[toolbar])
                    showToolbar();
                else if (mousePos.Y < gridHeight - MARGIN_SIZE * 3 && visibleStates[toolbar] && !visibleStates[menu])
                    hideToolbar();
            }
        }

        // tooltip

        private void moveTooltipTo(FrameworkElement el)
        {
            Point p = el.TranslatePoint(new Point(0, 0), niv);
            double x = p.X + el.ActualWidth / 2 - tooltip.ActualWidth / 2;

            double maxX = niv.ActualWidth - tooltip.ActualWidth - 4;
            double minX = 4;

            double dx = 0;
            if (x > maxX)
                dx = x - maxX;
            else if (x < minX)
                dx = x - minX;
            moveTooltipTip(dx);

            x -= dx;
            tooltip.Margin = new Thickness(x, 0, 0, 48);
        }

        private void moveTooltipTip(double deltaX)
        {
            PointCollection ps = borderTooltip.Points;
            borderTooltip.Points[3] = new Point(83 + deltaX, ps[3].Y);
            borderTooltip.Points[4] = new Point(75 + deltaX, ps[4].Y);
            borderTooltip.Points[5] = new Point(67 + deltaX, ps[5].Y);
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
        }
        public void hideMarginBottom()
        {
            isMarginBottomExist = false;
            animatorJar.marginTo(container, new Thickness(-MARGIN_SIZE + (visibleStates[info] ? INFO_WIDTH : 0), -MARGIN_SIZE, -MARGIN_SIZE, -MARGIN_SIZE));
        }

        private void toggleTheme()
        {
            Settings.Default.theme = Settings.Default.theme == "dark" ? "light" : "dark";
            Settings.Default.Save();
            setTheme();
        }

        // fullscreen
        public void toggleFullscreen()
        {
            if (isFullscreen)
                exitFullscreen();
            else
                enterFullscreen();

            saveWindowSate();
        }

        // Save the window state. 0 for Normal, 1 for Maximized, 2 for Fullscreen.
        private void saveWindowSate()
        {
            if (isFullscreen)
            {
                Settings.Default.windowState = 2;
            }
            else
            {
                if (window.WindowState == WindowState.Maximized)
                {
                    Settings.Default.windowState = 1;
                }
                else if (window.WindowState == WindowState.Normal)
                {
                    Settings.Default.windowState = 0;
                    Settings.Default.left = window.Left;
                    Settings.Default.top = window.Top;
                    Settings.Default.width = window.Width;
                    Settings.Default.height = window.Height;
                }
            }
            Settings.Default.Save();
        }

        private void enterFullscreen()
        {
            lastWindowState = this.WindowState;

            this.WindowStyle = System.Windows.WindowStyle.None;
            if (this.WindowState != System.Windows.WindowState.Normal)
                this.WindowState = System.Windows.WindowState.Normal;
            this.WindowState = System.Windows.WindowState.Maximized;

            btnExit.Visibility = System.Windows.Visibility.Visible;

            isFullscreen = true;
            isAutoHideToolbar = true;

            hideMarginBottom();
            if (mousePos.Y < gridHeight - MARGIN_SIZE && visibleStates[toolbar])
                hideToolbar();
        }
        private void exitFullscreen()
        {
            isFullscreen = false;
            isAutoHideToolbar = false;

            this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
            this.WindowState = lastWindowState;

            btnExit.Visibility = System.Windows.Visibility.Hidden;

            showMarginBottom();
            if (!visibleStates[toolbar]) showToolbar();
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
            tryHideToolbar();
        }

        private void prevImage()
        {
            setImageRotationBackToZero();
            walker.switchBackward();
            loadFromWalker();
        }

        private void nextImage()
        {
            setImageRotationBackToZero();
            walker.switchForward();
            loadFromWalker();
        }

        private void delete()
        {
            recycle.recieve(walker.currentImageInfo, walker.currentIndex);
            walker.removeCurrentImageInfo();

            if (walker.count > 0) loadFromWalker();

            onWalkerCountChanged();
            onRecycleCountChanged();
        }

        private void undeleteLast()
        {
            if (recycle.count > 0)
            {
                RecycleImageInfo recycleInfo = recycle.undeleteLast();
                walker.insertImageInfo(recycleInfo.originalIndex, recycleInfo.originalInfo);
                walker.currentIndex = recycleInfo.originalIndex;
                loadFromWalker();

                onWalkerCountChanged();
                onRecycleCountChanged();
            }
        }

        private void onRecycleCountChanged()
        {
            if (recycle.count > 0)
            {
                animatorJar.fadeIn(btnUndelete);
                btnUndelete.ToolTip = I18n._("tooltip.undelete");
            }
            else
            {
                animatorJar.fadeOut(btnUndelete);
                animatorJar.fadeOut(tooltip);
                btnUndelete.ToolTip = null;
            }
        }

        // Press key "D" to show something for debugging.
        private void debug()
        {
            //MessageBox.Show(this.Height.ToString());
        }

        private void exit()
        {
            saveWindowSate();
            setImageRotationBackToZero();
            recycle.clean();
            aboutWindow.exit();
            Application.Current.Shutdown();
        }

        // EOC
    }
}
