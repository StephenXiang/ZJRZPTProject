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
where BankId={0} and f.IsDeleted=0 order by f.Id Desc
", bank);
            DataTable dt = DBHelper.GetDataSet(sql);
            var reply = JSON.JsonHelper.TableToJson(dt.Rows.Count, JSON.JsonHelper.GetPagedTable(dt, page, rows));
            return reply;
        }

        public string GetIndexJRCPInfo()
        {
            List<IndexNewsInfo> list = new List<IndexNewsInfo>();
            var sql = @"select top(6) b.Name,a.Id,Title,CONVERT(varchar(5), PublishDate, 110) as createdate from JRCPFlow a 
left join Bank b on a.BankId = b.Id
 where Status = 1 and a.IsDeleted=0 order by b.sort asc";
            var dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    IndexNewsInfo ini = new IndexNewsInfo()
                    {
                        id = int.Parse(dr["Id"].ToString()),
                        Title = dr["Name"] + "--" + dr["Title"].ToString(),
                        date = dr["createdate"].ToString()
                    };
                    list.Add(ini);
                }
            }
            return JsonHelper.SerializeObject(list);
        }

        public string GetJRCPList(string dkqd, string dkqx, string dbfs, string dked, string jgmc, string cpmc)
        {
            string where = "";
            if (jgmc != "0")
            {
                where = where + " and c.BankName like '%" + jgmc + "%'";
            }
            if (cpmc != "0")
            {
                where = where + " and a.Title like '%" + cpmc + "%'";
            }
            if (dkqd != "0")
            {

            }
            if (dkqx != "0")
            {
                switch (dkqx)
                {
                    case "1":
                        where = where + " and a.QxLow <= '" + 3 + "'";
                        break;
                    case "2":
                        where = where + " and a.QxLow <= '" + 6 + "'";
                        break;
                    case "3":
                        where = where + " and a.QxLow <= '" + 12 + "'";
                        break;
                    case "4":
                        where = where + " and a.QxLow <= '" + 36 + "'";
                        break;
                    case "5":
                        where = where + " and a.QxLow >= '" + 36 + "'";
                        break;
                }
            }
            if (dbfs != "0")
            {
                switch (dbfs)
                {
                    case "1":
                        where = where + " and d.[Desc] = '抵押'";
                        break;
                    case "2":
                        where = where + " and d.[Desc] = '质押'";
                        break;
                    case "3":
                        where = where + " and d.[Desc] = '信保基金'";
                        break;
                    case "4":
                        where = where + " and d.[Desc] = '一般保证'";
                        break;
                    case "5":
                        where = where + " and d.[Desc] = '信用'";
                        break;
                }
            }
            if (dked != "0")
            {
                switch (dked)
                {
                    case "1":
                        where = where + " and a.DaikunLow <= '100'";
                        break;
                    case "2":
                        where = where + " and a.DaikunLow <= '200'";
                        break;
                    case "3":
                        where = where + " and a.DaikunLow <= '300'";
                        break;
                    case "4":
                        where = where + " and a.DaikunLow <= '500'";
                        break;
                    case "5":
                        where = where + " and a.DaikunLow <= '1000'";
                        break;
                    case "6":
                        where = where + " and a.DaikunLow >= '1000'";
                        break;
                }

            }
            var sql = @"select a.Id,b.Id as BankId,c.BankName,a.Title,a.LilvLow,a.LilvUp,a.DaikunLow,a.DaikuanUp,a.DanbaoId,d.[Desc],a.QxLow,a.QxUp,b.Logo2 from JRCPFlow a 
left join Bank b on a.BankId = b.Id
left join MainBank c on b.MainBankId = c.Id
left join (select Id,[Type],[Desc] from Lookup where Name='担保方式') d on a.DanbaoId = d.Id
 where a.Status  = 1 and a.IsDeleted=0 " + where + " order by b.sort asc";
            DataTable dt = DBHelper.GetDataSet(sql);
            return JsonHelper.SerializeObject(dt);
        }

        public string GetJRCPById(int id)
        {
            var sql = @"select BankId,b.Name,Title, a.LilvLow,a.LilvUp,a.DaikunLow,a.DaikuanUp,a.DanbaoId,d.[Desc],a.QxLow,a.QxUp,a.Jianjie,a.Tedian,a.Kehu,a.Tiaojian,a.Cailiao,Dianhua
  from JRCPFlow a 
 left join Bank b on a.BankId = b.Id
 left join (select Id,[Type],[Desc] from Lookup where Name='担保方式') d on a.DanbaoId = d.Id
 where a.Id = '" + id + "'";
            DataTable dt = DBHelper.GetDataSet(sql);
            return JsonHelper.SerializeObject(dt);
        }

        public bool Delete(int id)
        {
            var sql = string.Format("update JRCPFlow set IsDeleted=1 where Id={0}", id);
            DBHelper.Execute(sql);
            return true;
        }

        public string GetNewJRCP()
        {
            string id = "0";
            var sql = @"select top(1)Id from JRCPFlow where IsDeleted = 0 and Status = 1";
            DataTable dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count == 0)
            {
                return id;
            }
            else
            {
                DataRow dr = dt.Rows[0];
                id = dr[0].ToString();
            }
            return id;
        }
    }
}
