using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL.JSON;

namespace GeneralFrameworkDAL
{
    public class MessageService
    {
        public string GetMessages(int page, int rows, int? status = null)
        {
            var sql = @"select m.Id as ID,m.Type,m.Title,m.Content,m.Date,m.ReplyUser,m.ReplyContent,m.ReplyDate,
(case m.Status when '0' then '未处理' when '1' then '已回复' end) as Status,
(case when m.UserPhone is null then u.Phone else m.UserPhone end) as UserPhone,
(case when m.UserName is null then u.UserName else m.UserName end) as UserName
from Message m left join SysUser u on m.UserId=u.ID
" + (status == null ? "order by m.ReplyDate desc,m.Date desc" :
string.Format("where m.Status={0} {1}", status, status == 0 ? "order by m.Date desc" : "order by m.ReplyDate desc,m.Date desc"));
            var dt = DBHelper.GetDataSet(sql);
            return JsonHelper.TableToJson(dt.Rows.Count, JsonHelper.GetPagedTable(dt, page, rows));
        }

        public string GetUserMessages(int page, int rows, string username)
        {
            var sql = string.Format(@"select Id as ID,Type,UserPhone,Title,Content,UserName,Date,(
case Status when '0' then '未处理' when '1' then '已回复' end) as Status,
ReplyUser,ReplyContent,ReplyDate from Message where UserId in (select ID from SysUser where UserName='{0}')
order by Date desc",
                username);
            var dt = DBHelper.GetDataSet(sql);
            return JsonHelper.TableToJson(dt.Rows.Count, JsonHelper.GetPagedTable(dt, page, rows));
        }

        public bool LeaveMesage(string username, string title, string content)
        {
            var sql = string.Format("select ID,Phone from SysUser where UserName='{0}'", username);
            var dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count == 0) return false;
            var uid = Convert.ToInt32(dt.Rows[0][0]);
            //var phone = dt.Rows[0][1].ToString();
            sql = @"insert into Message (Title,Content,UserId,Date,Status)
values (@title,@cont,@ui,@date,0)";

            return DBHelper.Execute(sql,
                       new SqlParameter("@title", title),
                       new SqlParameter("@cont", content),
                       new SqlParameter("@ui", uid),
                       new SqlParameter("@date", DateTime.Now)) > 0;
        }

        public string GetReceivers()
        {
            var sql = "select ID,DepartmentName as TypeName from SysDepartment where RoleID=2";
            var dt = DBHelper.GetDataSet(sql);
            return JsonHelper.ConvertJosnData(dt);
        }

        public bool Reply(MessageInfo mi)
        {
            var sql = string.Format("update Message set ReplyUser='{0}',ReplyContent='{1}',ReplyDate=GETDATE(),Status=1 where Id={2}",
                mi.ReplyUser, mi.ReplyContent, mi.ID);
            return DBHelper.Execute(sql) > 0;
        }

        public bool DelMessage(int msgId)
        {
            return true;
        }

        public bool AddMessage(MessageInfo mi)
        {
            var sql = @"insert into Message (Title,Content,UserId,UserName,UserPhone,Date,Status)
values (@title,@cont,@ui,@un,@up,@date,0)";

            return DBHelper.Execute(sql,
                       new SqlParameter("@title", mi.Title),
                       new SqlParameter("@cont", mi.Content),
                       new SqlParameter("@un", mi.UserName),
                       new SqlParameter("@up", mi.UserPhone),
                       new SqlParameter("@ui", mi.UserId ?? 0),
                       new SqlParameter("@date", DateTime.Now)) > 0;
        }
    }
}
