using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfrofunkFeedManagement
{
    /*
     * this is raw data from Commission Factory-TheIconic CSV file
     *   all field name are matched from the csv except Image URL
     */ 
    public class DataItemRaw
    {
        public DateTime DateCreated { get; set; }       //index: 0
        public DateTime DateModified { get; set; }      //index: 1
        public string SKU { get; set; }                 //index: 2
        public string Name { get; set; }                //index: 3
        public string Category { get; set; }            //index: 4
        public string Description { get; set; }         //index: 5
        public string Url { get; set; }                 //index: 6
        public string OriginalUrl { get; set; }         //index: 7

        public string ImageUrl { get; set; }            //index: 8 

        public decimal Price { get; set; }              //index: 15
        public string Brand { get; set; }               //index: 16
        public string Colour { get; set; }              //index: 17
        public string Currency { get; set; }            //index: 18
        public string DeliveryCost { get; set; }        //index: 19
        public string DeliveryTime { get; set; }        //index: 20
        public string Feature { get; set; }             //index: 21
        public string Gender { get; set; }              //index: 22
        public string Size { get; set; }                //index: 23
        public string StockLevel { get; set; }          //index: 24
    }
}
