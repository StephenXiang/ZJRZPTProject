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
    /// MessageService 的摘要说明
    /// </summary>
    public class MessageService : IHttpHandler
    {
        HttpRequest Request;
        HttpResponse Response;
        HttpSessionState Session;
        HttpServerUtility Server;
        HttpCookie Cookie;
        HttpContext context;
        HttpFileCollection files;
        private readonly MessageManager _mm = new MessageManager();
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


            var method = Request["method"].ToString();
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

        public void SaveReply()
        {
            var data = Request;
            var sr = new StreamReader(data.InputStream);
            var stream = sr.ReadToEnd();
            var javaScriptSerializer = new JavaScriptSerializer();
            var mi = javaScriptSerializer.Deserialize<MessageInfo>(stream);
            Response.Write(_mm.Reply(mi));
        }

        public void LoadHandledMessages()
        {
            var userName = Request["UserName"];
            var page = int.Parse(Request["page"].ToString());
            var rows = int.Parse(Request["rows"].ToString());
            Response.Write(_mm.LoadHandledMessages(userName, page, rows));
        }

        public void LoadMessages()
        {
            var userName = Request["UserName"];
            var page = int.Parse(Request["page"].ToString());
            var rows = int.Parse(Request["rows"].ToString());
            Response.Write(_mm.LoadMessages(userName, page, rows));
        }

        public void AddMessage()
        {
            //var data = Request;
            var uname = Request["uname"].ToString();
            var title = Request["title"].ToString();
            var mobile = Request["mobile"].ToString();
            var content = Request["content"].ToString();
            if (_mm.AddMsg(title, content, uname, mobile))
            {
                Response.Write("<script language=javascript>alert('提示', '发布留言成功！请耐心等待回复', 'info');</script>");
            }
            else
            {
                Response.Write("<script language=javascript>alert('提示', '发布留言失败，请重试', 'error');</script>");
            }
            //var sr = new StreamReader(data.InputStream);
            //var stream = sr.ReadToEnd();
            //var javaScriptSerializer = new JavaScriptSerializer();
            //var mi = javaScriptSerializer.Deserialize<MessageInfo>(stream);
            //Response.Write(_mm.AddMsg(mi));
        }

        public void LeaveMsg()
        {
            var uname = Request["UserName"].ToString();
            var title = Request["Title"].ToString();
            var content = Request["Content"].ToString();
            Response.Write(_mm.LeaveMesage(uname, title, content));
        }

        public void GetUserMsg()
        {
            var userName = Request["UserName"];
            var page = int.Parse(Request["page"].ToString());
            var rows = int.Parse(Request["rows"].ToString());
            Response.Write(_mm.GetUserMessages(page, rows, userName));
        }
    }
}