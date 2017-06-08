using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace GeneralFrameworkDAL
{
    public class SmsService
    {
        private const string Url = @"http://api.app2e.com/smsBigSend.api.php";
        public bool Send(string phonenum, string msg)
        {
            try
            {
                var smsuser = ConfigurationManager.AppSettings["smsUser"];
                var smspwd = ConfigurationManager.AppSettings["smsPwd"];
                var paras = new Dictionary<string, string>
                {
                    {"username", smsuser},
                    {"pwd", smspwd},
                    {"p", phonenum},
                    {"charSetStr", "utf"},
                    {"extnum", "1"},
                    {"msg", msg}
                };
                var ret = WebHelper.Post(Url, paras);
                var smsRet = JsonConvert.DeserializeObject<SmsReturn>(ret);
                return smsRet.Status == 100;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class SmsReturn
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("list")]
        public IList<SmsItem> SmsItems { get; set; }
    }

    public class SmsItem
    {
        [JsonProperty("p")]
        public string Phone { get; set; }

        [JsonProperty("mid")]
        public string MsgId { get; set; }
    }
}
