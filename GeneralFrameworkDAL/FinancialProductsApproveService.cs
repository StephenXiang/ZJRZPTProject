using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL.JSON;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace GeneralFrameworkDAL
{
    public class FinancialProductsApproveService
    {
        public string GetJRCPTableJson(int page, int rows)
        {
            string sql = @"select a.Id,a.Title,b.Name,a.PublishDate,a.Status from JRCPFlow a 
left join Bank b on a.BankId = b.Id order by a.Id desc";
            DataTable dt = DBHelper.GetDataSet(sql);
            return JsonHelper.TableToJson(dt.Rows.Count, JsonHelper.GetPagedTable(dt, page, rows));
        }

        public string GetJRCPDetialById(int Id)
        {
            string reply = string.Empty;
            string sql = @"select a.Title,a.Dianhua,a.LilvLow,a.LilvUp,a.DaikunLow,a.DaikuanUp,a.QxLow,a.QxUp,b.[Desc],a.Jianjie,a.Tedian,a.Kehu,a.Tiaojian,a.Cailiao
 from JRCPFlow a 
left join (select * from Lookup where Name ='担保方式') b on a.DanbaoId = b.Id where  a.Id = '" + Id + "'";
            DataTable dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                var jsondata = new JRCPReply
                {
                    Title = dr["Title"].ToString(),
                    lilvfanwei = dr["LilvLow"].ToString() + "%-" + dr["LilvUp"].ToString() + "%",
                    daikuanqixian = dr["QxLow"].ToString() + "个月-" + dr["QxUp"].ToString() + "个月",
                    daikuanedu = dr["DaikunLow"].ToString() + "万元-" + dr["DaikuanUp"].ToString() + "万元",
                    danbaofangshi = dr["Desc"].ToString(),
                    jianjie = dr["Jianjie"].ToString(),
                    tedian = dr["Tedian"].ToString(),
                    shiyongkehu = dr["Kehu"].ToString(),
                    tiaojian = dr["Tiaojian"].ToString(),
                    lianxidianhua = dr["Dianhua"].ToString(),
                    cailiao = dr["Cailiao"].ToString()
                };
                reply = JsonHelper.SerializeObject(jsondata);
                return reply;
            }
            else
            {
                return "";
            }
        }

        public bool EditJRCPStatus(string status, int Id)
        {
            string sql = string.Empty;
            sql = @"update JRCPFlow set [Status] = @status where Id = @Id";
            var sid = DBHelper.Execute(sql, new SqlParameter("@status", status), new SqlParameter("@Id", Id));
            if (sid > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
