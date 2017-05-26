using System;
using System.Collections.Generic;
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

        public void SaveEnterpriseInfo()
        {
            HttpFileCollection files = Request.Files;
            HttpPostedFile file = files[0];
            Enterprise enterprise = new Enterprise
            {
                Name = Request.Form["EnterpriseName"].ToString(),
                Code = Request.Form["Code"].ToString(),
                //BusinessLicense= new byte[], 自己转流，上面的file就是文件，自己调试看看，shit
                RegistTypeId = (int)Request.Form["RegistTypeCmb"].ToString(),
                ProfessionId = (int)Request.Form["ProfessionCmb"].ToString(),
                EnterpriseTypeId = (int)Request.Form["EnterpriseTypeCmb"].ToString(),
                RegistRegionId = (int)Request.Form["RegistRegionCmb"].ToString(),
                HuanpingId = (int)Request.Form["HuanpingCmb"].ToString(),
                RegFinance = (int)Request.Form["RegFinanceCmb"].ToString(),
                RegFinanceMt = (int)Request.Form["RegFinanceMtCmb"].ToString(),
                Business = (int)Request.Form["BusinessCmb"].ToString(),
                MainProduction = Request.Form["MainProduction"].ToString(),
                CreateTime = Request.Form["CreateTime"].ToString(),
                JuridicalPerson = Request.Form["JuridicalPerson"].ToString(),
                ConectionPerson = Request.Form["ConectionPerson"].ToString(),
                ConnectionTelephone = Request.Form["ConnectionTelephone"].ToString(),
                Desc = Request.Form["EnterpriseDesc"].ToString()



            };



        }

        public void Login()
        {
            string UserName = Request.Form["UserName"].ToString();
            string Pwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Request.Form["Pwd"].ToString(), "MD5").ToLower();
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
            int DepartmentId = int.Parse(Request["DepartmentId"].ToString());
            Response.Write(sum.GetUserTBJsonForDepartmentId(DepartmentId));
        }

        public void EditUserPwd()
        {
            string UserName = Request.Form["UserName"].ToString();
            string Pwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Request.Form["Pwd"].ToString(), "MD5").ToLower();
            Response.Write(sum.EditUserPwd(UserName, Pwd));
        }
    }
}