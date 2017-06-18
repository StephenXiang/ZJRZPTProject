using GeneralFrameworkBLL;
using GeneralFrameworkBLLModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace GeneralFramework.WebServer
{
    /// <summary>
    /// BankInfoWebService 的摘要说明
    /// </summary>
    public class BankInfoWebService : IHttpHandler
    {

        HttpRequest _request;
        HttpResponse _response;
        HttpSessionState _session;
        HttpServerUtility _server;
        HttpCookie _cookie;
        HttpContext _context;
        HttpFileCollection _files;
        BankInfoManager _bi = new BankInfoManager();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Buffer = true;
            context.Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            context.Response.AddHeader("pragma", "no-cache");
            context.Response.AddHeader("cache-control", "");
            context.Response.CacheControl = "no-cache";
            context.Response.ContentType = "text/plain";

            _files = context.Request.Files;
            _request = context.Request;
            _response = context.Response;
            _session = context.Session;
            _server = context.Server;


            string method = _request["method"].ToString();
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

        public void SaveBankInfo()
        {
            var data = _request;
            var sr = new StreamReader(data.InputStream);
            var stream = sr.ReadToEnd();
            var javaScriptSerializer = new JavaScriptSerializer();
            var efi = javaScriptSerializer.Deserialize<Bank>(stream);
            _response.Write(_bi.SaveBankInfo(efi));
        }

        public void GetBankId()
        {
            string userName = _request["UserName"];
            _response.Write(_bi.GetBankId(userName));
        }

        public void GetBankInfolbForBankId()
        {
            string BankId = _request["BankId"];
            _response.Write(_bi.GetBankInfolbForBankId(BankId));
        }

        public void GetBankInfoList()
        {
            _response.Write(_bi.GetBankInfoList());
        }
    }
}