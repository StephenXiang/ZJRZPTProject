using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using GeneralFrameworkBLL;
using GeneralFrameworkBLLModel;

namespace GeneralFramework.WebServer
{
    /// <summary>
    /// EnterpriseService 的摘要说明
    /// </summary>
    public class EnterpriseService : IHttpHandler
    {

        HttpRequest Request;
        HttpResponse Response;
        HttpSessionState Session;
        HttpServerUtility Server;
        HttpCookie Cookie;
        HttpContext context;
        HttpFileCollection files;
        EnterpriseManager em = new EnterpriseManager();
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

        public void GetRegistType()
        {
            Response.Write(em.LookUp("注册类型"));
        }

        public void GetProfessionType()
        {
            Response.Write(em.LookUp("行业分类"));
        }

        public void GetEnterpriseType()
        {
            Response.Write(em.LookUp("企业类型"));
        }

        public void GetRegionType()
        {
            Response.Write(em.LookUp("注册地"));
        }

        public void GetHuanPingType()
        {
            Response.Write(em.LookUp("环评结果"));
        }

        public void GetMoneyType()
        {
            Response.Write(em.LookUp("币种"));
        }

        public void GetBusinessType()
        {
            Response.Write(em.LookUp("营业范围"));
        }

        public void GetRZQixianType()
        {
            Response.Write(em.LookUp("融资期限"));
        }

        public void GetRZUsageType()
        {
            Response.Write(em.LookUp("融资用途"));
        }

        public void SaveEnterpriseInfo()
        {
            var fs = Request.Files;
            var file = fs[0];
            var enterprise = new Enterprise
            {
                Name = Request.Form["EnterpriseName"],
                Code = Request.Form["Code"],
                BusinessLicense = StreamToBytes(file.InputStream),
                RegistTypeId = Convert.ToInt32(Request.Form["RegistTypeCmb"]),
                ProfessionId = Convert.ToInt32(Request.Form["ProfessionCmb"]),
                EnterpriseTypeId = Convert.ToInt32(Request.Form["EnterpriseTypeCmb"]),
                RegistRegionId = Convert.ToInt32(Request.Form["RegistRegionCmb"]),
                HuanpingId = Convert.ToInt32(Request.Form["HuanpingCmb"]),
                RegFinance = Convert.ToInt32(Request.Form["RegFinanceCmb"]),
                RegFinanceMt = Request.Form["RegFinanceMtCmb"],
                Business = Convert.ToInt32(Request.Form["BusinessCmb"]),
                MainProduction = Request.Form["MainProduction"],
                CreateTime = DateTime.Parse(Request.Form["CreateTime"]),
                JuridicalPerson = Request.Form["JuridicalPerson"],
                ConectionPerson = Request.Form["ConectionPerson"],
                ConnectionTelephone = Request.Form["ConnectionTelephone"],
                Desc = Request.Form["EnterpriseDesc"]
            };
            string err;
            em.Save(enterprise, out err);
        }

        private static byte[] StreamToBytes(Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        public bool SaveInfo()
        {
            var s = Request.Form["UserName"];
            var ins = new Enterprise
            {
                //Business = Request.Form["UserName"]
            };
            return true;
        }
    }
}