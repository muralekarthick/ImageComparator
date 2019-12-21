using System;
using System.Collections.Generic;

namespace Org.Proj.Image.DataAccess.Models
{
    public partial class FileStatusCode
    {
        public FileStatusCode()
        {
            CsxFileInfo = new HashSet<CsxFileInfo>();
        }

        public byte StatusCode { get; set; }
        public string Description { get; set; }

        public virtual ICollection<CsxFileInfo> CsxFileInfo { get; set; }
    }
}
