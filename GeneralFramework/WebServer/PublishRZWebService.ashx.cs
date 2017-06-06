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
    /// PublishRZWebService 的摘要说明
    /// </summary>
    public class PublishRZWebService : IHttpHandler
    {

        HttpRequest Request;
        HttpResponse Response;
        HttpSessionState Session;
        HttpServerUtility Server;
        HttpCookie Cookie;
        HttpContext context;
        HttpFileCollection files;
        EnterpriseManager _em = new EnterpriseManager();
        PublishRzManager _pm = new PublishRzManager();
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

        public void GetRzqxType()
        {
            Response.Write(_em.LookUp("融资期限"));
        }

        public void GetRzytType()
        {
            Response.Write(_em.LookUp("融资用途"));
        }

        public void GetRzBanks()
        {
            Response.Write(_pm.GetRzBanks());
        }

        public void SaveRZInfo()
        {
            var data = Request;
            var sr = new StreamReader(data.InputStream);
            var stream = sr.ReadToEnd();
            var javaScriptSerializer = new JavaScriptSerializer();
            var rzi = javaScriptSerializer.Deserialize<RzInfo>(stream);
            Response.Write(_pm.SaveRzInfo(rzi));
        }

        public void GetRZLBJson()
        {
            string UserName = Request["UserName"];
            Response.Write(_pm.GetRZLBJson(UserName));
        }

        public void GetRZBankstr()
        {
            string BankIds = Request["BankIds"];
            Response.Write(_pm.GetRZBankstr(BankIds));
        }
    }
}