using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingEngine
{
    public class ResponseData
    {
        public int Id { get; set; }
        public double Score { get; set; }
        public decimal ElapsedTime { get; set; }
        public Guid TransactionId { get; set; }
    }
}
