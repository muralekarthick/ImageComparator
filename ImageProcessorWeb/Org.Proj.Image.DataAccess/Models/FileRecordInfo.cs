using System;
using System.Collections.Generic;

namespace Org.Proj.Image.DataAccess.Models
{
    public partial class FileRecordInfo
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
        public decimal? Score { get; set; }
        public TimeSpan? ElapsedTime { get; set; }
        public byte? RecordStatusCode { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public virtual CsxFileInfo File { get; set; }
        public virtual FileRecordStatusCode RecordStatusCodeNavigation { get; set; }
    }
}
