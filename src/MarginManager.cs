﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using com.jarvisniu;

using System.Timers;

namespace Niv
{
    class MarginManager
    {
        // TODO remove this
        // In this fullwindow mode:
        //   1. Image size will follow the window size changing.
        //   2. When the image is scaling, the scaling state will not saved to folder walker.
        //   3. This mode will be breaking after the user zoom or pan the image proactively.

        /// Constants ----------------------------------------------------------

        // Set image.center to this value will move the image to the center of window.
        private static Point WINDOW_CENTER_POSITION = new Point(0.5, 0.5);

        // Determine how quickly the image transforms
        private double EASE_FACTOR = 0.2;

        /// Components ---------------------------------------------------------

        // The image container, a Grid widget
        private Grid container;

        // The Image widget
        private Image image;

        // The Bitmap data in Image widget
        private BitmapImage bitmap;

        // The destinatio margin of Image
        Thickness marginDestination = new Thickness(0);

        // FolderWalker is used to save the current image transform info
        FolderWalker walker;

        // Event type define
        public delegate void DoubleMethod();

        // Outside event interface
        public DoubleMethod onScaleChanged;

        // Transform animation timer
        Timer timerAnimation = new Timer(0.01);

        // Frames per second measure component, for debugging
        FPS fps = new FPS();

        /// Properties ---------------------------------------------------------

        // Proptertie private variables
        private Point _center = WINDOW_CENTER_POSITION;
        private double _scale = 1;

        // Normalized[0~1] position on the image where the container center at
        public Point center
        {
            get
            {
                return _center;
            }
            set
            {
                _center = value;
                walker.currentImageInfo.center = _center;
            }
        }

        // The transform scale of image
        public double scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
                walker.currentImageInfo.scale = _scale;
                if (onScaleChanged != null) onScaleChanged();
            }
        }

        public bool isFullwindow
        {
            get
            {
                //if (walker.currentImageInfo != null)
                    return walker.currentImageInfo.isFullwindow;
                //else
                //    return true;
            }
        }

        // Constructor
        public MarginManager(Grid _grid, Image _image, BitmapImage _bitmap, FolderWalker _walker)
        {
            container = _grid;
            image = _image;
            bitmap = _bitmap;
            walker = _walker;

            timerAnimation.Elapsed += timerAnimation_Elapsed;
        }
        
        // Perform a step of easing animation
        void timerAnimation_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Approach to margin
            if (isMarginClose())
            {
                image.Dispatcher.Invoke(new Action(delegate
                {
                    image.Margin = marginDestination;
                }));

                timerAnimation.Stop();
            }
            else
            {
                fps.update();
                image.Dispatcher.Invoke(new Action(delegate
                {
                    image.Margin = getEasedMargin();
                }));
            }
        }

        // Check if the current Margin is close enough to the destenation
        private bool isMarginClose()
        {
            Thickness marginFrom = new Thickness();
            image.Dispatcher.Invoke(new Action(delegate
            {
                marginFrom = image.Margin;
            }));
            Thickness marginTo = marginDestination;
            return Math.Abs(marginFrom.Left - marginTo.Left) < 1
                && Math.Abs(marginFrom.Right - marginTo.Right) < 1
                && Math.Abs(marginFrom.Top - marginTo.Top) < 1
                && Math.Abs(marginFrom.Bottom - marginTo.Bottom) < 1;
        }

        // Calculate the eased Margin of the image
        private Thickness getEasedMargin()
        {
            Thickness marginFrom = new Thickness();
            image.Dispatcher.Invoke(new Action(delegate
            {
                marginFrom = image.Margin;
            }));
            Thickness marginTo = marginDestination;
            Thickness mEase = new Thickness(
                marginFrom.Left + (marginTo.Left - marginFrom.Left) * EASE_FACTOR,
                marginFrom.Top + (marginTo.Top - marginFrom.Top) * EASE_FACTOR,
                marginFrom.Right + (marginTo.Right - marginFrom.Right) * EASE_FACTOR,
                marginFrom.Bottom + (marginTo.Bottom - marginFrom.Bottom) * EASE_FACTOR
            );
            return mEase;
        }

        // Switch to another image // TODO use setImage not Bitmap
        public void setImage(BitmapImage bitmap)
        {
            this.bitmap = bitmap;
            // TODO reset
            //zoom121();
            //initOne();
        }

        // Set to initial state used when the image appears the first time
        public MarginManager initOne()
        {
            exitFullwindowMode();

            screenCenter();
            scale = 1;

            setMarginDesByKeys();

            return this;
        }

        // Enter fullscreen mode when TODO rename to FitWindow, or fullwindow
        public MarginManager fullwindow()
        {
            if (bitmap == null) return this;

            fullsize();
            screenCenter();
            
            walker.currentImageInfo.isFullwindow = true;

            calcMarginDestination();

            return this;
        }

        // Set the image to 1:1, but keep the image position
        public MarginManager fullsize()
        {
            Size gridSize = container.RenderSize;
            double scaleX = (gridSize.Width - NivWindow.MARGIN_SIZE * 2) / bitmap.Width;
            double scaleY = (gridSize.Height - NivWindow.MARGIN_SIZE * 2) / bitmap.Height;

            scale = Math.Min(scaleX, scaleY);

            return this;
        }

        // Move the image to the center of window, but not zoom it.
        public MarginManager screenCenter()
        {
            center = WINDOW_CENTER_POSITION;

            return this;
        }

        // Zoom the image to 1:1 with the image center TODO or window center
        public MarginManager zoom121AtStand()
        {
            zoomTo(1, getImageCenter());

            return this;
        }

        // Zoom-by the image with the image center TODO or window center
        public MarginManager zoomAtStand(double deltaScale)
        {
            zoomTo(scale * deltaScale, getImageCenter());

            return this;
        }

        // Zoom-by the image at pivot which is the normalized position in the image.
        public MarginManager zoomBy(double dS, Point pivot)
        {
            exitFullwindowMode();

            Size gridSize = container.RenderSize;

            marginDestination = new Thickness(
                marginDestination.Left * dS + pivot.X * (1 - dS),
                marginDestination.Top * dS + pivot.Y * (1 - dS),
                marginDestination.Right * dS + (gridSize.Width - pivot.X) * (1 - dS),
                marginDestination.Bottom * dS + (gridSize.Height - pivot.Y) * (1 - dS)
            );

            scale *= dS;

            double L = gridSize.Width / 2 - marginDestination.Left;
            double T = gridSize.Height / 2 - marginDestination.Top;
            double X = L / (gridSize.Width - marginDestination.Left - marginDestination.Right);
            double Y = T / (gridSize.Height - marginDestination.Top - marginDestination.Bottom);
            center = new Point(X, Y);

            return this;
        }

        // Zoom-to the image at pivot which is the normalized position in the image.
        public MarginManager zoomTo(double s, Point pivot)
        {
            double dS = s / scale;
            zoomBy(dS, pivot);

            return this;
        }

        // Calculate the `marginDestination` or TODO what do about fullscreen?
        public MarginManager setMarginDesByKeys()
        {
                return calcMarginDestination();
        }

        // Calculate the `marginDestination` from `center` and `scale`
        public MarginManager calcMarginDestination()
        {
            if (bitmap == null) return this;

            /* [ Algorithm ]
             * From: center, scale, To: Margin
             * Width = imageWidth * scale; Height = imageHeight * scale;
             * Left = gridWidth/2 - Width * center.X, Top ..
             * Right = gridWidth - Left - Width, Bottom ..
             */

            Size imageSize = image.RenderSize;
            Size gridSize = container.RenderSize;

            double W = bitmap.Width * scale;
            double H = bitmap.Height * scale;
            //double W = bitmap.PixelWidth * scale;
            //double H = bitmap.PixelHeight * scale;
            double L = gridSize.Width / 2 - W * center.X;
            double T = gridSize.Height / 2 - H * center.Y;
            double R = gridSize.Width - L - W;
            double B = gridSize.Height - T - H;
            //MessageBox.Show("W: " + W + ", H: " + H);
            marginDestination = new Thickness(L, T, R, B);

            return this;
        }

        // Pan the `margin-destination` by a distance, in unit "px"
        public MarginManager pan(double dX, double dY)
        {
            exitFullwindowMode();

            Thickness m = marginDestination;
            marginDestination = new Thickness(m.Left + dX, m.Top + dY, m.Right - dX, m.Bottom - dY);

            return this;
        }

        // Set `scale` and `center` values
        public MarginManager setScaleCenter(double scale, Point center)
        {
            this.scale = scale;
            this.center = center;

            return this;
        }
        
        // Set the `scale`  value
        public MarginManager setScale(double scale)
        {
            this.scale = scale;

            return this;
        }

        // Set the `center`  value
        public MarginManager setCenter(Point center)
        {
            this.center = center;

            return this;
        }

        // Apply the `marginDestination` to the `Margin` of `image` instantly
        public MarginManager apply()
        {
            image.Margin = marginDestination;
            return this;
        }

        // Apply the `marginDestination` to the `Margin` of  image with animation effect
        public MarginManager animate()
        {
            timerAnimation.Start();
            return this;
        }

        // Update scale, center to keep consistent with margin
        public MarginManager setScaleCenterWithImage()
        {
            if (bitmap == null) return this;

            scale = image.RenderSize.Width / bitmap.Width;

            Size imageSize = image.RenderSize;
            Size gridSize = container.RenderSize;

            double L = gridSize.Width / 2 - image.Margin.Left;
            double T = gridSize.Height / 2 - image.Margin.Top;
            double X = L / imageSize.Width;
            double Y = T / imageSize.Height;
            center = new Point(X, Y);

            return this;
        }

        // Exit `fullwindow` mode
        public MarginManager exitFullwindowMode()
        {
            if (walker.currentImageInfo != null)
                walker.currentImageInfo.isFullwindow = false;

            return this;
        }

        // Get the coordinates of `image`'s position in the `containter`. The unit is px.
        private Point getImageCenter()
        {
            Thickness realMargin = image.Margin;
            double x = realMargin.Left + image.RenderSize.Width / 2;
            double y = realMargin.Top + image.RenderSize.Height / 2;
            return new Point(x, y);
        }

        // EOC
    }
}