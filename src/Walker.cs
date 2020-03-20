/**
 * Walker: Walk the folder which containing the image and get the rest images.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using System.Threading.Tasks;

namespace Niv
{
    class FolderWalker
    {
        /// Variables ----------------------------------------------------------

        // Supported image format, used to recognize the image files.
        public static string SUPPORTED_IMAGE_EXT = ".jpg .jpeg .jfif .png .bmp .ico .tif .tiff .gif .dds .webp";

        // The fullname of current loaded folder
        private string currentFolderName;

        // Image info list
        private List<ImageInfo> imageInfos = new List<ImageInfo>();

        // The index of last displayed image. Used to see if they are adjacent or at the two ends.
        private int lastIndex = -1;

        /// Properties ---------------------------------------------------------

        // Private values of properties
        private int _currentIndex = -1;

        // The index of current displaying image in the image list
        public int currentIndex
        {
            get
            {
                return _currentIndex;
            }
            set
            {
                lastIndex = _currentIndex;
                _currentIndex = value;
            }
        }

        // Get the ImageInfo of current image
        public ImageInfo currentImageInfo
        {
            get
            {
                return imageInfos.Count > 0 ? imageInfos[_currentIndex] : null;
            }
        }

        // The count of images in the folder
        public int count
        {
            get
            {
                return imageInfos.Count;
            }
        }

        /// Methods ------------------------------------------------------------

        // Check if a file is a supported format
        public static bool isFormatSupported(string filename)
        {
            string ext = Path.GetExtension(filename).ToLower();
            return ext.Length > 0 && SUPPORTED_IMAGE_EXT.IndexOf(ext) > -1;
        }

        // Get the index of a filename in the list. Return -1 if not exists.
        public int getImageFileIndex(string filename)
        {
            for (int i = 0; i < imageInfos.Count; i++)
                if (imageInfos[i].filename == filename.ToLower()) return i;
            return -1;
        }

        // Check if the last image switch is jumping between the ends of image list.
        public bool isJumpBetweenEnds()
        {
            return this.count > 2 && Math.Abs(currentIndex - lastIndex) > 1;
        }

        // Load all the image files in the specified folder.
        public void loadFolder(string droppedFileName)
        {
            string folderPath = Path.GetDirectoryName(droppedFileName);
            if (folderPath.Equals(currentFolderName)) return;

            DirectoryInfo di = new DirectoryInfo(folderPath);
            if (di.Exists)
            {
                imageInfos.Clear();
                FileInfo[] fis = di.GetFiles();
                int i = 0;
                foreach (FileInfo fi in fis)
                {
                    string filename = fi.FullName;
                    if (isFormatSupported(filename))
                    {
                        imageInfos.Add(new ImageInfo(filename));
                        if (droppedFileName == filename)
                            currentIndex = i;
                        i++;
                    }
                }
            }

            //MessageBox.Show(" Count: " + imageInfos.Count
            //    + "\n Index: " + currentIndex
            //    + "\n folderPath: " + folderPath
            //    + "\n currentFolderName: " + currentFolderName
            //    );

            currentFolderName = folderPath;
        }

        // Switch to the previous image in the list and return if the switch success.
        // Switch will fail if imageList.Count < 2
        public void switchBackward()
        {
            if (imageInfos.Count > 1)
            {
                currentIndex--;
                if (currentIndex == -1) currentIndex = imageInfos.Count - 1;
            }
        }

        // Switch to the next image in the list.
        public void switchForward()
        {
            if (imageInfos.Count > 1)
            {
                currentIndex++;
                if (currentIndex == imageInfos.Count) currentIndex = 0;
            }
        }

        // Remove current file info from the list.
        public void removeCurrentImageInfo()
        {
            imageInfos.RemoveAt(currentIndex);
            if (currentIndex == imageInfos.Count) currentIndex = 0;
        }

        public void insertImageInfo(int index, ImageInfo info)
        {
            imageInfos.Insert(index, info);

        }

        // EOC
    }
}
