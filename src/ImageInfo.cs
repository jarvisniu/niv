using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Niv
{
    class ImageInfo
    {
        // The filename of this image
        public string filename;

        // If this image is in Fit-Window mode, say, its size following the window size.
        public bool fitWindow = true;

        // If this image is smooth on rendering (anti-alias)
        public bool smooth = true;

        // Current rotation angle of this image
        public double rotationAngle = 0;

        // At which rotation angle this image was saved
        public double savedRotationAngle = 0;

        // If this image never been paned or zoomed
        public bool virgin = true;

        // If this image is broken or not supported
        public bool broken = true;

        // Properties
        private Point _center = new Point(0.5, 0.5);
        private double _scale;

        // The normalized center position of this image
        public double scale
        {
            get
            {
                return _scale;
            }
            set
            {
                this._scale = value;
                this.virgin = false;
            }
        }

        // The zoom scale of this image
        public Point center
        {
            get
            {
                return _center;
            }
            set
            {
                this._center = value;
                this.virgin = false;
            }
        }

        // Constructor
        public ImageInfo(string filename)
        {
            this.filename = filename;
        }
        
        // EOC
    }
}
