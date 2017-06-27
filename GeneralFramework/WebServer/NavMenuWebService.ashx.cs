using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using GeneralFrameworkBLL;
using System.Reflection;
using System.Web.Script.Serialization;

namespace GeneralFramework.WebServer
{
    /// <summary>
    /// NavMenuWebService 的摘要说明
    /// </summary>
    public class NavMenuWebService : IHttpHandler, IRequiresSessionState
    {
        HttpRequest Request;
        HttpResponse Response;
        HttpSessionState Session;
        HttpServerUtility Server;
        HttpCookie Cookie;
        NavMenuManager NavMenuManager = new NavMenuManager();

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

        public void GetNavMenuJson()
        {
            string UserName = HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies["UserInfo"].Value);
            var javaScriptSerializer = new JavaScriptSerializer();
            var efi = javaScriptSerializer.Deserialize<UserInfo>(UserName);
            Response.Write(NavMenuManager.GetNavMenuJson(efi.name));
        }

        public void GetSysMatMenuJson()
        {
            Response.Write(NavMenuManager.GetSysMatMenuJson());
        }

        //{"name":"jxw","status":true,"role":2,"msg":"密码错误！"}
        public class UserInfo
        {
            public string name;
            public bool status;
            public int role;
            public string msg;
        }
    }
}