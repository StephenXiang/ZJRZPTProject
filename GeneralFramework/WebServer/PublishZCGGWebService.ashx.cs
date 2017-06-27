using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using GeneralFrameworkBLL;
using GeneralFrameworkBLLModel;

namespace GeneralFramework.WebServer
{
    /// <summary>
    /// PublishZCGGWebService 的摘要说明
    /// </summary>
    public class PublishZCGGWebService : IHttpHandler
    {

        HttpRequest Request;
        HttpResponse Response;
        HttpSessionState _session;
        HttpServerUtility _server;
        HttpCookie _cookie;
        HttpContext _context;
        HttpFileCollection _files;
        private NewsInfoManager _nm = new NewsInfoManager();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Buffer = true;
            context.Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            context.Response.AddHeader("pragma", "no-cache");
            context.Response.AddHeader("cache-control", "");
            context.Response.CacheControl = "no-cache";
            context.Response.ContentType = "text/plain";

            _files = context.Request.Files;
            Request = context.Request;
            Response = context.Response;
            _session = context.Session;
            _server = context.Server;


            string method = Request["method"].ToString();
            MethodInfo methodInfo = this.GetType().GetMethod(method);
            try
            {
                methodInfo.Invoke(this, null);
            }
            catch (TargetInvocationException targetEx)
            {
                if (targetEx.InnerException != null)
                {
                    throw targetEx.InnerException;
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void GetZCDG()
        {
            var userName = Request["UserName"];
            var page = int.Parse(Request["page"].ToString());
            var rows = int.Parse(Request["rows"].ToString());
            Response.Write(_nm.GetNewsDg("zc", page, rows));
        }

        public void GetGGDG()
        {
            var userName = Request["UserName"];
            var page = int.Parse(Request["page"].ToString());
            var rows = int.Parse(Request["rows"].ToString());
            Response.Write(_nm.GetNewsDg("gg", page, rows));
        }

        public void GettpDG()
        {
            var userName = Request["UserName"];
            var page = int.Parse(Request["page"].ToString());
            var rows = int.Parse(Request["rows"].ToString());
            Response.Write(_nm.GetNewsDg("tp", page, rows));
        }

        public void AddZC()
        {
            var data = Request;
            var sr = new StreamReader(data.InputStream);
            var stream = sr.ReadToEnd();
            var javaScriptSerializer = new JavaScriptSerializer();
            var news = javaScriptSerializer.Deserialize<NewsInfo>(stream);
            news.NewsType = "zc";
            Response.Write(_nm.AddNews(news));
        }

        public void AddGG()
        {
            var data = Request;
            var sr = new StreamReader(data.InputStream);
            var stream = sr.ReadToEnd();
            var javaScriptSerializer = new JavaScriptSerializer();
            var news = javaScriptSerializer.Deserialize<NewsInfo>(stream);
            news.NewsType = "gg";
            Response.Write(_nm.AddNews(news));
        }

        public void Addtp()
        {
            var fs = Request.Files;
            var file = fs[0];
            var data = Request;
            var sr = new StreamReader(data.InputStream);
            var stream = sr.ReadToEnd();
            NewsInfo news = new NewsInfo();
            news.CreateUser = data["CreateUser"];
            news.NewsTitle = data["titletp"];
            news.NewsContent = HttpUtility.UrlDecode(data["contenttp"]);
            news.NewsType = "tp";
            news.image = StreamToBytes(file.InputStream);
            Response.Write(_nm.AddNews(news));
        }

        private static byte[] StreamToBytes(Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }


        public void GetNews()
        {
            var newsId = int.Parse(Request["newsId"]);
            var javaScriptSerializer = new JavaScriptSerializer();
            Response.Write(javaScriptSerializer.Serialize(_nm.GetNewContent(newsId)));
        }

        public void DelNews()
        {
            var newsId = int.Parse(Request["newsId"]);
            Response.Write(_nm.DelNews(newsId));
        }

        public void GetIndexNewsInfo()
        {
            var newstype = Request["NewsType"];
            Response.Write(_nm.GetIndexNewsInfo(newstype));
        }

        public void GetZCNewsList()
        {
            var newstype = Request["NewsType"];
            var PageIndex = int.Parse(Request["PageIndex"]);
            var PageSize = int.Parse(Request["PageSize"]);
            Response.Write(_nm.GetZCNewsList(newstype, PageIndex, PageSize));
        }

        public void GetNewsImage()
        {
            var newsId = Request.QueryString["newsid"].ToString();
            var bytes = _nm.GetNewsImage(int.Parse(newsId));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }

        public void GetZCNewsDetail()
        {
            var Id = int.Parse(Request["Id"]);
            Response.Write(_nm.GetZCNewsDetail(Id));
        }

        public void GetDefaultNewsImage()
        {
            Response.Write(_nm.GetDefaultNewsImage());
        }
    }
}