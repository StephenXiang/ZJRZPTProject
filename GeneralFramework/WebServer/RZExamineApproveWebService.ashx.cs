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
    /// RZExamineApproveWebService 的摘要说明
    /// </summary>
    public class RZExamineApproveWebService : IHttpHandler
    {

        HttpRequest Request;
        HttpResponse Response;
        HttpSessionState Session;
        HttpServerUtility Server;
        HttpCookie Cookie;
        HttpContext context;
        HttpFileCollection files;
        RZExamineApproveManager rm = new RZExamineApproveManager();
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
        public void GetRZDataTable()
        {
            string userName = Request["UserName"];
            int rows = int.Parse(Request["rows"].ToString());
            int page = int.Parse(Request["page"].ToString());
            Response.Write(rm.GetRZDataTable(userName, page, rows));
        }

        public void GetRZInfoByRZID()
        {
            string RZID = Request["RZID"];
            string UserNmae = Request["UserName"];
            Response.Write((rm.GetRZInfoByRZID(RZID, UserNmae)));
        }

        public void EditRZStatus()
        {
            string status = Request["status"];
            string zzdid = Request["rzid"];
            string UserName = Request["UserName"];
            Response.Write(rm.EditRZStatus(status, zzdid, UserName, 0));
        }
        public void EditRZStatusAndFeedback()
        {
            string status = Request["status"];
            string rzid = Request["rzid"];
            string liyou = Request["Feedback"];
            Response.Write(rm.EditRZStatus(status, rzid, liyou));
        }
    }
}