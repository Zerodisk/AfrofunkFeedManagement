using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfrofunkFeedManagement
{
    public class Downloader
    {
        private string _url;                //url to download file from
        private string _fullPathFileName;   //full path of where to save the file, including filename

        //passing location and full path filename where to save the file to
        public Downloader(string fullPathFilename, string url)
        {
            Console.WriteLine("Initial Downloader class");
            _url = url;
            _fullPathFileName = fullPathFilename;
        }

        public bool DoDownload()
        {
            try
            {
                Console.WriteLine("Start downloading - " + _url);
                new System.Net.WebClient().DownloadFile(_url, _fullPathFileName);
                Console.WriteLine("Download is finished, file is stored at - " + _fullPathFileName);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Download file failed, URL: " + _url + "\nSave file at: " + _fullPathFileName + "\n" + e.ToString());
                return false;
            }
        }
    }
}
