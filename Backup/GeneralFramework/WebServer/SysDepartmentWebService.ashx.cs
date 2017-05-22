using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using GeneralFrameworkBLL;
using System.Reflection;

namespace GeneralFramework.WebServer
{
    /// <summary>
    /// SysDepartmentWebService 的摘要说明
    /// </summary>
    public class SysDepartmentWebService : IHttpHandler, IRequiresSessionState
    {

        HttpRequest Request;
        HttpResponse Response;
        HttpSessionState Session;
        HttpServerUtility Server;
        HttpCookie Cookie;
        SysDepartmentManager sdm = new SysDepartmentManager();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Buffer = true;
            context.Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            context.Response.AddHeader("pragma", "no-cache");
            context.Response.AddHeader("cache-control", "");
            context.Response.CacheControl = "no-cache";
            context.Response.ContentType = "text/plain";

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

        public void GetSysDeparementJson()
        {
            string UserName = Session["UserName"].ToString();
            Response.Write(sdm.GetSysUserJson(UserName));
        }
    }
}