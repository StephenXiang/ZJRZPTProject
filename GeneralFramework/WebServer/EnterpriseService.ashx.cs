using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
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

        HttpRequest _request;
        HttpResponse _response;
        HttpSessionState _session;
        HttpServerUtility _server;
        HttpCookie _cookie;
        HttpContext _context;
        HttpFileCollection _files;
        EnterpriseManager _em = new EnterpriseManager();
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

        public void GetRegistType()
        {
            _response.Write(_em.LookUp("注册类型"));
        }

        public void GetProfessionType()
        {
            _response.Write(_em.LookUp("行业分类"));
        }

        public void GetEnterpriseType()
        {
            _response.Write(_em.LookUp("企业类型"));
        }

        public void GetRegionType()
        {
            _response.Write(_em.LookUp("注册地"));
        }

        public void GetHuanPingType()
        {
            _response.Write(_em.LookUp("环评结果"));
        }

        public void GetMoneyType()
        {
            _response.Write(_em.LookUp("币种"));
        }

        public void GetBusinessType()
        {
            _response.Write(_em.LookUp("营业范围"));
        }

        public void GetRzQixianType()
        {
            _response.Write(_em.LookUp("融资期限"));
        }

        public void GetRzUsageType()
        {
            _response.Write(_em.LookUp("融资用途"));
        }

        public void GetRegFinance()
        {
            _response.Write(_em.LookUp("注册资金额度"));
        }

        public void SaveEnterpriseInfo()
        {
            var fs = _request.Files;
            var file = fs[0];
            int registTypeId = 0, regFinance = 0, regFinanceMt = 0, business = 0;
            string mainProduction = "", juridicalPerson = "", desc = "";
            string enterpriseName = _request.Form["EnterpriseName"].Trim();
            string userName = _request.Form["UserNametxt"];
            if (_request.Form["RegistTypeCmb"].Trim() != "" && _request.Form["RegistTypeCmb"] != null)
            {
                registTypeId = Convert.ToInt32(_request.Form["RegistTypeCmb"]);
            }
            if (_request.Form["RegFinanceCmb"].Trim() != "" && _request.Form["RegFinanceCmb"] != null)
            {
                regFinance = Convert.ToInt32(_request.Form["RegFinanceCmb"]);
            }
            if (_request.Form["RegFinanceMtCmb"].Trim() != "" && _request.Form["RegFinanceMtCmb"] != null)
            {
                regFinanceMt = Convert.ToInt32(_request.Form["RegFinanceMtCmb"]);
            }
            if (_request.Form["BusinessCmb"].Trim() != "" && _request.Form["BusinessCmb"] != null)
            {
                business = Convert.ToInt32(_request.Form["BusinessCmb"]);
            }
            if (_request.Form["MainProduction"].Trim() != "" && _request.Form["MainProduction"] != null)
            {
                mainProduction = _request.Form["MainProduction"];
            }
            if (_request.Form["JuridicalPerson"].Trim() != "" && _request.Form["JuridicalPerson"] != null)
            {
                juridicalPerson = _request.Form["JuridicalPerson"];
            }
            if (_request.Form["EnterpriseDesc"].Trim() != "" && _request.Form["EnterpriseDesc"] != null)
            {
                desc = _request.Form["EnterpriseDesc"];
            }

            var enterprise = new Enterprise
            {
                Name = _request.Form["EnterpriseName"].Trim(),
                Code = _request.Form["Code"].Trim(),
                BusinessLicense = StreamToBytes(file.InputStream),
                RegistTypeId = registTypeId,
                ProfessionId = Convert.ToInt32(_request.Form["ProfessionCmb"]),
                EnterpriseTypeId = Convert.ToInt32(_request.Form["EnterpriseTypeCmb"]),
                RegistRegionId = Convert.ToInt32(_request.Form["RegistRegionCmb"]),
                HuanpingId = Convert.ToInt32(_request.Form["HuanpingCmb"]),
                RegFinance = regFinance,
                RegFinanceMt = regFinanceMt,
                Business = business,
                MainProduction = mainProduction,
                CreateTime = DateTime.Parse(_request.Form["CreateTime"]),
                JuridicalPerson = juridicalPerson,
                ConectionPerson = _request.Form["ConectionPerson"].Trim(),
                ConnectionTelephone = _request.Form["ConnectionTelephone"],
                Desc = desc
            };
            string err;
            bool isSaveOk = _em.Save(enterprise, out err);
            if (isSaveOk == true)
            {
                _em.GetEntetpriseForName(enterpriseName, userName);
            }
            _response.Write(isSaveOk);
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
            var s = _request.Form["UserName"];
            var ins = new Enterprise
            {
                //Business = Request.Form["UserName"]
            };
            return true;
        }

        public void GetEnterpriseInfoForUserName()
        {
            var userName = _request.Form["UserName"];
            _response.Write(_em.GetEnterpriseInfoForUserName(userName));
        }

        public void LoadBusinessLicenseImg()
        {
            var code = _request.QueryString["Code"].ToString();
            var bytes = _em.GetImgForCode(code);
            _response.BinaryWrite(bytes);
            _response.Flush();
            _response.End();
        }

        public void SaveEnterpriseFinanceInfo()
        {
            var data = _request;
            var sr = new StreamReader(data.InputStream);
            var stream = sr.ReadToEnd();
            var javaScriptSerializer = new JavaScriptSerializer();
            var efi = javaScriptSerializer.Deserialize<EnterpriseFinanceInfo>(stream);
            _response.Write(_em.SaveEnterpriseFianceInfo(efi));
            //参考http://www.cnblogs.com/jaxu/p/3698404.html
        }

        public void GetEnterpriseFinanceInfoForUserName()
        {
            var userName = _request["UserName"].ToString();
            _response.Write(_em.GetEnterpriseFianceInfoByUserName(userName));
            //返回一个json数组给前端
            //例：[{"year":"2015" "zzze":"125" , "fzze":"50" },{"year":"2016" "zzze":"125" , "fzze":"50" },{"year":"2017" "zzze":"125" , "fzze":"50" }]
        }
    }
}