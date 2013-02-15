using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfrofunkFeedManagement
{
    public enum BatchStatus
    {
        New = 0,                    //brand new batch
        FileDownloaded = 1,         //download completed, file is stored in temp folder
        FileProcessed = 2,          //finished processing raw data, new data for push are ready
        Pushed = 3,                 //push new-change-update to remote server
        Done = 99               //all done :)
    }
}
