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
            Response.Write(_pm.SaveJRCPInfo(ji));
        }

        public void GetJRCPLBJson()
        {
            int page = int.Parse(Request["page"].ToString());
            int rows = int.Parse(Request["rows"].ToString());
            string UserName = Request["UserName"];
            Response.Write(_pm.GetJRCPLBJson(UserName, page, rows));
        }
    }
}