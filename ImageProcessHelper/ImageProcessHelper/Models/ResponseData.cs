using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessHelper.Models
{
    public class ResponseData
    {
        public int Id { get; set; }
        public double Score { get; set; }
        public decimal ElapsedTime { get; set; }
        public Guid TransactionId { get; set; }
    }
}
