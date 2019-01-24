using System;
using System.Collections.Generic;
using System.IO;
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

        public static async Task<NetResult> UpFileAsync(String url,string filePath) {

            WebClient client = new WebClient();
            var qs = new System.Collections.Specialized.NameValueCollection
            {
                { "table", "image" },{"fileName",Path.GetFileName(filePath)}
            };
            byte[] res = await client.UploadFileTaskAsync(url, filePath);
            
           NetResult result = (NetResult)MyHelper.JsonHelper.JsonToObject(res.ToString(), typeof(NetResult));
           return result;
        }
    }
    

    public class NetResult
    {
        public int errCode { get; set; }
        public String msg { get; set; }
        public object Data { get; set; }
    }
}
