using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TicketCheckStation
{
    public class NetHelper
    {

        public static NetResult Get(String url,  string table,string lastSyncTime,string stationid)
        {
            WebClient client = new WebClient();
            var qs = new System.Collections.Specialized.NameValueCollection
            {
                { "table", table }, { "time", lastSyncTime} , { "stationid", stationid}
            };
            client.QueryString = qs;
            //client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            //client.ResponseHeaders.Add("*/*");
            String res = client.DownloadString(url);
            NetResult result = (NetResult)MyHelper.JsonHelper.JsonToObject(res, typeof(NetResult));
            return result;
        }

        public static NetResult Post(String url, Object data, string table)
        {
          
            String josn = MyHelper.JsonHelper.ObjectToJson(data);                 
            WebClient client = new WebClient();
            var qs = new System.Collections.Specialized.NameValueCollection
            {
                { "table", table }
            };
            client.QueryString = qs;
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            String res = client.UploadString(url, "Post", josn);   
            NetResult result = (NetResult)MyHelper.JsonHelper.JsonToObject(res, typeof(NetResult));
            return result;
        }

        public static NetResult UpFile(String table,string filePath) {

            return null;
        }
    }
    

    public class NetResult
    {
        public int errCode { get; set; }
        public String msg { get; set; }
        public object Data { get; set; }
    }
}
