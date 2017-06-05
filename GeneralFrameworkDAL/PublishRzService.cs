using System.Data.SqlClient;
using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL.JSON;

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
                new SqlParameter("@hc", string.IsNullOrEmpty(rzi.dywdesc)),
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
    }
}
