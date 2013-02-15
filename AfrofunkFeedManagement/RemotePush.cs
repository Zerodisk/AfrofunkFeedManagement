using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;

namespace AfrofunkFeedManagement
{
    /*
     * handle http post new/modified feed data to remot server (www.afrofunk.com.au)
     * 
     */ 
    public class RemotePush
    {
        private string _url_push;
        private string _url_finalise;

        public RemotePush()
        {
            _url_push = ConfigManager.Current.UrlRemotePush;
            _url_finalise = ConfigManager.Current.UrlRemotePushFinalise;
        }

        //main function to push list of items
        public void DoPush(Batch batch, List<DataItemPush> items)
        {
            Console.WriteLine("RemotePush.DoPush is started");
            int totalItems = items.Count;
            int count = 1;
            foreach (DataItemPush item in items)
            {
                if (DoPush(batch, item))
                {
                    DatabaseManager.Current.UpdateDataPushSent(item.PushId);
                }

                Console.WriteLine(count.ToString() + " of " + totalItems.ToString());
                count = count + 1;
            }
            Console.WriteLine("RemotePush.DoPush is finished");
        }

        //push last finalise update once everything has been done
        public bool DoPushFinalise()
        {
            Console.WriteLine("RemotePush.DoPushFinalise is started");
            try
            {
                string result = Post(_url_finalise, new NameValueCollection()
                                {
                                    {"secret_code", ConfigManager.Current.RemoteSecretKey}
                                });


                if (result == "OK")
                {
                    Console.WriteLine("RemotePush.DoPushFinalise has been successed");
                    return true;
                }
                else
                {
                    int trimLength = 100;
                    if (result.Length < 100) { trimLength = result.Length; }
                    Console.WriteLine("RemotePush.DoPushFinalise seems to have some problem, remote server return: " + result.Substring(0, trimLength));        //truncate to up to 100 characters error
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RemotePush.DoPushFinalise has failed: " + ex.ToString());
                return false;
            }
        }

        //push a single item to remote server
        private bool DoPush(Batch batch, DataItemPush item)
        {
            try
            {
                string result = Post(_url_push, new NameValueCollection()
                                {
                                    {"secret_code",   ConfigManager.Current.RemoteSecretKey},
                                    {"sku",           item.SKU.ToString()},
                                    {"product_name",  item.ProductName.ToString()},
                                    {"description",   item.Description.ToString()},
                                    {"url",           item.Url.ToString()},
                                    {"original_url",  item.OriginalUrl.ToString()},
                                    {"image_url",     item.ImageUrl.ToString()},
                                    {"price",         item.Price.ToString("#.##")},
                                    {"delivery_cost", item.DeliveryCost.ToString()},
                                    {"currency_code", item.CurrencyCode.ToString()},
                                    {"brand",         item.Brand.ToString()},
                                    {"colour",        item.Colour.ToString()},
                                    {"gender",        item.Gender.ToString()},
                                    {"size",          item.Size.ToString()},
                                    {"mid",           batch.Merchant.MerchantID.ToString()}
                                });
                

                if (result == "OK"){
                    return true;
                }
                else{
                    int trimLength = 100;
                    if (result.Length < 100) { trimLength = result.Length; }
                    Console.WriteLine("RemotePush.DoPush seems good, SKU: " + item.SKU + ", but remote server return: " + result.Substring(0, trimLength));        //truncate to up to 100 characters error
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RemotePush.DoPush has failed: " + ex.ToString());
                return false;
            }
        }

        //method to do http post for pairs value
        private string Post(string url, NameValueCollection pairs)
        {
            byte[] byteResponse = null;
            using (WebClient client = new WebClient())
            {
                byteResponse = client.UploadValues(url, pairs);
            }

            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            return enc.GetString(byteResponse);
        }

    }
}
