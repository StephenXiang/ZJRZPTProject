using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.SessionState;

namespace GeneralFramework.WebServer
{
    /// <summary>
    /// CooperativeBank 的摘要说明
    /// </summary>
    public class CooperativeBank : IHttpHandler
    {

        HttpRequest _request;
        HttpResponse _response;
        HttpSessionState _session;
        HttpServerUtility _server;
        HttpCookie _cookie;
        HttpContext _context;
        HttpFileCollection _files;
        CooperativeBankService cbService = new CooperativeBankService();
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

        public void AddCB()
        {
            var fs = _request.Files;
            var logo1 = fs["Logo1"];
            var logo2 = fs["logo2"];
            GeneralFrameworkBLLModel.CooperativeBank BankInfo = new GeneralFrameworkBLLModel.CooperativeBank
            {
                BankName = _request.Form[0],
                Leader = _request.Form[1],
                Phone = _request.Form[2],
                sort = int.Parse(_request.Form[3].ToString()),
                BankDesc = _request.Form[4],
                IsDesplay = int.Parse(_request.Form["IsDesplay"].ToString()),
                logo1 = StreamToBytes(logo1.InputStream),
                logo2 = StreamToBytes(logo2.InputStream)
            };
            _response.Write(cbService.AddCooperativeBank(BankInfo));
        }

        public void EditCB()
        {
            var fs = _request.Files;
            var logo1 = fs["Logo1"];
            var logo2 = fs["logo2"];
            GeneralFrameworkBLLModel.CooperativeBank BankInfo = new GeneralFrameworkBLLModel.CooperativeBank
            {
                Id = int.Parse(_request.Form[6].ToString()),
                BankName = _request.Form[0],
                Leader = _request.Form[1],
                Phone = _request.Form[2],
                sort = int.Parse(_request.Form[3].ToString()),
                BankDesc = _request.Form[4],
                IsDesplay = int.Parse(_request.Form["IsDesplay"].ToString()),
                logo1 = StreamToBytes(logo1.InputStream),
                logo2 = StreamToBytes(logo2.InputStream)
            };
            _response.Write(cbService.EditCooperativeBank(BankInfo));
        }

        public void DelCooperativeBank()
        {
            int Id = int.Parse(_request.Form["Id"].ToString());
            _response.Write(cbService.DelCooperativeBank(Id));
        }

        public void GetCooperativeBankDG()
        {
            int page = int.Parse(_request["page"].ToString());
            int rows = int.Parse(_request["rows"].ToString());
            _response.Write(cbService.GetCooperativeBankDG(page, rows));
        }

        public byte[] StreamToBytes(Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        public void GetBankInfoList()
        {
            _response.Write(cbService.GetBankInfoList());
        }

        public void GetBankInfolbForBankId()
        {
            int BankId = int.Parse(_request["BankId"].ToString());
            _response.Write(cbService.GetBankInfolbForBankId(BankId));
        }
        public void LoadLogoImg()
        {
            var code = _request.QueryString["Id"].ToString();
            var bytes = cbService.GetLogoImgForId(code);
            _response.BinaryWrite(bytes);
            _response.Flush();
            _response.End();
        }

        public void GetMainBankCmb()
        {
            _response.Write(cbService.GetMainBankCmb());
        }
    }
}