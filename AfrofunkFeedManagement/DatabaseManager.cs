using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace AfrofunkFeedManagement
{
    public class DatabaseManager
    {
        private static DatabaseManager _Current;
        private SqlConnection sqlConn;

        public static DatabaseManager Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new DatabaseManager();
                    _Current.OpenConnection();
                }

                if (_Current.SqlConnectionState != ConnectionState.Open)
                {
                    _Current.OpenConnection();
                }

                return _Current;
            }
        }

        public ConnectionState SqlConnectionState
        {
            get { return sqlConn.State; }
        }

        public void OpenConnection()
        {
            try
            {
                sqlConn = new SqlConnection(ConfigManager.Current.ConnectionString);
                sqlConn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("DB open connection failed");
                throw e;
            }
        }

        /*
        public List<Merchant> GetMerchantList()
        {
            List<Merchant> result = new List<Merchant>();
            string sql = "select * from Merchant where IsActive = 1";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.CommandType = CommandType.Text;
            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        Merchant merchant = new Merchant();
                        merchant.MerchantID = (int)reader["MID"];
                        merchant.Name = reader["Name"].ToString();
                        merchant.Description = reader["Description"].ToString();
                        merchant.UrlFeed = reader["URLFeed"].ToString();

                        merchant.FileStoreLocal = reader["FileStoreLocal"].ToString();
                        merchant.FileStoreLocal = merchant.FileStoreLocal.Replace("YYYYMMDD", DateTime.Now.ToString("yyyyMMdd"));
                        
                        result.Add(merchant);
                    }
                    reader.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("DatabaseManager.GetMerchantList is failed - " + ex.ToString());
                return null;
            }
        }
        */

        public List<Batch> GetPendingBatch()
        {
            Console.WriteLine("DatabaseManager.GetPendingBatch - getting pending batch to run");
            List<Batch> result = new List<Batch>();
            string sql = "Select m.MID, m.Name, m.URLFeed, m.FileStoreLocal, b.* From FeedBatch b inner join Merchant m on b.MID = m.MID " +
                         "Where m.IsActive = 1 and b.DatePushed is NULL";
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.CommandType = CommandType.Text;
            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        Batch batch = new Batch();

                        batch.BatchId = (int)reader["BatchId"];
                          Merchant merchant = new Merchant();
                          merchant.MerchantID = (int)reader["MID"];
                          merchant.Name = reader["Name"].ToString();
                          merchant.UrlFeed = reader["URLFeed"].ToString();
                          merchant.FileStoreLocal = reader["FileStoreLocal"].ToString();
                          merchant.FileStoreLocal = merchant.FileStoreLocal.Replace("YYYYMMDD", DateTime.Now.ToString("yyyyMMdd"));
                        batch.Merchant = merchant;

                        //setup batch status depend on date stamp
                        if (reader.IsDBNull(reader.GetOrdinal("DateDownloaded")) &&
                            reader.IsDBNull(reader.GetOrdinal("DateProcessed")) &&
                            reader.IsDBNull(reader.GetOrdinal("DatePushed")))
                        {
                            batch.Status = BatchStatus.New; 
                        }
                        else if (!reader.IsDBNull(reader.GetOrdinal("DateDownloaded")) &&
                                 reader.IsDBNull(reader.GetOrdinal("DateProcessed")) &&
                                 reader.IsDBNull(reader.GetOrdinal("DatePushed")))
                        {
                            batch.Status = BatchStatus.FileDownloaded; 
                        }
                        else if (!reader.IsDBNull(reader.GetOrdinal("DateDownloaded")) &&
                                 !reader.IsDBNull(reader.GetOrdinal("DateProcessed")) &&
                                 reader.IsDBNull(reader.GetOrdinal("DatePushed")))
                        {
                            batch.Status = BatchStatus.FileProcessed;
                        }

                        result.Add(batch);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DatabaseManager.GetPendingBatch is failed - " + ex.ToString());
            }

            Console.WriteLine("DatabaseManager.GetPendingBatch - number of batch is: " + result.Count.ToString());
            return result;
        }


        //update push date when sent
        public bool UpdateDataPushSent(int pushId)
        {
            string sql = "Update FeedChangePush Set DatePushed = getDate() where PushId = " + pushId.ToString();
            try
            {
                SqlCommand cmd = new SqlCommand(sql, sqlConn);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("DatabaseManager.UpdateDataPushSent is failed at PushId: " + pushId.ToString() + " - " + e.ToString());
                return false;
            }
        }


        //get list of data that ready to be pushed
        public List<DataItemPush> GetDataForPush(int batchId)
        {
            Console.WriteLine("DatabaseManager.GetDataForPush - read data for push");
            List<DataItemPush> result = new List<DataItemPush>();
            string sql = "Select PushId, BatchId, SKU, ProductName, Description, Url, OriginalUrl, ImageUrl, Price, DeliveryCost, " +
                         "CurrencyCode, Brand, Colour, Gender, Size " +
                         "From FeedChangePush where BatchId = " + batchId + " And DatePushed is Null";

            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.CommandType = CommandType.Text;
            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        DataItemPush item = new DataItemPush();
                        item.PushId = (int)reader["PushId"];
                        item.BatchId = batchId;
                        item.SKU = reader["SKU"].ToString();
                        item.ProductName = reader["ProductName"].ToString();
                        item.Description = reader["Description"].ToString();
                        item.Url = reader["Url"].ToString();
                        item.OriginalUrl = reader["OriginalUrl"].ToString();
                        item.ImageUrl = reader["ImageUrl"].ToString();
                        item.Price = (decimal)reader["Price"];
                        item.DeliveryCost = reader["DeliveryCost"].ToString();
                        item.CurrencyCode = reader["CurrencyCode"].ToString();
                        item.Brand = reader["Brand"].ToString();
                        item.Colour = reader["Colour"].ToString();
                        item.Gender = reader["Gender"].ToString();
                        item.Size = reader["Size"].ToString();

                        result.Add(item);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DatabaseManager.GetDataForPush is failed at BatchId: " + batchId.ToString() + " - " + ex.ToString());
            }

            Console.WriteLine("DatabaseManager.GetDataForPush - Number of push items: " + result.Count.ToString());
            return result;
        }


        //return list of string for selected theiconic brand name
        public List<string> GetSelectedBrandName(int merchantId)
        {
            Console.WriteLine("Read TheIconic selected brand list");
            List<string> result = new List<string>();
            string sql = "select * from CFBrandSelected where MID = " + merchantId.ToString();
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            cmd.CommandType = CommandType.Text;
            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        result.Add(reader["BrandName"].ToString().Trim().ToLower());  //convert to lower case
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DatabaseManager.GetTheIconicSelectedBrandName is failed - " + ex.ToString());
            }

            Console.WriteLine("Number of TheIconic selected brand: " + result.Count.ToString());
            return result;
        }

        public void CommissionFactoryAddFeedItem(Batch batch, List<DataItemRaw> itemsRaw)
        {
            int batchId = batch.BatchId;
            List<string> selectedBrand = GetSelectedBrandName(batch.Merchant.MerchantID);

            try
            {
                foreach (DataItemRaw itemRaw in itemsRaw)
                {
                    if (selectedBrand.Contains(itemRaw.Brand.Trim().ToLower()))      //do selected brand filter and compare lower case
                    {
                        DatabaseManager.Current.CommissionFactoryAddFeedItem(batch, itemRaw);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DatabaseManager.CommissionFactoryAddFeedItem failed - " + ex.ToString());
            }
        }

        //mark batch - file has been downloaded
        public bool BatchMarkFileDownloaded(int batchId)
        {
            string sql = "Update FeedBatch Set DateDownloaded = getDate() where BatchId = " + batchId.ToString();
            try
            {
                SqlCommand cmd = new SqlCommand(sql, sqlConn);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("DatabaseManager.BacthMarkFileDownloaded is failed at BatchId: " + batchId.ToString() + " - " + e.ToString());
                return false;
            }
        }

        //mark batch - file is read and processed
        public bool BatchMarkFileReadAndProcessed(int batchId)
        {
            string sql = "Update FeedBatch Set DateProcessed = getDate() where BatchId = " + batchId.ToString();
            try
            {
                SqlCommand cmd = new SqlCommand(sql, sqlConn);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("DatabaseManager.BatchMarkFileReadAndProcessed is failed at BatchId: " + batchId.ToString() + " - " + e.ToString());
                return false;
            }
        }

        //maek batch - data is pushed to remote server
        public bool BatchMarkDataPushed(int batchId)
        {
            string sql = "Update FeedBatch Set DatePushed = getDate() where BatchId = " + batchId.ToString();
            try
            {
                SqlCommand cmd = new SqlCommand(sql, sqlConn);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("DatabaseManager.BatchMarkDataPush is failed at BatchId: " + batchId.ToString() + " - " + e.ToString());
                return false;
            }
        }
















        /*
         * the is specific procedure for CommissionFactory(now used only for The Iconic)
         *  - it will either add or update the feed item to local database
         */
        private void CommissionFactoryAddFeedItem(Batch batch, DataItemRaw item)
        {
            int batchId = batch.BatchId;
            int MID     = batch.Merchant.MerchantID;

            SqlCommand cmd = new SqlCommand("ProcessCFRawData", sqlConn);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                //passing parameter
                cmd.Parameters.Add(new SqlParameter("@BatchId", batchId));
                cmd.Parameters.Add(new SqlParameter("@MID", MID));
                cmd.Parameters.Add(new SqlParameter("@SKU", item.SKU));
                cmd.Parameters.Add(new SqlParameter("@Name", item.Name));
                cmd.Parameters.Add(new SqlParameter("@Category", item.Category));
                cmd.Parameters.Add(new SqlParameter("@Description", item.Description));
                cmd.Parameters.Add(new SqlParameter("@Url", item.Url));
                cmd.Parameters.Add(new SqlParameter("@OriginalUrl", item.OriginalUrl));
                cmd.Parameters.Add(new SqlParameter("@ImageUrl", item.ImageUrl));
                cmd.Parameters.Add(new SqlParameter("@Price", item.Price));
                cmd.Parameters.Add(new SqlParameter("@DeliveryCost", RemNull(item.DeliveryCost)));
                cmd.Parameters.Add(new SqlParameter("@CurrencyCode", RemNull(item.Currency)));
                cmd.Parameters.Add(new SqlParameter("@Brand", item.Brand));
                cmd.Parameters.Add(new SqlParameter("@Colour", RemNull(item.Colour)));
                cmd.Parameters.Add(new SqlParameter("@Gender", RemNull(item.Gender)));
                cmd.Parameters.Add(new SqlParameter("@Size", RemNull(item.Size)));

                //execute
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("DatabaseManager.CommissionFactoryAddFeedItem is failed at Item SKU: " + item.SKU + " - " + ex.ToString());
            }
        }

        private string RemNull(string str)
        {
            if (str == null) { str = ""; }
            return str;
        }

        
    }
}
