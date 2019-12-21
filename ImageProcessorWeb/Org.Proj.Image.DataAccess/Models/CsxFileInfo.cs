using System;
using System.Collections.Generic;

namespace Org.Proj.Image.DataAccess.Models
{
    public partial class CsxFileInfo
    {
        public CsxFileInfo()
        {
            FileRecordInfo = new HashSet<FileRecordInfo>();
        }

        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public byte? FileStatusCode { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public virtual FileStatusCode FileStatusCodeNavigation { get; set; }
        public virtual ICollection<FileRecordInfo> FileRecordInfo { get; set; }
    }
}
