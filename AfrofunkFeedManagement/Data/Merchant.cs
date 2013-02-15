using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfrofunkFeedManagement
{
    public class Merchant
    {
        public int MerchantID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlFeed { get; set; }
        public string FileStoreLocal { get; set; }
    }
}
