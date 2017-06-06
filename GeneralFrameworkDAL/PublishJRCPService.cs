using System;
using System.Collections.Generic;
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
insert into JRCPFlow(BankId,QxLow,QxUp,DanbaoId,DaikunLow,DaikuanUp,LilvLow,LilvUp
,Dianhua,Jianjie,Tedian,Kehu,Tiaojian,Cailiao,Logo,PublishDate,Status)
values(@BankId,@QxLow,@QxUp,@DanbaoId,@DaikunLow,@DaikuanUp,@LilvLow,@LilvUp
,@Dianhua,@Jianjie,@Tedian,@Kehu,@Tiaojian,@Cailiao,@Logo,@PublishDate,0)";
            var efc = DBHelper.Execute(sql,
                new SqlParameter("@BankId", bank),
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
    }
}
