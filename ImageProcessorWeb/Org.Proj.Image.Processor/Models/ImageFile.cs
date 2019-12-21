using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Org.Proj.Image.Processor.Models
{
    public class ImageFile
    {
        public int FileInfoId { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public int FileStatusCode { get; set; }

        public string FileStatusDescription { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public IList<ImageFileRecord> ImageFileDetail { get; set; }
    }
}
