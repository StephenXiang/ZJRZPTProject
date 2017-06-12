using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace GeneralFrameworkDAL
{
    public class WebHelper
    {
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

        public static string Get(string url, string postDataStr)
        {
            var request = (HttpWebRequest)WebRequest.Create(url + (postDataStr == "" ? "" : 
                ("?" + string.Format("para={0}", HttpUtility.UrlEncode(Encrypter.EncryptDes(postDataStr))))));
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            var response = (HttpWebResponse)request.GetResponse();
            var myResponseStream = response.GetResponseStream();
            if (myResponseStream == null) return "";
            var myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            var retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        public static string Get(string url, Dictionary<string, string> dic, string joiner = ",")
        {
            var builder = new StringBuilder();
            var i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append(joiner);
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            return Get(url, builder.ToString());
        }

        [Obsolete]
        private string HttpPost(string url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            //request.CookieContainer = cookie;
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            //response.Cookies = cookie.GetCookies(response.ResponseUri);
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
    }
}
