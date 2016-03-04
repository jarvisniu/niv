using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niv
{
    class RecycleImageInfo
    {
        // The original ImageInfo, used to undelete it.
        public ImageInfo originalInfo;

        // The id(filename) in recycle folder.
        public int id;

        // The original index in the image list. Used to insert it in the original index.
        public int originalIndex;

        // New filename
        public string newFilename;

        public RecycleImageInfo(ImageInfo info, int id, int originalIndex)
        {
            this.originalInfo = info;
            this.id = id;
            this.originalIndex = originalIndex;
        }

        // EOC
    }
}
