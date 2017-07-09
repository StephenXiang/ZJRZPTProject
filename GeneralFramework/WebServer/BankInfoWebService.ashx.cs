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
            // Edit
            if (bool.Parse(_request.Form["IsEdit"]))
            {
                //var fs = _request.Files;
                //var logo1 = fs["Logo1"];
                //var logo2 = fs["logo2"];
                var id = int.Parse(_request.Form[8]);
                if (id <= 0) return;
                Bank BankInfo = new Bank
                {
                    ID = id,
                    UserName = _request.Form[0],
                    ParentBankId = _request.Form[1],
                    Name = _request.Form[2],
                    Address = _request.Form[3],
                    //Connector = _request.Form[4],
                    //ConnectorPhone = _request.Form[5],
                    iszzd = int.Parse(_request.Form[6]),
                    BankType = int.Parse(_request.Form[7])
                    //BankDesc = _request.Form[7],
                    //logo1 = logo1 == null ? null : StreamToBytes(logo1.InputStream),
                    //logo2 = logo2 == null ? null : StreamToBytes(logo2.InputStream)
                };
                _response.Write(_bi.SaveBankInfo(BankInfo));
                return;
            }
            // Add
            if (int.Parse(_request.Form[6]) == 1)
            {
                //var fs = _request.Files;
                //var logo1 = fs["Logo1"];
                //var logo2 = fs["logo2"];
                Bank BankInfo = new Bank
                {
                    UserName = _request.Form[0],
                    ParentBankId = _request.Form[1],
                    Name = _request.Form[2],
                    Address = _request.Form[3],
                    //Connector = _request.Form[4],
                    //ConnectorPhone = _request.Form[5],
                    iszzd = int.Parse(_request.Form[6]),
                    BankType = int.Parse(_request.Form[7])
                    //BankDesc = _request.Form[7],
                    //logo1 = StreamToBytes(logo1.InputStream),
                    //logo2 = StreamToBytes(logo2.InputStream)
                };
                _response.Write(_bi.SaveBankInfo(BankInfo));
            }
            else
            {
                Bank BankInfo = new Bank
                {
                    UserName = _request.Form[0],
                    ParentBankId = _request.Form[1],
                    Name = _request.Form[2],
                    Address = _request.Form[3],
                    Connector = _request.Form[4],
                    ConnectorPhone = _request.Form[5],
                    iszzd = int.Parse(_request.Form[6]),
                    BankDesc = _request.Form[7]
                };
                _response.Write(_bi.SaveBankInfo(BankInfo));
            }
            //var data = _request;
            //var sr = new StreamReader(data.InputStream);
            //var stream = sr.ReadToEnd();
            //var javaScriptSerializer = new JavaScriptSerializer();
            //var efi = javaScriptSerializer.Deserialize<Bank>(stream);

        }
        public byte[] StreamToBytes(Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
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

        public void LoadLogo2Img()
        {
            var code = _request.QueryString["Id"].ToString();
            var bytes = _bi.GetLogo2ImgForId(code);
            _response.BinaryWrite(bytes);
            _response.Flush();
            _response.End();
        }

        public void LoadLogoImg()
        {
            var code = _request.QueryString["Id"].ToString();
            var bytes = _bi.GetLogoImgForId(code);
            _response.BinaryWrite(bytes);
            _response.Flush();
            _response.End();
        }

        public void GetBankDg()
        {
            string uname = _request["UserName"];
            int page = int.Parse(_request["page"].ToString());
            int rows = int.Parse(_request["rows"].ToString());
            _response.Write(_bi.GetBankDg(page, rows));
        }

        public void GetMainBank()
        {
            int page = int.Parse(_request["page"]);
            int rows = int.Parse(_request["rows"]);
            _response.Write(_bi.GetMainBank(page, rows));
        }

        public void AddMainBank()
        {
            string bankname = _request["bankname"].ToString();
            _response.Write(_bi.AddMainBank(bankname));
        }

        public void DelMainBank()
        {
            int mainbanid = int.Parse(_request["bankid"]);
            _response.Write(_bi.DelMainBank(mainbanid));
        }



        public void AddLiaisonanMan()
        {
            LiaisonanMan lman = new LiaisonanMan();
            var data = _request;
            var LiaisonanManName = data.Form["LiaisonanManName"].ToString();
            var LiaisonanManPost = data.Form["LiaisonanManPost"].ToString();
            var LiaisonanManPhone = data.Form["LiaisonanManPhone"].ToString();
            var LiaisonanManBankId = data.Form["LiaisonanManBankId"].ToString();
            lman.Name = LiaisonanManName;
            lman.Post = LiaisonanManPost;
            lman.Phone = LiaisonanManPhone;
            lman.BankId = int.Parse(LiaisonanManBankId);
            _response.Write(_bi.AddLiaisonanMan(lman));
        }

        public void GetLiaisonanManDG()
        {
            int BankId; int page; int rows;
            BankId = int.Parse(_request["BankID"].ToString());
            page = int.Parse(_request["page"].ToString());
            rows = int.Parse(_request["rows"].ToString());
            _response.Write(_bi.GetLiaisonanManDG(BankId, page, rows));
        }

        public void EditLiaisonanMan()
        {
            LiaisonanMan lman = new LiaisonanMan();
            var data = _request;
            var Id = data.Form["LiaisonanManUserIdtxt"].ToString();
            var LiaisonanManName = data.Form["LiaisonanManName"].ToString();
            var LiaisonanManPost = data.Form["LiaisonanManPost"].ToString();
            var LiaisonanManPhone = data.Form["LiaisonanManPhone"].ToString();
            var LiaisonanManBankId = data.Form["LiaisonanManBankId"].ToString();
            lman.Name = LiaisonanManName;
            lman.Post = LiaisonanManPost;
            lman.Phone = LiaisonanManPhone;
            lman.Id = int.Parse(Id);
            _response.Write(_bi.EditLiaisonanMan(lman));
        }

        public void DelLiaisonanMan()
        {
            LiaisonanMan lman = new LiaisonanMan();
            var Id = int.Parse(_request.Form["Userid"].ToString());
            lman.Id = Id;
            _response.Write(_bi.DelLiaisonanMan(lman));
        }
    }
}