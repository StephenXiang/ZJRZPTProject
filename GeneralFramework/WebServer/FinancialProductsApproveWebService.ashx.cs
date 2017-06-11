using GeneralFrameworkBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.SessionState;

namespace GeneralFramework.WebServer
{
    /// <summary>
    /// FinancialProductsApproveWebService 的摘要说明
    /// </summary>
    public class FinancialProductsApproveWebService : IHttpHandler
    {

        HttpRequest Request;
        HttpResponse Response;
        HttpSessionState Session;
        HttpServerUtility Server;
        HttpCookie Cookie;
        HttpContext context;
        HttpFileCollection files;
        FinancialProductsApproveManager fm = new FinancialProductsApproveManager();
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


            var method = Request["method"];
            var methodInfo = this.GetType().GetMethod(method);
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

        public void GetJRCPTableJson()
        {
            int page = int.Parse(Request["page"]);
            int rows = int.Parse(Request["rows"]);
            Response.Write(fm.GetJRCPTableJson(page, rows));
        }

        public void GetJRCPDetialById()
        {
            int id = int.Parse(Request["Id"]);
            Response.Write(fm.GetJRCPDetialById(id));

        }

        public void EditJRCPStatus()
        {
            int id = int.Parse(Request["Id"]);
            string status = Request["status"];
            Response.Write(fm.EditJRCPStatus(status, id));
        }
    }
}