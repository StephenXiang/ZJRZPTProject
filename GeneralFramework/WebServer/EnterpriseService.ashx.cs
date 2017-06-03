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

        public void GetRegFinance()
        {
            Response.Write(em.LookUp("注册资金额度"));
        }

        public void SaveEnterpriseInfo()
        {
            var fs = Request.Files;
            var file = fs[0];
            int RegistTypeId = 0, RegFinance = 0, RegFinanceMt = 0, Business = 0;
            string MainProduction = "", JuridicalPerson = "", Desc = "";
            string EnterpriseName = Request.Form["EnterpriseName"].Trim();
            string UserName = Request.Form["UserNametxt"];
            if (Request.Form["RegistTypeCmb"].Trim() != "" && Request.Form["RegistTypeCmb"] != null)
            {
                RegistTypeId = Convert.ToInt32(Request.Form["RegistTypeCmb"]);
            }
            if (Request.Form["RegFinanceCmb"].Trim() != "" && Request.Form["RegFinanceCmb"] != null)
            {
                RegFinance = Convert.ToInt32(Request.Form["RegFinanceCmb"]);
            }
            if (Request.Form["RegFinanceMtCmb"].Trim() != "" && Request.Form["RegFinanceMtCmb"] != null)
            {
                RegFinanceMt = Convert.ToInt32(Request.Form["RegFinanceMtCmb"]);
            }
            if (Request.Form["BusinessCmb"].Trim() != "" && Request.Form["BusinessCmb"] != null)
            {
                Business = Convert.ToInt32(Request.Form["BusinessCmb"]);
            }
            if (Request.Form["MainProduction"].Trim() != "" && Request.Form["MainProduction"] != null)
            {
                MainProduction = Request.Form["MainProduction"];
            }
            if (Request.Form["JuridicalPerson"].Trim() != "" && Request.Form["JuridicalPerson"] != null)
            {
                JuridicalPerson = Request.Form["JuridicalPerson"];
            }
            if (Request.Form["EnterpriseDesc"].Trim() != "" && Request.Form["EnterpriseDesc"] != null)
            {
                Desc = Request.Form["EnterpriseDesc"];
            }

            var enterprise = new Enterprise
            {
                Name = Request.Form["EnterpriseName"].Trim(),
                Code = Request.Form["Code"].Trim(),
                BusinessLicense = StreamToBytes(file.InputStream),
                RegistTypeId = RegistTypeId,
                ProfessionId = Convert.ToInt32(Request.Form["ProfessionCmb"]),
                EnterpriseTypeId = Convert.ToInt32(Request.Form["EnterpriseTypeCmb"]),
                RegistRegionId = Convert.ToInt32(Request.Form["RegistRegionCmb"]),
                HuanpingId = Convert.ToInt32(Request.Form["HuanpingCmb"]),
                RegFinance = RegFinance,
                RegFinanceMt = RegFinanceMt,
                Business = Business,
                MainProduction = MainProduction,
                CreateTime = DateTime.Parse(Request.Form["CreateTime"]),
                JuridicalPerson = JuridicalPerson,
                ConectionPerson = Request.Form["ConectionPerson"].Trim(),
                ConnectionTelephone = Request.Form["ConnectionTelephone"],
                Desc = Desc
            };
            string err;
            bool isSaveOK = em.Save(enterprise, out err);
            if (isSaveOK == true)
            {
                em.GetEntetpriseForName(EnterpriseName, UserName);
            }
            Response.Write(isSaveOK);
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

        public void GetEnterpriseInfoForUserName()
        {
            var UserName = Request.Form["UserName"];
            Response.Write(em.GetEnterpriseInfoForUserName(UserName));
        }

        public void LoadBusinessLicenseImg()
        {
            var Code = Request.QueryString["Code"].ToString();
            var bytes = em.GetImgForCode(Code);
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }

        public void SaveEnterpriseFinanceInfo()
        {
            var data = Request;
            var sr = new StreamReader(data.InputStream);
            var stream = sr.ReadToEnd();
            //参考http://www.cnblogs.com/jaxu/p/3698404.html
        }

        public void GetEnterpriseFinanceInfoForUserName()
        {
            //返回一个json数组给前端
            //例：[{"year":"2015" "zzze":"125" , "fzze":"50" },{"year":"2016" "zzze":"125" , "fzze":"50" },{"year":"2017" "zzze":"125" , "fzze":"50" }]
        }
    }
}