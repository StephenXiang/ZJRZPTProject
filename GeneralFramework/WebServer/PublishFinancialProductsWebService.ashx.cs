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
    /// PublishFinancialProductsWebService 的摘要说明
    /// </summary>
    public class PublishFinancialProductsWebService : IHttpHandler
    {
        HttpRequest Request;
        HttpResponse Response;
        HttpSessionState Session;
        HttpServerUtility Server;
        HttpCookie Cookie;
        HttpContext context;
        HttpFileCollection files;
        EnterpriseManager _em = new EnterpriseManager();
        PublishJRCPManager _pm = new PublishJRCPManager();
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

        public void GetDanbaoType()
        {
            Response.Write(_em.LookUp("担保方式"));
        }

        public void SaveJRCPInfo()
        {
            var data = Request;
            var sr = new StreamReader(data.InputStream);
            var stream = sr.ReadToEnd();
            var javaScriptSerializer = new JavaScriptSerializer();
            var ji = javaScriptSerializer.Deserialize<JRCPInfo>(stream);
            if (ji.llfwstart == null)
            {
                ji.llfwstart = 0.00;
            }
            if (ji.llfwend == null)
            {
                ji.llfwend = 0.00;
            }
            Response.Write(_pm.SaveJRCPInfo(ji));
        }

        public void GetJRCPLBJson()
        {
            int page = int.Parse(Request["page"].ToString());
            int rows = int.Parse(Request["rows"].ToString());
            string UserName = Request["UserName"];
            Response.Write(_pm.GetJRCPLBJson(UserName, page, rows));
        }

        public void GetIndexJRCPInfo()
        {
            Response.Write(_pm.GetIndexJRCPInfo());
        }

        public void GetJRCPList()
        {
            string jgmc = Request["jgmc"].ToString();
            string cpmc = Request["cpmc"].ToString();
            string dkqd = Request["dkqd"].ToString();
            string dkqx = Request["dkqx"].ToString();
            string dbfs = Request["dbfs"].ToString();
            string dked = Request["dked"].ToString();
            Response.Write(_pm.GetJRCPList(dkqd, dkqx, dbfs, dked, jgmc, cpmc));
        }
        public void GetJRCPById()
        {
            int id = int.Parse(Request["jrcpid"].ToString());
            Response.Write(_pm.GetJRCPById(id));
        }

        public void Delete()
        {
            var id = Convert.ToInt32(Request["id"]);
            Response.Write(_pm.Delete(id));
        }

        public void GetNewJRCP()
        {
            Response.Write(_pm.GetNewJRCP());
        }
    }
}