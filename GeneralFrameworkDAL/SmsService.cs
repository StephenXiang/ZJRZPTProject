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
                var ret = Post(Url, paras);
                var smsRet = JsonConvert.DeserializeObject<SmsReturn>(ret);
                return smsRet.Status == 100;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <param name="content">Post提交数据内容(utf-8编码的)</param>
        /// <returns></returns>
        public static string Post(string url, string content)
        {
            string result;
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            #region 添加Post 参数
            var data = Encoding.UTF8.GetBytes(content);
            req.ContentLength = data.Length;
            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion

            var resp = (HttpWebResponse)req.GetResponse();
            var stream = resp.GetResponseStream();
            //获取响应内容
            if (stream == null) return null;
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> dic)
        {
            string result;
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            #region 添加Post 参数
            var builder = new StringBuilder();
            var i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            var data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            var resp = (HttpWebResponse)req.GetResponse();
            var stream = resp.GetResponseStream();
            if (stream == null) return null;
            //获取响应内容
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
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
