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
    public class JRCPApplyService
    {
        public bool Add(JRCPApply jrcpapply)
        {
            string sql = @"insert into JRCPApply(JRCPID,ApplyEnterpriseId,ApplyEnterpriseName,Phone)values(@JRCPID,@ApplyEnterpriseId,@ApplyEnterpriseName,@Phone)";
            int? id = DBHelper.Execute(sql,
                new SqlParameter("@JRCPID", jrcpapply.JRCPID),
                new SqlParameter("@ApplyEnterpriseId", jrcpapply.ApplyEnterpriseId),
                new SqlParameter("@ApplyEnterpriseName", jrcpapply.ApplyEnterpriseName),
                new SqlParameter("@Phone", jrcpapply.Phone)) as int?;
            if (id > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int CheckApply(JRCPApply jrcpapply)
        {
            string sql = "select * from JRCPApply where 1=1 and Status= 0 and ApplyEnterpriseId = '" + jrcpapply.ApplyEnterpriseId + "' and JRCPID ='" + jrcpapply.JRCPID + "'";
            DataTable dt = DBHelper.GetDataSet(sql);
            return dt.Rows.Count;
        }

        public string GetApplyDg(string UserName, int page, int rows)
        {
            string reply = string.Empty;
            string BankID = string.Empty;
            DataTable dt = new DataTable();
            string sql = @"select BankId from SysUser  where UserName = '" + UserName + "'";
            dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                BankID = dr[0].ToString();
                if (BankID != "")
                {
                    sql = @"select a.Id,a.JRCPID,b.Title,a.ApplyEnterpriseName,a.Phone,a.CreateDate from JRCPApply a 
left join JRCPFlow b on a.JRCPID = b.Id
left join Bank c on b.BankId = c.Id
where b.BankId = '" + BankID + "'";
                    dt = DBHelper.GetDataSet(sql);
                    reply = JsonHelper.TableToJson(dt.Rows.Count, JsonHelper.GetPagedTable(dt, page, rows));
                    return reply;
                }
                return reply;
            }
            else
            {
                return reply;
            }

        }
    }
}
