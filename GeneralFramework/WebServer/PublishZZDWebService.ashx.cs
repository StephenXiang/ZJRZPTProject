﻿using System;
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
    /// PublishZZDWebService 的摘要说明
    /// </summary>
    public class PublishZZDWebService : IHttpHandler
    {

        HttpRequest Request;
        HttpResponse Response;
        HttpSessionState Session;
        HttpServerUtility Server;
        HttpCookie Cookie;
        HttpContext context;
        HttpFileCollection files;
        EnterpriseManager _em = new EnterpriseManager();
        PublishZzdManager _zm = new PublishZzdManager();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Buffer = true;
            context.Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            context.Response.AddHeader("pragma", "no-cache");
            context.Response.AddHeader("cache-control", "");
            context.Response.CacheControl = "no-cache";
            context.Response.ContentType = "text/plain";

            files = context.Request.Files;
            Request = context.Request;
            Response = context.Response;
            Session = context.Session;
            Server = context.Server;


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
        public void GetMainBank()
        {
            Response.Write(_zm.GetMainBank());
        }
        public void GetZzdBanks()
        {
            Response.Write(_zm.GetBanks());
        }

        public void GetZzdMastBanks()
        {
            var mainbankid = HttpUtility.UrlDecode(Request.QueryString["mainbankid"].ToString());
            Response.Write(_zm.GetMastBanks(mainbankid));
        }

        public void SaveZZDInfo()
        {
            var data = Request;
            var sr = new StreamReader(data.InputStream);
            var stream = sr.ReadToEnd();
            var javaScriptSerializer = new JavaScriptSerializer();
            var zi = javaScriptSerializer.Deserialize<ZzdInfo>(stream);
            Response.Write(_zm.Save(zi));
        }

        public void GetZZDLBJson()
        {
            var userName = Request["UserName"];
            int page = int.Parse(Request["page"].ToString());
            int rows = int.Parse(Request["rows"].ToString());
            Response.Write(_zm.GetZZDLBJson(userName, page, rows));
        }

        public void Delete()
        {
            var id = Convert.ToInt32(Request["id"]);
            Response.Write(_zm.Delete(id));
        }
    }
}