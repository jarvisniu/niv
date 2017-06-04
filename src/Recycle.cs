using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;

namespace Niv
{
    class Recycle
    {
        // Folder of recycle
        private static string recyclePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\NivRecycle\";
        private DirectoryInfo di = new DirectoryInfo(recyclePath);

        // Id counter for every recycle files.
        private int recycleId = 0;

        // List of deleted files
        private List<RecycleImageInfo> recycleInfos = new List<RecycleImageInfo>();

        // Get the image count in recycle list.
        public int count
        {
            get
            {
                return recycleInfos.Count;
            }
        }

        public Recycle()
        {
            clean();
        }
        
        // Move a image here.
        public void recieve(ImageInfo info, int index)
        {
            // Create the recycle folder if not exists.
            di.Refresh();
            if (!di.Exists) di.Create();

            // Create the info
            RecycleImageInfo recycleInfo = new RecycleImageInfo(info, recycleId, index);

            // Move the file and rename it.
            FileInfo fi = new FileInfo(info.filename);
            string newName = recyclePath + recycleId + Path.GetExtension(info.filename);
            recycleInfo.newFilename = newName;
            fi.MoveTo(newName);

            // Record the info
            recycleInfos.Add(recycleInfo);

            recycleId++;
        }

        // Move back the last deleted image.
        public RecycleImageInfo undeleteLast()
        {
            return undelete(recycleInfos.Count - 1);
        }

        // Move a image out of here, return the original index.
        public RecycleImageInfo undelete(int index)
        {
            RecycleImageInfo recycleInfo = recycleInfos[index];

            // Move the file back.
            FileInfo fi = new FileInfo(recycleInfo.newFilename);
            fi.MoveTo(recycleInfo.originalInfo.filename);

            recycleInfos.Remove(recycleInfo);

            return recycleInfo;
        }

        // Delete all the files in recycle and the recycle folder.
        public void clean()
        {
            di.Refresh();
            if (di.Exists) di.Delete(true);
        }

        // EOC
    }
}
