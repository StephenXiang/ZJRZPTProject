using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using GeneralFrameworkBLLModel;

namespace GeneralFrameworkDAL
{
    public class PublishJRCPService
    {
        public bool SaveJRCPInfo(JRCPInfo ji)
        {
            var sql = string.Format(@"select BankId from SysUser where UserName='{0}'", ji.UserName);
            var bank = DBHelper.GetScalar(sql) as int?;
            if (bank == null) return false;
            sql = @"
insert into JRCPFlow(BankId,Title,QxLow,QxUp,DanbaoId,DaikunLow,DaikuanUp,LilvLow,LilvUp
,Dianhua,Jianjie,Tedian,Kehu,Tiaojian,Cailiao,Logo,PublishDate,Status)
values(@BankId,@Title,@QxLow,@QxUp,@DanbaoId,@DaikunLow,@DaikuanUp,@LilvLow,@LilvUp
,@Dianhua,@Jianjie,@Tedian,@Kehu,@Tiaojian,@Cailiao,@Logo,@PublishDate,0)";
            var efc = DBHelper.Execute(sql,
                new SqlParameter("@BankId", bank),
                new SqlParameter("@Title", ji.jrname),
                new SqlParameter("@QxLow", ji.dkqxstart),
                new SqlParameter("@QxUp", ji.dkqxend),
                new SqlParameter("@DanbaoId", ji.jrdanbao),
                new SqlParameter("@DaikunLow", ji.dkedstart),
                new SqlParameter("@DaikuanUp", ji.dkedend),
                new SqlParameter("@LilvLow", ji.llfwstart),
                new SqlParameter("@LilvUp", ji.llfwend),
                new SqlParameter("@Dianhua", ji.jrlxdh),
                new SqlParameter("@Jianjie", ji.jrcpjj),
                new SqlParameter("@Tedian", ji.jrcptd),
                new SqlParameter("@Kehu", ji.jrsykh),
                new SqlParameter("@Tiaojian", ji.jrsqtj),
                new SqlParameter("@Cailiao", ji.jrcailiao),
                new SqlParameter("@Logo", ji.logo ?? new byte[0]),
                new SqlParameter("@PublishDate", DateTime.Now));
            return efc > 0;
        }

        public string GetJRCPJson(string username, int page, int rows)
        {
            var sql = string.Format(@"select BankId from SysUser where UserName='{0}'", username);
            var bank = DBHelper.GetScalar(sql) as int?;
            if (bank == null) return "";
            sql = string.Format(
                @"select f.Id as ID,Title,CAST(QxLow as varchar)+'个月~'+CAST(QxUp as varchar)+'个月' as Qixian,
l.[Desc] as Danbao,CAST(CAST(DaikunLow as float) as varchar)+'~'+CAST(CAST(DaikuanUp as float) as varchar) as Edu,
CAST(CAST(LilvLow as float) as varchar)+'%~'+CAST(CAST(LilvUp as float) as varchar)+'%' as Lilv,
Dianhua,Jianjie,CONVERT(varchar(30),PublishDate,102) as PublishDate,
case Status when 0 then '待审批' when 1 then '审核已通过' else '审核未通过' end as Status
from JRCPFlow f
left join [Lookup] l on l.Id=f.DanbaoId
where BankId={0} order by f.Id Desc
", bank);
            DataTable dt = DBHelper.GetDataSet(sql);
            var reply = JSON.JsonHelper.TableToJson(dt.Rows.Count, JSON.JsonHelper.GetPagedTable(dt, page, rows));
            return reply;
        }
    }
}
