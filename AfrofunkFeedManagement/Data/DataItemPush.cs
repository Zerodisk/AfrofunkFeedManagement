using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfrofunkFeedManagement
{
    /*
     * there are the data that will be pushed to remote server
     *   all variable name will be matched with data post name 
     */ 
    public class DataItemPush
    {
        public int PushId { get; set; }
        public int BatchId { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string OriginalUrl { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string DeliveryCost { get; set; }
        public string CurrencyCode { get; set; }
        public string Brand { get; set; }
        public string Colour { get; set; }
        public string Gender { get; set; }
        public string Size { get; set; }

    }
}
