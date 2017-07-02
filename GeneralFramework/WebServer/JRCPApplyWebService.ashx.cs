using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.SessionState;

namespace GeneralFramework.WebServer
{
    /// <summary>
    /// JRCPApplyWebService 的摘要说明
    /// </summary>
    public class JRCPApplyWebService : IHttpHandler
    {

        HttpRequest _request;
        HttpResponse _response;
        HttpSessionState _session;
        HttpServerUtility _server;
        HttpCookie _cookie;
        HttpContext _context;
        HttpFileCollection _files;
        JRCPApplyService service = new JRCPApplyService();
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

        GeneralFrameworkDAL.EnterpriseService enterservice = new GeneralFrameworkDAL.EnterpriseService();
        JRCPApply jrcpapp = new JRCPApply();
        public void AddApply()
        {
            string UserName = _request["UserName"].ToString();
            string jrcpid = _request["jrcpId"].ToString();
            var ent = enterservice.GetEnterpriseInfoForUserNameTwo(UserName);
            jrcpapp.JRCPID = int.Parse(jrcpid);
            if (ent.ID == 0)
            {
                _response.Write(false);
            }
            else
            {
                jrcpapp.ApplyEnterpriseId = ent.ID;
                jrcpapp.ApplyEnterpriseName = ent.Name;
                jrcpapp.Phone = ent.ConnectionTelephone;
                _response.Write(service.Add(jrcpapp));
            }
        }

        public void CheckApply()
        {
            string UserName = _request["UserName"].ToString();
            string jrcpid = _request["jrcpId"].ToString();
            var ent = enterservice.GetEnterpriseInfoForUserNameTwo(UserName);
            if (ent.ID == 0)
            {
                _response.Write("flase");
            }
            else
            {
                jrcpapp.JRCPID = int.Parse(jrcpid);
                jrcpapp.ApplyEnterpriseId = ent.ID;
                jrcpapp.ApplyEnterpriseName = ent.Name;
                jrcpapp.Phone = ent.ConnectionTelephone;

                _response.Write(service.CheckApply(jrcpapp));
            }
        }
        public void GetApplyDg()
        {
            string UserName = _request.Form["UserName"];
            int page = int.Parse(_request.Form["page"]);
            int rows = int.Parse(_request.Form["rows"]);
            _response.Write(service.GetApplyDg(UserName, page, rows));
        }
    }
}