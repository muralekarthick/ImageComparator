using System;
using System.Collections.Generic;

namespace Org.Proj.Image.DataAccess.Models
{
    public partial class FileRecordStatusCode
    {
        public FileRecordStatusCode()
        {
            FileRecordInfo = new HashSet<FileRecordInfo>();
        }

        public byte StatusCode { get; set; }
        public string Description { get; set; }

        public virtual ICollection<FileRecordInfo> FileRecordInfo { get; set; }
    }
}
