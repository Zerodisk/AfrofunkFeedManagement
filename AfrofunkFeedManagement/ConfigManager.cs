using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace AfrofunkFeedManagement
{
    public class ConfigManager
    {
        private static ConfigManager _Current;
 
        public static ConfigManager Current{
            get {
                if (_Current == null) { _Current = new ConfigManager(); }
                return _Current;
            }
        }

        public string ConnectionString
        {
            get { return System.Configuration.ConfigurationSettings.AppSettings["Db_Connection_String"]; }
        }

        public string UrlRemotePush
        {
            get { return System.Configuration.ConfigurationSettings.AppSettings["Remote_Push_URL"]; }
        }

        public string UrlRemotePushFinalise
        {
            get { return System.Configuration.ConfigurationSettings.AppSettings["Remote_Push_Finalise_URL"]; }
        }

        public string RemoteSecretKey
        {
            get { return System.Configuration.ConfigurationSettings.AppSettings["Remote_Secret_Key"]; }
        }
    }
}
