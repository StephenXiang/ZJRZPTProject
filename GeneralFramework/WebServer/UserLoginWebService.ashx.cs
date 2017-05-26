using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Reflection;
using GeneralFrameworkBLL;
using GeneralFrameworkDAL.JSON;
using GeneralFrameworkBLLModel;

namespace GeneralFramework.WebServer
{
    /// <summary>
    /// UserLoginWebService 的摘要说明
    /// </summary>
    public class UserLoginWebService : IHttpHandler, IRequiresSessionState
    {

        HttpRequest Request;
        HttpResponse Response;
        HttpSessionState Session;
        HttpServerUtility Server;
        HttpCookie Cookie;
        HttpContext context;
        HttpFileCollection files;
        SysUserManager sum = new SysUserManager();
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

        public void Login()
        {
            var UserName = Request.Form["UserName"];
            var Pwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Request.Form["Pwd"], "MD5").ToLower();
            var loginInfo = sum.Login(UserName, Pwd);
            if (loginInfo != null)
            {
                Session["UserName"] = UserName;
                Session.Timeout = 60;
            }
            Response.Write(JsonHelper.SerializeObject(new
            {
                name = UserName,
                status = loginInfo != null,
                role = loginInfo != null ? loginInfo.RoleId : 0
            }));
        }

        public void GetLoginUserInfo()
        {
            Response.Write(Session["UserName"].ToString());
        }

        public void GetLoginUserRole()
        {
            Response.Write(Session["Role"].ToString());
        }

        public void GetUserTBJson()
        {
            var DepartmentId = int.Parse(Request["DepartmentId"]);
            Response.Write(sum.GetUserTBJsonForDepartmentId(DepartmentId));
        }

        public void EditUserPwd()
        {
            var UserName = Request.Form["UserName"];
            var Pwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Request.Form["Pwd"], "MD5").ToLower();
            Response.Write(sum.EditUserPwd(UserName, Pwd));
        }
    }
}