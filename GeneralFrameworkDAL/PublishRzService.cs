using System.Data.SqlClient;
using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL.JSON;
using System.Data;

namespace GeneralFrameworkDAL
{
    public class PublishRzService
    {
        public string GetBanks()
        {
            var sql = "select Id as ID,[Name] as TypeName from [Bank]";
            var dt = DBHelper.GetDataSet(sql);
            return JsonHelper.ConvertJosnData(dt);
        }

        public bool SaveRzInfo(RzInfo rzi)
        {
            bool isDyw = false;
            if (rzi.dywdesc.Trim() != "" && rzi.dywdesc != null)
            {
                isDyw = true;
            }
            var sql = string.Format(@"select EnterpriseId from SysUser where UserName='{0}'", rzi.UserName.Trim());
            var ent = DBHelper.GetScalar(sql) as int?;
            if (ent == null) return false;
            sql = @"
insert into RZDemandInfo (Quota,TermId,PurposeId,HadCollateral,CollateralDesc)
values(@qua,@tem,@pup,@hc,@cd)
SELECT @@IDENTITY";
            var demid = DBHelper.GetScalar(sql,
                new SqlParameter("@qua", rzi.RZED),
                new SqlParameter("@tem", rzi.RZQX),
                new SqlParameter("@pup", rzi.RZQT),
                new SqlParameter("@hc", isDyw),
                new SqlParameter("@cd", rzi.dywdesc.Trim()));
            if (demid == null) return false;
            sql = @"
insert into RZFlow(EnterpriseId,BankIds,FinanceId,DemandId,[Status])
values(@ent,@bks,(select top 1 Id from RZFinance where EnterpriseId=@ent),@dm,0)";
            var efc = DBHelper.Execute(sql,
                new SqlParameter("@ent", ent),
                new SqlParameter("@bks", rzi.RZYH),
                new SqlParameter("@dm", demid));
            return efc > 0;
        }

        public string GetRZLBJson(string UserName)
        {
            var sql = string.Format(@"select EnterpriseId from SysUser where UserName='{0}'", UserName);
            var ent = DBHelper.GetScalar(sql) as int?;
            if (ent == null) return "";
            sql = string.Format(@"select a.Quota as RZED,c.[Desc] as RZQX,b.BankIds as SXYH, d.[Desc] as RZYT,a.CollateralDesc as DYW,b.PublishDate,b.[Status] from RZDemandInfo a 
left join RZFlow b on a.Id = b.DemandId
left join (select * from Lookup where Type = 8) c on a.TermId = c.Id
left join (select * from Lookup where Type = 9) d on a.PurposeId = d.Id
where b.EnterpriseId = {0}", ent);
            DataTable dt = DBHelper.GetDataSet(sql);
            var reply = JSON.JsonHelper.SerializeObject(dt);
            return reply;
        }

        public string GetRZBankstr(string BankIds)
        {
            string Bankstr = string.Empty;
            var sql = string.Format(@"select Name from Bank where Id in({0})", BankIds);
            DataTable dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    if (i == 0)
                    {
                        Bankstr = dr["Name"].ToString();
                    }
                    else
                    {
                        Bankstr = Bankstr + "," + dr["Name"].ToString();
                    }

                }
                return Bankstr;
            }
            else
            {
                return "无";
            }
        }
    }
}
