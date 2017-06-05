using System.Data.SqlClient;
using GeneralFrameworkBLLModel;

namespace GeneralFrameworkDAL
{
    public class PublishZzdService
    {
        public bool Save(ZzdInfo zi)
        {
            var sql = string.Format(@"select EnterpriseId from SysUser where UserName='{0}'", zi.UserName.Trim());
            var ent = DBHelper.GetScalar(sql) as int?;
            if (ent == null) return false;
            sql = @"
insert into ZZDFlow(EnterpriseId,BankId,MastBankId,Manager,ManagerPhone,OriginalQuota,ThisQuota,ExpirationDate,[Status])
values(@ent,@bk,@mbk,@ma,@map,@oq,@q,@ed,0)";
            var efc = DBHelper.Execute(sql, new SqlParameter("@eid", ent),
                new SqlParameter("@ent", ent),
                new SqlParameter("@bk", zi.ydkyh),
                new SqlParameter("@mbk", zi.zbh),
                new SqlParameter("@ma", zi.khjlxm),
                new SqlParameter("@map", zi.khjllxdh),
                new SqlParameter("@oq", zi.ydkje),
                new SqlParameter("@q", zi.bcdkje),
                new SqlParameter("@ed", zi.dkdqsj));
            return efc > 0;
        }
    }
}
