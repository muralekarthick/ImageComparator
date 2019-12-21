﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Proj.Image.Common.Model
{
    public class ImageFileRecordInfo
    {
        public int FileRecordId { get; set; }
        public int FileId { get; set; }
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
        public double Score { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public int RecordStatusCode { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
