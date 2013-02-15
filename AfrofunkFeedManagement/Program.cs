using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfrofunkFeedManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Application is started");
            bool canMoveNext = true;
            List<Batch> batchList = DatabaseManager.Current.GetPendingBatch();
            foreach (Batch batch in batchList)
            {
                //download file
                if ((batch.Status == BatchStatus.New) && canMoveNext)
                {
                    Downloader download = new Downloader(batch.Merchant.FileStoreLocal, batch.Merchant.UrlFeed);
                    download.DoDownload();

                    canMoveNext = DatabaseManager.Current.BatchMarkFileDownloaded(batch.BatchId);
                    batch.Status = BatchStatus.FileDownloaded;
                }

                //read csv and add to db
                if ((batch.Status == BatchStatus.FileDownloaded) && canMoveNext)
                {
                    //read csv file
                    XmlFileReader reader = new XmlFileReader(batch.Merchant.FileStoreLocal);
                    List<DataItemRaw> itemsRaw = reader.DoRead();

                    //add to db
                    DatabaseManager.Current.CommissionFactoryAddFeedItem(batch, itemsRaw);

                    canMoveNext = DatabaseManager.Current.BatchMarkFileReadAndProcessed(batch.BatchId);
                    batch.Status = BatchStatus.FileProcessed; 
                }

                //get new or update for pushing and push to remote server
                if ((batch.Status == BatchStatus.FileProcessed) && canMoveNext)
                {
                    //get new or update for pushing
                    List<DataItemPush> pushItems = DatabaseManager.Current.GetDataForPush(batch.BatchId);

                    //pushing to remote server
                    RemotePush rm = new RemotePush();
                    rm.DoPush(batch, pushItems);
                    canMoveNext = DatabaseManager.Current.BatchMarkDataPushed(batch.BatchId);
                    batch.Status = BatchStatus.Pushed;

                    //do finalise push
                    rm.DoPushFinalise();
                }

                //1. download from remote server the list of SKU already published.  http://www.afrofunk.com/store/theiconic/sku-online  will return in format of xml ???? or json ????
                //2. so we can look at what need to be take offline (what take offline are SKU that we didn't get them TODAY)
                //3. push to remote server about and SKU that need to be take offline
                //    3.1 do push finalise for update brand name and calculate new items
            }

            Console.WriteLine("\nApplication processing is finished... press any key to exit the application");
            Console.Read();
        }
    }
}
