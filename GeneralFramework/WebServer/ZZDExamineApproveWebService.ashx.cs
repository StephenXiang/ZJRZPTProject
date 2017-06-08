using GeneralFrameworkBLL;
using GeneralFrameworkDAL.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.SessionState;

namespace GeneralFramework.WebServer
{
    /// <summary>
    /// ZZDExamineApproveWebService 的摘要说明
    /// </summary>
    public class ZZDExamineApproveWebService : IHttpHandler
    {

        HttpRequest Request;
        HttpResponse Response;
        HttpSessionState Session;
        HttpServerUtility Server;
        HttpCookie Cookie;
        HttpContext context;
        HttpFileCollection files;
        ZZDExamineApproveManager zm = new ZZDExamineApproveManager();
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
        public void GetZZDDataTable()
        {
            string userName = Request["UserName"];
            Response.Write(zm.GetZZDDataTable(userName));
        }
        public void EditZZDStatus()
        {
            string status = Request["status"];
            string zzdid = Request["zzdid"];
            Response.Write(zm.EditZZDStatus(status, zzdid));
        }
        public void EditZZDStatusAndFeedback()
        {
            string status = Request["status"];
            string zzdid = Request["zzdid"];
            string liyou = Request["Feedback"];
            Response.Write(zm.EditZZDStatus(status, zzdid, liyou));
        }

        public void GetEnterpriseInfoForEnterpriseId()
        {
            string enterpriseId = Request["EnterpriseId"];
            Response.Write(zm.GetEnterpriseInfoForEnterpriseId(enterpriseId));
        }
        public void GetEnterpriseFinanceInfoByEnterpriseId()
        {
            string enterpriseId = Request["EnterpriseId"];
            Response.Write(zm.GetEnterpriseFinanceInfoByEnterpriseId(enterpriseId));
        }
        public void GetZZDInfoByZZDID()
        {
            string ZZDID = Request["ZZDID"];
            Response.Write(zm.GetZZDInfoByZZDID(ZZDID));
        }
    }
}