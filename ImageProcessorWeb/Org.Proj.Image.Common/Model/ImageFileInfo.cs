using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Proj.Image.Common.Model
{
    public class ImageFileInfo
    {
        public int FileInfoId { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public int FileStatusCode { get; set; }

        public string FileStatusDescription { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public IList<ImageFileRecordInfo> ImageFileDetail { get; set; }
    }
}
