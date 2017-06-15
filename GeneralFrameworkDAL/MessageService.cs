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
            var sql = @"select Id as ID,Type,UserPhone,Title,Content,UserName,Date,(
case Status when '0' then '未回复' when '1' then '已回复' end) as Status,
ReplyUser,ReplyContent,ReplyDate from Message
" + (status == null ? "" : string.Format("where Status={0}", status));
            var dt = DBHelper.GetDataSet(sql);
            return JsonHelper.TableToJson(dt.Rows.Count, JsonHelper.GetPagedTable(dt, page, rows));
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
            var dep = string.IsNullOrEmpty(mi.LoginName) ? "" : SysUserService.GetUserDepartment(mi.LoginName);
            var sql = @"insert into Message (Title,Content,UserName,UserPhone,UserDepartment,Date,Status)
values (@title,@cont,@un,@up,@ud,@date,0)";

            return DBHelper.Execute(sql,
                       new SqlParameter("@title", mi.Title),
                       new SqlParameter("@cont", mi.Content),
                       new SqlParameter("@un", mi.UserName),
                       new SqlParameter("@up", mi.UserPhone),
                       new SqlParameter("@date", DateTime.Now),
                       new SqlParameter("@ud", dep)) > 0;
        }
    }
}
