using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfrofunkFeedManagement
{
    public class CSVFileReader
    {
        private string _fullPathFileName;

        public CSVFileReader(string fullPathFileName)
        {
            Console.WriteLine("Initial CSVFileReader class");
            _fullPathFileName = fullPathFileName;
        }

        public List<DataItemRaw> DoRead()
        {
            int counter = 0;
            string line;
            List<DataItemRaw> result = new List<DataItemRaw>();

            try
            {
                Console.WriteLine("Start reading the CSV file: " + _fullPathFileName + "   .....");

                System.IO.StreamReader file = new System.IO.StreamReader(_fullPathFileName);
                while ((line = file.ReadLine()) != null)    // Read the file and display it line by line.
                {
                    counter++;
                    if (counter > 1)    //ignore file line in CSV file as it's the header
                    {
                        result.Add(ExtractLineData(line));
                    }
                    else
                    {   //header file validation
                        if (isCSVFileValid(line) == false){
                            Console.WriteLine("Header file is invalid");
                            throw new Exception("Header file is invalid");
                        }
                    }
                }
                Console.WriteLine("Finishing read the CSV file: " + _fullPathFileName + "   .....");
                file.Close();
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine("Read file failed at line: " + counter + "\n" + e.ToString());
                return result;  //still return result although got exception
            }
        }

        //read csv one line and extract to DataItemRaw object, if fail will throw exception. this mean if one line fail - the whole thing fail (for now)
        private DataItemRaw ExtractLineData(string oneLine)
        {
            DataItemRaw lineItem = new DataItemRaw();
            try
            {
                //remove = for some field
                /*
                oneLine = oneLine.Replace(",=\"", ",\"");
                oneLine = oneLine.Substring(1, oneLine.Length - 2);
                string[] items = oneLine.Split(new string[] {"\",\""},StringSplitOptions.None );
                */
                string[] items = oneLine.Split(new string[] { "," }, StringSplitOptions.None);
                items = TrimValue(items);
                
                lineItem.DateCreated  = DateTime.ParseExact(items[0], "yyyy-MM-dd HH:mm:ss", null);
                lineItem.DateModified = DateTime.ParseExact(items[1], "yyyy-MM-dd HH:mm:ss", null);
                lineItem.SKU = items[2];
                lineItem.Name = items[3];
                lineItem.Category = items[4];
                lineItem.Description = items[5];
                lineItem.Url = items[6];
                lineItem.OriginalUrl = items[7];
                //image
                lineItem.ImageUrl = items[8];
                //price
                lineItem.Price = Convert.ToDecimal(items[15]);

                lineItem.Brand = items[16];
                lineItem.Colour = items[17];
                lineItem.Currency = items[18];
                lineItem.DeliveryCost = items[19];
                lineItem.DeliveryTime = items[20];
                lineItem.Feature = items[21];
                lineItem.Gender = items[22];
                lineItem.Size = items[23];
                lineItem.StockLevel = items[24];

            }
            catch (Exception e)
            {
                throw e;
            }

            return lineItem;
        }

        private bool isCSVFileValid(string headerLine)
        {
            string[] items = headerLine.Split(new Char[] { ',' });
            items = TrimValue(items);

            if (items[0] != "DateCreated") { return false; }
            if (items[1] != "DateModified") { return false; }
            if (items[2] != "SKU") { return false; }
            if (items[3] != "Name") { return false; }
            if (items[4] != "Category") { return false; }
            if (items[5] != "Description") { return false; }
            if (items[6] != "Url") { return false; }
            if (items[7] != "OriginalUrl") { return false; }

            if (items[8] != "Image") { return false; }

            if (items[15] != "Price") { return false; }
            if (items[16] != "Brand") { return false; }
            if (items[17] != "Colour") { return false; }
            if (items[18] != "Currency") { return false; }
            if (items[19] != "DeliveryCost") { return false; }
            if (items[20] != "DeliveryTime") { return false; }
            if (items[21] != "Features") { return false; }
            if (items[22] != "Gender") { return false; }
            if (items[23] != "Size") { return false; }
            if (items[24] != "StockLevel") { return false; }

            return true;
        }

        private string[] TrimValue(string[] items)
        {
            for (int i = 0; i <= items.Length - 1; i++)
            {
                items[i] = items[i].Trim();
            }
            return items;
        }


    }
}
