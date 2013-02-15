using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


namespace AfrofunkFeedManagement
{
    public class XmlFileReader
    {

        private string _fullPathFileName;

        public XmlFileReader(string fullPathFileName)
        {
            Console.WriteLine("Initial XmlFileReader class");
            _fullPathFileName = fullPathFileName;
        }

        public List<DataItemRaw> DoRead()
        {
            List<DataItemRaw> result = new List<DataItemRaw>();

            try
            {
                Console.WriteLine("Start reading the XML file: " + _fullPathFileName + "   .....");
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_fullPathFileName);

                //serealization
                Schema.FeedItems oResult = new Schema.FeedItems();
                Schema.FeedItems feedItems = (Schema.FeedItems)DeserializaXml(xmlDoc.InnerXml, oResult.GetType());
                if (feedItems != null)
                {
                    Console.WriteLine("Number of items read from XML: " + feedItems.Items.Length.ToString());
                }

                //loop through
                //  create DateItemRaw
                //  add to result
                foreach (Schema.FeedItemsFeedItem item in feedItems.Items)
                {
                    DataItemRaw rawItem = new DataItemRaw();
                    rawItem.DateCreated     = DateTime.ParseExact(item.DateCreated, "yyyy-MM-dd HH:mm:ss", null);
                    rawItem.DateModified    = DateTime.ParseExact(item.DateModified, "yyyy-MM-dd HH:mm:ss", null);
                    rawItem.SKU             = item.SKU;
                    rawItem.Name            = item.Name;
                    rawItem.Category        = item.Category;
                    rawItem.Description     = item.Description;
                    rawItem.Url             = item.Url;
                    rawItem.OriginalUrl     = item.OriginalUrl;
                    rawItem.ImageUrl        = item.Image;
                    rawItem.Price           = Convert.ToDecimal(item.Price);
                    rawItem.Brand           = item.Brand;
                    rawItem.Colour          = item.Colour;
                    rawItem.Currency        = item.Currency;
                    rawItem.DeliveryCost    = item.DeliveryCost;
                    rawItem.DeliveryTime    = item.DeliveryTime;
                    rawItem.Feature         = item.Features;
                    rawItem.Gender          = item.Gender;
                    rawItem.Size            = item.Size;
                    rawItem.StockLevel      = item.StockLevel;

                    result.Add(rawItem);
                }


                Console.WriteLine("Finishing read the XML file: " + _fullPathFileName + "   .....");
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine("Read xml file failed\n" + e.ToString());
                return result;  //still return result although got exception
            }
        }



        private string[] TrimValue(string[] items)
        {
            for (int i = 0; i <= items.Length - 1; i++)
            {
                items[i] = items[i].Trim();
            }
            return items;
        }

        private object DeserializaXml(string xml, System.Type objectType)
        {
            object result = null;

            XmlTextReader oXmlReader;
            System.IO.StringReader oStringReader;
             
            try{
                //parse response
                XmlSerializer oDeserializer = new XmlSerializer(objectType);
                oStringReader = new System.IO.StringReader(xml);
                oXmlReader = new XmlTextReader(oStringReader);

                //try to deserialize
                if (oDeserializer.CanDeserialize(oXmlReader)) {
                    result = oDeserializer.Deserialize(oXmlReader);
                }
                else{
                    try{
                        /* **********************************************
                        '
                        ' error!! pws xml response can't be deserialize
                        '
                        ' do something here 
                        '
                        '********************************************** */
                    }
                    catch(Exception ex){
                        throw ex;
                    }
                }
            }
            catch (Exception ex){
                throw ex;
            }
            finally{
                oXmlReader = null;
                oStringReader = null;
            }

            return result;
        }


    }
}
