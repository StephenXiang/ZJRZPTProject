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
            var sql = @"select Id,Title,Content,UserName,Date,(
case Status when '0' then '未回复' when '1' then '已回复' end) as Status from Message
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

        public bool DelMessage(int msgId)
        {
            return true;
        }

        public bool Reply()
        {
            return true;
        }

        public bool AddMessage(MessageInfo mi)
        {
            var dep = SysUserService.GetUserDepartment(mi.UserName);
            var sql = @"
insert into Message (Title,Content,ReceiverDepartmentId,UserName,UserDepartment)
values (@title,@cont,@recv,@un,@ud)";

            return DBHelper.Execute(sql,
                       new SqlParameter("@title", mi.Title),
                       new SqlParameter("@cont", mi.Content),
                       new SqlParameter("@recv", mi.DepartmentId),
                       new SqlParameter("@un", mi.UserName),
                       new SqlParameter("@ud", dep)) > 0;
        }
    }
}
