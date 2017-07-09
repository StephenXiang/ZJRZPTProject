using System.Data;
using System.Data.SqlClient;
using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL.JSON;
using System.Collections.Generic;
using System;

namespace GeneralFrameworkDAL
{
    public class PublishZzdService
    {
        public string GetMainBank()
        {
            var sql = "select Id as ID,BankName as TypeName from MainBank";
            var dt = DBHelper.GetDataSet(sql);
            return JsonHelper.ConvertJosnData(dt);
        }

        public string GetBanks()
        {
            var sql = "select Id as ID,[Name] as TypeName from [Bank] where BankType = 1";
            var dt = DBHelper.GetDataSet(sql);
            return JsonHelper.ConvertJosnData(dt);
        }

        public string GetMastBanks(string mainbankid)
        {
            string sqlPbank = "select BankName from MainBank where Id='" + mainbankid + "'";
            var pbankdt = DBHelper.GetDataSet(sqlPbank);
            if (pbankdt.Rows.Count > 0)
            {
                var pbankdr = pbankdt.Rows[0];
                mainbankid = pbankdr[0].ToString();
            }

            if (mainbankid != "中国银行")
            {
                mainbankid = mainbankid.Replace("中国", "");
            }
            var sql = "select Id as ID,[Name] as TypeName from [Bank] where 1=1 and iszzd = 0 and Name Like '%" + mainbankid + "%'";
            var dt = DBHelper.GetDataSet(sql);
            return JsonHelper.ConvertJosnData(dt);
        }

        public bool Save(ZzdInfo zi)
        {
            if (zi.Id > 0) return Update(zi);
            var sql = string.Format(@"select EnterpriseId from SysUser where UserName='{0}'", zi.UserName.Trim());
            var ent = DBHelper.GetScalar(sql) as int?;
            if (ent == null) return false;
            sql = @"
insert into ZZDFlow(EnterpriseId,BankId,MastBankId,Manager,ManagerPhone,OriginalQuota,ThisQuota,ExpirationDate,[Status],PublishDate)
values(@ent,@bk,@mbk,@ma,@map,@oq,@q,@ed,0,@date)";
            var efc = DBHelper.Execute(sql, new SqlParameter("@eid", ent),
                new SqlParameter("@ent", ent),
                new SqlParameter("@bk", zi.ydkyh),
                new SqlParameter("@mbk", zi.zbh),
                new SqlParameter("@ma", zi.khjlxm),
                new SqlParameter("@map", zi.khjllxdh),
                new SqlParameter("@oq", zi.ydkje),
                new SqlParameter("@q", zi.bcdkje),
                new SqlParameter("@ed", zi.dkdqsj),
                new SqlParameter("@date", DateTime.Now));
            if (efc > 0)
            {
                List<string> phonelist = new List<string>();
                sql = "select Phone from SysUser where RolesID = 2";
                DataTable dt = DBHelper.GetDataSet(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string Phone = dr[0].ToString();
                        phonelist.Add(Phone);
                    }
                }
                sql = string.Format(@"select Phone from Bank a right join LiaisonanMan b on a.Id = b.BankId where 1=1 and b.IsDeleteed = 0 and a.Id in({0})", zi.ydkyh);
                DataTable yhdt = DBHelper.GetDataSet(sql);
                if (yhdt.Rows.Count > 0)
                {
                    foreach (DataRow yhdr in yhdt.Rows)
                    {
                        string Phone = yhdr[0].ToString();
                        phonelist.Add(Phone);
                    }
                }
                SmsService sms = new SmsService();
                for (int h = 0; h < phonelist.Count; h++)
                {
                    sms.Send(phonelist[h].ToString(), "有新的周转贷申请，请及时登陆镇江融资平台查看详细信息");
                }
            }
            return efc > 0;
        }

        public bool Update(ZzdInfo zi)
        {
            var sql = string.Format(@"select EnterpriseId from SysUser where UserName='{0}'", zi.UserName.Trim());
            var ent = DBHelper.GetScalar(sql) as int?;
            if (ent == null) return false;
            sql = @"
update ZZDFlow set BankId=@bk,MastBankId=@mbk,Manager=@ma,ManagerPhone=@map,OriginalQuota=@oq,ThisQuota=@q,ExpirationDate=@ed where Id=@zzdid";
            var efc = DBHelper.Execute(sql,
                new SqlParameter("@zzdid", zi.Id),
                new SqlParameter("@bk", zi.ydkyh),
                new SqlParameter("@mbk", zi.zbh),
                new SqlParameter("@ma", zi.khjlxm),
                new SqlParameter("@map", zi.khjllxdh),
                new SqlParameter("@oq", zi.ydkje),
                new SqlParameter("@q", zi.bcdkje),
                new SqlParameter("@ed", zi.dkdqsj));
            return efc > 0;
        }

        public string GetZZDLBJson(string UserName, int page, int rows)
        {
            var sql = string.Format(@"select EnterpriseId from SysUser where UserName='{0}'", UserName);
            var ent = DBHelper.GetScalar(sql) as int?;
            if (ent == null) return "";
            sql = string.Format(@"select f.Id as ZZDID,b.Name as ZZDBk,b2.Name as ZZDMBK,b.Id as ZZDBkid,b2.Id as ZZDMBKid,
Manager as ZZDManager,ManagerPhone as ZZDManagerPhone,
OriginalQuota as ZZDOriginalQuota,
ThisQuota as ZZDThisQuota,
CONVERT(varchar(100), ExpirationDate, 23) as ZZDExpirationDate,
[Status] as ZZDStatus,
CONVERT(varchar(100), PublishDate, 23) as ZZDPublishDate,f.Feedback
from ZZDFlow f
left join Bank b on b.Id=f.BankId
left join Bank b2 on b2.Id=f.MastBankId
where f.EnterpriseId={0} and f.IsDeleted=0 order by f.Id Desc", ent);
            var dt = DBHelper.GetDataSet(sql);
            var reply = JSON.JsonHelper.TableToJson(dt.Rows.Count, JsonHelper.GetPagedTable(dt, page, rows));
            return reply;
        }

        public bool Delete(int id)
        {
            var sql = string.Format("update ZZDFlow set IsDeleted=1 where Id={0}", id);
            DBHelper.Execute(sql);
            return true;
        }
    }
}
