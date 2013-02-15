using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfrofunkFeedManagement
{
    public class Batch
    {
        public int BatchId { get; set; }
        public Merchant Merchant { get; set; }
        public BatchStatus Status { get; set; }
       
    }

}
